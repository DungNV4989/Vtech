using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
using VTECHERP.Domain.Shared.Helper.Excel.Model;
using VTECHERP.DTOs.Attachment;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.BillCustomers.Params;
using VTECHERP.DTOs.BillCustomers.Respons;
using VTECHERP.DTOs.DraftTickets;
using VTECHERP.DTOs.WarehouseTransferBillProducts;
using VTECHERP.DTOs.WarehouseTransferBills;
using VTECHERP.DTOs.WarehousingBills;
using VTECHERP.Entities;
using VTECHERP.Enums;
using VTECHERP.Enums.WarehouseTransferBill;
using DraftTicket = VTECHERP.Entities.DraftTicket;
using WarehouseTransferBill = VTECHERP.Entities.WarehouseTransferBill;
using WarehouseTransferBillProduct = VTECHERP.Entities.WarehouseTransferBillProduct;

namespace VTECHERP.Services
{
    public class WarehouseTransferBillService : IWarehouseTransferBillService
    {
        private readonly IRepository<WarehouseTransferBill> _warehouseTransferBillRepository;
        private readonly IRepository<WarehouseTransferBillProduct> _warehouseTransferBillProductRepository;
        private readonly IWarehouseTransferBillProductService _warehouseTransferBillProductService;
        private readonly IWarehousingBillService _warehousingBillService;
        private readonly IStoreService _storeService;
        private readonly IProductService _productService;
        private readonly IRepository<Stores> _storeRepository;
        private readonly IStoreProductService _storeProductService;
        private readonly IAttachmentService _attachmentService;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IClock _clock;
        private readonly IObjectMapper _objectMapper;
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Products> _productRepository;
        private readonly IRepository<WarehousingBill> _warehousingBillRepository;
        private readonly IRepository<StoreProduct> _storeProductRepository;
        private readonly IRepository<DraftTicket> _draftTicketRepository;
        private readonly IRepository<UserStore> _userStoreRepository;
        //private readonly IAttachmentRepository _attachmentRepository;

        public WarehouseTransferBillService(
            IRepository<WarehouseTransferBill> warehouseTransferBillRepository,
            IRepository<WarehousingBill> warehousingBillRepository,
            IRepository<WarehouseTransferBillProduct> warehouseTransferBillProductRepository,
            IWarehouseTransferBillProductService warehouseTransferBillProductService,
            IWarehousingBillService warehousingBillService,
            IStoreService storeService,
            IProductService productService,
            IStoreProductService storeProductService,
            IAttachmentService attachmentService,
            IIdentityUserRepository userRepository,
            IObjectMapper objectMapper,
            IRepository<Stores> storeRepository,
            // IAttachmentRepository attachmentRepository,
            ICurrentUser currentUser,
            IRepository<StoreProduct> storeProductRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IClock clock,
            IRepository<Products> productRepository,
            IRepository<DraftTicket> draftTicketRepository,
            IRepository<UserStore> userStoreRepository)
        {
            _warehouseTransferBillRepository = warehouseTransferBillRepository;
            _warehouseTransferBillProductRepository = warehouseTransferBillProductRepository;
            _warehouseTransferBillProductService = warehouseTransferBillProductService;
            _warehousingBillRepository = warehousingBillRepository;
            _warehousingBillService = warehousingBillService;
            _storeService = storeService;
            _productService = productService;
            _storeProductService = storeProductService;
            _attachmentService = attachmentService;
            _userRepository = userRepository;
            _objectMapper = objectMapper;
            _currentUser = currentUser;
            _unitOfWorkManager = unitOfWorkManager;
            _productRepository = productRepository;
            _clock = clock;
            _storeRepository = storeRepository;
            _draftTicketRepository = draftTicketRepository;
            //_attachmentRepository = attachmentRepository;
            _userStoreRepository = userStoreRepository;
            _storeProductRepository = storeProductRepository;
        }


        public async Task<WarehouseTransferBillDetaildDto> GetByIdAsync(Guid id)
        {
            var userStores = await _userStoreRepository.GetListAsync(p => p.UserId == _currentUser.Id);
            if (!userStores.Any())
            {
                throw new BusinessException(ErrorMessages.UserStore.StoreNotDefine);
            }
            var userStoreIds = userStores.Select(p => p.StoreId).ToList();
            var result = new WarehouseTransferBillDetaildDto();
            var warehouseTransferBill = await _warehouseTransferBillRepository.FindAsync(x => x.Id == id);
            if (warehouseTransferBill == null)
                throw new AbpValidationException($"{ErrorMessages.WarehouseTransferBill.NotExist} => Id : {id}");
            if (!userStoreIds.Contains(warehouseTransferBill.SourceStoreId) && !userStoreIds.Contains(warehouseTransferBill.DestinationStoreId))
            {
                throw new BusinessException(ErrorMessages.UserStore.StoreNotDefine);
            }
            result = _objectMapper.Map<WarehouseTransferBill, WarehouseTransferBillDetaildDto>(warehouseTransferBill);
            result.SourceStoreName = (await _storeService.GetByIdAsync(result.SourceStoreId)).Name;
            result.DestinationStoreName = (await _storeService.GetByIdAsync(result.DestinationStoreId)).Name;

            var attachments = await _attachmentService.GetByObjectId(id);
            result.Attachments = _objectMapper.Map<List<AttachmentDetailDto>, List<AttachmentShortDto>>(attachments);

            result.WarehouseTransferBillProducts = await _warehouseTransferBillProductService.GetByWarehouseTransferBillId(id);
            return result;
        }

        public async Task<PagingResponse<SearchWarehouseTransferResponse>> SearchWarehouseTransfer(SearchWarehouseTransferRequest request)
        {
            var userStores = await _userStoreRepository.GetListAsync(p => p.UserId == _currentUser.Id);
            if (!userStores.Any())
            {
                throw new BusinessException(ErrorMessages.UserStore.StoreNotDefine);
            }
            var userStoreIds = userStores.Select(p => p.StoreId).ToList();
            if (request.FromDate != null)
            {
                request.FromDate = _clock.Normalize(request.FromDate.Value);
            }
            if (request.ToDate != null)
            {
                request.ToDate = _clock.Normalize(request.ToDate.Value).AddDays(1);
            }

            var query = await _warehouseTransferBillRepository.GetQueryableAsync();
            query = query.Where(x => x.TenantId == _currentUser.TenantId)
                            .WhereIf(request.FromStoreIds != null && request.FromStoreIds.Any(), x => request.FromStoreIds.Any(z => z == x.SourceStoreId))
                         .WhereIf(request.ToStoreIds != null && request.ToStoreIds.Any(), x => request.ToStoreIds.Any(z => z == x.DestinationStoreId))
                         .WhereIf(!string.IsNullOrEmpty(request.WarehouseTransferCode), x => x.Code.Contains(request.WarehouseTransferCode))
                         .WhereIf(request.ToDate != null, x => x.TranferDate < request.ToDate)
                         .WhereIf(request.FromDate != null, x => x.TranferDate >= request.FromDate)
                         .WhereIf(request.TransferBillType != null, x => x.TransferBillType == request.TransferBillType)
                         .Where(p => userStoreIds.Contains(p.SourceStoreId) || userStoreIds.Contains(p.DestinationStoreId));

            var resPage = query.OrderByDescending(x => x.Code).Skip(request.Offset).Take(request.PageSize).ToList();

            var tranferBillIds = resPage.Select(x => x.Id).ToList();
            var userIds = resPage.Select(x => x.CreatorId).ToList();

            var listStoreIds = resPage.Select(x => x.SourceStoreId).ToList();
            listStoreIds.AddRange(resPage.Select(x => x.DestinationStoreId).ToList());

            var stores = await _storeRepository.GetListAsync(x => listStoreIds.Any(z => z == x.Id));

            var tranferProduct = await _warehouseTransferBillProductRepository.GetListAsync(x => tranferBillIds.Contains(x.WarehouseTransferBillId));

            var users = (await _userRepository.GetListAsync())
                .Where(p => userIds.ToList().Contains(p.Id));

            var listTransferBillIds = resPage.Select(x => x.Id).ToList();

            //var attachmentIds = _attachmentRepository.GetQueryableAsync().Result.Where(x => x.ObjectType == Enums.AttachmentObjectType.WarehouseTransferBill && tranferBillIds.Any(z => z == x.ObjectId)).Select(x => x.ObjectId).ToList();

            var wBills = new List<WarehousingBill>();
            if (listTransferBillIds != null && listTransferBillIds.Count > 0)
                wBills = _warehousingBillRepository.GetDbSetAsync().Result.AsNoTracking().AsEnumerable().Where(x => x.IsFromWarehouseTransfer != null && x.IsFromWarehouseTransfer.Value && listTransferBillIds.Any(z => z == x.SourceId)).ToList();

            var res = new List<SearchWarehouseTransferResponse>();
            var attachments = (await _attachmentService.ListAttachmentByObjectIdAsync(resPage.Select(x => x.Id).ToList())).OrderBy(x => x.CreationTime).ToList();
            foreach (var item in resPage)
            {
                var sourceStoreName = stores.FirstOrDefault(x => x.Id == item.SourceStoreId)?.Name;
                var destinationStoreName = stores.FirstOrDefault(x => x.Id == item.DestinationStoreId)?.Name;
                var tranferProductItem = tranferProduct.Where(x => x.WarehouseTransferBillId == item.Id);
                var creatorName = users.FirstOrDefault(x => x.Id == item.CreatorId)?.Name;
                var bills = wBills.Where(x => x.SourceId == item.Id).Select(x => new SearchWarehouseTransferResponse.WarehousingBill()
                {
                    BillType = x.BillType,
                    WarehousingBillCode = x.Code,
                    WarehousingBillId = x.Id
                });

                res.Add(new SearchWarehouseTransferResponse()
                {
                    Id = item.Id,
                    Note = item.Note,
                    InputStoreName = sourceStoreName,
                    ExportStoreName = destinationStoreName,
                    TransferBillType = item.TransferBillType,
                    WarehouseTransferCreatedTime = item.CreationTime,
                    WarehouseTransferCode = item.Code,
                    Sp = tranferProductItem.Count(),
                    Quantity = tranferProductItem.Sum(x => x.Quantity),
                    TotalMoney = tranferProductItem.Sum(x => (x.Quantity * x.CostPrice)),
                    CreatorName = creatorName,
                    WarehousingBills = bills.ToList(),
                    Attachments = attachments.Where(x => x.ObjectId == item.Id).ToList() ?? new List<DetailAttachmentDto>(),
                });
            }

            return new PagingResponse<SearchWarehouseTransferResponse>(query.Count(), res);
        }

        public async Task<PagingResponse<SearchWarehouseTransferMovingResponse>> SearchWarehouseTransferMoving(SearchWarehouseTransferMovingRequest request)
        {
            var userStores = await _userStoreRepository.GetListAsync(p => p.UserId == _currentUser.Id);
            if (!userStores.Any())
            {
                throw new BusinessException(ErrorMessages.UserStore.StoreNotDefine);
            }
            var userStoreIds = userStores.Select(p => p.StoreId).ToList();
            var warehouseBillIds = new List<Guid?>();
            if (!string.IsNullOrEmpty(request.WarehouseBillCode))
            {
                var bill = _warehousingBillRepository.GetQueryableAsync().Result.Where(x => (x.IsFromWarehouseTransfer != null && x.IsFromWarehouseTransfer == true) && x.Code.Contains(request.WarehouseBillCode)).Select(x => x.SourceId).ToList();
                warehouseBillIds = bill;
            }

            var query = await _warehouseTransferBillRepository.GetQueryableAsync();

            query = query
                         .WhereIf(request.FromStoreIds != null && request.FromStoreIds.Any(), x => request.FromStoreIds.Any(z => z == x.SourceStoreId))
                         .WhereIf(request.ToStoreIds != null && request.ToStoreIds.Any(), x => request.ToStoreIds.Any(z => z == x.DestinationStoreId))
                         .WhereIf(!string.IsNullOrEmpty(request.WarehouseTransferCode), x => x.Code.Contains(request.WarehouseTransferCode))
                         .WhereIf(!string.IsNullOrEmpty(request.WarehouseBillCode), x => warehouseBillIds.Any(z => z == x.Id))
                         .Where(x => x.TransferStatus == TransferStatuses.InDelivery)
                         .Where(x => userStoreIds.Contains(x.SourceStoreId));

            var resPage = query.OrderByDescending(x => x.Code).Skip(request.Offset).Take(request.PageSize).ToList();

            var tranferBillIds = resPage.Select(x => x.Id).ToList();
            var userIds = resPage.Select(x => x.CreatorId).ToList();
            userIds.AddRange(resPage.Select(x => x.DraftApprovedUserId));

            var listStoreIds = resPage.Select(x => x.SourceStoreId).ToList();
            listStoreIds.AddRange(resPage.Select(x => x.DestinationStoreId).ToList());

            var stores = await _storeRepository.GetListAsync(x => listStoreIds.Any(z => z == x.Id));

            var tranferProduct = await _warehouseTransferBillProductRepository.GetListAsync(x => tranferBillIds.Contains(x.WarehouseTransferBillId));

            var users = (await _userRepository.GetListAsync())
                .Where(p => userIds.ToList().Contains(p.Id));

            var listTransferBillIds = resPage.Select(x => x.Id).ToList();

            var wBills = new List<WarehousingBill>();
            if (listTransferBillIds != null && listTransferBillIds.Count > 0)
                wBills = _warehousingBillRepository.GetDbSetAsync().Result.AsNoTracking().AsEnumerable().Where(x => x.IsFromWarehouseTransfer != null && x.IsFromWarehouseTransfer.Value && listTransferBillIds.Any(z => z == x.SourceId)).ToList();

            var res = new List<SearchWarehouseTransferMovingResponse>();

            foreach (var item in resPage)
            {
                var sourceStoreName = stores.FirstOrDefault(x => x.Id == item.SourceStoreId)?.Name;
                var destinationStoreName = stores.FirstOrDefault(x => x.Id == item.DestinationStoreId)?.Name;
                var tranferProductItem = tranferProduct.Where(x => x.WarehouseTransferBillId == item.Id);
                var creatorName = users.FirstOrDefault(x => x.Id == item.CreatorId)?.Name;
                var draftApprovedUserName = users.FirstOrDefault(x => x.Id == item.DraftApprovedUserId)?.Name;
                var bill = wBills.FirstOrDefault(x => x.SourceId == item.Id);

                res.Add(new SearchWarehouseTransferMovingResponse()
                {
                    Id = item.Id,
                    Note = item.Note,
                    SourceStoreName = sourceStoreName,
                    DestinationStoreName = destinationStoreName,
                    TransferBillType = item.TransferBillType,
                    CreatedTime = item.CreationTime,
                    Code = item.Code,
                    Sp = tranferProductItem.Count(),
                    Quantity = tranferProductItem.Sum(x => x.Quantity),
                    IsDraftApproved = item.DraftApprovedUserId == null ? false : true,
                    CreatorName = creatorName,
                    DraftApprovedUserName = draftApprovedUserName,
                    DraftApprovedDate = item.DraftApprovedDate,
                    BillType = bill?.BillType,
                    WarehousingBillCode = bill?.Code,
                    WarehousingBillId = bill?.Id,
                });
            }

            return new PagingResponse<SearchWarehouseTransferMovingResponse>(query.Count(), res);
        }

        public async Task<PagingResponse<SearchWarehouseTransferComingResponse>> SearchWarehouseTransferComing(SearchWarehouseTransferComingRequest request)
        {
            var userStores = await _userStoreRepository.GetListAsync(p => p.UserId == _currentUser.Id);
            if (!userStores.Any())
            {
                throw new BusinessException(ErrorMessages.UserStore.StoreNotDefine);
            }
            var userStoreIds = userStores.Select(p => p.StoreId).ToList();
            var warehouseBillIds = new List<Guid?>();
            if (!string.IsNullOrEmpty(request.WarehouseBillCode))
            {
                var bill = _warehousingBillRepository.GetQueryableAsync().Result.Where(x => (x.IsFromWarehouseTransfer != null && x.IsFromWarehouseTransfer == true) && x.Code.Contains(request.WarehouseBillCode)).Select(x => x.SourceId).ToList();
                warehouseBillIds = bill;
            }

            var query = await _warehouseTransferBillRepository.GetQueryableAsync();

            query = query
                    .WhereIf(request.FromStoreIds != null && request.FromStoreIds.Any(), x => request.FromStoreIds.Any(z => z == x.SourceStoreId))
                    .WhereIf(request.ToStoreIds != null && request.ToStoreIds.Any(), x => request.ToStoreIds.Any(z => z == x.DestinationStoreId))
                    .WhereIf(!string.IsNullOrEmpty(request.WarehouseBillCode), x => warehouseBillIds.Any(z => z == x.Id))
                    .WhereIf(!string.IsNullOrEmpty(request.WarehouseTransferCode), x => x.Code.Contains(request.WarehouseTransferCode))
                    .Where(x => userStoreIds.Contains(x.DestinationStoreId));

            var resPage = query.OrderByDescending(x => x.Code).Skip(request.Offset).Take(request.PageSize).ToList();

            var tranferBillIds = resPage.Select(x => x.Id).ToList();
            var userIds = resPage.Select(x => x.CreatorId).ToList();
            userIds.AddRange(resPage.Select(x => x.DraftApprovedUserId));
            userIds.AddRange(resPage.Select(x => x.DeliveryConfirmedUserId));

            var listStoreIds = resPage.Select(x => x.SourceStoreId).ToList();
            listStoreIds.AddRange(resPage.Select(x => x.DestinationStoreId).ToList());

            var stores = await _storeRepository.GetListAsync(x => listStoreIds.Any(z => z == x.Id));

            var tranferProduct = await _warehouseTransferBillProductRepository.GetListAsync(x => tranferBillIds.Contains(x.WarehouseTransferBillId));

            var users = (await _userRepository.GetListAsync())
                .Where(p => userIds.Distinct().Where(x=>x.HasValue).ToList().Contains(p.Id));

            var res = new List<SearchWarehouseTransferComingResponse>();

            foreach (var item in resPage)
            {
                var sourceStore = stores.FirstOrDefault(x => x.Id == item.SourceStoreId);
                var sourceStoreName = sourceStore == null ? null : sourceStore.Name;

                var destinationStore = stores.FirstOrDefault(x => x.Id == item.DestinationStoreId);
                var destinationStoreName = destinationStore == null ? null : destinationStore.Name;

                var creator = users.FirstOrDefault(x => x.Id == item.CreatorId);
                var creatorName = creator == null ? null : creator.Name;

                var draftApprovedUser = users.FirstOrDefault(x => x.Id == (item.DraftApprovedUserId ?? Guid.Empty));
                var draftApprovedUserName = draftApprovedUser == null ? null : draftApprovedUser.Name;

                var deliveryConfirmedUser = users.FirstOrDefault(x => x.Id == (item.DeliveryConfirmedUserId ?? Guid.Empty));
                var deliveryConfirmedUserName = deliveryConfirmedUser == null ? null : deliveryConfirmedUser.Name;

                var tranferProductItem = tranferProduct.Where(x => x.WarehouseTransferBillId == item.Id);

                res.Add(new SearchWarehouseTransferComingResponse()
                {
                    Id = item.Id,
                    Note = item.Note,
                    SourceStoreName = sourceStoreName,
                    DestinationStoreName = destinationStoreName,
                    TransferBillType = item.TransferBillType,
                    CreatedTime = item.CreationTime,
                    Code = item.Code,
                    Sp = tranferProductItem.Count(),
                    Quantity = tranferProductItem.Sum(x => x.Quantity),
                    //TotalMoney = tranferProductItem.Sum(x => (x.Quantity * x.CostPrice)),
                    IsDraftApproved = item.DraftApprovedUserId == null ? false : true,
                    CreatorName = creatorName,
                    DraftApprovedUserName = draftApprovedUserName,
                    DraftApprovedDate = item.DraftApprovedDate,
                    DeliveryConfirmedUserName = deliveryConfirmedUserName,
                    DeliveryConfirmedDate = item.DeliveryConfirmedDate,
                });
            }

            return new PagingResponse<SearchWarehouseTransferComingResponse>(query.Count(), res);
        }

        public async Task<byte[]> ExportWarehouseTransferMoving(SearchWarehouseTransferMovingRequest request)
        {
            var userStores = await _userStoreRepository.GetListAsync(p => p.UserId == _currentUser.Id);
            if (!userStores.Any())
            {
                throw new BusinessException(ErrorMessages.UserStore.StoreNotDefine);
            }
            var userStoreIds = userStores.Select(p => p.StoreId).ToList();
            var query = await _warehouseTransferBillRepository.GetQueryableAsync();

            query = query
                         .WhereIf(request.FromStoreIds != null && request.FromStoreIds.Any(), x => request.FromStoreIds.Any(z => z == x.SourceStoreId))
                         .WhereIf(request.ToStoreIds != null && request.ToStoreIds.Any(), x => request.ToStoreIds.Any(z => z == x.DestinationStoreId))
                         .WhereIf(!string.IsNullOrEmpty(request.WarehouseTransferCode), x => x.Code.Contains(request.WarehouseTransferCode))
                         .Where(x => x.TransferStatus == TransferStatuses.InDelivery)
                         .Where(x => userStoreIds.Contains(x.SourceStoreId));

            var resPage = query.OrderByDescending(x => x.Code).ToList();

            var tranferBillIds = resPage.Select(x => x.Id).ToList();
            var userIds = resPage.Select(x => x.CreatorId).ToList();
            userIds.AddRange(resPage.Select(x => x.DraftApprovedUserId));

            var listStoreIds = resPage.Select(x => x.SourceStoreId).ToList();
            listStoreIds.AddRange(resPage.Select(x => x.DestinationStoreId).ToList());

            var stores = await _storeRepository.GetListAsync(x => listStoreIds.Any(z => z == x.Id));

            var tranferProduct = await _warehouseTransferBillProductRepository.GetListAsync(x => tranferBillIds.Contains(x.WarehouseTransferBillId));

            var users = (await _userRepository.GetListAsync())
                .Where(p => userIds.ToList().Contains(p.Id));

            var exportData = new List<ExportWarehouseTransferMovingResponse>();

            foreach (var item in resPage)
            {
                var sourceStoreName = stores.FirstOrDefault(x => x.Id == item.SourceStoreId)?.Name;
                var destinationStoreName = stores.FirstOrDefault(x => x.Id == item.DestinationStoreId)?.Name;
                var tranferProductItem = tranferProduct.Where(x => x.WarehouseTransferBillId == item.Id);
                var creatorName = users.FirstOrDefault(x => x.Id == item.CreatorId)?.Name;
                var draftApprovedUserName = users.FirstOrDefault(x => x.Id == item.DraftApprovedUserId)?.Name;

                exportData.Add(new ExportWarehouseTransferMovingResponse()
                {
                    Code = item.Code,
                    CreatedTime = item.CreationTime.ToString("dd-MM-yyyy"),
                    SourceStoreName = sourceStoreName,
                    DestinationStoreName = destinationStoreName,
                    BillType = item.TransferBillType == TransferBillType.Import ? "Nhập chuyển kho" : "Xuất chuyển kho",
                    Sp = tranferProductItem.Count(),
                    Quantity = tranferProductItem.Sum(x => x.Quantity),
                    CreatorName = creatorName,
                    DraftApprovedUserName = draftApprovedUserName,
                    DraftApprovedTime = item.DraftApprovedDate.HasValue ? item.DraftApprovedDate.Value.ToString("dd-MM-yyyy") : "",
                    Note = item.Note,
                });
            }

            return ExcelHelper.ExportExcel(exportData);
        }
        public async Task<byte[]> ExportWarehouseTransferComing(SearchWarehouseTransferComingRequest request)
        {
            var userStores = await _userStoreRepository.GetListAsync(p => p.UserId == _currentUser.Id);
            if (!userStores.Any())
            {
                throw new BusinessException(ErrorMessages.UserStore.StoreNotDefine);
            }
            var userStoreIds = userStores.Select(p => p.StoreId).ToList();
            var query = await _warehouseTransferBillRepository.GetQueryableAsync();

            query = query
                         .WhereIf(request.FromStoreIds != null && request.FromStoreIds.Any(), x => request.FromStoreIds.Any(z => z == x.SourceStoreId))
                         .WhereIf(request.ToStoreIds != null && request.ToStoreIds.Any(), x => request.ToStoreIds.Any(z => z == x.DestinationStoreId))
                         .WhereIf(!string.IsNullOrEmpty(request.WarehouseTransferCode), x => x.Code.Contains(request.WarehouseTransferCode))
                         .Where(x => x.TransferStatus == TransferStatuses.InDelivery)
                         .Where(x => userStoreIds.Contains(x.DestinationStoreId))
                ;

            var resPage = query.OrderByDescending(x => x.Code).ToList();

            var tranferBillIds = resPage.Select(x => x.Id).ToList();
            var userIds = resPage.Select(x => x.CreatorId).ToList();
            userIds.AddRange(resPage.Select(x => x.DraftApprovedUserId));

            var listStoreIds = resPage.Select(x => x.SourceStoreId).ToList();
            listStoreIds.AddRange(resPage.Select(x => x.DestinationStoreId).ToList());

            var stores = await _storeRepository.GetListAsync(x => listStoreIds.Any(z => z == x.Id));

            var tranferProduct = await _warehouseTransferBillProductRepository.GetListAsync(x => tranferBillIds.Contains(x.WarehouseTransferBillId));

            var users = (await _userRepository.GetListAsync())
                .Where(p => userIds.ToList().Contains(p.Id));

            var exportData = new List<ExportWarehouseTransferComingResponse>();

            foreach (var item in resPage)
            {
                var sourceStoreName = stores.FirstOrDefault(x => x.Id == item.SourceStoreId)?.Name;
                var destinationStoreName = stores.FirstOrDefault(x => x.Id == item.DestinationStoreId)?.Name;
                var tranferProductItem = tranferProduct.Where(x => x.WarehouseTransferBillId == item.Id);
                var creatorName = users.FirstOrDefault(x => x.Id == item.CreatorId)?.Name;
                var draftApprovedUserName = users.FirstOrDefault(x => x.Id == item.DraftApprovedUserId)?.Name;
                var deliveryConfirmedUserName = users.FirstOrDefault(x => x.Id == item.DeliveryConfirmedUserId)?.Name;

                exportData.Add(new ExportWarehouseTransferComingResponse()
                {
                    Code = item.Code,
                    CreatedTime = item.CreationTime.ToString("dd-MM-yyyy hh:mm"),
                    SourceStoreInfo = sourceStoreName,
                    DestinationStoreInfo = destinationStoreName,
                    BillType = item.TransferBillType == TransferBillType.Import ? "Nhập chuyển kho" : "Xuất chuyển kho",
                    SP = tranferProductItem.Count(),
                    Quantity = tranferProductItem.Sum(x => x.Quantity),
                    CreatorName = creatorName,
                    DraftApprovedUserName = draftApprovedUserName,
                    DraftApprovedTime = item.DraftApprovedDate.HasValue ? item.DraftApprovedDate.Value.ToString("dd-MM-yyyy hh:mm") : "",
                    DeliveryConfirmedUserName = deliveryConfirmedUserName,
                    DeliveryConfirmedTime = item.DeliveryConfirmedDate.HasValue ? item.DeliveryConfirmedDate.Value.ToString("dd-MM-yyyy hh:mm") : "",
                    Note = item.Note,
                });
            }

            return ExcelHelper.ExportExcel(exportData);
        }

        public async Task<byte[]> ExportWarehouseTransfer(SearchWarehouseTransferRequest request)
        {
            var userStores = await _userStoreRepository.GetListAsync(p => p.UserId == _currentUser.Id);
            if (!userStores.Any())
            {
                throw new BusinessException(ErrorMessages.UserStore.StoreNotDefine);
            }
            var userStoreIds = userStores.Select(p => p.StoreId).ToList();
            var query = await _warehouseTransferBillRepository.GetQueryableAsync();

            query = query.WhereIf(request.FromStoreIds != null && request.FromStoreIds.Any(), x => request.FromStoreIds.Any(z => z == x.SourceStoreId))
                         .WhereIf(request.ToStoreIds != null && request.ToStoreIds.Any(), x => request.ToStoreIds.Any(z => z == x.DestinationStoreId))
                         .WhereIf(!string.IsNullOrEmpty(request.WarehouseTransferCode), x => x.Code.Contains(request.WarehouseTransferCode))
                         .WhereIf(request.ToDate != null, x => x.TranferDate < request.ToDate)
                         .WhereIf(request.FromDate != null, x => x.TranferDate >= request.FromDate)
                         .WhereIf(request.TransferBillType != null, x => x.TransferBillType == request.TransferBillType)
                         .Where(p => userStoreIds.Contains(p.SourceStoreId) || userStoreIds.Contains(p.DestinationStoreId));

            var resPage = query.OrderByDescending(x => x.Code).ToList();

            var tranferBillIds = resPage.Select(x => x.Id).ToList();
            var userIds = resPage.Select(x => x.CreatorId).ToList();
            userIds.AddRange(resPage.Select(x => x.DraftApprovedUserId));

            var listStoreIds = resPage.Select(x => x.SourceStoreId).ToList();
            listStoreIds.AddRange(resPage.Select(x => x.DestinationStoreId).ToList());

            var stores = await _storeRepository.GetListAsync(x => listStoreIds.Any(z => z == x.Id));

            var tranferProduct = await _warehouseTransferBillProductRepository.GetListAsync(x => tranferBillIds.Contains(x.WarehouseTransferBillId));

            var users = (await _userRepository.GetListAsync())
                .Where(p => userIds.ToList().Contains(p.Id));

            var exportData = new List<ExportWarehouseTransferResponse>();

            foreach (var item in resPage)
            {
                var sourceStoreName = stores.FirstOrDefault(x => x.Id == item.SourceStoreId)?.Name;
                var destinationStoreName = stores.FirstOrDefault(x => x.Id == item.DestinationStoreId)?.Name;
                var tranferProductItem = tranferProduct.Where(x => x.WarehouseTransferBillId == item.Id);
                var creatorName = users.FirstOrDefault(x => x.Id == item.CreatorId)?.Name;
                var draftApprovedUserName = users.FirstOrDefault(x => x.Id == item.DraftApprovedUserId)?.Name;
                var deliveryConfirmedUserName = users.FirstOrDefault(x => x.Id == item.DeliveryConfirmedUserId)?.Name;

                exportData.Add(new ExportWarehouseTransferResponse()
                {
                    Code = item.Code,
                    CreatedTime = item.CreationTime.ToString("dd-MM-yyyy"),
                    ExportStoreName = sourceStoreName,
                    InputStoreName = destinationStoreName,
                    BillType = item.TransferBillType == TransferBillType.Import ? "Nhập chuyển kho" : "Xuất chuyển kho",
                    SP = tranferProductItem.Count(),
                    Quantity = tranferProductItem.Sum(x => x.Quantity),
                    TotalMoney = tranferProductItem.Sum(x => (x.Quantity * x.CostPrice)),
                    Discount = 0,
                    CreatorName = creatorName,
                    Note = item.Note,
                });
            }

            return ExcelHelper.ExportExcel(exportData);
        }

        /// <summary>
        /// Xác nhận phiếu xnk
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task ConfirmAsync(WarehouseTransferBillConfirmRequest request)
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var warehouseTransferBill = await _warehouseTransferBillRepository.FindAsync(x => x.Id == request.Id);

                var warehouseTransferBillProducts = await _warehouseTransferBillProductRepository.GetListAsync(x => x.WarehouseTransferBillId == request.Id);

                ValidateConfirmTransferBill(warehouseTransferBill, warehouseTransferBillProducts, request);

                warehouseTransferBill.TransferStatus = TransferStatuses.DeliveryCompleted;
                warehouseTransferBill.DeliveryConfirmedUserId = _currentUser.Id;
                warehouseTransferBill.DeliveryConfirmedDate = _clock.Now;
                warehouseTransferBill.TransferBillType = TransferBillType.Import;

                await _warehouseTransferBillRepository.UpdateAsync(warehouseTransferBill);

                foreach (var item in warehouseTransferBillProducts)
                {
                    item.ConfirmQuatity = request.WarehouseTransferBillProducts.FirstOrDefault(x => x.Id == item.Id).ConfirmQuantity;
                }

                await _warehouseTransferBillProductRepository.UpdateManyAsync(warehouseTransferBillProducts);

                var warehousebill = new CreateWarehousingBillRequest()
                {
                    BillType = Enums.WarehousingBillType.Import,
                    StoreId = warehouseTransferBill.DestinationStoreId,
                    DocumentDetailType = Enums.DocumentDetailType.ImportTransfer,
                    IsFromWarehouseTransfer = true,
                    AudienceType = Enums.AudienceTypes.Other,
                    Products = new List<WarehousingBillProductRequest>(),
                    SourceId = warehouseTransferBill.Id
                };

                var listProductIds = warehouseTransferBillProducts.Select(x => x.ProductId).ToList();
                var listProduct = await _productRepository.GetListAsync(x => listProductIds.Any(z => z == x.Id));
                var storeProducts = await _storeProductService.GetByStoreId(warehouseTransferBill.SourceStoreId);

                foreach (var item in request.WarehouseTransferBillProducts)
                {
                    var billProduct = warehouseTransferBillProducts.FirstOrDefault(p => p.Id == item.Id);
                    var product = listProduct.Find(x => x.Id == billProduct.ProductId);
                    var storeProduct = storeProducts.Find(p => p.ProductId == billProduct.ProductId);
                    var productInBill = new WarehousingBillProductRequest()
                    {
                        ProductId = product.Id,
                        Unit = product.Unit,
                        Quantity = item.ConfirmQuantity,
                        Price = storeProduct.StockPrice,
                    };
                    warehousebill.Products.Add(productInBill);
                }

                await _warehousingBillService.CreateBill(warehousebill);

                if (warehouseTransferBill.DraftTicketId != null)
                {
                    //Th phiếu chuyển kho tạo từ phiếu nháp, update lại trạng thái phiếu nháp
                    var draft = await _draftTicketRepository.GetAsync(x => x.Id == warehouseTransferBill.DraftTicketId);
                    draft.Status = Enums.DraftTicket.Status.Confirmed;
                    draft.DeliveryConfirmedDate = _clock.Now;
                    draft.DeliveryConfirmedUserId = _currentUser.Id;
                    draft.TransferBillType = TransferBillType.Import;

                    await _draftTicketRepository.UpdateAsync(draft);
                }

                await uow.SaveChangesAsync();
                await uow.CompleteAsync();
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                throw;
            }
        }

        private void ValidateConfirmTransferBill(WarehouseTransferBill warehouseTransferBill, List<WarehouseTransferBillProduct> warehouseTransferBillProducts, WarehouseTransferBillConfirmRequest request)
        {
            var validationErrors = new List<ValidationResult>();

            if (warehouseTransferBill.TransferStatus != TransferStatuses.InDelivery)
                validationErrors.Add(new ValidationResult(
                    ErrorMessages.WarehouseTransferBill.CanNotConfirmNotInDelivery,
                    new List<string> { "TransferStatus" }));

            warehouseTransferBillProducts.ForEach(x =>
            {
                var warehouseTransferBillProduct = request.WarehouseTransferBillProducts.FirstOrDefault(z => z.Id == x.Id);

                if (warehouseTransferBillProduct.ConfirmQuantity != x.Quantity)
                    validationErrors.Add(new ValidationResult(
                       ErrorMessages.WarehouseTransferBillProduct.SQOtherAQConfirm,
                       new List<string> { "StockQuantity != ConfirmQuantity", $"{x.Quantity} < {warehouseTransferBillProduct.ConfirmQuantity}" }));
            });

            request.WarehouseTransferBillProducts.ForEach(x =>
            {
                if (x.ConfirmQuantity <= 0)
                    validationErrors.Add(new ValidationResult(
                        ErrorMessages.WarehouseTransferBillProduct.AQLessZero,
                        new List<string> { $"{x.Id}", $"{x.ConfirmQuantity}" }));
            });

            if (validationErrors.Any())
                throw new AbpValidationException(validationErrors);
        }

        public async Task DeleteMoving(Guid id)
        {

            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var warehouseTransferBill = await _warehouseTransferBillRepository.FindAsync(x => x.Id == id);
                if (warehouseTransferBill == null)
                    throw new BusinessException($"{ErrorMessages.WarehouseTransferBill.NotExist} => Id : {id}");
                await _warehouseTransferBillRepository.DeleteAsync(warehouseTransferBill);

                var transferBillProduct = await _warehouseTransferBillProductRepository.GetListAsync(x => x.WarehouseTransferBillId == id);

                await _warehouseTransferBillProductRepository.DeleteManyAsync(transferBillProduct);


                await uow.SaveChangesAsync();
                await uow.CompleteAsync();
            }
            catch (Exception)
            {
                await uow.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteComing(Guid id)
        {

            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var warehouseTransferBill = await _warehouseTransferBillRepository.FindAsync(x => x.Id == id);
                if (warehouseTransferBill == null)
                    throw new BusinessException($"{ErrorMessages.WarehouseTransferBill.NotExist} => Id : {id}");

                var transferBillProduct = await _warehouseTransferBillProductRepository.GetListAsync(x => x.WarehouseTransferBillId == id);

                await _warehouseTransferBillProductRepository.DeleteManyAsync(transferBillProduct);


                await uow.SaveChangesAsync();
                await uow.CompleteAsync();
            }
            catch (Exception)
            {
                await uow.RollbackAsync();
                throw;
            }
        }

        public async Task<Guid> DeleteAsync(Guid id)
        {
            await ValidateDelete(id);
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var warehouseTransferBill = await _warehouseTransferBillRepository.FindAsync(x => x.Id == id);

                warehouseTransferBill.IsDeleted = true;
                warehouseTransferBill.DeleterId = _currentUser.Id;

                await _warehouseTransferBillRepository.UpdateAsync(warehouseTransferBill);
                await uow.SaveChangesAsync();

                await _warehouseTransferBillProductService.DeleteRangeAsync(warehouseTransferBill.Id);
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
                    var warehouseTransferBill = await _warehouseTransferBillRepository.FindAsync(x => x.Id == id);

                    warehouseTransferBill.IsDeleted = true;
                    warehouseTransferBill.DeleterId = _currentUser.Id;

                    await _warehouseTransferBillRepository.UpdateAsync(warehouseTransferBill);
                    await uow.SaveChangesAsync();

                    await _warehouseTransferBillProductService.DeleteRangeAsync(warehouseTransferBill.Id);
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

        private async Task ValidateAdd(WarehouseTransferBillCreateRequest request)
        {
            var validationErrors = new List<ValidationResult>();

            if (request.DestinationStoreId == request.SourceStoreId)
                validationErrors.Add(new ValidationResult(ErrorMessages.WarehouseTransferBill.NotDuplicate));

            var storeExistSourceStoreId = await _storeService.Exist(request.SourceStoreId);
            if (!storeExistSourceStoreId)
                validationErrors.Add(new ValidationResult($"{ErrorMessages.WarehouseTransferBill.SourceStore} {ErrorMessages.WarehouseTransferBill.NotExistVN}"));

            var storeExistDestinationStoreId = await _storeService.Exist(request.DestinationStoreId);
            if (!storeExistDestinationStoreId)
                validationErrors.Add(new ValidationResult($"{ErrorMessages.WarehouseTransferBill.DestinationStore} {ErrorMessages.WarehouseTransferBill.NotExistVN}"));

            if (!request.WarehouseTransferBillProducts.Any())
                validationErrors.Add(new ValidationResult(ErrorMessages.WarehouseTransferBillProduct.NullVN));
            else
            {
                var productDuplicates = request.WarehouseTransferBillProducts.GroupBy(x => x.ProductId).Where(x => x.Count() > 1).Select(x => x.Key);
                if (productDuplicates.Any())
                    validationErrors.Add(new ValidationResult(ErrorMessages.Product.DuplicateVN));

                var storeProducts = await _productService.GetProductByStordId(request.SourceStoreId);
                foreach (var item in request.WarehouseTransferBillProducts)
                {
                    var productExist = await _productService.Exist(item.ProductId);
                    if (!productExist)
                        validationErrors.Add(new ValidationResult(ErrorMessages.Product.NotExistVN));
                    else
                    {
                        var storeProduct = storeProducts.FirstOrDefault(x => x.Id == item.ProductId);
                        if (storeProduct.StockQuantity < item.Quantity)
                            validationErrors.Add(new ValidationResult($"{ErrorMessages.WarehouseTransferBill.SQLessQVN} ({storeProduct.StockQuantity} < {item.Quantity})" ));
                    }

                    if (item.Quantity <= 0)
                        validationErrors.Add(new ValidationResult($"{ErrorMessages.WarehouseTransferBill.NumericLessThanZero} ({item.Quantity})"));
                }
            }

            if (validationErrors.Any())
                throw new AbpValidationException(validationErrors);
        }

        private async Task ValidateDelete(Guid id)
        {
            var validationErrors = new List<ValidationResult>();

            var warehouseTransferBill = await _warehouseTransferBillRepository.FindAsync(x => x.Id == id);
            if (warehouseTransferBill == null)
                validationErrors.Add(new ValidationResult(
                           ErrorMessages.WarehouseTransferBill.Null,
                           new List<string> { "Id", $"{id}" }));

            var warehouseTransferBillProduct = await _warehouseTransferBillProductService.GetByWarehouseTransferBillId(id);
            if (!warehouseTransferBillProduct.Any())
                validationErrors.Add(new ValidationResult(
                           ErrorMessages.WarehouseTransferBillProduct.Null,
                           new List<string> { "WarehouseTransferBillId", $"{id}" }));

            if (validationErrors.Any())
                throw new AbpValidationException(validationErrors);
        }

        private async Task ValidateDeleteRange(List<Guid> ids)
        {
            var validationErrors = new List<ValidationResult>();

            foreach (var id in ids)
            {
                var warehouseTransferBill = await _warehouseTransferBillRepository.FindAsync(x => x.Id == id);
                if (warehouseTransferBill == null)
                    validationErrors.Add(new ValidationResult(
                               ErrorMessages.WarehouseTransferBill.Null,
                               new List<string> { "Id", $"{id}" }));


                var warehouseTransferBillProduct = await _warehouseTransferBillProductService.GetByWarehouseTransferBillId(id);
                if (!warehouseTransferBillProduct.Any())
                    validationErrors.Add(new ValidationResult(
                               ErrorMessages.WarehouseTransferBillProduct.Null,
                               new List<string> { "WarehouseTransferBillId", $"{id}" }));
            }

            if (validationErrors.Any())
                throw new AbpValidationException(validationErrors);
        }

        /// <summary>
        /// Tạo mới phiếu chuyển kho
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task<Guid> AddWarehouseTransferBillAsync(WarehouseTransferBillCreateRequest request)
        {
            await ValidateAdd(request);
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false);
            try
            {
                var warehouseTransferBill = _objectMapper.Map<WarehouseTransferBillCreateRequest, WarehouseTransferBill>(request);

                warehouseTransferBill.CreatedFrom = WarehouseTransferBillCreatedFrom.Transfer;
                warehouseTransferBill.TransferStatus = TransferStatuses.InDelivery;
                warehouseTransferBill.DraftApprovedUserId = _currentUser.Id;
                warehouseTransferBill.DraftApprovedDate = _clock.Now;
                warehouseTransferBill.TranferDate = _clock.Now;
                warehouseTransferBill.TransferBillType = TransferBillType.Export;
                warehouseTransferBill.CreatorId = _currentUser.Id;

                await _warehouseTransferBillRepository.InsertAsync(warehouseTransferBill);
                await uow.SaveChangesAsync();

                var listProductIds = request.WarehouseTransferBillProducts.Select(x => x.ProductId).ToList();
                var listProduct = await _productRepository.GetListAsync(x => listProductIds.Any(z => z == x.Id));
                var storeProducts = await _storeProductService.GetByStoreId(warehouseTransferBill.SourceStoreId);

                var warehouseTransferBillProducts = _objectMapper.Map<List<WarehouseTransferBillProductCreateRequest>, List<WarehouseTransferBillProduct>>(request.WarehouseTransferBillProducts.ToList());

                warehouseTransferBillProducts.ForEach(x =>
                {
                    var canTransfer = request.WarehouseTransferBillProducts.FirstOrDefault(p => p.ProductId == x.ProductId)?.CanTransfer;
                    x.WarehouseTransferBillId = warehouseTransferBill.Id;
                    x.CanTransfer = canTransfer ?? 0;
                    x.CreatorId = _currentUser.Id;
                    x.CostPrice = listProduct.FirstOrDefault(z => z.Id == x.ProductId)?.StockPrice ?? 0;
                });

                await _warehouseTransferBillProductRepository.InsertManyAsync(warehouseTransferBillProducts);

                await uow.SaveChangesAsync();

                var warehousebill = new CreateWarehousingBillRequest()
                {
                    BillType = Enums.WarehousingBillType.Export,
                    StoreId = request.SourceStoreId,
                    DocumentDetailType = Enums.DocumentDetailType.ExportTransfer,
                    IsFromWarehouseTransfer = true,
                    AudienceType = Enums.AudienceTypes.Other,
                    Products = new List<WarehousingBillProductRequest>(),
                    SourceId = warehouseTransferBill.Id
                };


                foreach (var item in request.WarehouseTransferBillProducts)
                {
                    var product = listProduct.Find(x => x.Id == item.ProductId);
                    var storeProduct = storeProducts.FirstOrDefault(x => x.ProductId == item.ProductId);
                    var productInBill = new WarehousingBillProductRequest()
                    {
                        ProductId = item.ProductId,
                        Unit = product.Unit,
                        Quantity = item.Quantity,
                        Price = storeProduct?.StockPrice ?? 0,
                    };
                    warehousebill.Products.Add(productInBill);
                }

                await _warehousingBillService.CreateBill(warehousebill);
                await uow.SaveChangesAsync();
                await uow.CompleteAsync();
                return warehouseTransferBill.Id;
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
        }

        /// <summary>
        /// Xóa phiếu chuyển kho
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Guid> DeleteWarehouseTransferBill(Guid id, bool isManualDelete)
        {
            using var uow = _unitOfWorkManager.Begin(isTransactional: true, requiresNew: true);
            try
            {
                var bill = await _warehouseTransferBillRepository.GetAsync(p => p.Id == id && !p.IsDeleted);
                var billProducts = await _warehouseTransferBillProductRepository.GetListAsync(p => p.WarehouseTransferBillId == id && !p.IsDeleted);

                if (bill != null)
                {
                    await _warehouseTransferBillRepository.DeleteAsync(bill);

                }
                if (billProducts.Any())
                {
                    await _warehouseTransferBillProductRepository.DeleteManyAsync(billProducts);
                }

                // trong trường hợp xóa thủ công phiếu chuyển kho trước, tự động xóa tất cả phiếu xuất nhập
                // bỏ qua trong trường hợp xóa tự động phiếu chuyển kho sau khi xóa thủ công phiếu xuất/nhập
                if (isManualDelete)
                {
                    await _warehousingBillService.AutoDeleteBillByWarehouseTransferBill(bill.Id);
                }

                await uow.SaveChangesAsync();
                await uow.CompleteAsync();

                return bill.Id;
            }
            catch
            {
                await uow.RollbackAsync();
                throw;
            }
        }

        public async Task<Guid> AcceptWarehouseTransferBill(Guid id)
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var warehouseTransferBill = await _warehouseTransferBillRepository.GetAsync(p => p.Id == id);

                warehouseTransferBill.TransferStatus = TransferStatuses.DeliveryCompleted;
                warehouseTransferBill.DeliveryConfirmedDate = _clock.Now;
                warehouseTransferBill.DeliveryConfirmedUserId = _currentUser.Id;

                await _warehouseTransferBillRepository.UpdateAsync(warehouseTransferBill);
                await uow.SaveChangesAsync();

                var warehouseTransferBillProducts = await _warehouseTransferBillProductService.GetByWarehouseTransferBillId(id);

                var warehousebill = new CreateWarehousingBillRequest()
                {
                    BillType = Enums.WarehousingBillType.Import,
                    StoreId = warehouseTransferBill.DestinationStoreId,
                    DocumentDetailType = Enums.DocumentDetailType.ImportTransfer,
                    IsFromWarehouseTransfer = true,
                    AudienceType = Enums.AudienceTypes.Other,
                    Products = new List<WarehousingBillProductRequest>(),
                    SourceId = warehouseTransferBill.Id
                };
                var listProduct = await _productRepository.GetListAsync();
                // phiếu XNK tại nơi xuất
                var exportBill = await _warehousingBillService.GetByWarehouseTransferBillId(id, Enums.WarehousingBillType.Export);

                foreach (var item in warehouseTransferBillProducts)
                {
                    var product = listProduct.Find(x => x.Id == item.ProductId);
                    var billProduct = exportBill.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
                    var productInBill = new WarehousingBillProductRequest()
                    {
                        ProductId = item.ProductId,
                        Unit = product.Unit,
                        Quantity = item.Quantity,
                        Price = billProduct.CurrentStockPrice,
                    };
                    warehousebill.Products.Add(productInBill);
                }

                await _warehousingBillService.CreateBill(warehousebill);

                await uow.SaveChangesAsync();
                await uow.CompleteAsync();

                return warehouseTransferBill.Id;
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
        }

        public async Task<byte[]> DownloadTemplateImport()
        {
            var workbook = new CustomWorkBook();
            var sheet = RenderTemplateImport();
            workbook.Sheets.Add(sheet);

            return ExcelHelper.ExportExcel(workbook);
        }

        private CustomSheet RenderTemplateImport()
        {
            var sheet = new CustomSheet("Sheet 1");

            var startRow = 1;

            var header = new CustomDataTable()
            {
                StartRowIndex = startRow,
                StartColumnIndex = 1,
                RowDirection = Directions.Horizontal,
                Rows = new List<DataRow>
                    {
                        new DataRow(
                            new HeaderCell("Sản phẩm"),
                            new HeaderCell("Số lượng"),
                            new HeaderCell("Ghi chú")
                            )
                    }
            };

            sheet.Tables.Add(header);

            return sheet;
        }

        public async Task<(string message, bool success, Guid? data, byte[] fileRespon)> ImportBillCustomer(WarehouseTransferImportParam param)
        {
            var workbook = new CustomWorkBook();
            var listData = await MappingDataImportBillCustomerProduct(param.File, param.SourceStoreId);

            var dataToInsert = listData.Where(x => x.Success)
                .Select(x => new WarehouseTransferBillProductCreateRequest
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                })
                .ToList();

            if (!dataToInsert.Any())
                return ("Danh sách sản phẩm không hợp lệ", false, null, null);

            var paramCreate = new WarehouseTransferBillCreateRequest
            {
                SourceStoreId = param.SourceStoreId,
                DestinationStoreId = param.DestinationStoreId,
                Note = param.Note,
                WarehouseTransferBillProducts = dataToInsert
            };

            var responCreate = await AddWarehouseTransferBillAsync(paramCreate);

            var sheet = RenderTemplateAfterImport(listData);
            workbook.Sheets.Add(sheet);

            var fileReturn = ExcelHelper.ExportExcel(workbook);
            return ("Import thành công", true, responCreate, fileReturn);
        }

        private async Task<List<WarehouseTransferBillProductImportRespon>> MappingDataImportBillCustomerProduct(IFormFile file, Guid StoreId)
        {
            var result = new List<WarehouseTransferBillProductImportRespon>();

            using (var workbook = new XLWorkbook(file.OpenReadStream()))
            {
                var worksheet = workbook.Worksheet(1); // Chỉ định số trang tính trong tệp Excel (index bắt đầu từ 1)

                var firstRowUsed = worksheet.FirstRowUsed();
                var headers = firstRowUsed.CellsUsed()
                    .Select(c => c.Value.ToString().Trim())
                    .ToList();

                var products = await _productRepository.GetListAsync();
                var storeProduct = await _storeProductRepository.GetListAsync();
                Products objOrigin = null;

                foreach (var row in worksheet.RowsUsed().Skip(1)) // Bỏ qua hàng tiêu đề (hàng đầu tiên)
                {
                    var obj = new WarehouseTransferBillProductImportRespon();

                    for (int i = 0; i < headers.Count; i++)
                    {
                        var cellValue = row.Cell(i + 1).Value == null ? "" : row.Cell(i + 1).Value.ToString().Trim();

                        // Gán giá trị cho thuộc tính tương ứng trong đối tượng
                        if (headers[i] == "Sản phẩm")
                        {
                            obj.ColExcel1 = cellValue;
                            if (string.IsNullOrEmpty(cellValue))
                            {
                                obj.Success = false;
                                obj.Message += "Thông tin sản phẩm trống,";
                            }

                            objOrigin = products.FirstOrDefault(x => x.Name == cellValue || x.BarCode == cellValue || x.Code == cellValue);

                            if (objOrigin == null)
                            {
                                obj.Success = false;
                                obj.Message += "Không tìm thấy thông tin sản phẩm,";
                            }
                            else
                            {
                                obj.ProductId = objOrigin.Id;
                            }
                        }
                        else if (headers[i] == "Số lượng")
                        {
                            obj.ColExcel2 = cellValue;

                            if (obj.Success)
                            {
                                int quantityParse = 0;
                                if (!int.TryParse(cellValue, out quantityParse))
                                {
                                    obj.Success = false;
                                    obj.Message += "Dữ liệu số lượng không hợp lệ,";
                                }

                                var productStock = storeProduct.FirstOrDefault(x => x.ProductId == objOrigin.Id && x.StoreId == StoreId);
                                if (productStock == null)
                                {
                                    obj.Success = false;
                                    obj.Message += "Số lượng sản phẩm lớn hơn tồn kho,";
                                }
                                if (quantityParse > productStock.StockQuantity)
                                {
                                    obj.Success = false;
                                    obj.Message += "Số lượng sản phẩm lớn hơn tồn kho,";
                                }

                                obj.Quantity = quantityParse;
                            }
                        }
                        else if (headers[i] == "Ghi chú")
                        {
                            obj.ColExcel3 = cellValue;
                        }
                    }

                    result.Add(obj);
                }

                return result;
            }
        }

        private CustomSheet RenderTemplateAfterImport(List<WarehouseTransferBillProductImportRespon> list)
        {
            var sheet = new CustomSheet("Sheet 1");

            var startRow = 1;

            var header = new CustomDataTable()
            {
                StartRowIndex = startRow,
                StartColumnIndex = 1,
                RowDirection = Directions.Horizontal,
                Rows = new List<DataRow>
                    {
                        new DataRow(
                            new HeaderCell("Sản phẩm"),
                            new HeaderCell("Số lượng"),
                            new HeaderCell("Ghi chú"),
                            new HeaderCell("Trạng thái"),
                            new HeaderCell("Ghi chú")
                            )
                    }
            };
            sheet.Tables.Add(header);

            var indexSaleOrderColumn = startRow + 1;
            foreach (var item in list)
            {
                var row = new CustomDataTable()
                {
                    StartRowIndex = indexSaleOrderColumn++,
                    StartColumnIndex = 1,
                    RowDirection = Directions.Horizontal,
                    Rows = new List<DataRow>
                        {
                        new DataRow(
                            new Cell(item.ColExcel1),
                            new Cell(item.ColExcel2),
                            new Cell(item.ColExcel3),
                            new Cell(item.Success ? "Thành công" : "Thất bại"),
                            new Cell(item.Message)
                            )
                    }
                };

                sheet.Tables.Add(row);
            }

            return sheet;
        }
    }
}