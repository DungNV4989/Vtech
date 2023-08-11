using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Timing;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using Volo.Abp.Validation;
using VTECHERP.Constants;
using VTECHERP.DTOs.Attachment;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.DraftTicketProducts;
using VTECHERP.DTOs.DraftTickets;
using VTECHERP.DTOs.WarehouseTransferBills;
using VTECHERP.DTOs.WarehousingBills;
using VTECHERP.Entities;
using VTECHERP.Enums;
using VTECHERP.Enums.DraftTicket;
using VTECHERP.Enums.WarehouseTransferBill;

namespace VTECHERP.Services
{
    public class DraftTicketService : IDraftTicketService
    {
        private readonly IRepository<DraftTicket> _draftTicketRepository;
        private readonly IRepository<Products> _productRepository;
        private readonly IRepository<DraftTicketProduct> _draftTicketProductRepository;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IRepository<WarehouseTransferBill> _warehouseTransferBillRepository;
        private readonly IRepository<WarehouseTransferBillProduct> _warehouseTransferBillProductRepository;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly IStoreService _storeService;
        private readonly IProductService _productService;
        private readonly IStoreProductService _storeProductService;
        private readonly IWarehousingBillService _warehousingBillService;
        private readonly IDraftTicketProductService _draftTicketProductService;
        private readonly IAttachmentService _attachmentService;

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IObjectMapper _objectMapper;
        private readonly ICurrentUser _currentUser;
        private readonly IClock _clock;
        private readonly IRepository<Stores> _storeRepository;

        public DraftTicketService(
            IRepository<DraftTicket> draftTicketRepository,
            IRepository<Products> productRepository,
            IRepository<DraftTicketProduct> draftTicketProductRepository,
            IIdentityUserRepository userRepository,
            IRepository<WarehouseTransferBill> warehouseTransferBillRepository,
            IRepository<WarehouseTransferBillProduct> warehouseTransferBillProductRepository,
            IRepository<UserStore> userStoreRepository,
            IStoreService storeService,
            IProductService productService,
            IStoreProductService storeProductService,
            IWarehousingBillService warehousingBillService,
            IDraftTicketProductService draftTicketProductService,
            IAttachmentService attachmentService,
            IRepository<Stores> storeRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IObjectMapper objectMapper,
            ICurrentUser currentUser,
            IClock clock)
        {
            _draftTicketRepository = draftTicketRepository;
            _productRepository = productRepository;
            _draftTicketProductRepository = draftTicketProductRepository;
            _userRepository = userRepository;
            _warehouseTransferBillRepository = warehouseTransferBillRepository;
            _warehouseTransferBillProductRepository = warehouseTransferBillProductRepository;
            _userStoreRepository = userStoreRepository;
            _storeService = storeService;
            _productService = productService;
            _storeProductService = storeProductService;
            _warehousingBillService = warehousingBillService;
            _draftTicketProductService = draftTicketProductService;
            _attachmentService = attachmentService;
            _unitOfWorkManager = unitOfWorkManager;
            _objectMapper = objectMapper;
            _currentUser = currentUser;
            _clock = clock;
            _storeRepository = storeRepository;
        }

        public async Task<Guid> AddDraftAsync(DraftTicketCreateRequest request)
        {
            await ValidateAdd(request);
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var draftTicket = _objectMapper.Map<DraftTicketCreateRequest, DraftTicket>(request);

                draftTicket.Status = Status.New;
                draftTicket.TransferBillType = TransferBillType.Export;
                draftTicket.CreatorId = _currentUser.Id;

                await _draftTicketRepository.InsertAsync(draftTicket);
                await uow.SaveChangesAsync();

                var listProductIds = request.DraftTicketProducts.Select(x => x.ProductId).ToList();
                var listProduct = await _productRepository.GetListAsync(x => listProductIds.Any(z => z == x.Id));
                var storeProducts = await _storeProductService.GetByStoreId(draftTicket.SourceStoreId);

                var draftTicketProducts = _objectMapper.Map<List<DraftTicketProductCreateRequest>, List<DraftTicketProduct>>(request.DraftTicketProducts.ToList());

                draftTicketProducts.ForEach(x =>
                {
                    var canTranfer = request.DraftTicketProducts.FirstOrDefault(p => p.ProductId == x.ProductId)?.CanTransfer;
                    x.CanTransfer = canTranfer ?? 0;
                    x.DraftTicketId = draftTicket.Id;
                    x.CreatorId = _currentUser.Id;
                    x.CostPrice = listProduct.FirstOrDefault(z => z.Id == x.ProductId)?.StockPrice ?? 0;
                });

                await _draftTicketProductRepository.InsertManyAsync(draftTicketProducts);

                await uow.SaveChangesAsync();
                await uow.CompleteAsync();
                return draftTicket.Id;
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
        }

        public async Task<DraftTicketDetailDto> GetByIdAsync(Guid id)
        {
            var result = new DraftTicketDetailDto();

            var userStores = await _userStoreRepository.GetListAsync(p => p.UserId == _currentUser.Id);
            if (!userStores.Any())
            {
                throw new BusinessException(ErrorMessages.UserStore.StoreNotDefine);
            }
            var userStoreIds = userStores.Select(p => p.StoreId).ToList();
            var draftTicket = await _draftTicketRepository.FindAsync(x => x.Id == id);
            if (draftTicket == null)
                throw new AbpValidationException($"{ErrorMessages.DraftTicket.NotExist} => Id : {id}");
            else if(!userStoreIds.Contains(draftTicket.SourceStoreId) && !userStoreIds.Contains(draftTicket.DestinationStoreId))
            {
                throw new BusinessException(ErrorMessages.UserStore.StoreNotDefine);
            }
            result = _objectMapper.Map<DraftTicket, DraftTicketDetailDto>(draftTicket);
            result.SourceStoreName = (await _storeService.GetByIdAsync(result.SourceStoreId)).Name;
            result.DestinationStoreName = (await _storeService.GetByIdAsync(result.DestinationStoreId)).Name;

            var attachments = await _attachmentService.GetByObjectId(id);
            result.Attachments = _objectMapper.Map<List<AttachmentDetailDto>, List<AttachmentShortDto>>(attachments);

            result.DraftTicketProducts = await _draftTicketProductService.GetByDraftTicketId(id);
            return result;
        }

        public async Task<PagingResponse<DraftTicketDto>> GetListAsync(SearchDraftTicketRequest request)
        {
            try
            {
                var userStores = await _userStoreRepository.GetListAsync(p => p.UserId == _currentUser.Id);
                if (!userStores.Any())
                {
                    throw new BusinessException(ErrorMessages.UserStore.StoreNotDefine);
                }
                var userStoreIds = userStores.Select(p => p.StoreId).ToList();
                var result = new List<DraftTicketDto>();
                var ids = new List<Guid>();
                if (!request.WarehousingBillCode.IsNullOrWhiteSpace())
                {
                    var warehousingBills = (await _warehousingBillService.GetContainCodeAsync(request.WarehousingBillCode)).Where(x => x.SourceId != null).ToList();
                    if (warehousingBills.Any())
                        foreach (var warehousingBill in warehousingBills)
                            if (warehousingBill.SourceId != null)
                                ids.Add(warehousingBill.SourceId ?? Guid.Empty);
                }
                var query = (await _draftTicketRepository.GetQueryableAsync())
                    .WhereIf(request.SourceStoreIds.Any(), x => request.SourceStoreIds.Any(s => s == x.SourceStoreId))
                    .WhereIf(request.DestinationStoreIds.Any(), x => request.DestinationStoreIds.Any(d => d == x.DestinationStoreId))
                    .WhereIf(!request.Code.IsNullOrWhiteSpace(), x => x.Code.Contains(request.Code))
                    .WhereIf(!request.WarehousingBillCode.IsNullOrWhiteSpace(), x => ids.Any(id => id == x.Id))
                    .Where(x => userStoreIds.Contains(x.SourceStoreId) || userStoreIds.Contains(x.DestinationStoreId))
                .ToList();
                var draftTickets = query
                    .OrderByDescending(p => p.Code)
                    .Skip(request.Offset)
                    .Take(request.PageSize)
                    .ToList();
                var draftTicketIds = draftTickets.Select(x => x.Id).ToList();
                var draftTicketProducts = await _draftTicketProductService.GetByDraftTicketIds(draftTicketIds);

                var sourceStoreIds = draftTickets.Select(x => x.SourceStoreId);
                var destinationStoreIds = draftTickets.Select(x => x.DestinationStoreId);
                var storeIds = sourceStoreIds.Union(destinationStoreIds);

                var draftApprovedUserIds = draftTickets.Select(x => x.DraftApprovedUserId);
                var deliveryConfirmedUserIds = draftTickets.Select(x => x.DeliveryConfirmedUserId);
                var creatorIds = draftTickets.Select(x => x.CreatorId);
                var userIds = draftApprovedUserIds.Union(deliveryConfirmedUserIds).Union(creatorIds).ToList();

                var stores = await _storeService.GetByIdsAsync(storeIds.ToList());
                var users = (await _userRepository.GetListAsync())
                    .Where(p => userIds.Contains(p.Id));

                if (!draftTickets.Any())
                    return new PagingResponse<DraftTicketDto>(0, result);

                result = _objectMapper.Map<List<DraftTicket>, List<DraftTicketDto>>(draftTickets);

                var warehouseTransferBills = (await _warehouseTransferBillRepository.GetQueryableAsync()).Where(x => draftTicketIds.Any(id => id == (x.DraftTicketId ?? Guid.Empty)));

                var wBills = await _warehousingBillService.GetBySourceIdsAsync(warehouseTransferBills.Select(x => new Guid? (x.Id)).ToList());

                result.ForEach(r =>
                {
                    var products = draftTicketProducts.Where(x => x.DraftTicketId == r.Id);
                    var warehouseTransferBill = warehouseTransferBills.Any() ? warehouseTransferBills.FirstOrDefault(x => x.DraftTicketId == r.Id) : null;
                    if (warehouseTransferBill != null)
                    {
                        r.WarehouseTransferBillId = warehouseTransferBill.Id;
                        r.WarehouseTransferBillCode = warehouseTransferBill.Code;
                    }

                    var warehousingBills = warehouseTransferBill != null ? wBills.Where(x => x.SourceId == warehouseTransferBill.Id) : null;
                    if (warehousingBills != null)
                    {
                        var warehousingBillPNK = warehousingBills.FirstOrDefault(x => x.BillType == WarehousingBillType.Import);
                        if(warehousingBillPNK != null)
                        {
                            r.WarehousingBillIdPNK = warehousingBillPNK.Id;
                            r.WarehousingBillCodePNK = warehousingBillPNK.Code;
                        }
                        var warehousingBillPXK = warehousingBills.FirstOrDefault(x => x.BillType == WarehousingBillType.Export);
                        if (warehousingBillPXK != null)
                        {
                            r.WarehousingBillIdPXK = warehousingBillPXK.Id;
                            r.WarehousingBillCodePXK = warehousingBillPXK.Code;
                        }
                    }
                    r.TotalProductCode = products.Count();
                    r.TotalNumberProduct = products.Sum(x => x.Quantity);
                    r.SourceStoreName = stores.FirstOrDefault(s => s.Id == r.SourceStoreId).Name;
                    r.DestinationStoreName = stores.FirstOrDefault(s => s.Id == r.DestinationStoreId).Name;
                    r.DraftApprovedName = r.DraftApprovedUserId == null ? null : users.FirstOrDefault(u => u.Id == r.DraftApprovedUserId).Name;
                    r.DeliveryConfirmedName = r.DeliveryConfirmedUserId == null ? null : users.FirstOrDefault(u => u.Id == r.DeliveryConfirmedUserId).Name;
                    r.CreatorName = users.FirstOrDefault(x => x.Id == r.CreatorId).Name;
                });

                return new PagingResponse<DraftTicketDto>(query.Count(), result);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<DraftTicketApproveDto> SetApproveByIdAsync(SearchDraftTicketApproveRequest request)
        {
            var result = new DraftTicketApproveDto();
            var draftTicket = await _draftTicketRepository.FindAsync(x => x.Id == request.Id);
            if (draftTicket == null)
                throw new AbpValidationException($"{ErrorMessages.DraftTicket.NotExist} => Id : {request.Id}");
            result = _objectMapper.Map<DraftTicket, DraftTicketApproveDto>(draftTicket);
            result.SourceStoreName = (await _storeService.GetByIdAsync(result.SourceStoreId)).Name;
            result.DestinationStoreName = (await _storeService.GetByIdAsync(result.DestinationStoreId)).Name;

            result.DraftTicketProducts = await _draftTicketProductService
                .SetApproveByDraftTicketIdAsync(request.Id, draftTicket.SourceStoreId, request.ProductName);

            var attachments = await _attachmentService.GetByObjectId(request.Id);
            result.Attachments = _objectMapper.Map<List<AttachmentDetailDto>, List<AttachmentShortDto>>(attachments);

            return result;
        }

        public async Task<DraftTicketApproveRequest> ApproveAsync(DraftTicketApproveRequest request)
        {
            await ValidateApprove(request);
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var billProductIds = request.DraftTicketProducts.Select(x => x.Id).ToList();
                var draftTicketProducts = await _draftTicketProductService.GetByIdsAsync(billProductIds);
                var draftTicket = await _draftTicketRepository.FindAsync(x => x.Id == draftTicketProducts.FirstOrDefault().DraftTicketId);

                draftTicket.Status = Enums.DraftTicket.Status.Approved;
                draftTicket.DraftApprovedUserId = _currentUser.Id;
                draftTicket.DraftApprovedDate = _clock.Now;
                draftTicket.TranferDate = _clock.Now;
                draftTicket.TransferBillType = TransferBillType.Export;
                draftTicket.Note = request.Note;

                await _draftTicketRepository.UpdateAsync(draftTicket);
                await uow.SaveChangesAsync();

                await _draftTicketProductService.ApprovesAsync(request.DraftTicketProducts);

                //  var storeProducts = await _storeProductService.GetByStoreId(draftTicket.SourceStoreId);


                #region Sinh phiếu chuyển kho

                var warehouseTransferBill = new WarehouseTransferBill()
                {
                    SourceStoreId = draftTicket.SourceStoreId,
                    DestinationStoreId = draftTicket.DestinationStoreId,
                    Note = draftTicket.Note,
                    TransferStatus = TransferStatuses.InDelivery,
                    CreatedFrom = WarehouseTransferBillCreatedFrom.Draft,
                    TransferBillType = TransferBillType.Export,
                    DraftApprovedUserId = _currentUser.Id,
                    DraftApprovedDate = _clock.Now,
                    TranferDate = _clock.Now,
                    DraftTicketId = draftTicket.Id,
                };
                await _warehouseTransferBillRepository.InsertAsync(warehouseTransferBill);

                var warehouseTransferBillProducts = new List<WarehouseTransferBillProduct>();
                foreach (var item in request.DraftTicketProducts)
                {
                    var draftTicketProduct = draftTicketProducts.FirstOrDefault(x => x.Id == item.Id);
                    warehouseTransferBillProducts.Add(new WarehouseTransferBillProduct()
                    {
                        WarehouseTransferBillId = warehouseTransferBill.Id,
                        ProductId = draftTicketProduct.ProductId,
                        Quantity = item.ApproveQuantity,
                        CostPrice = draftTicketProduct.CostPrice ?? 0,
                        CreatorId = _currentUser.Id,
                        CreationTime = _clock.Now,
                    });
                }
                if (warehouseTransferBillProducts.Any())
                    await _warehouseTransferBillProductRepository.InsertManyAsync(warehouseTransferBillProducts);

                #region Sinh phiếu XNK tương ứng phiếu chuyển kho

                var warehousebill = new CreateWarehousingBillRequest()
                {
                    BillType = Enums.WarehousingBillType.Export,
                    StoreId = warehouseTransferBill.SourceStoreId,
                    DocumentDetailType = Enums.DocumentDetailType.ExportTransfer,
                    IsFromWarehouseTransfer = true,
                    AudienceType = Enums.AudienceTypes.Other,
                    Products = new List<WarehousingBillProductRequest>(),
                    SourceId = warehouseTransferBill.Id
                };

                var listProductIds = draftTicketProducts.Select(x => x.ProductId).ToList();
                var listProduct = await _productRepository.GetListAsync(x => listProductIds.Any(z => z == x.Id));

                foreach (var item in request.DraftTicketProducts)
                {
                    var billProduct = draftTicketProducts.FirstOrDefault(p => p.Id == item.Id);
                    var product = listProduct.Find(x => x.Id == billProduct.ProductId);
                    var productInBill = new WarehousingBillProductRequest()
                    {
                        ProductId = product.Id,
                        Unit = product.Unit,
                        Quantity = item.ApproveQuantity,
                        Price = billProduct.CostPrice ?? 0,
                    };
                    warehousebill.Products.Add(productInBill);
                }

                await _warehousingBillService.CreateBill(warehousebill);

                #endregion Sinh phiếu xuất kho

                #endregion Sinh phiếu chuyển kho


                await uow.SaveChangesAsync();
                await uow.CompleteAsync();
                return request;
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
        }

        public async Task<Guid> DeleteAsync(Guid id)
        {
            await ValidateDelete(id);
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var draftTicket = await _draftTicketRepository.FindAsync(x => x.Id == id);

                draftTicket.IsDeleted = true;
                draftTicket.DeleterId = _currentUser.Id;

                await _draftTicketRepository.UpdateAsync(draftTicket);
                await uow.SaveChangesAsync();

                await _draftTicketProductService.DeleteRangeAsync(draftTicket.Id);
                await uow.SaveChangesAsync();
                await uow.CompleteAsync();
                return id;
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
        }

        public async Task DeleteRangeAsync(List<Guid> ids)
        {
            await ValidateDeleteRange(ids);
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                foreach (var id in ids)
                {
                    var draftTicket = await _draftTicketRepository.FindAsync(x => x.Id == id);

                    draftTicket.IsDeleted = true;
                    draftTicket.DeleterId = _currentUser.Id;

                    await _draftTicketRepository.UpdateAsync(draftTicket);
                    await uow.SaveChangesAsync();

                    await _draftTicketProductService.DeleteRangeAsync(draftTicket.Id);
                    await uow.SaveChangesAsync();
                    await uow.CompleteAsync();
                }
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
        }

        public async Task ChangeStatus(Guid? id, Status status)
        {
            var draftTicket = await _draftTicketRepository.FindAsync(x => x.Id == id);

            if (draftTicket == null)
                return;

            draftTicket.Status = status;
            await _draftTicketRepository.UpdateAsync(draftTicket);
        }

        private async Task ValidateAdd(DraftTicketCreateRequest request)
        {
            var validationErrors = new List<ValidationResult>();

            if (request.DestinationStoreId == request.SourceStoreId)
                validationErrors.Add(new ValidationResult(
                    ErrorMessages.DraftTicket.Duplicate,
                    new List<string> { "SourceStoreId => DestinationStoreId", request.SourceStoreId.ToString() }));

            var storeExistSourceStoreId = await _storeService.Exist(request.SourceStoreId);
            if (!storeExistSourceStoreId)
                validationErrors.Add(new ValidationResult(
                    ErrorMessages.Store.NotExist,
                    new List<string> { "SourceStoreId", request.SourceStoreId.ToString() }));

            var storeExistDestinationStoreId = await _storeService.Exist(request.DestinationStoreId);
            if (!storeExistDestinationStoreId)
                validationErrors.Add(new ValidationResult(
                    ErrorMessages.Store.NotExist,
                    new List<string> { "DestinationStoreId", request.DestinationStoreId.ToString() }));

            if (!request.DraftTicketProducts.Any())
                validationErrors.Add(new ValidationResult(
                    ErrorMessages.DraftTicketProduct.Null,
                    new List<string> { "DraftTicketProducts", "Null" }));
            else
            {
                var productDuplicates = request.DraftTicketProducts.GroupBy(x => x.ProductId).Where(x => x.Count() > 1).Select(x => x.Key);
                if (productDuplicates.Any())
                    validationErrors.Add(new ValidationResult(
                            ErrorMessages.Product.Duplicate,
                            new List<string> { "ProductIds", productDuplicates.JoinAsString("; ") }));

                var storeProducts = await _productService.GetProductByStordId(request.SourceStoreId);
                foreach (var item in request.DraftTicketProducts)
                {
                    var productExist = await _productService.Exist(item.ProductId);
                    if (!productExist)
                        validationErrors.Add(new ValidationResult(
                            ErrorMessages.Product.NotExist,
                            new List<string> { "ProductId", item.ProductId.ToString() }));

                    if (item.Quantity <= 0)
                        validationErrors.Add(new ValidationResult(
                        ErrorMessages.WarehouseTransferBill.NumericLessThanZero,
                        new List<string> { "Quantity", $"{item.Quantity}" }));
                }
            }

            if (validationErrors.Any())
                throw new AbpValidationException(validationErrors);
        }

        private async Task ValidateApprove(DraftTicketApproveRequest request)
        {
            var validationErrors = new List<ValidationResult>();

            if (!request.DraftTicketProducts.Any())
                validationErrors.Add(new ValidationResult(
                   ErrorMessages.DraftTicketProduct.Null,
                   new List<string> { "DraftTicketProducts", "" }));

            var draftTicketProducts = await _draftTicketProductService.GetByIdsAsync(request.DraftTicketProducts.Select(x => x.Id).ToList());

            var warehouseTransferBill = await _draftTicketRepository.FindAsync(x => x.Id == draftTicketProducts.FirstOrDefault().DraftTicketId);

            var checkWarehouseTransferBillProducts = await _draftTicketProductService.GetByDraftTicketId(draftTicketProducts.FirstOrDefault().DraftTicketId);

            if (checkWarehouseTransferBillProducts.Count() != request.DraftTicketProducts.Count() ||
                draftTicketProducts.DistinctBy(x => x.DraftTicketId).Count() > 1)
                validationErrors.Add(new ValidationResult(
                   ErrorMessages.WarehouseTransferBillProduct.Async,
                   new List<string> { "WarehouseTransferBillProducts", "" }));

            request.DraftTicketProducts.ForEach(x =>
            {
                if (x.ApproveQuantity <= 0)
                    validationErrors.Add(new ValidationResult(
                        ErrorMessages.WarehouseTransferBillProduct.AQLessZero,
                        new List<string> { $"{x.Id}", $"{x.ApproveQuantity}" }));
            });

            if (validationErrors.Any())
                throw new AbpValidationException(validationErrors);
        }

        private async Task ValidateDelete(Guid id)
        {
            var validationErrors = new List<ValidationResult>();

            var draftTicket = await _draftTicketRepository.FindAsync(x => x.Id == id);
            if (draftTicket == null)
                validationErrors.Add(new ValidationResult(
                           ErrorMessages.DraftTicket.Null,
                           new List<string> { "Id", $"{id}" }));

            var draftTicketProducts = await _draftTicketProductService.GetByDraftTicketId(id);
            if (!draftTicketProducts.Any())
                validationErrors.Add(new ValidationResult(
                           ErrorMessages.DraftTicketProduct.Null,
                           new List<string> { "DraftTicketId", $"{id}" }));

            if (validationErrors.Any())
                throw new AbpValidationException(validationErrors);
        }

        private async Task ValidateDeleteRange(List<Guid> ids)
        {
            var validationErrors = new List<ValidationResult>();

            foreach (var id in ids)
            {
                var draftTicket = await _draftTicketRepository.FindAsync(x => x.Id == id);
                if (draftTicket == null)
                    validationErrors.Add(new ValidationResult(
                               ErrorMessages.DraftTicket.Null,
                               new List<string> { "Id", $"{id}" }));

                if (draftTicket.Status != Enums.DraftTicket.Status.New && draftTicket.Status != Status.Cancel)
                    validationErrors.Add(new ValidationResult(
                               ErrorMessages.DraftTicket.NotDraft_CancelStatus,
                               new List<string> { "Id", $"{id}" }));

                var warehouseTransferBillProduct = await _draftTicketProductService.GetByDraftTicketId(id);
                if (!warehouseTransferBillProduct.Any())
                    validationErrors.Add(new ValidationResult(
                               ErrorMessages.DraftTicketProduct.Null,
                               new List<string> { "DraftTicketId", $"{id}" }));
            }

            if (validationErrors.Any())
                throw new AbpValidationException(validationErrors);
        }

        public async Task<byte[]> ExportDraftTicket(SearchDraftTicketRequest request)
        {
            var userStores = await _userStoreRepository.GetListAsync(p => p.UserId == _currentUser.Id);
            if (!userStores.Any())
            {
                throw new BusinessException(ErrorMessages.UserStore.StoreNotDefine);
            }
            var userStoreIds = userStores.Select(p => p.StoreId).ToList();
            var result = new List<DraftTicketDto>();
            var ids = new List<Guid>();
            if (!request.WarehousingBillCode.IsNullOrWhiteSpace())
            {
                var warehousingBills = (await _warehousingBillService.GetContainCodeAsync(request.WarehousingBillCode)).Where(x => x.SourceId != null).ToList();
                if (warehousingBills.Any())
                    foreach (var warehousingBill in warehousingBills)
                        if (warehousingBill.SourceId != null)
                            ids.Add(warehousingBill.SourceId ?? Guid.Empty);
            }
           

            var query = (await _draftTicketRepository.GetQueryableAsync())
                    .WhereIf(request.SourceStoreIds.Any(), x => request.SourceStoreIds.Any(s => s == x.SourceStoreId))
                    .WhereIf(request.DestinationStoreIds.Any(), x => request.DestinationStoreIds.Any(d => d == x.DestinationStoreId))
                    .WhereIf(!request.Code.IsNullOrWhiteSpace(), x => x.Code.Contains(request.Code))
                    .WhereIf(!request.WarehousingBillCode.IsNullOrWhiteSpace(), x => ids.Any(id => id == x.Id))
                    .Where(x => userStoreIds.Contains(x.SourceStoreId) || userStoreIds.Contains(x.DestinationStoreId))
                .ToList();

            var userIds = query.Select(x => x.CreatorId).ToList();
            userIds.AddRange(query.Select(x => x.DraftApprovedUserId));
            var listStoreIds = query.Select(x => x.SourceStoreId).ToList();
            listStoreIds.AddRange(query.Select(x => x.DestinationStoreId).ToList());
            var stores = await _storeRepository.GetListAsync(x => listStoreIds.Any(z => z == x.Id));
            var tranferBillIds = query.Select(x => x.Id).ToList();
            var tranferProduct = await _warehouseTransferBillProductRepository.GetListAsync(x => tranferBillIds.Contains(x.WarehouseTransferBillId));

            var users = (await _userRepository.GetListAsync())
                .Where(p => userIds.ToList().Contains(p.Id));

            var exportData = new List<ExportDraftTicketResponse>();
            foreach (var item in query)
            {
                var sourceStoreName = stores.FirstOrDefault(x => x.Id == item.SourceStoreId)?.Name;
                var destinationStoreName = stores.FirstOrDefault(x => x.Id == item.DestinationStoreId)?.Name;
                var tranferProductItem = tranferProduct.Where(x => x.WarehouseTransferBillId == item.Id);
                var creatorName = users.FirstOrDefault(x => x.Id == item.CreatorId)?.Name;
                var draftApprovedUserName = users.FirstOrDefault(x => x.Id == item.DraftApprovedUserId)?.Name;
                var draftConfirmUserName = users.FirstOrDefault(x => x.Id == item.DeliveryConfirmedUserId)?.Name;
                

                exportData.Add(new ExportDraftTicketResponse()
                {
                    Code = item.Code,
                    CreatedTime = item.CreationTime.ToString("dd-MM-yyyy hh: mm"),
                    ExportStoreName = sourceStoreName,
                    InputStoreName = destinationStoreName,
                    BillType = item.TransferBillType == TransferBillType.Import ? "Nhập chuyển kho" : "Xuất chuyển kho",
                    SP = tranferProductItem.Count(),
                    Quantity = tranferProductItem.Sum(x => x.Quantity),
                    CreatorName = creatorName,
                    DraftApprovedUserName = draftApprovedUserName,
                    DraftApprovedDate = item.CreationTime.ToString("dd-MM-yyyy hh:mm"),
                    DeliveryConfirmedUserName = draftConfirmUserName,
                    DeliveryConfirmedDate = item.DeliveryConfirmedDate.HasValue ? item.DeliveryConfirmedDate.Value.ToString("dd-MM-yyyy hh:mm") : "",
                    TransferStatus = item.Status == Status.New ? "Mới" : item.Status == Status.Approved ? "Đã duyệt" : item.Status == Status.Confirmed ? "Xác nhận" : item.Status == Status.Cancel ? "Đã hủy" : "",
                    Note = item.Note,
                });
            }
            return ExcelHelper.ExportExcel(exportData);
        }
    }
}