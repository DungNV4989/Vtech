
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using System.Threading.Tasks;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Timing;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VTECHERP.Constants;
using VTECHERP.Datas;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.ProductXnk;
using VTECHERP.DTOs.WarehousingBills;
using VTECHERP.Entities;
using VTECHERP.Enums;
using VTECHERP.Enums.DraftTicket;
using VTECHERP.Extensions;
using static VTECHERP.Constants.EntryConfig.WarehousingImport;

namespace VTECHERP.Services
{
    public class WarehousingBillService : IWarehousingBillService
    {
        private readonly IRepository<WarehousingBill> _warehousingBillRepository;
        private readonly IRepository<WarehousingBillProduct> _warehousingBillProductRepository;
        private readonly IEntryService _entryService;
        private readonly IPaymentReceiptService _paymentReceiptService;
        private readonly IRepository<Suppliers> _supplierRepository;
        private readonly IRepository<Entities.Customer> _customerRepository;
        private readonly IRepository<Products> _productRepository;
        private readonly IRepository<StoreProduct> _storeProductRepository;
        private readonly IRepository<Stores> _storeRepository;
        private readonly IRepository<WarehouseTransferBill> _warehouseTransferBillRepository;
        private readonly IRepository<WarehouseTransferBillProduct> _warehouseTransferBillProductRepository;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IRepository<SaleOrders> _saleOrderRepository;
        private readonly IRepository<WarehousingBillLogs> _warehousingBillLogsRepository;
        private readonly IRepository<DraftTicket> _draftTicketRepository;
        private readonly IRepository<CustomerReturn> _customerReturnRepository;
        private readonly IRepository<CustomerReturnProduct> _customerReturnProductRepository;
        private readonly IObjectMapper _mapper;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IClock _clock;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly ICurrentUser _userManager;
        private readonly IAttachmentService _attachmentService;

        public WarehousingBillService(
            IRepository<WarehousingBill> warehousingBillRepository,
            IRepository<WarehousingBillProduct> warehousingBillProductRepository,
            IRepository<WarehouseTransferBill> warehouseTransferBillRepository,
            IRepository<WarehouseTransferBillProduct> warehouseTransferBillProductRepository,
            IRepository<Suppliers> supplierRepository,
            IRepository<Entities.Customer> customerRepository,
            IRepository<Products> productRepository,
            IRepository<StoreProduct> storeProductRepository,
            IEntryService entryService,
            IPaymentReceiptService paymentReceiptService,
            IIdentityUserRepository userRepository,
            IRepository<SaleOrders> saleOrderRepository,
            IRepository<Stores> storeRepository,
            IRepository<WarehousingBillLogs> warehousingBillLogsRepository,
            IRepository<DraftTicket> draftTicketRepository,
            IRepository<CustomerReturn> customerReturnRepository,
            IRepository<CustomerReturnProduct> customerReturnProductRepository,
            IObjectMapper mapper,
            IUnitOfWorkManager unitOfWorkManager,
            IClock clock,
            IRepository<UserStore> userStoreRepository,
            ICurrentUser userManager,
            IAttachmentService attachmentService
            )
        {
            _warehousingBillRepository = warehousingBillRepository;
            _warehousingBillProductRepository = warehousingBillProductRepository;
            _warehouseTransferBillRepository = warehouseTransferBillRepository;
            _warehouseTransferBillProductRepository = warehouseTransferBillProductRepository;
            _supplierRepository = supplierRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _storeProductRepository = storeProductRepository;
            _entryService = entryService;
            _paymentReceiptService = paymentReceiptService;
            _userRepository = userRepository;
            _saleOrderRepository = saleOrderRepository;
            _storeRepository = storeRepository;
            _warehousingBillLogsRepository = warehousingBillLogsRepository;
            _draftTicketRepository = draftTicketRepository;
            _customerReturnRepository = customerReturnRepository;
            _customerReturnProductRepository = customerReturnProductRepository;
            _mapper = mapper;
            _unitOfWorkManager = unitOfWorkManager;
            _clock = clock;
            _storeRepository = storeRepository;
            _userStoreRepository = userStoreRepository;
            _userManager = userManager;
            _attachmentService = attachmentService;
        }

        public async Task<PagingResponse<WarehousingBillDto>> SearchBills(SearchWarehousingBillRequest request)
        {
            var allUsers = await _userRepository.GetListAsync();
            if (request.DateFrom != null)
            {
                request.DateFrom = _clock.Normalize(request.DateFrom.Value);
            }
            if (request.DateTo != null)
            {
                request.DateTo = _clock.Normalize(request.DateTo.Value);
            }
            var billWithProducts = new List<Guid>();
            if (request.ProductId != null)
            {
                billWithProducts = _warehousingBillProductRepository
                    .GetQueryableAsync()
                    .Result
                    .Where(p => !p.IsDeleted && p.ProductId == request.ProductId)
                    .Select(p => p.WarehousingBillId)
                    .Distinct()
                    .ToList();
            }
            var draftTransferBillIds = new List<Guid>();
            if (!request.DraftTransferBillCode.IsNullOrWhiteSpace())
            {
                draftTransferBillIds = _warehouseTransferBillRepository
                    .GetQueryableAsync()
                    .Result
                    .Where(p => !p.IsDeleted && p.Code.Contains(request.DraftTransferBillCode))
                    .Select(p => p.Id)
                    .ToList();
            }

            var orderIds = new List<Guid>();
            if (!request.OrderCode.IsNullOrWhiteSpace())
            {
                orderIds = _saleOrderRepository
                    .GetQueryableAsync()
                    .Result
                    .Where(p => !p.IsDeleted && p.Code.Contains(request.OrderCode))
                    .Select(p => p.Id)
                    .ToList();
            }
            var userId = _userManager.Id;
            var userStores = (await _userStoreRepository.GetQueryableAsync()).Where(x => x.UserId == userId);
            var query =
                from bill in _warehousingBillRepository.GetQueryableAsync().Result
                join userStore in userStores on bill.StoreId equals userStore.StoreId
                where
                    !bill.IsDeleted
                    && (request.StoreIds == null || request.StoreIds.Count == 0 || request.StoreIds.Contains(bill.StoreId))
                    && (request.BillCode.IsNullOrWhiteSpace() || bill.Code.Contains(request.BillCode))
                    && (request.BillType == null || bill.BillType == request.BillType)
                    && (request.DateFrom == null || bill.CreationTime >= request.DateFrom)
                    && (request.DateTo == null || bill.CreationTime <= request.DateTo)
                    && (request.AudienceType == null || (bill.AudienceType == request.AudienceType))
                    && (request.DocumentDetailType == null || request.DocumentDetailType.Count == 0 || (request.DocumentDetailType.Contains(bill.DocumentDetailType)))
                    && (request.AudienceId == null || bill.AudienceId == request.AudienceId)
                    && (request.Note.IsNullOrWhiteSpace() || bill.Note.Contains(request.Note))
                    && (request.DraftTransferBillCode.IsNullOrWhiteSpace() || (bill.IsFromWarehouseTransfer == true && draftTransferBillIds.Contains(bill.SourceId ?? Guid.Empty)))
                    && (request.OrderCode.IsNullOrWhiteSpace() || (bill.IsFromOrderConfirmation == true && orderIds.Contains(bill.SourceId ?? Guid.Empty)))
                select bill;

            var paged = query
                .OrderByDescending(p => p.Code)
                .Skip(request.Offset)
                .Take(request.PageSize)
                .ToList();

            var pagedDto = _mapper.Map<List<WarehousingBill>, List<WarehousingBillDto>>(paged);
            var billIds = pagedDto.Select(p => p.Id).ToList();
            var storeIds = pagedDto.Select(p => p.StoreId).ToList();

            var billProducts = await _warehousingBillProductRepository.GetListAsync(p => billIds.Contains(p.WarehousingBillId));
            var billStores = await _storeRepository.GetListAsync(p => storeIds.Contains(p.Id));
            var suppliers = await _supplierRepository.GetListAsync();
            var customers = await _customerRepository.GetListAsync();

            var attachments = (await _attachmentService.ListAttachmentByObjectIdAsync(pagedDto.Select(x => x.Id.Value).ToList())).OrderBy(x => x.CreationTime).ToList();
            foreach (var item in pagedDto)
            {
                switch (item.AudienceType)
                {
                    case AudienceTypes.SupplierCN:
                    case AudienceTypes.SupplierVN:
                        var supplier = suppliers.FirstOrDefault(p => p.Id == item.AudienceId);
                        item.AudienceCode = supplier?.Code;
                        item.AudienceName = supplier?.Name;
                        item.AudiencePhone = supplier?.PhoneNumber;
                        break;
                    case AudienceTypes.Customer:
                        var customer = customers.FirstOrDefault(x => x.Id == item.AudienceId);
                        if (customer != null)
                        {
                            item.AudienceCode = customer.Code;
                            item.AudienceName = customer.Name;
                            item.AudiencePhone = customer.PhoneNumber;
                            item.DateOfBirth = customer.DateOfBirth;
                        }
                        break;
                }

                var itemProducts = billProducts.Where(p => p.WarehousingBillId == item.Id).ToList();

                item.NumberOfProduct = itemProducts.Count();
                item.TotalProductAmount = itemProducts.Sum(p => p.Quantity);

                if (item.CreatorId != null)
                {
                    var user = allUsers.FirstOrDefault(p => p.Id == item.CreatorId);
                    item.CreatorName = user?.Name;
                }

                if (item.LastModifierId != null)
                {
                    var user = allUsers.FirstOrDefault(p => p.Id == item.LastModifierId);
                    item.LastModifierName = user?.Name;
                }

                var store = billStores.FirstOrDefault(p => p.Id == item.StoreId);
                item.StoreName = store?.Name;

                var ddt = DocumentDetailTypeData.Datas.FirstOrDefault(p => p.DocumentDetailType == item.DocumentDetailType);
                item.DocumentDetailTypeName = ddt?.Name;

                if (item.BillDiscountType == null)
                {
                    item.TotalDiscountAmount = itemProducts.Sum(p =>
                    {
                        if (p.DiscountType != null)
                        {
                            if (p.DiscountType == MoneyModificationType.Percent)
                            {
                                return p.TotalPriceBeforeDiscount * p.DiscountAmount / 100;
                            }
                            return p.DiscountAmount;
                        }
                        return 0m;
                    });
                }
                else if (item.BillDiscountType == MoneyModificationType.Percent)
                {
                    item.TotalDiscountAmount = item.TotalPriceProduct * item.BillDiscountAmount / 100;
                }
                else
                {
                    item.TotalDiscountAmount = item.BillDiscountAmount;
                }

                if (item.IsFromWarehouseTransfer == true)
                {
                    item.IsEditable = false;
                }

                item.Attachments = attachments.Where(x => x.ObjectId == item.Id).ToList() ?? new List<DTOs.Attachment.DetailAttachmentDto>();
            }

            return new PagingResponse<WarehousingBillDto>(query.Count(), pagedDto);
        }

        public async Task<PagingResponse<SearchProductXnkResponse>> SearchProductXnk(SearchProductXnkRequest request)
        {

            var userStores = (await _userStoreRepository.GetQueryableAsync()).Where(x => x.UserId == _userManager.GetId());
            if(!userStores.Any())
                return new PagingResponse<SearchProductXnkResponse>(0, new List<SearchProductXnkResponse>());
            if (request.StartDate != null)
            {
                request.StartDate = _clock.Normalize(request.StartDate.Value);
            }
            if (request.EndDate != null)
            {
                request.EndDate = _clock.Normalize(request.EndDate.Value).AddDays(1);
            }

            var orderIds = new List<Guid>();
            if (!request.OrderCode.IsNullOrWhiteSpace())
            {
                orderIds = _saleOrderRepository
                    .GetQueryableAsync()
                    .Result
                    .Where(p => !p.IsDeleted && p.Code.Contains(request.OrderCode))
                    .Select(p => p.Id)
                    .ToList();
            }
            var draftTransferBillIds = new List<Guid>();
            if (!request.DraftTransferBillCode.IsNullOrWhiteSpace())
            {
                draftTransferBillIds = _warehouseTransferBillRepository
                    .GetQueryableAsync()
                    .Result
                    .Where(p => !p.IsDeleted && p.Code.Contains(request.DraftTransferBillCode))
                    .Select(p => p.Id)
                    .ToList();
            }

            var queryBill = (await _warehousingBillRepository.GetQueryableAsync()).Where(x=>userStores.Any(uS=>uS.StoreId == x.StoreId));
            var queryBillProduct = (await _warehousingBillProductRepository.GetQueryableAsync()).Where(x=>queryBill.Any(qB => qB.Id == x.WarehousingBillId));

            var query = from billProduct in queryBillProduct
                        join b in queryBill on billProduct.WarehousingBillId equals b.Id into bp
                        from bill in bp.DefaultIfEmpty()
                        join p in _productRepository.GetQueryableAsync().Result on billProduct.ProductId equals p.Id into pd
                        from product in pd.DefaultIfEmpty()
                        select new { billProduct, bill, product };

            query = query.WhereIf(!string.IsNullOrEmpty(request.ProductName), x => x.product.Name.Contains(request.ProductName) || x.product.Code.Contains(request.ProductName))
                        .WhereIf(!string.IsNullOrEmpty(request.ProductId), x => x.product.Code.Contains(request.ProductId))
                        .WhereIf(request.StoreIds != null && request.StoreIds.Any(), x => request.StoreIds.Any(z => z == x.bill.StoreId))
                        .WhereIf(!string.IsNullOrEmpty(request.BillId), x => x.bill.Code.Contains(request.BillId))
                        .WhereIf(request.SupplierIds != null, x => request.SupplierIds == x.bill.AudienceId && (x.bill.AudienceType == AudienceTypes.SupplierCN || x.bill.AudienceType == AudienceTypes.SupplierVN))
                        .WhereIf(request.BillType != null, x => request.BillType == x.bill.BillType)
                        .WhereIf(request.XnkTypes != null && request.XnkTypes.Any(), x => request.XnkTypes.Contains(x.bill.DocumentDetailType))
                        .WhereIf(!string.IsNullOrEmpty(request.Note), x => x.bill.Note.Contains(request.Note))
                        .WhereIf(request.StartDate != null, x => x.bill.CreationTime >= request.StartDate.Value)
                        .WhereIf(request.EndDate != null, x => x.bill.CreationTime < request.EndDate.Value)
                        .WhereIf(!request.OrderCode.IsNullOrWhiteSpace(), x => (x.bill.IsFromOrderConfirmation == true && orderIds.Contains(x.bill.SourceId ?? Guid.Empty)))
                        .WhereIf(!request.DraftTransferBillCode.IsNullOrWhiteSpace(), x => (x.bill.IsFromWarehouseTransfer == true && draftTransferBillIds.Contains(x.bill.SourceId ?? Guid.Empty)))
                        .OrderByDescending(p => p.bill.Code);

            var resPage = query.Skip(request.Offset).Take(request.PageSize).Select(x => new SearchProductXnkResponse()
            {
                Id = x.billProduct.Id,
                Code = x.bill.Code,
                StoreId = x.bill.StoreId,
                ProductId = x.product.Id,
                ProductCode = x.product.Code,
                ProductName = x.product.Name,
                Price = x.billProduct.Price,
                CostPrice = x.product.StockPrice,
                Unit = x.billProduct.Unit,
                BillType = x.bill.BillType,
                TotalMoney = x.billProduct.TotalPrice,
                CreatorId = x.billProduct.CreatorId,
                CreationTime = x.bill.CreationTime,
                DiscountAmount = x.billProduct.DiscountAmount,
                Money = x.billProduct.Quantity * x.billProduct.Price,
                Quantity = x.billProduct.Quantity,
                IsEditable = !(x.bill.IsFromWarehouseTransfer == true),
                IsDeletable = !(x.bill.IsFromWarehouseTransfer == true),
                Inventory = x.billProduct.UpdatedStockQuantity
            }).ToList();

            var storedIds = resPage.Select(x => x.StoreId).ToList();

            var productIds = resPage.Select(x => x.ProductId).ToList();

            //var listStordProduct = (await _storeProductRepository.GetQueryableAsync())
            //    .Where(x => productIds.Contains(x.ProductId) && userStores.Any(us=>us.StoreId == x.StoreId));

            var listStores = await _storeRepository.GetListAsync(x => storedIds.Contains(x.Id));

            foreach (var item in resPage)
            {
                //item.Inventory = listStordProduct.Where(x => item.StoreId == x.StoreId && item.ProductId == x.ProductId).Sum(x => x.StockQuantity);
                item.StoreName = listStores.FirstOrDefault(x => x.Id == item.StoreId)?.Name;
            }

            var listCreatorIds = resPage.Select(x => x.CreatorId).ToList();
            if (listCreatorIds.Count > 0)
            {
                var allUsers = (await _userRepository.GetListAsync()).Where(x => listCreatorIds.Contains(x.Id));
                foreach (var item in resPage)
                {
                    item.CreatorName = allUsers.FirstOrDefault(x => x.Id == item.CreatorId)?.Name;
                }
            }

            return new PagingResponse<SearchProductXnkResponse>(query.Count(), resPage);

        }

        public async Task<Guid> CreateBill(CreateWarehousingBillRequest request, bool calculateStockPrice = true, bool confirm = false)
        {
            var uow = _unitOfWorkManager.Current;
            //using var uow = _unitOfWorkManager.Current;
            try
            {
                await ValidateCreate(request);
                var bill = _mapper.Map<CreateWarehousingBillRequest, WarehousingBill>(request);
                var billProducts = _mapper.Map<List<WarehousingBillProductRequest>, List<WarehousingBillProduct>>(request.Products);

                if (calculateStockPrice)
                    CalculateBillPrice(bill, billProducts);
                if (bill.CashPaymentAmount > 0)
                {
                    bill.CashPaymentHaveValueDate = _clock.Now;
                }
                if (bill.BankPaymentAmount > 0)
                {
                    bill.BankPaymentHaveValueDate = _clock.Now;
                }
                if (bill.VATAmount > 0)
                {
                    bill.VATHaveValueDate = _clock.Now;
                }
                var productIds = billProducts.Select(p => p.Id).ToList();

                await _warehousingBillRepository.InsertAsync(bill);
                await uow.SaveChangesAsync();

                var billProductIds = billProducts.Select(x => x.ProductId);
                var products = await _productRepository.GetListAsync(x => billProductIds.Contains(x.Id));
                foreach (var billProduct in billProducts)
                {
                    billProduct.WarehousingBillId = bill.Id;
                    var product = products.FirstOrDefault(x => x.Id == billProduct.ProductId);
                    if (product != null)
                    {
                        product.EntryPrice = billProduct.Price;
                    }
                }

                await _warehousingBillProductRepository.InsertManyAsync(billProducts);
                await uow.SaveChangesAsync();

                await UpdateStockOnBillCreate(bill, billProducts);
                if (calculateStockPrice)
                    await UpdateBillStockPrice(bill, billProducts);

                if (request.IsFromWarehouseTransfer != true)
                {
                    if(!confirm)
                    {
                        await _entryService.AutoCreateEntryByCreateWarehousingBill(bill.Id);
                        await _paymentReceiptService.AutoCreatePaymentReceiptOnWarehousingBill(bill.Id);
                    }
                }

                await uow.SaveChangesAsync();

                return bill.Id;
            }
            catch
            {
                await uow.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateBill(UpdateWarehousingBillRequest request)
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                await ValidateUpdate(request);
                var bill = await _warehousingBillRepository.GetAsync(p => p.Id == request.Id);

                // capture value before compare
                var billDTOBefore = _mapper.Map<WarehousingBill, WarehousingBillDto>(bill);
                if (billDTOBefore.AudienceType == AudienceTypes.Customer)
                {
                    var customer = _customerRepository.GetAsync(x => x.Id == billDTOBefore.AudienceId).Result;
                    if (customer != null)
                    {
                        billDTOBefore.AudienceName = customer.Name;
                        billDTOBefore.AudiencePhone = customer.PhoneNumber;
                    }
                }
                else if (billDTOBefore.AudienceType == AudienceTypes.SupplierVN || billDTOBefore.AudienceType == AudienceTypes.SupplierCN)
                {
                    var supplier = _supplierRepository.GetAsync(x => x.Id == billDTOBefore.AudienceId).Result;
                    if (supplier != null)
                    {
                        billDTOBefore.AudienceName = supplier.Name;
                        billDTOBefore.AudiencePhone = supplier.PhoneNumber;
                    }
                }
                bill.StoreId = request.StoreId;
                bill.BillType = request.BillType;

                bill.AudienceType = request.AudienceType;
                bill.AudienceId = request.AudienceId;

                bill.BillDiscountType = request.BillDiscountType;
                bill.BillDiscountAmount = request.BillDiscountAmount;

                bill.CashPaymentAccountCode = request.CashPaymentAccountCode;
                if (request.CashPaymentAmount > 0)
                {
                    bill.CashPaymentHaveValueDate = _clock.Now;
                }
                else
                {
                    bill.CashPaymentHaveValueDate = null;
                }

                bill.CashPaymentAmount = request.CashPaymentAmount;

                bill.BankPaymentAccountCode = request.BankPaymentAccountCode;
                if (request.BankPaymentAmount > 0)
                {
                    bill.BankPaymentHaveValueDate = _clock.Now;
                }
                else
                {
                    bill.BankPaymentHaveValueDate = null;
                }

                bill.BankPaymentAmount = request.BankPaymentAmount;

                bill.VATType = request.VATType;

                if (request.VATAmount > 0)
                {
                    bill.VATHaveValueDate = _clock.Now;
                }
                else
                {
                    bill.VATHaveValueDate = null;
                }

                bill.VATAmount = request.VATAmount;
                bill.VATBillCode = request.VATBillCode;
                bill.VATBillDate = request.VATBillDate;
                bill.DocumentDetailType = request.DocumentDetailType;
                bill.Note = request.Note;

                // delete ds product cũ, insert list mới
                var previousBillProducts = await _warehousingBillProductRepository.GetListAsync(p => p.WarehousingBillId == bill.Id);

                // capture products before compare
                var billProductsBefore = _mapper.Map<List<WarehousingBillProduct>, List<WarehousingBillProductDto>>(previousBillProducts);
                billDTOBefore.Products = billProductsBefore;
                setProductName(ref billDTOBefore);
                setProductCode(ref billDTOBefore);

                await uow.SaveChangesAsync();

                var currentBillProducts = _mapper.Map<List<WarehousingBillProductRequest>, List<WarehousingBillProduct>>(request.Products);

                CalculateBillPrice(bill, currentBillProducts);

                await _warehousingBillRepository.UpdateAsync(bill);
                await uow.SaveChangesAsync();

                foreach (var billProduct in currentBillProducts)
                {
                    billProduct.WarehousingBillId = bill.Id;
                }

                await _warehousingBillProductRepository.InsertManyAsync(currentBillProducts);
                await uow.SaveChangesAsync();

                await UpdateStockOnBillUpdate(bill, previousBillProducts, currentBillProducts);
                await UpdateBillStockPrice(bill, currentBillProducts);
                await _entryService.AutoUpdateEntryByUpdateWarehousingBill(bill.Id);
                await _paymentReceiptService.AutoCreatePaymentReceiptOnWarehousingBill(bill.Id);

                await _warehousingBillProductRepository.HardDeleteAsync(previousBillProducts);
                await uow.CompleteAsync();

                // capture value after update
                var billDTOAfter = _mapper.Map<WarehousingBill, WarehousingBillDto>(bill);
                var billProductsAfter = _mapper.Map<List<WarehousingBillProduct>, List<WarehousingBillProductDto>>(currentBillProducts);
                billDTOAfter.Products = billProductsAfter;
                if (billDTOAfter.AudienceType == AudienceTypes.Customer)
                {
                    var customer = _customerRepository.GetAsync(x => x.Id == billDTOAfter.AudienceId).Result;
                    if (customer != null)
                    {
                        billDTOAfter.AudienceName = customer.Name;
                        billDTOAfter.AudiencePhone = customer.PhoneNumber;
                    }
                }
                else if (billDTOAfter.AudienceType == AudienceTypes.SupplierVN || billDTOAfter.AudienceType == AudienceTypes.SupplierCN)
                {
                    var supplier = _supplierRepository.GetAsync(x => x.Id == billDTOAfter.AudienceId).Result;
                    if (supplier != null)
                    {
                        billDTOAfter.AudienceName = supplier.Name;
                        billDTOAfter.AudiencePhone = supplier.PhoneNumber;
                    }
                }
                setProductName(ref billDTOAfter);
                setProductCode(ref billDTOAfter);
                // compare value and write log
                await Task.Run(() => CompareObjectAndWriteLogsUpdate(billDTOBefore, billDTOAfter));
            }
            catch
            {
                await uow.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Xóa thủ công phiếu XNK
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteBill(Guid id)
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var bill = await _warehousingBillRepository.GetAsync(p => p.Id == id);
                var currentBillProducts = await _warehousingBillProductRepository.GetListAsync(p => p.WarehousingBillId == bill.Id);

                await UpdateStockOnBillDelete(bill, currentBillProducts);
                await _entryService.AutoDeleteEntryByDeleteWarehousingBill(bill.Id);

                // Khi xóa phiếu nhập/xuất kho, kiểm tra phiếu nhập/xuất có nguồn từ phiếu chuyển kho
                // nếu tất cả phiếu nhập/xuất từ phiếu chuyển kho bị xóa, tự động xóa phiếu chuy
                if (bill.IsFromWarehouseTransfer == true && bill.SourceId != null)
                {
                    var warehousingBillFromTransfer = await _warehousingBillRepository.GetListAsync(p =>
                        p.IsFromWarehouseTransfer == true
                        && p.SourceId != null
                        && bill.SourceId == p.SourceId);

                    if (!warehousingBillFromTransfer.Any())
                    {
                        var warehouseTransferBill = await _warehouseTransferBillRepository.FindAsync(p => p.Id == bill.SourceId && !p.IsDeleted);
                        var billProducts = await _warehouseTransferBillProductRepository.GetListAsync(p => p.WarehouseTransferBillId == warehouseTransferBill.Id && !p.IsDeleted);

                        if (warehouseTransferBill != null)
                        {
                            await _warehouseTransferBillRepository.DeleteAsync(warehouseTransferBill);

                        }
                        if (billProducts.Any())
                        {
                            await _warehouseTransferBillProductRepository.DeleteManyAsync(billProducts);
                        }
                    }
                }

                await _warehousingBillProductRepository.DeleteManyAsync(currentBillProducts);
                await _warehousingBillRepository.DeleteAsync(bill);

                //Xoá phiếu nháp tương ứng nếu có.
                if (bill.SourceId != null)
                {
                    var warehouseTransferBill = await _warehouseTransferBillRepository.FindAsync(x => x.Id == bill.SourceId);
                    if (warehouseTransferBill != null)
                    {
                        //Nếu là phiếu xuất kho thì xóa phiếu chuyển
                        if (bill.BillType == WarehousingBillType.Export)
                        {
                            warehouseTransferBill.IsDeleted = true;
                        }
                        //Nếu là phiếu nhập kho thì cập nhật trạng thái phiếu chuyển
                        else if (bill.BillType == WarehousingBillType.Import)
                        {
                            warehouseTransferBill.TransferBillType = Enums.WarehouseTransferBill.TransferBillType.Export;
                            warehouseTransferBill.DeliveryConfirmedUserId = null;
                            warehouseTransferBill.DeliveryConfirmedDate = null;
                        }
                        await _warehouseTransferBillRepository.UpdateAsync(warehouseTransferBill);

                        //Cập nhật trạng thái phiếu nháp
                        var draftTicket = await _draftTicketRepository.FindAsync(x => x.Id == warehouseTransferBill.DraftTicketId);
                        if (draftTicket != null)
                        {
                            draftTicket.Status = Status.Cancel;
                            //Nếu là phiếu nhập kho thì phiếu nháp chuyển thành trạng thái đã duyệt
                            if (bill.BillType == WarehousingBillType.Import)
                            {
                                draftTicket.Status = Status.Approved;
                                draftTicket.DeliveryConfirmedUserId = null;
                                draftTicket.DeliveryConfirmedDate = null;
                            }
                            await _draftTicketRepository.UpdateAsync(draftTicket);
                        }
                    }
                }

                await uow.SaveChangesAsync();
                await _paymentReceiptService.AutoDeletePaymentReceiptOnWarehousingBill(bill.Id);
                await uow.CompleteAsync();

                // log
                await _warehousingBillLogsRepository.InsertAsync(new Entities.WarehousingBillLogs()
                {
                    Action = EntityActions.Delete,
                    WarehousingBillId = bill.Id,
                });
            }
            catch
            {
                await uow.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Xóa tự động tất cả phiếu XNK khi xóa phiếu chuyển kho
        /// </summary>
        /// <param name="warehouseTransferBill"></param>
        /// <returns></returns>
        public async Task AutoDeleteBillByWarehouseTransferBill(Guid warehouseTransferBill)
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var warehousingBillFromTransfer = await _warehousingBillRepository.GetListAsync(p =>
                    p.IsFromWarehouseTransfer == true
                    && p.SourceId != null
                    && warehouseTransferBill == p.SourceId);
                var billIds = warehousingBillFromTransfer.Select(p => p.Id).ToList();
                var currentBillProducts = await _warehousingBillProductRepository.GetListAsync(p => billIds.Contains(p.WarehousingBillId));

                foreach (var bill in warehousingBillFromTransfer)
                {
                    var billProducts = currentBillProducts.Where(p => p.WarehousingBillId == bill.Id).ToList();
                    await UpdateStockOnBillDelete(bill, currentBillProducts);
                    await _entryService.AutoDeleteEntryByDeleteWarehousingBill(bill.Id);

                    // log
                    await _warehousingBillLogsRepository.InsertAsync(new Entities.WarehousingBillLogs()
                    {
                        Action = EntityActions.Delete,
                        WarehousingBillId = bill.Id,
                    });
                }

                await _warehousingBillProductRepository.DeleteManyAsync(currentBillProducts);
                await _warehousingBillRepository.DeleteManyAsync(warehousingBillFromTransfer);

                await uow.SaveChangesAsync();
                await uow.CompleteAsync();
            }
            catch
            {
                await uow.RollbackAsync();
                throw;
            }
        }

        public async Task<WarehousingBillDto> GetBill(Guid id)
        {
            var bill = await _warehousingBillRepository.GetAsync(p => p.Id == id);
            var currentBillProducts = await _warehousingBillProductRepository.GetListAsync(p => p.WarehousingBillId == bill.Id);

            var billDto = _mapper.Map<WarehousingBill, WarehousingBillDto>(bill);
            var billProductDtos = _mapper.Map<List<WarehousingBillProduct>, List<WarehousingBillProductDto>>(currentBillProducts);

            billDto.Products = billProductDtos;

            if (billDto.AudienceId != null)
            {
                switch (billDto.AudienceType)
                {
                    case AudienceTypes.SupplierCN:
                    case AudienceTypes.SupplierVN:
                        var supplier = _supplierRepository.GetQueryableAsync().Result.FirstOrDefault(p => p.Id == billDto.AudienceId);
                        billDto.AudienceName = supplier?.Name;
                        billDto.AudienceCode = supplier?.Code;
                        billDto.AudiencePhone = supplier?.PhoneNumber;
                        break;
                    case AudienceTypes.Customer:
                        break;
                }
            }

            var productIds = billDto.Products.Select(p => p.ProductId).Distinct().ToList();
            var products = _productRepository.GetQueryableAsync().Result.Where(p => productIds.Contains(p.Id)).ToList();
            var storeProducts = _storeProductRepository.GetQueryableAsync().Result.Where(p => productIds.Contains(p.ProductId) && p.StoreId == bill.StoreId).ToList();
            foreach (var billProduct in billDto.Products)
            {
                var product = products.FirstOrDefault(p => p.Id == billProduct.ProductId);
                var storeProduct = storeProducts.FirstOrDefault(p => p.ProductId == billProduct.ProductId);
                billProduct.ProductName = product?.Name;
                billProduct.ProductCode = product?.Code;
                if(bill.BillType == WarehousingBillType.Export)
                {
                    billProduct.ProductStockQuantity = (storeProduct?.StockQuantity ?? 0) + billProduct.Quantity;
                }

                billProduct.Unit = product.Unit;
                //billProduct.UnitName = MasterDatas.ProductUnits.First(p => p.Id == product.Unit).Name;
                billProduct.UnitName = product.Unit;
            }

            if (billDto.BillDiscountType == null)
            {
                billDto.TotalDiscountAmount = billDto.Products.Sum(p =>
                {
                    if (p.DiscountType != null)
                    {
                        if (p.DiscountType == MoneyModificationType.Percent)
                        {
                            return p.TotalPriceBeforeDiscount * p.DiscountAmount / 100;
                        }
                        return p.DiscountAmount;
                    }
                    return 0m;
                });
            }
            else if (billDto.BillDiscountType == MoneyModificationType.Percent)
            {
                billDto.TotalDiscountAmount = billDto.TotalPriceProduct * billDto.BillDiscountAmount / 100;
            }
            else
            {
                billDto.TotalDiscountAmount = billDto.BillDiscountAmount;
            }

            var store = _storeRepository.GetQueryableAsync().Result.FirstOrDefault(p => p.Id == billDto.StoreId);
            billDto.StoreName = store?.Name;

            var ddt = DocumentDetailTypeData.Datas.FirstOrDefault(p => p.DocumentDetailType == billDto.DocumentDetailType);
            billDto.DocumentDetailTypeName = ddt?.Name;
            billDto.IsEditable = !(bill.IsFromWarehouseTransfer == true);
            billDto.Attachments = await _attachmentService.GetAttachmentByObjectIdAsync(billDto.Id.Value);
            return billDto;
        }

        public async Task DeleteListProductInBillProduct(DeleteListProductInBillProduct requert)
        {
            var billProducts = _warehousingBillProductRepository.GetDbSetAsync().Result.Where(x => requert.BillProductIds.Any(z => z == x.Id)).ToList();
            if (billProducts == null || billProducts.Count == 0 || billProducts.Count != requert.BillProductIds.Count)
                throw new BusinessException(ErrorMessages.WarehouseTransferBillProduct.NotExist);

            var listBills = billProducts.Select(x => x.WarehousingBillId).ToList();

            var billProductss = _warehousingBillProductRepository.GetDbSetAsync().Result.AsNoTracking().Where(x => listBills.Any(z => z == x.WarehousingBillId)).ToList();
            var bills = _warehousingBillRepository.GetDbSetAsync().Result.AsNoTracking().Where(x => listBills.Any(z => z == x.Id)).ToList();

            foreach (var item in bills)
            {
                var billpro = billProductss.Where(x => x.WarehousingBillId == item.Id).ToList();
                var billProductIds = billProducts.Where(x => x.WarehousingBillId == item.Id).Select(z => z.Id).ToList();
                await PrivateDeleteProductInBillProduct(item, billpro, billProductIds);
            }
        }

        public async Task DeleteProductInBillProduct(Guid warehousingBillProductId)
        {
            var billProduct = _warehousingBillProductRepository.GetDbSetAsync().Result.FirstOrDefault(x => x.Id == warehousingBillProductId);
            if (billProduct == null)
                throw new BusinessException(ErrorMessages.WarehouseTransferBillProduct.NotExist);

            //await _warehousingBillProductRepository.HardDeleteAsync(billProduct);

            var billProducts = _warehousingBillProductRepository.GetDbSetAsync().Result.AsNoTracking().Where(x => x.WarehousingBillId == billProduct.WarehousingBillId).ToList();
            var bill = _warehousingBillRepository.GetDbSetAsync().Result.AsNoTracking().FirstOrDefault(x => x.Id == billProduct.WarehousingBillId);

            await PrivateDeleteProductInBillProduct(bill, billProducts, new List<Guid> { billProduct.Id });
        }

        /// <summary>
        /// Hàm convert update bill khi xóa product, và xóa bill khi không còn product nào trong bill
        /// </summary>
        /// <param name="bill"></param>
        /// <param name="productIds">List id xóa</param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        private async Task PrivateDeleteProductInBillProduct(WarehousingBill bill, List<WarehousingBillProduct> billProducts, List<Guid> productIds)
        {
            if (bill == null)
                throw new BusinessException(ErrorMessages.WarehouseTransferBill.NotExist);

            var updateBill = new UpdateWarehousingBillRequest()
            {
                BillType = bill.BillType,
                Id = bill.Id,
                StoreId = bill.StoreId,
                AudienceType = bill.AudienceType,
                AudienceId = bill.AudienceId,
                DocumentDetailType = bill.DocumentDetailType,
                Note = bill.Note,
                VATType = bill.VATType,
                VATAmount = bill.VATAmount,
                VATBillCode = bill.VATBillCode,
                VATBillDate = bill.VATBillDate,
                BillDiscountType = bill.BillDiscountType,
                BillDiscountAmount = bill.BillDiscountAmount,
                CashPaymentAccountCode = bill.CashPaymentAccountCode,
                CashPaymentAmount = bill.CashPaymentAmount,
                BankPaymentAccountCode = bill.BankPaymentAccountCode,
                BankPaymentAmount = bill.BankPaymentAmount,
            };

            updateBill.Products = new List<WarehousingBillProductRequest>();
            foreach (var item in billProducts)
            {
                if (productIds.Contains(item.Id))
                {
                    var uProduct = new WarehousingBillProductRequest()
                    {
                        ProductId = item.ProductId,
                        DiscountAmount = item.DiscountType == MoneyModificationType.Percent ? item.DiscountAmount * item.TotalPriceBeforeDiscount : item.DiscountAmount,
                        DiscountType = item.DiscountType,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        Unit = item.Unit,
                        Note = item.Note
                    };
                    updateBill.Products.Add(uProduct);
                }
            }

            if (updateBill.Products.Count > 0)
            {
                await UpdateBill(updateBill);
            }

            await DeleteBill(bill.Id);
        }

        public async Task<GetUpdateProductInBillProductResponse> GetUpdateProductInBillProduct(Guid warehousingBillId)
        {

            var billProduct = await _warehousingBillProductRepository.GetAsync(x => x.Id == warehousingBillId);
            if (billProduct == null)
                throw new BusinessException(ErrorMessages.WarehouseTransferBillProduct.Null);

            var product = await _productRepository.GetAsync(x => x.Id == billProduct.ProductId);

            var billQuery = _warehousingBillRepository.GetQueryableAsync().Result
                            .Join(_storeRepository.GetQueryableAsync().Result, x => x.StoreId, z => z.Id, (x, z) => new { StoreName = z.Name, StoreId = x.StoreId, billId = x.Id })
                            .FirstOrDefault(x => x.billId == billProduct.WarehousingBillId);

            var res = new GetUpdateProductInBillProductResponse()
            {
                WarehousingBillId = billProduct.Id,
                StoreName = billQuery?.StoreName,
                ProductName = product.Name,
                Quantity = billProduct.Quantity,
                Price = billProduct.Price,
                Note = billProduct.Note,
            };
            return res;
        }

        public async Task UpdateProductInBillProduct(UpdateProductInBillProductRequest request)
        {
            var billProduct = await _warehousingBillProductRepository.GetAsync(x => x.Id == request.WarehousingBillProductId);
            if (billProduct == null)
                throw new BusinessException(ErrorMessages.WarehouseTransferBillProduct.Null);

            var bill = _warehousingBillRepository.GetDbSetAsync().Result.AsNoTracking().FirstOrDefault(x => x.Id == billProduct.WarehousingBillId);
            if (bill == null)
                throw new BusinessException(ErrorMessages.WarehouseTransferBill.NotExist);

            var updateBill = new UpdateWarehousingBillRequest()
            {
                BillType = bill.BillType,
                Id = bill.Id,
                StoreId = bill.StoreId,
                AudienceType = bill.AudienceType,
                AudienceId = bill.AudienceId,
                DocumentDetailType = bill.DocumentDetailType,
                Note = bill.Note,
                VATType = bill.VATType,
                VATAmount = bill.VATAmount,
                VATBillCode = bill.VATBillCode,
                VATBillDate = bill.VATBillDate,
                BillDiscountType = bill.BillDiscountType,
                BillDiscountAmount = bill.BillDiscountAmount,
                CashPaymentAccountCode = bill.CashPaymentAccountCode,
                CashPaymentAmount = bill.CashPaymentAmount,
                BankPaymentAccountCode = bill.BankPaymentAccountCode,
                BankPaymentAmount = bill.BankPaymentAmount,

            };
            var billProducts = (await _warehousingBillProductRepository
                .GetDbSetAsync())
                .AsNoTracking()
                .Where(x => x.WarehousingBillId == bill.Id)
                .ToList();
            updateBill.Products = new List<WarehousingBillProductRequest>();
            foreach (var item in billProducts)
            {
                var uProduct = new WarehousingBillProductRequest()
                {
                    ProductId = item.ProductId,
                    DiscountAmount = item.DiscountType == MoneyModificationType.Percent ? item.TotalPriceBeforeDiscount * item.DiscountAmount : item.DiscountAmount,
                    DiscountType = item.DiscountType,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Unit = item.Unit,
                    Note = item.Note
                };

                if (item.Id == request.WarehousingBillProductId)
                {
                    if (request.Price != null)
                        uProduct.Price = request.Price.Value;
                    if (request.Quantity != null)
                        uProduct.Quantity = request.Quantity.Value;
                    if (request.Note != null)
                        uProduct.Note = request.Note;
                }

                updateBill.Products.Add(uProduct);
            }

            await UpdateBill(updateBill);
        }

        public async Task<List<WarehousingBillDto>> GetContainCodeAsync(string code)
        {
            var result = new List<WarehousingBillDto>();
            var warehousingBills = (await _warehousingBillRepository.GetQueryableAsync())
                .Where(p => p.Code.ToLower().Contains(code.ToLower())).ToList();
            result = _mapper.Map<List<WarehousingBill>, List<WarehousingBillDto>>(warehousingBills);
            return result;
        }

        public async Task<List<WarehousingBillDto>> GetBySourceIdsAsync(List<Guid?> sourceIds)
        {
            var result = new List<WarehousingBillDto>();
            var warehousingBills = (await _warehousingBillRepository.GetQueryableAsync())
                .Where(p => sourceIds.Any(sourceId => sourceId == p.SourceId)).ToList();
            result = _mapper.Map<List<WarehousingBill>, List<WarehousingBillDto>>(warehousingBills);
            return result;
        }
        public async Task<WarehousingBillDto> GetByWarehouseTransferBillId(Guid warehouseTransferId, WarehousingBillType billType)
        {
            var bill = await _warehousingBillRepository.FirstOrDefaultAsync(p =>
                p.IsFromWarehouseTransfer == true &&
                p.SourceId == warehouseTransferId &&
                p.BillType == billType);
            var billDto = _mapper.Map<WarehousingBill, WarehousingBillDto>(bill);
            var billProducts = await _warehousingBillProductRepository.GetListAsync(p => p.WarehousingBillId == billDto.Id);
            billDto.Products = _mapper.Map<List<WarehousingBillProduct>, List<WarehousingBillProductDto>>(billProducts);
            return billDto;
        }

        private async Task ValidateCreate(CreateWarehousingBillRequest request)
        {
            if (request.AudienceType != AudienceTypes.Other && request.AudienceId == null)
            {
                throw new BusinessException(ErrorMessages.WarehousingBills.AudienceNull);
            }

            var existStore = await _storeRepository.FindAsync(p => p.Id == request.StoreId);
            if (existStore == null)
            {
                throw new BusinessException(ErrorMessages.Store.NotExist);
            }

            if ((request.CashPaymentAccountCode.IsNullOrEmpty() && request.CashPaymentAmount > 0)
                || (!request.CashPaymentAccountCode.IsNullOrEmpty() && (request.CashPaymentAmount == null || request.CashPaymentAmount == 0)))
            {
                throw new BusinessException(ErrorMessages.WarehousingBills.CashLackBillInfo);
            }

            if ((request.BankPaymentAccountCode.IsNullOrEmpty() && request.BankPaymentAmount > 0)
                || (!request.BankPaymentAccountCode.IsNullOrEmpty() && (request.BankPaymentAmount == null || request.BankPaymentAmount == 0)))
            {
                throw new BusinessException(ErrorMessages.WarehousingBills.BankLackBillInfo);
            }

            if (request.VATType != null && request.VATAmount > 0 && (request.VATBillDate == null || request.VATBillCode.IsNullOrWhiteSpace()))
            {
                throw new BusinessException(ErrorMessages.WarehousingBills.VATLackBillInfo);
            }
        }

        private async Task ValidateUpdate(UpdateWarehousingBillRequest request)
        {
            if (request.AudienceType != AudienceTypes.Other && request.AudienceId == null)
            {
                throw new BusinessException(ErrorMessages.WarehousingBills.AudienceNull);
            }
            var existBill = await _warehousingBillRepository.FindAsync(p => p.Id == request.Id);
            if (existBill == null)
            {
                throw new BusinessException(ErrorMessages.WarehousingBills.NotExist);
            }
            var existStore = await _storeRepository.FindAsync(p => p.Id == request.StoreId);
            if (existStore == null)
            {
                throw new BusinessException(ErrorMessages.Store.NotExist);
            }

            if ((request.CashPaymentAccountCode.IsNullOrEmpty() && request.CashPaymentAmount > 0)
                || (!request.CashPaymentAccountCode.IsNullOrEmpty() && (request.CashPaymentAmount == null || request.CashPaymentAmount == 0)))
            {
                throw new BusinessException(ErrorMessages.WarehousingBills.CashLackBillInfo);
            }

            if ((request.BankPaymentAccountCode.IsNullOrEmpty() && request.BankPaymentAmount > 0)
                || (!request.BankPaymentAccountCode.IsNullOrEmpty() && (request.BankPaymentAmount == null || request.BankPaymentAmount == 0)))
            {
                throw new BusinessException(ErrorMessages.WarehousingBills.BankLackBillInfo);
            }

            if (request.VATType != null && request.VATAmount > 0 && (request.VATBillDate == null || request.VATBillCode.IsNullOrWhiteSpace()))
            {
                throw new BusinessException(ErrorMessages.WarehousingBills.VATLackBillInfo);
            }
        }

        /// <summary>
        /// Update thông tin hàng tồn của cửa hàng
        /// </summary>
        /// <param name="bill"></param>
        /// <param name="products"></param>
        private void CalculateBillPrice(WarehousingBill bill, List<WarehousingBillProduct> products)
        {
            var allProductPrices = products.Sum(p => p.Quantity * p.Price);
            // nếu nhập chiết khấu trên hóa đơn, override chiết khấu trên từng sản phẩm
            var totalDiscountAmount = 0m;
            var totalDiscountPercent = 0m;
            if (bill.BillDiscountType != null && bill.BillDiscountAmount != null && bill.BillDiscountAmount > 0)
            {
                if (bill.BillDiscountType == MoneyModificationType.Percent)
                {
                    totalDiscountAmount = allProductPrices * (bill.BillDiscountAmount ?? 0) / 100;
                    totalDiscountPercent = bill.BillDiscountAmount ?? 0;
                }
                else
                {
                    totalDiscountAmount = bill.BillDiscountAmount ?? 0;
                    totalDiscountPercent = (totalDiscountAmount / allProductPrices) * 100;
                }

                if (totalDiscountAmount > allProductPrices)
                {
                    throw new BusinessException(ErrorMessages.WarehousingBills.TotalDiscountLarge);
                }

                foreach (var product in products)
                {
                    product.TotalPriceBeforeDiscount = product.Quantity * product.Price;
                    var productDiscountAmount = totalDiscountPercent * product.TotalPriceBeforeDiscount / 100;
                    product.TotalPrice = product.TotalPriceBeforeDiscount - productDiscountAmount;
                }
            }
            else
            {
                bill.BillDiscountType = null;
                foreach (var product in products)
                {
                    product.TotalPriceBeforeDiscount = product.Quantity * product.Price;
                    if (product.DiscountType != null && product.DiscountAmount > 0)
                    {
                        switch (product.DiscountType)
                        {
                            case MoneyModificationType.VND:
                                product.TotalPrice = product.TotalPriceBeforeDiscount - (product.DiscountAmount ?? 0);
                                totalDiscountAmount += product.DiscountAmount ?? 0;
                                break;
                            case MoneyModificationType.Percent:
                                var productDiscountAmount = product.TotalPriceBeforeDiscount * product.DiscountAmount / 100;
                                totalDiscountAmount += productDiscountAmount ?? 0;
                                product.TotalPrice = product.TotalPriceBeforeDiscount - (productDiscountAmount ?? 0);
                                break;
                        }

                        if (product.DiscountAmount > product.TotalPriceBeforeDiscount)
                        {
                            throw new BusinessException(ErrorMessages.WarehousingBills.TotalDiscountLarge, $"Số tiền triết khấu lớn hơn giá sản phẩm - sản phầm {product.ProductId}");
                        }
                    }
                    else
                    {
                        product.DiscountAmount = 0;
                        product.TotalPrice = product.TotalPriceBeforeDiscount;
                    }
                }
            }

            if (totalDiscountAmount > allProductPrices)
            {
                throw new BusinessException(ErrorMessages.WarehousingBills.ProductDiscountLarge, "Tổng triết khấu lớn hơn tổng giá của sản phẩm");
            }

            bill.TotalPriceProduct = allProductPrices;
            bill.TotalPriceBeforeTax = bill.TotalPriceProduct - totalDiscountAmount;

            // tổng tiền sau thuế
            if (bill.VATType != null && bill.VATAmount != null && bill.VATAmount > 0)
            {
                var vatAmount = 0m;
                switch (bill.VATType)
                {
                    case MoneyModificationType.VND:
                        vatAmount = bill.VATAmount ?? 0;
                        break;
                    case MoneyModificationType.Percent:
                        vatAmount = bill.TotalPriceBeforeTax * (bill.VATAmount ?? 0) / 100;
                        break;
                }

                bill.TotalPrice = bill.TotalPriceBeforeTax + vatAmount;
            }
            else
            {
                bill.TotalPrice = bill.TotalPriceBeforeTax;
            }
        }

        private async Task UpdateStockOnBillCreate(
            WarehousingBill bill,
            List<WarehousingBillProduct>? current)
        {
            var uow = _unitOfWorkManager.Current;
            var productIds = current.Select(p => p.ProductId).ToList();
            var stocks =
                (await _storeProductRepository.GetQueryableAsync())
                    .Where(stock => stock.StoreId == bill.StoreId
                    && productIds.Contains(stock.ProductId)
                    && !stock.IsDeleted)
                .ToList();

            var storeProductQuantityBefore = stocks.Select(p => new { p.ProductId, p.StockQuantity });

            var productQuantityBefore = (await _storeProductRepository.GetQueryableAsync())
                .GroupBy(p => p.ProductId)
                .Select(p => new
                {
                    ProductId = p.Key,
                    TotalStockQuantity = p.Sum(product => product.StockQuantity)
                }).ToList();

            var allProducts = (await _productRepository.GetQueryableAsync())
                .Where(p => productIds.Contains(p.Id))
                .ToList();
            var productToInsert = new List<StoreProduct>();
            var productToUpdate = new List<StoreProduct>();

            switch (bill.BillType)
            {
                case WarehousingBillType.Import:
                    // khi tạo mới đơn nhập, insert sản phẩm hoặc cộng tồn
                    foreach (var product in current)
                    {
                        ModifyIncreaseStock(bill.StoreId, stocks, product, productToInsert, productToUpdate);
                    }
                    break;
                case WarehousingBillType.Export:
                    // khi tạo mới đơn xuất, update trừ tồn
                    foreach (var product in current)
                    {
                        ModifyDecreaseStock(bill.StoreId, stocks, product, productToUpdate);
                    }
                    break;
            }
            var updateStockPriceProducts = new List<Products>();
            var updateBillProducts = productToInsert.Union(productToUpdate).ToList();
            // nếu tạo mới phiếu nhập, cập nhật lại giá vốn sản phẩm trong cửa hàng
            if (bill.BillType == WarehousingBillType.Import && bill.IsFromWarehouseTransfer != true)
            {
                foreach (var updateStoreProduct in updateBillProducts)
                {
                    var currentProduct = current.FirstOrDefault(p => p.ProductId == updateStoreProduct.ProductId);
                    var product = allProducts.FirstOrDefault(p => p.Id == updateStoreProduct.ProductId);
                    var cache = productQuantityBefore.FirstOrDefault(p => p.ProductId == updateStoreProduct.ProductId);
                    var storeCache = storeProductQuantityBefore.FirstOrDefault(p => p.ProductId == updateStoreProduct.ProductId);

                    product.TotalQuantityBeforeLatest = cache?.TotalStockQuantity ?? 0;
                    updateStoreProduct.QuantityBeforeLatest = storeCache?.StockQuantity ?? 0;
                    product.LatestWarehousingBillId = bill.Id;
                    var currentStockPrice = product.StockPrice;
                    product.StockPrice = ((currentStockPrice * cache?.TotalStockQuantity ?? 0) + ((currentProduct.Price + (currentProduct.TransportPrice == null ? 0 : currentProduct.TransportPrice.Value)) * currentProduct.Quantity)) 
                        / (product.TotalQuantityBeforeLatest + currentProduct.Quantity);

                    currentProduct.UpdatedStockPrice = product.StockPrice;
                    product.StockPriceBeforeLatest = currentStockPrice;
                    product.EntryPrice = currentProduct.Price + (currentProduct.TransportPrice == null ? 0 : currentProduct.TransportPrice.Value);

                    updateStockPriceProducts.Add(product);
                    
                }
            }
            else
            {
                foreach (var updateStoreProduct in updateBillProducts)
                {
                    var currentProduct = current.FirstOrDefault(p => p.ProductId == updateStoreProduct.ProductId);
                    var product = allProducts.FirstOrDefault(p => p.Id == updateStoreProduct.ProductId);
                    var cache = productQuantityBefore.FirstOrDefault(p => p.ProductId == updateStoreProduct.ProductId);
                    var storeCache = storeProductQuantityBefore.FirstOrDefault(p => p.ProductId == updateStoreProduct.ProductId);
                    if (currentProduct != null)
                    {
                        currentProduct.CurrentStockPrice = product.StockPrice;
                    }
                }
            }

            if (productToUpdate.Any())
            {
                await _storeProductRepository.UpdateManyAsync(productToUpdate);
            }

            if (productToInsert.Any())
            {
                await _storeProductRepository.InsertManyAsync(productToInsert);
            }

            if (updateStockPriceProducts.Any())
            {
                await _productRepository.UpdateManyAsync(updateStockPriceProducts);
            }

            await _warehousingBillProductRepository.UpdateManyAsync(current);

            await uow.SaveChangesAsync();

            var productQuantities = (await _storeProductRepository.GetQueryableAsync())
                .Where(p => productIds.Contains(p.ProductId))
                .GroupBy(p => p.ProductId)
                .Select(p => new
                {
                    ProductId = p.Key,
                    TotalStockQuantity = p.Sum(product => product.StockQuantity)
                }).ToList();
            allProducts.ForEach(p =>
            {
                var productQuantity = productQuantities.FirstOrDefault(pq => pq.ProductId == p.Id);
                if (productQuantity != null)
                {
                    p.TotalQuantity = productQuantity.TotalStockQuantity;
                }
            });

            await _productRepository.UpdateManyAsync(allProducts);
            await uow.SaveChangesAsync();
        }

        private async Task UpdateStockOnBillUpdate(
            WarehousingBill bill,
            List<WarehousingBillProduct>? previous,
            List<WarehousingBillProduct>? current)
        {
            var uow = _unitOfWorkManager.Current;
            var productIds = current
                .Select(p => p.ProductId)
                .Union(previous.Select(p => p.ProductId))
                .Distinct()
                .ToList();

            var stocks =
                (await _storeProductRepository.GetQueryableAsync())
                .Where(stock => stock.StoreId == bill.StoreId && productIds.Contains(stock.ProductId) && !stock.IsDeleted)
                .ToList();

            var allProducts = (await _productRepository.GetQueryableAsync())
                .Where(p => productIds.Contains(p.Id))
                .ToList();

            var productToInsert = new List<StoreProduct>();
            var productToUpdate = new List<StoreProduct>();

            var currentAddProduct = current.Where(cur => !previous.Any(pre => pre.ProductId == cur.ProductId)).ToList();
            var currentRemovedProduct = previous.Where(pre => !current.Any(cur => cur.ProductId == pre.ProductId)).ToList();
            var currentUpdatedProduct = current.Where(cur => previous.Any(pre => pre.ProductId == cur.ProductId)).ToList();

            switch (bill.BillType)
            {
                case WarehousingBillType.Import:
                    // compare list sp cũ và mới
                    // nếu sp có ở list mới, không có ở list cũ -> insert sp hoặc update cộng tồn
                    // nếu sp không có ở list mới, có ở list cũ -> update trừ tồn
                    // nếu có ở cả 2 -> compare số lượng ra cộng hoặc trừ tồn
                    foreach (var product in currentAddProduct)
                    {
                        ModifyIncreaseStock(bill.StoreId, stocks, product, productToInsert, productToUpdate);
                    }
                    foreach (var product in currentRemovedProduct)
                    {
                        ModifyDecreaseStock(bill.StoreId, stocks, product, productToUpdate);
                    }
                    foreach (var product in currentUpdatedProduct)
                    {
                        var prev = previous.FirstOrDefault(p => p.ProductId == product.ProductId);
                        ModifyUpdateStock(bill.StoreId, stocks, prev, product, productToUpdate, 1);
                    }
                    break;
                case WarehousingBillType.Export:
                    // compare list sp cũ và mới
                    // nếu sp có ở list mới, không có ở list cũ -> update trừ tồn
                    // nếu sp không có ở list mới, có ở list cũ -> update cộng tồn
                    // nếu có ở cả 2 -> compare số lượng ra cộng hoặc trừ tồn
                    foreach (var product in currentAddProduct)
                    {
                        ModifyDecreaseStock(bill.StoreId, stocks, product, productToUpdate);
                    }
                    foreach (var product in currentRemovedProduct)
                    {
                        ModifyIncreaseStock(bill.StoreId, stocks, product, productToInsert, productToUpdate);
                    }
                    foreach (var product in currentUpdatedProduct)
                    {
                        var prev = previous.FirstOrDefault(p => p.ProductId == product.ProductId);
                        ModifyUpdateStock(bill.StoreId, stocks, prev, product, productToUpdate, -1);
                    }
                    break;
            }

            var updateStockPriceProducts = new List<Products>();
            var updateBillProducts = productToInsert.Union(productToUpdate).ToList();

            // nếu cập nhật phiếu nhập, cập nhật lại giá vốn sản phẩm nếu phiếu nhập là mới nhất
            if (bill.BillType == WarehousingBillType.Import && bill.IsFromWarehouseTransfer != true)
            {
                foreach (var updateProduct in updateBillProducts)
                {
                    var product = allProducts.FirstOrDefault(p => p.Id == updateProduct.ProductId);
                    if (product.LatestWarehousingBillId == bill.Id)
                    {
                        var currentProduct = current.FirstOrDefault(p => p.ProductId == updateProduct.ProductId);

                        product.StockPrice =
                            ((product.StockPriceBeforeLatest * product.TotalQuantityBeforeLatest)
                            + (currentProduct.Price * currentProduct.Quantity))
                            / (product.TotalQuantityBeforeLatest + currentProduct.Quantity);
                        currentProduct.UpdatedStockPrice = product.StockPrice;
                        product.EntryPrice = currentProduct.Price;
                        updateStockPriceProducts.Add(product);
                    }
                }
            }
            else
            {
                foreach (var updateProduct in updateBillProducts)
                {
                    var currentProduct = current.FirstOrDefault(p => p.ProductId == updateProduct.ProductId);
                    var product = allProducts.FirstOrDefault(p => p.Id == updateProduct.ProductId);
                    if (currentProduct != null)
                    {
                        currentProduct.CurrentStockPrice = product.StockPrice;
                    }
                }
            }

            if (productToUpdate.Any())
            {
                await _storeProductRepository.UpdateManyAsync(productToUpdate);
            }

            if (productToInsert.Any())
            {
                await _storeProductRepository.InsertManyAsync(productToInsert);
            }

            if (updateStockPriceProducts.Any())
            {
                await _productRepository.UpdateManyAsync(updateStockPriceProducts);
            }

            await _warehousingBillProductRepository.UpdateManyAsync(current);
            await uow.SaveChangesAsync();

            var productQuantities = (await _storeProductRepository.GetQueryableAsync())
                .Where(p => productIds.Contains(p.ProductId))
                .GroupBy(p => p.ProductId)
                .Select(p => new
                {
                    ProductId = p.Key,
                    TotalStockQuantity = p.Sum(product => product.StockQuantity)
                }).ToList();
            allProducts.ForEach(p =>
            {
                var productQuantity = productQuantities.FirstOrDefault(pq => pq.ProductId == p.Id);
                if (productQuantity != null)
                {
                    p.TotalQuantity = productQuantity.TotalStockQuantity;
                }
            });

            await _productRepository.UpdateManyAsync(allProducts);
            await uow.SaveChangesAsync();
        }



        private async Task UpdateStockOnBillDelete(
            WarehousingBill bill,
            List<WarehousingBillProduct>? current)
        {
            var uow = _unitOfWorkManager.Current;
            var productIds = current
                .Select(p => p.ProductId)
                .Distinct()
                .ToList();

            var stocks =
                (await _storeProductRepository.GetQueryableAsync())
                .Where(stock => stock.StoreId == bill.StoreId && productIds.Contains(stock.ProductId) && !stock.IsDeleted)
                .ToList();

            var allProducts = (await _productRepository.GetQueryableAsync())
                .Where(p => productIds.Contains(p.Id))
                .ToList();

            var productToInsert = new List<StoreProduct>();
            var productToUpdate = new List<StoreProduct>();

            switch (bill.BillType)
            {
                case WarehousingBillType.Import:
                    // khi xóa đơn nhập, update trừ tồn
                    foreach (var product in current)
                    {
                        ModifyDecreaseStock(bill.StoreId, stocks, product, productToUpdate);
                    }
                    break;
                case WarehousingBillType.Export:
                    // khi xóa đơn xuất, update cộng tồn
                    foreach (var product in current)
                    {
                        ModifyIncreaseStock(bill.StoreId, stocks, product, productToInsert, productToUpdate);
                    }
                    break;
            }

            var updateStockPriceProducts = new List<Products>();
            var updateBillProducts = productToInsert.Union(productToUpdate).ToList();

            // nếu xóa phiếu nhập, cập nhật lại giá vốn sản phẩm trong cửa hàng nếu phiếu nhập là mới nhất
            if (bill.BillType == WarehousingBillType.Import && bill.IsFromWarehouseTransfer != true)
            {
                foreach (var updateProduct in updateBillProducts)
                {
                    var product = allProducts.FirstOrDefault(p => p.Id == updateProduct.ProductId);
                    if (product.LatestWarehousingBillId == bill.Id)
                    {
                        var currentProduct = current.FirstOrDefault(p => p.ProductId == updateProduct.ProductId);

                        product.StockPrice = product.StockPriceBeforeLatest;
                        //updateProduct.StockQuantity = updateProduct.QuantityBeforeLatest;
                        product.LatestWarehousingBillId = null;

                        updateStockPriceProducts.Add(product);
                    }
                }
            }

            if (productToUpdate.Any())
            {
                await _storeProductRepository.UpdateManyAsync(productToUpdate);
            }

            if (productToInsert.Any())
            {
                await _storeProductRepository.InsertManyAsync(productToInsert);
            }

            if (updateStockPriceProducts.Any())
            {
                await _productRepository.UpdateManyAsync(updateStockPriceProducts);
            }

            await uow.SaveChangesAsync();

            var productQuantities = (await _storeProductRepository.GetQueryableAsync())
                .Where(p => productIds.Contains(p.ProductId))
                .GroupBy(p => p.ProductId)
                .Select(p => new
                {
                    ProductId = p.Key,
                    TotalStockQuantity = p.Sum(product => product.StockQuantity)
                }).ToList();
            allProducts.ForEach(p =>
            {
                var productQuantity = productQuantities.FirstOrDefault(pq => pq.ProductId == p.Id);
                if (productQuantity != null)
                {
                    p.TotalQuantity = productQuantity.TotalStockQuantity;
                }
            });

            await _productRepository.UpdateManyAsync(allProducts);
            await uow.SaveChangesAsync();
        }

        private async Task UpdateBillStockPrice(
            WarehousingBill bill,
            List<WarehousingBillProduct> current)
        {
            // tính lại tổng giá vốn của phiếu xuất tại thời điểm xuất kho
            if (bill.BillType == WarehousingBillType.Export)
            {
                var productIds = current.Select(p => p.ProductId).ToList();
                var allProductStockPrice =
                    (from sp in _storeProductRepository.GetQueryableAsync().Result
                    join p in _productRepository.GetQueryableAsync().Result on sp.ProductId equals p.Id
                    where
                        sp.StoreId == bill.StoreId
                        && productIds.Contains(p.Id)
                    select new
                    {
                        sp.ProductId,
                        p.StockPrice
                    }).ToList();

                var totalStockPrice =
                    (from c in current
                     join p in allProductStockPrice on c.ProductId equals p.ProductId
                     select new
                     {
                         c.ProductId,
                         c.Quantity,
                         p.StockPrice
                     }).Sum(p => p.Quantity * p.StockPrice);

                if (bill.TotalStockPrice != totalStockPrice)
                {
                    bill.TotalStockPrice = totalStockPrice;
                    await _warehousingBillRepository.UpdateAsync(bill);
                }
            }
        }

        private void ModifyIncreaseStock(
            Guid storeId,
            List<StoreProduct> stocks,
            WarehousingBillProduct product,
            List<StoreProduct> productToInsert,
            List<StoreProduct> productToUpdate)
        {
            var existStock = stocks.FirstOrDefault(p => p.ProductId == product.ProductId);
            if (existStock != null)
            {
                product.CurrentStockQuantity = existStock.StockQuantity;
                existStock.StockQuantity += product.Quantity;
                product.UpdatedStockQuantity = existStock.StockQuantity;
                productToUpdate.Add(existStock);
            }
            else
            {
                product.CurrentStockQuantity = 0;
                product.UpdatedStockQuantity = product.Quantity;
                product.UpdatedStockPrice = product.Price;
                productToInsert.Add(new StoreProduct
                {
                    StoreId = storeId,
                    ProductId = product.ProductId,
                    StockQuantity = product.Quantity,
                });
            }
        }

        private void ModifyDecreaseStock(
            Guid storeId,
            List<StoreProduct> stocks,
            WarehousingBillProduct product,
            List<StoreProduct> productToUpdate)
        {
            var existStock = stocks.FirstOrDefault(p => p.ProductId == product.ProductId);
            if (existStock != null)
            {
                if (existStock.StockQuantity < product.Quantity)
                {
                    var productDB = _productRepository.FindAsync(x => x.Id == product.ProductId).Result;
                    throw new BusinessException(
                        ErrorMessages.WarehousingBills.CanNotExport,
                        //ErrorMessages.StoreProducts.LackOfStock,
                        $"Sản phẩm {productDB?.Name} có tồn kho nhỏ hơn số lượng yêu cầu",
                        $"ProductId: {product.ProductId} Stock: {existStock.StockQuantity} Export: {product.Quantity}"); ;
                }
                product.CurrentStockQuantity = existStock.StockQuantity;
                existStock.StockQuantity -= product.Quantity;
                product.UpdatedStockQuantity = existStock.StockQuantity;
                productToUpdate.Add(existStock);
            }
            else
            {
                var productDB = _productRepository.FindAsync(x => x.Id == product.ProductId).Result;
                throw new BusinessException(
                    ErrorMessages.WarehousingBills.CanNotExport,
                    //ErrorMessages.StoreProducts.NotExist,
                    $"Sản phẩm {productDB?.Name} không tồn tại trong kho",
                    $"ProductId: {product.ProductId}");
            }
        }

        private void ModifyUpdateStock(
            Guid storeId,
            List<StoreProduct> stocks,
            WarehousingBillProduct previousProduct,
            WarehousingBillProduct currentProduct,
            List<StoreProduct> productToUpdate,
            int modifier)
        {
            // modifier:
            // case update phiếu nhập
            // nếu số hàng mới > số hàng cũ -> update cộng tồn
            // nếu số hàng mới < số hàng cũ -> update trừ tồn
            // ngược lại với phiếu xuất
            var quantityDiff = (currentProduct.Quantity - previousProduct.Quantity) * modifier;

            var existStock = stocks.FirstOrDefault(p => p.ProductId == currentProduct.ProductId);
            if (quantityDiff == 0)
            {
                if (currentProduct.Price != previousProduct.Price && existStock != null)
                {
                    productToUpdate.Add(existStock);
                }
                // nếu số lượng không khác nhau, không update tồn
                return;
            }

            if (existStock != null)
            {
                if (existStock.StockQuantity + quantityDiff < 0)
                {
                    throw new BusinessException(
                        ErrorMessages.WarehousingBills.CanNotExport,
                        ErrorMessages.StoreProducts.LackOfStock,
                        $"ProductId: {currentProduct.ProductId} Stock: {existStock.StockQuantity} Export: {quantityDiff}");
                }

                currentProduct.CurrentStockQuantity = existStock.StockQuantity;
                existStock.StockQuantity += quantityDiff;
                currentProduct.UpdatedStockQuantity = existStock.StockQuantity;
                productToUpdate.Add(existStock);
            }
            else
            {
                throw new BusinessException(
                    ErrorMessages.WarehousingBills.CanNotExport,
                    ErrorMessages.StoreProducts.NotExist,
                    $"ProductId: {currentProduct.ProductId}");
            }
        }

        private async void CompareObjectAndWriteLogsUpdate(WarehousingBillDto valueBefore, WarehousingBillDto valueAfter)
        {
            var compare = valueBefore.CompareObjectValues(valueAfter);
            if (compare.Item1.Count > 0 || compare.Item2.Count > 0)
            {
                List<WarehousingBillProductDto> listProdBefore = valueBefore.Products;
                List<WarehousingBillProductDto> listProdAfter = valueAfter.Products;
                var prodFromValue = listProdBefore.Where(x => !listProdAfter.Contains(x)).OrderBy(x => x.ProductId).ToList();
                var prodToValue = listProdAfter.Where(x => !listProdBefore.Contains(x)).OrderBy(x => x.ProductId).ToList();

                compare.Item1["Products"] = prodFromValue;
                compare.Item2["Products"] = prodToValue;
                var log = new Entities.WarehousingBillLogs()
                {
                    Action = EntityActions.Update,
                    WarehousingBillId = valueBefore.Id.Value,
                    ToValue = JsonSerializer.Serialize(compare.Item2),
                    FromValue = JsonSerializer.Serialize(compare.Item1),
                };
                _ = await _warehousingBillLogsRepository.InsertAsync(log);
            }
        }

        private void setProductName(ref WarehousingBillDto billDto)
        {
            var productIds = billDto.Products.Select(p => p.ProductId).Distinct().ToList();
            var products = _productRepository.GetQueryableAsync().Result.Where(p => productIds.Contains(p.Id)).ToList();
            foreach (var billProduct in billDto.Products)
            {
                var product = products.FirstOrDefault(p => p.Id == billProduct.ProductId);
                billProduct.ProductName = product?.Name;
            }
        }
        private void setProductCode(ref WarehousingBillDto billDto)
        {
            var productIds = billDto.Products.Select(p => p.ProductId).Distinct().ToList();
            var products = _productRepository.GetQueryableAsync().Result.Where(p => productIds.Contains(p.Id)).ToList();
            foreach (var billProduct in billDto.Products)
            {
                var product = products.FirstOrDefault(p => p.Id == billProduct.ProductId);
                billProduct.ProductName = product?.Code;
            }
        }

        public async Task AutoCreateWearhousingBillForReturnProduct(Guid id)
        {
            var uow = _unitOfWorkManager.Current;
            try
            {
                var returnProduct = await _customerReturnRepository.GetAsync(x => x.Id == id);
                var products = await _customerReturnProductRepository.GetListAsync(x => x.CustomerReturnId == id);
                var storeProduct = await _productRepository.GetListAsync(x => products.Select(y => y.ProductId).Contains(x.Id));

                List<WarehousingBillProduct> billProducts = new List<WarehousingBillProduct>();
                products.ForEach(item =>
                {
                    decimal stockPrice = 0;
                    var prod = storeProduct.First(x => x.Id == item.ProductId);
                    if (prod != null)
                    {
                        stockPrice = prod.StockPrice;
                    }
                    item.StockPrice = stockPrice;
                    var totalPriceBeforeDiscount = stockPrice * item.Quantity;
                    //var totalPrice = totalPriceBeforeDiscount - (item.DiscountUnit == MoneyModificationType.VND ? item.DiscountValue : (totalPriceBeforeDiscount * item.DiscountValue / 100));
                    billProducts.Add(new WarehousingBillProduct()
                    {
                        ProductId = item.ProductId.Value,
                        Price = stockPrice,
                        TotalPrice = totalPriceBeforeDiscount.Value,
                        TotalPriceBeforeDiscount = totalPriceBeforeDiscount.Value,
                        DiscountAmount = item.DiscountValue,
                        DiscountType = item.DiscountUnit,
                        Quantity = item.Quantity.Value
                    });
                });

                WarehousingBill bill = new WarehousingBill()
                {
                    BillType = Enums.WarehousingBillType.Import,
                    StoreId = returnProduct.StoreId.Value,
                    AudienceType = AudienceTypes.Customer,
                    AudienceId = returnProduct.CustomerId.Value,
                    SourceId = returnProduct.Id,
                    IsFromOrderConfirmation = false,
                    IsFromWarehouseTransfer = false,
                    IsFromCustomerReturn = true,
                    TotalPriceProduct = billProducts.Sum(x => x.TotalPrice),
                    TotalPrice = billProducts.Sum(x => x.TotalPrice),
                    TotalPriceBeforeTax = billProducts.Sum(x => x.TotalPrice),
                    DocumentDetailType = DocumentDetailType.ImportCustomer,
                    Note = returnProduct.PayNote
                };

                if (bill.CashPaymentAmount > 0)
                {
                    bill.CashPaymentHaveValueDate = _clock.Now;
                }
                if (bill.BankPaymentAmount > 0)
                {
                    bill.BankPaymentHaveValueDate = _clock.Now;
                }
                if (bill.VATAmount > 0)
                {
                    bill.VATHaveValueDate = _clock.Now;
                }
                var productIds = billProducts.Select(p => p.Id).ToList();

                await _warehousingBillRepository.InsertAsync(bill);
                await uow.SaveChangesAsync();
                var billx = await _warehousingBillRepository. FindAsync(p => p.Id == bill.Id);
                foreach (var billProduct in billProducts)
                {
                    billProduct.WarehousingBillId = bill.Id;
                }

                await _warehousingBillProductRepository.InsertManyAsync(billProducts);
                await uow.SaveChangesAsync();

                await UpdateStockOnBillCreate(bill, billProducts);
                await _entryService.AutoCreateEntryByCreateWarehousingBill(bill.Id, false);
                await _customerReturnProductRepository.UpdateManyAsync(products);
                await uow.SaveChangesAsync();

            }
            catch
            {
                await uow.RollbackAsync();
                throw;
            }
        }

        public async Task AutoDeleteWearhousingBillForReturnProduct(Guid id)
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var bill = await _warehousingBillRepository.GetAsync(p => p.Id == id && p.BillType == WarehousingBillType.Import);
                var currentBillProducts = await _warehousingBillProductRepository.GetListAsync(p => p.WarehousingBillId == bill.Id);

                await _entryService.AutoDeleteEntryByDeleteWarehousingBill(bill.Id);

                await _warehousingBillProductRepository.DeleteManyAsync(currentBillProducts);
                await _warehousingBillRepository.DeleteAsync(bill);

                await uow.SaveChangesAsync();
                await uow.CompleteAsync();

                // log
                await _warehousingBillLogsRepository.InsertAsync(new Entities.WarehousingBillLogs()
                {
                    Action = EntityActions.Delete,
                    WarehousingBillId = bill.Id,
                });
            }
            catch
            {
                await uow.RollbackAsync();
                throw;
            }
        }

        public async Task RebuildProductStock()
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                // reset product stock quantity = 0
                // reset product stock price = 0
                var allProducts = await _productRepository.GetListAsync();
                var allStoreProducts = await _storeProductRepository.GetListAsync();
                allProducts.ForEach(p =>
                {
                    p.TotalQuantity = 0;
                    p.TotalQuantityBeforeLatest = 0;
                    p.StockPrice = 0;
                    p.StockPriceBeforeLatest = 0;
                });
                await _productRepository.UpdateManyAsync(allProducts);
                allStoreProducts.ForEach(sp =>
                {
                    sp.StockQuantity = 0;
                });
                await _storeProductRepository.UpdateManyAsync(allStoreProducts);
                await uow.SaveChangesAsync();
                // recalculate stock quantity/price from all bill
                var allWarehousingBills = await _warehousingBillRepository.GetListAsync();
                var allWarehousingBillProducts = await _warehousingBillProductRepository.GetListAsync();

                allWarehousingBills = allWarehousingBills.OrderBy(p => p.CreationTime).ToList();

                foreach (var bill in allWarehousingBills)
                {
                    var currentBillProducts = allWarehousingBillProducts.Where(p => p.WarehousingBillId == bill.Id).ToList();
                    await UpdateStockOnBillCreate(bill, currentBillProducts);
                    await UpdateBillStockPrice(bill, currentBillProducts);
                }
                await uow.CompleteAsync();
            }
            catch
            {
                await uow.RollbackAsync();
                throw;
            }
        }

        public async Task<byte[]> ExportWarehousingBill(SearchWarehousingBillRequest request)
        {
            var draftTransferBillIds = new List<Guid>();
            if (!request.DraftTransferBillCode.IsNullOrWhiteSpace())
            {
                draftTransferBillIds = _warehouseTransferBillRepository
                    .GetQueryableAsync()
                    .Result
                    .Where(p => !p.IsDeleted && p.Code.Contains(request.DraftTransferBillCode))
                    .Select(p => p.Id)
                    .ToList();
            }
            var orderIds = new List<Guid>();
            if (!request.OrderCode.IsNullOrWhiteSpace())
            {
                orderIds = _saleOrderRepository
                    .GetQueryableAsync()
                    .Result
                    .Where(p => !p.IsDeleted && p.Code.Contains(request.OrderCode))
                    .Select(p => p.Id)
                    .ToList();
            }
            var userId = _userManager.Id;
            var userStores = (await _userStoreRepository.GetQueryableAsync()).Where(x => x.UserId == userId);
            var query =
                from bill in _warehousingBillRepository.GetQueryableAsync().Result
                join userStore in userStores on bill.StoreId equals userStore.StoreId
                where
                    !bill.IsDeleted
                    && (request.StoreIds == null || request.StoreIds.Count == 0 || request.StoreIds.Contains(bill.StoreId))
                    && (request.BillCode.IsNullOrWhiteSpace() || bill.Code.Contains(request.BillCode))
                    && (request.BillType == null || bill.BillType == request.BillType)
                    && (request.DateFrom == null || bill.CreationTime >= request.DateFrom)
                    && (request.DateTo == null || bill.CreationTime <= request.DateTo)
                    && (request.AudienceType == null || (bill.AudienceType == request.AudienceType))
                    && (request.DocumentDetailType == null || request.DocumentDetailType.Count == 0 || (request.DocumentDetailType.Contains(bill.DocumentDetailType)))
                    && (request.AudienceId == null || bill.AudienceId == request.AudienceId)
                    && (request.Note.IsNullOrWhiteSpace() || bill.Note.Contains(request.Note))
                    && (request.DraftTransferBillCode.IsNullOrWhiteSpace() || (bill.IsFromWarehouseTransfer == true && draftTransferBillIds.Contains(bill.SourceId ?? Guid.Empty)))
                    && (request.OrderCode.IsNullOrWhiteSpace() || (bill.IsFromOrderConfirmation == true && orderIds.Contains(bill.SourceId ?? Guid.Empty)))
                select bill;


            var billIds = query.Select(p => p.Id).ToList();

            var billProducts = await _warehousingBillProductRepository.GetListAsync(p => billIds.Contains(p.WarehousingBillId));
            
            var userIds = query.Select(x => x.CreatorId).ToList();
            var users = (await _userRepository.GetListAsync())
                    .Where(p => userIds.ToList().Contains(p.Id));
            var listStoreIds = query.Select(x => x.StoreId).ToList();
            var stores = await _storeRepository.GetListAsync(x => listStoreIds.Any(z => z == x.Id));
            var exportData = new List<ExportWarehousingBillResponse>();
            var suppliers = await _supplierRepository.GetListAsync();
            var customers = await _customerRepository.GetListAsync();
            foreach (var item in query)
            {
                var itemProducts = billProducts.Where(p => p.WarehousingBillId == item.Id).ToList();
                var numberOfProduct = itemProducts.Count();
                var totalProductAmount = itemProducts.Sum(p => p.Quantity);
                var sourceStoreName = stores.FirstOrDefault(x => x.Id == item.StoreId)?.Name;
                var creatorName = users.FirstOrDefault(x => x.Id == item.CreatorId)?.Name;
                var supplierCode = "";
                var supplierName = "";
                var audienceCode = "";
                var audienceName = "";
                var audiencePhone = "";
                var dateOfBirth = "";
                switch (item.AudienceType)
                {
                    case AudienceTypes.SupplierCN:
                    case AudienceTypes.SupplierVN:
                        var supplier = suppliers.FirstOrDefault(p => p.Id == item.AudienceId);
                        supplierCode = supplier?.Code;
                        supplierName = supplier?.Name;
                        break;
                    case AudienceTypes.Customer:
                        var customer = customers.FirstOrDefault(x => x.Id == item.AudienceId);
                        if (customer != null)
                        {
                            audienceCode = customer.Code;
                            audienceName = customer.Name;
                            audiencePhone = customer.PhoneNumber;
                            dateOfBirth = customer.DateOfBirth?.ToString("dd-MM-yyyy");
                        }
                        break;
                }

                exportData.Add(new ExportWarehousingBillResponse()
                {
                    Code = item.Code,
                    AudienceCode = "",
                    StoreName = sourceStoreName,
                    WarrantyCardCode = "",
                    InventorySheetCode = "",
                    SourceCode = "",
                    BillTypeName = item.BillType == WarehousingBillType.Import ? "Nhập chuyển kho" : "Xuất chuyển kho",
                    SupplierName = supplierName,
                    NumberOfProduct = 0, 
                    TotalProductAmount = 0,
                    TotalPrice = item.TotalPriceProduct,
                    TotalDiscountAmount = item.BillDiscountAmount.HasValue ? item.BillDiscountAmount.Value : 0,
                    CreatorName = creatorName,
                    AudienceName = audienceName,
                    DateOfBirth = dateOfBirth,
                    AudiencePhone = audiencePhone,
                    Note = item.Note,
                    VATBillCode = item.VATBillCode,
                    CreationTime = item.CreationTime.ToString("dd-MM-yyyy")
                });
            }
            return ExcelHelper.ExportExcel(exportData);
        }

        public async Task<byte[]> ExportProductXnk(SearchProductXnkRequest request)
        {
            var userStores = (await _userStoreRepository.GetQueryableAsync()).Where(x => x.UserId == _userManager.GetId());
            if (!userStores.Any())
                return ExcelHelper.ExportExcel(new List<ExportProductXnkResponse>()); ;
            if (request.StartDate != null)
            {
                request.StartDate = _clock.Normalize(request.StartDate.Value);
            }
            if (request.EndDate != null)
            {
                request.EndDate = _clock.Normalize(request.EndDate.Value).AddDays(1);
            }

            var orderIds = new List<Guid>();
            if (!request.OrderCode.IsNullOrWhiteSpace())
            {
                orderIds = _saleOrderRepository
                    .GetQueryableAsync()
                    .Result
                    .Where(p => !p.IsDeleted && p.Code.Contains(request.OrderCode))
                    .Select(p => p.Id)
                    .ToList();
            }
            var draftTransferBillIds = new List<Guid>();
            if (!request.DraftTransferBillCode.IsNullOrWhiteSpace())
            {
                draftTransferBillIds = _warehouseTransferBillRepository
                    .GetQueryableAsync()
                    .Result
                    .Where(p => !p.IsDeleted && p.Code.Contains(request.DraftTransferBillCode))
                    .Select(p => p.Id)
                    .ToList();
            }

            var queryBill = (await _warehousingBillRepository.GetQueryableAsync()).Where(x => userStores.Any(uS => uS.StoreId == x.StoreId));
            var queryBillProduct = (await _warehousingBillProductRepository.GetQueryableAsync()).Where(x => queryBill.Any(qB => qB.Id == x.WarehousingBillId));

            var query = from billProduct in queryBillProduct
                        join b in queryBill on billProduct.WarehousingBillId equals b.Id into bp
                        from bill in bp.DefaultIfEmpty()
                        join p in _productRepository.GetQueryableAsync().Result on billProduct.ProductId equals p.Id into pd
                        from product in pd.DefaultIfEmpty()
                        select new { billProduct, bill, product };

            query = query.WhereIf(!string.IsNullOrEmpty(request.ProductName), x => x.product.Name.Contains(request.ProductName) || x.product.Code.Contains(request.ProductName))
                        .WhereIf(!string.IsNullOrEmpty(request.ProductId), x => x.product.Code.Contains(request.ProductId))
                        .WhereIf(request.StoreIds != null && request.StoreIds.Any(), x => request.StoreIds.Any(z => z == x.bill.StoreId))
                        .WhereIf(!string.IsNullOrEmpty(request.BillId), x => x.bill.Code.Contains(request.BillId))
                        .WhereIf(request.SupplierIds != null, x => request.SupplierIds == x.bill.AudienceId && (x.bill.AudienceType == AudienceTypes.SupplierCN || x.bill.AudienceType == AudienceTypes.SupplierVN))
                        .WhereIf(request.BillType != null, x => request.BillType == x.bill.BillType)
                        .WhereIf(request.XnkTypes != null && request.XnkTypes.Any(), x => request.XnkTypes.Contains(x.bill.DocumentDetailType))
                        .WhereIf(!string.IsNullOrEmpty(request.Note), x => x.bill.Note.Contains(request.Note))
                        .WhereIf(request.StartDate != null, x => x.bill.CreationTime >= request.StartDate.Value)
                        .WhereIf(request.EndDate != null, x => x.bill.CreationTime < request.EndDate.Value)
                        .WhereIf(!request.OrderCode.IsNullOrWhiteSpace(), x => (x.bill.IsFromOrderConfirmation == true && orderIds.Contains(x.bill.SourceId ?? Guid.Empty)))
                        .WhereIf(!request.DraftTransferBillCode.IsNullOrWhiteSpace(), x => (x.bill.IsFromWarehouseTransfer == true && draftTransferBillIds.Contains(x.bill.SourceId ?? Guid.Empty)))
                        .OrderByDescending(p => p.bill.Code);
            var result =  query.Select(x => new SearchProductXnkResponse()
            {
                Id = x.billProduct.Id,
                Code = x.bill.Code,
                StoreId = x.bill.StoreId,
                ProductId = x.product.Id,
                ProductCode = x.product.Code,
                ProductName = x.product.Name,
                Price = x.billProduct.Price,
                CostPrice = x.product.StockPrice,
                Unit = x.billProduct.Unit,
                BillType = x.bill.BillType,
                TotalMoney = x.billProduct.TotalPrice,
                CreatorId = x.billProduct.CreatorId,
                CreationTime = x.bill.CreationTime,
                DiscountAmount = x.billProduct.DiscountAmount,
                Money = x.billProduct.Quantity * x.billProduct.Price,
                Quantity = x.billProduct.Quantity,
                IsEditable = !(x.bill.IsFromWarehouseTransfer == true),
                IsDeletable = !(x.bill.IsFromWarehouseTransfer == true),
                Inventory = x.billProduct.UpdatedStockQuantity
            }).ToList();
            var userIds = result.Select(x => x.CreatorId).ToList();
            var users = (await _userRepository.GetListAsync())
                    .Where(p => userIds.ToList().Contains(p.Id));
            var listStoreIds = result.Select(x => x.StoreId).ToList();
            var stores = await _storeRepository.GetListAsync(x => listStoreIds.Any(z => z == x.Id));
            var exportData = new List<ExportProductXnkResponse>();
            foreach (var item in result)
            {
                var sourceStoreName = stores.FirstOrDefault(x => x.Id == item.StoreId)?.Name;
                var creatorName = users.FirstOrDefault(x => x.Id == item.CreatorId)?.Name;

                exportData.Add(new ExportProductXnkResponse {
                    Code = item.Code,
                    CreationTime = item.CreationTime.ToString("dd-MM-yyyy"),
                    StoreName = sourceStoreName,
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,
                    Unit = item.Unit,
                    BillTypeName = item.BillType == WarehousingBillType.Import ? "Nhập chuyển kho" : "Xuất chuyển kho",
                    Quantity = item.Quantity,
                    Inventory = item.Inventory.HasValue ? item.Inventory.Value : 0,
                    Price = item.Price.HasValue ? item.Price.Value : 0,
                    CostPrice = item.CostPrice.HasValue ? item.CostPrice.Value : 0,
                    Money = item.Money.HasValue ? item.Money.Value : 0,
                    TotalPrice = item.TotalMoney.HasValue ? item.TotalMoney.Value : 0,
                    DiscountAmount = item.DiscountAmount.HasValue ? item.DiscountAmount.Value : 0,
                    CreatorName = creatorName,
                    Note = item.Note
                });
            }
            return ExcelHelper.ExportExcel(exportData);
        }
    }
}
