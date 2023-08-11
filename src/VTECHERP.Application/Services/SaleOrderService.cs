using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Timing;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using Volo.Abp.Validation;
using VTECHERP.Constants;
using VTECHERP.Domain.Shared.Helper.Excel.Model;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.Product;
using VTECHERP.DTOs.SaleOrders;
using VTECHERP.DTOs.WarehousingBills;
using VTECHERP.Entities;
using VTECHERP.Enums;
using VTECHERP.Extensions;

namespace VTECHERP.Services
{
    public class SaleOrderService : ISaleOrderService
    {
        private readonly IRepository<SaleOrders> _saleOrderRepository;
        private readonly ISupplierService _supplierService;
        private readonly IRepository<Suppliers> _supplierRepository;
        private readonly IRepository<Stores> _storeRepository;
        private readonly IRepository<Products> _productRepository;
        private readonly IRepository<StoreProduct> _storeProductRepository;
        private readonly IStoreService _storeService;
        private readonly IProductService _productService;
        private readonly ISaleOrderLineService _saleOrderLineService;
        private readonly IObjectMapper _objectMapper;
        private readonly IEntryService _entryService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<SaleOrderLines> _saleOrderLineRepository;
        private readonly IRepository<WarehousingBill> _warehousingBillRepository;
        private readonly IRepository<WarehousingBillProduct> _warehousingBillProductRepository;
        private readonly IRepository<SaleOrderLineConfirm> _saleOrderLineConfirmRepository;
        private readonly IWarehousingBillService _warehousingBillService;
        private readonly IClock _clock;
        private readonly ICommonService _commonService;
        private readonly IRepository<SupplierOrderReport> _supplierOrderReportRepository;
        private readonly IRepository<OrderTransport> _orderTransportRepository;
        private readonly IRepository<OrderTransportSale> _orderTransportSaleRepository;
        private readonly IRepository<PaymentReceipt> _paymentReceiptRepository;
        private readonly IRepository<IdentityUser> _identityUserRepository;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly IIdentityUserRepository _userRepository;

        private readonly IAttachmentService _attachmentService;

        public SaleOrderService(
            IRepository<SaleOrders> saleOrderRepository,
            IRepository<Suppliers> supplierRepository,
            IRepository<Stores> storeRepository,
            ISaleOrderLineService saleOrderLineService,
            ISupplierService supplierService,
            IStoreService storeService,
            IProductService productService,
            IObjectMapper objectMapper,
            IEntryService entryService,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<SaleOrderLines> saleOrderLineRepository,
            IRepository<WarehousingBill> warehousingBillRepository,
            IRepository<SaleOrderLineConfirm> saleOrderLineConfirmRepository,
            IRepository<WarehousingBillProduct> warehousingBillProductRepository,
            IWarehousingBillService warehousingBillService,
            IRepository<Products> productRepository,
            IRepository<StoreProduct> storeProductRepository,
            ICurrentUser currentUser,
            IClock clock,
            ICommonService commonService,
            IRepository<SupplierOrderReport> supplierOrderReportRepository,
            IRepository<OrderTransport> orderTransportRepository,
            IRepository<OrderTransportSale> orderTransportSaleRepository,
            IRepository<PaymentReceipt> paymentReceiptRepository,
            IRepository<IdentityUser> identityUserRepository,
            IRepository<UserStore> userStoreRepository,
            IIdentityUserRepository userRepository,
            IAttachmentService attachmentService)
        {
            _saleOrderRepository = saleOrderRepository;
            _supplierService = supplierService;
            _supplierRepository = supplierRepository;
            _storeService = storeService;
            _saleOrderLineService = saleOrderLineService;
            _objectMapper = objectMapper;
            _productService = productService;
            _entryService = entryService;
            _unitOfWorkManager = unitOfWorkManager;
            _currentUser = currentUser;
            _saleOrderLineRepository = saleOrderLineRepository;
            _warehousingBillRepository = warehousingBillRepository;
            _warehousingBillProductRepository = warehousingBillProductRepository;
            _productRepository = productRepository;
            _storeProductRepository = storeProductRepository;
            _saleOrderLineConfirmRepository = saleOrderLineConfirmRepository;
            _warehousingBillService = warehousingBillService;
            _clock = clock;
            _commonService = commonService;
            _supplierOrderReportRepository = supplierOrderReportRepository;
            _orderTransportRepository = orderTransportRepository;
            _orderTransportSaleRepository = orderTransportSaleRepository;
            _paymentReceiptRepository = paymentReceiptRepository;
            _storeRepository = storeRepository;
            _identityUserRepository = identityUserRepository;
            _userStoreRepository = userStoreRepository;
            _userRepository = userRepository;
            _attachmentService = attachmentService;
        }

        public async Task<object> AddAsync(SaleOrderCreateRequest request)
        {
            await ValidateAdd(request);
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var saleOrder = _objectMapper.Map<SaleOrderCreateRequest, SaleOrders>(request);

                saleOrder.Rate = request.Rate;
                saleOrder.Status = Enums.SaleOrder.Status.Unfinished;
                saleOrder.Confirm = Enums.SaleOrder.Confirm.Lack;
                saleOrder.CreatorId = _currentUser.Id;

                await _saleOrderRepository.InsertAsync(saleOrder);
                await uow.SaveChangesAsync();

                await _saleOrderLineService.AddRangeAsync(saleOrder.Id, request.SaleOrderlines.ToList());
                //var lstProduct = new List<Products>();
                //if (request.SaleOrderlines.ToList() != null && request.SaleOrderlines.ToList().Count >0)
                //{
                //    foreach (var item in request.SaleOrderlines.ToList())
                //    {
                //        var product = await _productRepository.FindAsync(x => x.Id == item.ProductId);
                //        if (product != null)
                //        {
                //            var storeProduct = await _storeProductRepository.GetQueryableAsync();
                //            var stockPrice = storeProduct.Where(x => x.ProductId == product.Id).Sum(i => i.StockQuantity);
                //            if (stockPrice == 0)
                //            {
                //                product.EntryPrice = item.RequestPrice * request.Rate + (product.RatePrice == null ? 0 : product.RatePrice.Value);
                //                product.StockPrice = item.RequestPrice.Value * request.Rate.Value + (product.RatePrice == null ? 0 : product.RatePrice.Value);
                //                lstProduct.Add(product);
                //            }
                //            //else
                //            //{
                //            //    product.StockPrice = (((product.StockPrice * stockPrice) + (item.RequestQuantity.Value *item.RequestPrice.Value)) / (stockPrice + item.RequestQuantity.Value));
                //            //    lstProduct.Add(product);
                //            //}
                //        }
                //    }
                //}
                //if (lstProduct.Count > 0)
                //{
                //    await _productRepository.UpdateManyAsync(lstProduct);
                //}
                await uow.SaveChangesAsync();
                await _entryService.AutoCreateEntryByCreateSupplierOrder(saleOrder.Id);
                await _commonService.TriggerCalculateSupplierOrderReport(saleOrder.SupplierId, saleOrder.OrderDate.Date,true);
                await uow.CompleteAsync();
                return saleOrder.Id;
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
        }

        public async Task<object> UpdateAsync(SaleOrderUpdateRequest request)
        {
            await ValidateUpdate(request);
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var saleOrder = await _saleOrderRepository.FindAsync(x => x.Id == request.Id);
                saleOrder.Package = request.Package;
                saleOrder.InvoiceNumber = request.InvoiceNumber;
                var oldOrderDate = saleOrder.OrderDate;

                saleOrder.OrderDate = request.OrderDate;
                saleOrder.Note = request.Note;
                saleOrder.LastModifierId = _currentUser.Id;

                await _saleOrderRepository.UpdateAsync(saleOrder);
                await uow.SaveChangesAsync();

                await _saleOrderLineService.UpdateRangeAsync(saleOrder.Id, request.SaleOrderLines.ToList());
                await uow.SaveChangesAsync();
                await _entryService.AutoUpdateEntryByUpdateSupplierOrder(saleOrder.Id);

                var dateOlder = oldOrderDate.Date < request.OrderDate.Date ? oldOrderDate.Date : request.OrderDate.Date;
                await _commonService.TriggerCalculateSupplierOrderReport(saleOrder.SupplierId, dateOlder, true);
                if(request.OrderDate.Date > oldOrderDate.Date)
                {
                    await _commonService.TriggerCalculateSupplierOrderReport(saleOrder.SupplierId, request.OrderDate.Date, true);
                }

                await uow.CompleteAsync();
                return request;
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
        }

        /// <summary>
        /// Hoàn thành đơn hàng thủ công 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Complete(Guid id)
        {

            var saleOrder = await _saleOrderRepository.GetAsync(x => x.Id.Equals(id));
            var saleOrderLine = await _saleOrderLineRepository.GetListAsync(x => x.OrderId.Equals(id));

            ValidateComplete(saleOrder, saleOrderLine);

            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                saleOrder.CompleteDate = _clock.Now;
                saleOrder.CompleteBy = _currentUser.Id;
                saleOrder.Status = SaleOrder.Status.Finished;
                saleOrder.ConpleteType = SaleOrder.ConpleteType.Manually;

                await _entryService.AutoCreateEntryByCompleteOrder(saleOrder.Id);
                await uow.SaveChangesAsync();

                await _commonService.TriggerCalculateSupplierOrderReport(saleOrder.SupplierId, saleOrder.CompleteDate.Value.Date, false);
                await uow.CompleteAsync();
            }
            catch (Exception)
            {
                await uow.RollbackAsync();
                throw;
            }
        }

        public async Task RebuildSupplierOrderReport()
        {
            var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var allReports = await _supplierOrderReportRepository.GetListAsync();
                // delete all reports
                if (allReports.Any())
                {
                    await _supplierOrderReportRepository.HardDeleteAsync(allReports);
                }
                var allOrders = await _saleOrderRepository.GetListAsync();
                await uow.SaveChangesAsync();
                if (allOrders.Any())
                {
                    // reset all order calculation
                    allOrders.ForEach(p =>
                    {
                        p.DebtBeforeCNY = 0;
                        p.DebtFinalCNY = 0;
                        p.DeliveryBeforeCNY = 0;
                        p.DeliveryFinalCNY = 0;
                        p.ConfirmedBeforeCNY = 0;
                        p.ConfirmedFinalCNY = 0;
                        p.PaidCNY = 0;
                    });
                    await _saleOrderRepository.UpdateManyAsync(allOrders);
                    await uow.SaveChangesAsync();
                    var orderSupplierGroups = allOrders
                        .GroupBy(p => new { Date = p.OrderDate.Date, p.SupplierId })
                        .Select(p => p.Key)
                        .OrderBy(p => p.Date)
                        .ToList();

                    foreach (var group in orderSupplierGroups)
                    {
                        await _commonService.TriggerCalculateSupplierOrderReport(group.SupplierId, group.Date.Date, true);
                    }
                }
                await uow.CompleteAsync();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await uow.RollbackAsync();
            }

        }


        private async Task CreateWarehousingBillAndEntry(List<SaleOrderProductCompleteRequest> productCompleteRequest, Entities.SaleOrders saleOrder, List<SaleOrderLines> saleOrderLine, List<SaleOrderLineConfirm> confirms, bool surplus = false)
        {

            #region Tạo phiếu nhập kho

            var listBillProductRequest = new List<WarehousingBillProductRequest>();

            decimal? moneyEntry = 0;

            foreach (var item in productCompleteRequest)
            {
                if (item != null && item.Quantity > 0)
                {
                    var billProduct = saleOrderLine.FirstOrDefault(x => x.Id == item.Id);
                    billProduct.ImportQuantity = confirms.Where(x => x.SaleOrderLineId == item.Id).Sum(x => x.Quantity) + item?.Quantity;
                    moneyEntry = (billProduct.RequestPrice ?? 0) * (item?.Quantity ?? 0);
                    listBillProductRequest.Add(new WarehousingBillProductRequest()
                    {
                        ProductId = billProduct.ProductId,
                        Price = (billProduct.RequestPrice ?? 0) * (item?.Quantity ?? 0) + (item.RatePrice ?? 1),
                        Quantity = item?.Quantity ?? 0,
                        Unit = "Lô",
                        Note = item?.Note,
                        TransportPrice = item.RatePrice ?? 0,
                    });

                }
            }

            var billRequest = new CreateWarehousingBillRequest()
            {
                Products = listBillProductRequest,
                BillType = Enums.WarehousingBillType.Import,
                StoreId = saleOrder.StoreId,
                AudienceType = AudienceTypes.SupplierCN,
                AudienceId = saleOrder.SupplierId,
                SourceId = saleOrder.Id,
                IsFromOrderConfirmation = true,
                IsFromWarehouseTransfer = false,
                DocumentDetailType = DocumentDetailType.ImportSupplier,

            };
            var billId = await _warehousingBillService.CreateBill(billRequest);

            #endregion

            #region Tạo Bút toán

            if (surplus)
            {
                await _entryService.AutoCreateEntryByConfirmOrderSurplus(saleOrder.Id, billId, moneyEntry);
            }
            else
            {
                await _entryService.AutoCreateEntryByConfirmSupplierOrder(saleOrder.Id, billId, moneyEntry);
            }

            #endregion

        }

        /// <summary>
        /// Xác nhận
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task Confirm(SaleOrderConfirmRequest request)
        {

            var saleOrder = await _saleOrderRepository.GetAsync(x => x.Id.Equals(request.Id));
            var saleOrderLine = await _saleOrderLineRepository.GetListAsync(x => x.OrderId.Equals(request.Id));

            ValidateConfirm(saleOrder, saleOrderLine, request);

            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var confirms = await _saleOrderLineConfirmRepository.GetListAsync(x => x.SaleOrderId.Equals(request.Id)); 
                var listSaleOrderLines = saleOrderLine.Where(x => request.ProductDeatails.Any(z => z.Id == x.Id));

                var listProductIds = listSaleOrderLines.Select(x => x.ProductId).ToList();

                var listProducts = await _productRepository.GetListAsync(x => listProductIds.Any(z => z == x.Id));
                var listConfirm = new List<SaleOrderLineConfirm>();
                foreach (var item in request.ProductDeatails)
                {
                    if (item.Quantity <= 0)
                    {
                        continue;
                    }
                    var saleOrderProduct = saleOrderLine.FirstOrDefault(x => x.Id.Equals(item.Id));
                    var isSurplus = saleOrderProduct.RequestQuantity < (saleOrderProduct.ImportQuantity ?? 0) + (item.Quantity ?? 0);
                    var surplusQuantity = (saleOrderProduct.ImportQuantity ?? 0) + (item.Quantity ?? 0) - saleOrderProduct.RequestQuantity;
                    if (surplusQuantity < 0)
                    {
                        surplusQuantity = 0;
                    }
                    var confirm = new SaleOrderLineConfirm()
                    {
                        SupplierId = saleOrder.SupplierId,
                        SaleOrderId = saleOrder.Id,
                        SaleOrderLineId = item.Id,
                        Quantity = item.Quantity,
                        UpdatedImportedQuantity = (saleOrderProduct.ImportQuantity ?? 0) + (item.Quantity ?? 0),
                        RequestQuantity = saleOrderProduct.RequestQuantity ?? 0,
                        SurplusQuantity = surplusQuantity ?? 0,
                        IsSurplus = isSurplus,
                        RatePrice = item.RatePrice,
                        EntryPrice = saleOrderProduct.RequestPrice,
                        Note = item.Note,
                        ConfirmDate = _clock.Normalize(DateTime.UtcNow.Date)
                    };

                    listConfirm.Add(confirm);

                    //update sp
                    var product = listProducts.FirstOrDefault(x => x.Id == saleOrderProduct.ProductId);
                    if (product != null)
                    {
                        //product.RatePrice = item.RatePrice;
                        product.EntryPrice = (confirm.EntryPrice * saleOrder.Rate) + confirm.RatePrice;
                    }
                }

                //var listBillProductRequestLack = new List<WarehousingBillProductRequest>();
                var listBillProductRequestEnough = new List<WarehousingBillProductRequest>();
                var listBillProductRequestExcess = new List<WarehousingBillProductRequest>();

                foreach (var item in request.ProductDeatails)
                {
                    if (item != null && item.Quantity > 0)
                    {
                        var billProduct = saleOrderLine.FirstOrDefault(x => x.Id == item.Id);

                        // Số lượng nhập hiện tại
                        var inputQuantity = confirms.Where(x => x.SaleOrderLineId == item.Id).Sum(x => x.Quantity);

                        // Số lượng nhập sau khi confirm
                        billProduct.ImportQuantity = inputQuantity + item?.Quantity;

                        var modelBillProduct = new WarehousingBillProductRequest()
                        {
                            ProductId = billProduct.ProductId,
                            Price = (billProduct.RequestPrice ?? 0) * (saleOrder.Rate ??1),
                            Quantity = item?.Quantity ?? 0,
                            Unit = "Lô",
                            Note = item?.Note,
                            TransportPrice = item.RatePrice ?? 0,
                        };

                        if (billProduct.RequestQuantity > billProduct.ImportQuantity)
                        {
                            listBillProductRequestEnough.Add(modelBillProduct);
                        }
                        else if (billProduct.RequestQuantity < billProduct.ImportQuantity)
                        {
                            if (billProduct.RequestQuantity > inputQuantity)
                            {
                                var modelBillProductExcess = modelBillProduct.Clone();

                                var quantityEnough = billProduct.RequestQuantity - inputQuantity;
                                modelBillProduct.Quantity = quantityEnough ?? 0;
                                listBillProductRequestEnough.Add(modelBillProduct);
                                modelBillProductExcess.Quantity = (item?.Quantity ?? 0) - (quantityEnough ?? 0);
                                listBillProductRequestExcess.Add(modelBillProductExcess);
                            }
                            else
                            {
                                listBillProductRequestExcess.Add(modelBillProduct);
                            }
                        }
                        else
                        {
                            listBillProductRequestEnough.Add(modelBillProduct);
                        }
                    }
                }

                if (listConfirm.Any())
                {
                    await _saleOrderLineConfirmRepository.InsertManyAsync(listConfirm);
                }
                await uow.SaveChangesAsync();

                //Case: Confirm thiếu
                saleOrder.Confirm = Enums.SaleOrder.Confirm.Lack;

                if (saleOrderLine.All(x => x.RequestQuantity == x.ImportQuantity))
                {
                    ////Case: Confirm đủ
                    saleOrder.Confirm = Enums.SaleOrder.Confirm.Enough;
                    saleOrder.ConpleteType = Enums.SaleOrder.ConpleteType.Auto;
                    saleOrder.Status = Enums.SaleOrder.Status.Finished;
                }
                else if(saleOrderLine.All(x=>x.RequestQuantity <= x.ImportQuantity))
                {
                    ////Case: Confirm thừa
                    saleOrder.Confirm = Enums.SaleOrder.Confirm.Excess;
                    saleOrder.ConpleteType = Enums.SaleOrder.ConpleteType.Auto;
                    saleOrder.Status = Enums.SaleOrder.Status.Finished;
                }

                saleOrder.ConfirmDate = _clock.Now;
                saleOrder.ConfirmBy = _currentUser.Id;
                saleOrder.Note = request.Note;
                saleOrder.Package = request.Package.Value;
                saleOrder.InvoiceNumber = request.InvoiceNumber;

                await _saleOrderRepository.UpdateAsync(saleOrder);

                var billRequest = new CreateWarehousingBillRequest()
                {

                    BillType = Enums.WarehousingBillType.Import,
                    StoreId = saleOrder.StoreId,
                    AudienceType = AudienceTypes.SupplierCN,
                    AudienceId = saleOrder.SupplierId,
                    SourceId = saleOrder.Id,
                    IsFromOrderConfirmation = true,
                    IsFromWarehouseTransfer = false,
                    DocumentDetailType = DocumentDetailType.ImportSupplier,
                };

                if (listBillProductRequestEnough.Count > 0)
                {
                    var billCreate = billRequest.Clone();
                    billCreate.Products = listBillProductRequestEnough;
                    var billId = await _warehousingBillService.CreateBill(billCreate, true, true);

                    var money = listBillProductRequestEnough.Sum(x => (x.Price * x.Quantity));

                    await _entryService.AutoCreateEntryByConfirmSupplierOrder(saleOrder.Id, billId, money, true);
                }
                if (listBillProductRequestExcess.Count > 0)
                {
                    var billCreate = billRequest.Clone();
                    billCreate.Products = listBillProductRequestExcess;
                    var billId = await _warehousingBillService.CreateBill(billCreate, true, true);

                    var money = listBillProductRequestExcess.Sum(x => (x.Price * x.Quantity));

                    await _entryService.AutoCreateEntryByConfirmOrderSurplus(saleOrder.Id, billId, money);
                }


                
                await _productRepository.UpdateManyAsync(listProducts);
                await _saleOrderLineRepository.UpdateManyAsync(saleOrderLine);
                await uow.SaveChangesAsync();
                await _commonService.TriggerCalculateSupplierOrderReport(saleOrder.SupplierId, _clock.Now.Date, false);

                await uow.SaveChangesAsync();
                await uow.CompleteAsync();
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                throw;
            }
        }

        private void ValidateConfirm(SaleOrders saleOrder, List<SaleOrderLines> saleOrderLine, SaleOrderConfirmRequest request)
        {
            var validationErrors = new List<ValidationResult>();
            if (saleOrder == null)
                throw new BusinessException(ErrorMessages.SaleOrder.NotExist);

            if (saleOrderLine == null || saleOrderLine.Count == 0)
                throw new BusinessException(ErrorMessages.SaleOrderLine.NotExist);

            var requestProductIds = request.ProductDeatails.Select(x => x.Id).ToList();

            if (saleOrderLine.Count(x => requestProductIds.Any(z => z == x.Id)) != requestProductIds.Count)
            {
                // Có product confirm không tồn tại
                throw new BusinessException(ErrorMessages.SaleOrderLine.NotExist, "One or more products don't exist");
            }

            if (saleOrder.Status == Enums.SaleOrder.Status.Finished)
                validationErrors.Add(new ValidationResult(
                    ErrorMessages.SaleOrder.IsComplete,
                    new List<string> { ErrorMessages.SaleOrder.IsComplete }));

            var totalRequestQuatity = saleOrderLine.Sum(x => x.RequestQuantity);
            var importQuantity = saleOrderLine.Sum(x => x.ImportQuantity);

            //if (totalRequestQuatity <= importQuantity)
            //    validationErrors.Add(new ValidationResult(
            //         ErrorMessages.SaleOrder.ConfirmNotLack,
            //         new List<string> { ErrorMessages.SaleOrder.ConfirmNotLack }));

            if (validationErrors.Any())
                throw new AbpValidationException(validationErrors);
        }

        private void ValidateComplete(SaleOrders saleOrder, List<SaleOrderLines> saleOrderLine)
        {
            var validationErrors = new List<ValidationResult>();

            if (saleOrder == null)
                throw new BusinessException(ErrorMessages.SaleOrder.NotExist);

            var totalRequestQuatity = saleOrderLine.Sum(x => x.RequestQuantity);
            var importQuantity = saleOrderLine.Sum(x => x.ImportQuantity);

            if (saleOrder.Status == Enums.SaleOrder.Status.Finished)
                validationErrors.Add(new ValidationResult(
                    ErrorMessages.SaleOrder.IsComplete,
                    new List<string> { ErrorMessages.SaleOrder.IsComplete }));

            //if (totalRequestQuatity <= importQuantity)
            //    validationErrors.Add(new ValidationResult(
            //         ErrorMessages.SaleOrder.ConfirmNotLack,
            //         new List<string> { ErrorMessages.SaleOrder.ConfirmNotLack }));

            if (validationErrors.Any())
                throw new AbpValidationException(validationErrors);
        }

        /// <summary>
        /// Get thông tin xác nhận phiếu đặt hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<GetDetailConfirmByIdResponse> GetDetailConfirmById(Guid id)
        {

            var saleOrder = await _saleOrderRepository.GetAsync(x => x.Id.Equals(id));
            if (saleOrder == null)
                throw new BusinessException(ErrorMessages.SaleOrder.NotExist + ": SaleOrderId => " + id.ToString());

            var saleOrderProducts = await _saleOrderLineRepository.GetListAsync(x => x.OrderId == id);

            var productIds = saleOrderProducts.Select(x => x.ProductId).ToList();
            var products = (await _productRepository.GetDbSetAsync()).AsNoTracking().Where(x => productIds.Any(z => z == x.Id));

            var confirmeds = (await _saleOrderLineConfirmRepository.GetDbSetAsync()).AsNoTracking().Where(x => x.SaleOrderId == id);

            var result = _objectMapper.Map<SaleOrders, GetDetailConfirmByIdResponse>(saleOrder);
            result.PackageRes = saleOrder.Package;

            var resProducts = new List<GetDetailConfirmProudctByIdResponse>();
            foreach (var item in saleOrderProducts)
            {
                var confirmInpurt = confirmeds.Where(x => x.SaleOrderLineId == item.Id);
                var product = products.FirstOrDefault(x => x.Id == item.ProductId);
                resProducts.Add(new GetDetailConfirmProudctByIdResponse()
                {
                    Id = item.Id,
                    Code = item.Code,
                    OrderId = item.OrderId,
                    ImportQuantity = item.ImportQuantity,
                    RequestQuantity = item.RequestQuantity,
                    RequestPrice = item.RequestPrice,
                    UnitPriceVnd = item.RequestPrice * saleOrder.Rate,
                    Note = item.Note,
                    ProductId = item.ProductId,
                    ProductName = product?.Name,
                    TotalRequestVnd = item.RequestQuantity * item.RequestPrice * saleOrder.Rate,
                    TotalInputVnd = confirmInpurt.Sum(x => x.EntryPrice * x.Quantity),
                    RatePrice = product?.RatePrice
                });
            }

            result.TotalRequestVnd = resProducts.Sum(x => x.TotalRequestVnd);
            result.TotalInputVnd = resProducts.Sum(x => x.TotalInputVnd);
            result.SupplierName = (await _supplierService.GetByIdAsync(result.SupplierId)).Name;
            result.StoreName = (await _storeService.GetByIdAsync(result.StoreId)).Name;

            result.DetailProudcts = resProducts;

            return result;
        }

        public async Task Delete(Guid id)
        {

            var saleOrder = await _saleOrderRepository.GetAsync(x => x.Id.Equals(id));
            var saleOrderLine = await _saleOrderLineRepository.GetListAsync(x => x.OrderId.Equals(id));

            ValidateDelete(saleOrder, saleOrderLine);

            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                //Tự động xóa bút toán và công nợ
                await _entryService.AutoDeleteEntryOnDeleteSaleOrder(id);

                await _saleOrderLineRepository.DeleteManyAsync(saleOrderLine);

                await _saleOrderRepository.DeleteAsync(saleOrder);

                await uow.SaveChangesAsync();
                await _commonService.TriggerCalculateSupplierOrderReport(saleOrder.SupplierId, saleOrder.OrderDate.Date, false);
                await uow.CompleteAsync();
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                throw;
            }
        }

        private void ValidateDelete(SaleOrders saleOrder, List<SaleOrderLines> saleOrderLine)
        {
            if (saleOrder == null)
                throw new BusinessException(ErrorMessages.SaleOrder.NotExist);

            var validationErrors = new List<ValidationResult>();

            if (saleOrder.Status == Enums.SaleOrder.Status.Finished)
                validationErrors.Add(new ValidationResult(
                     ErrorMessages.SaleOrder.CanNotDeleteStatusFinished,
                     new List<string> { ErrorMessages.SaleOrder.CanNotDeleteStatusFinished }));

            if (saleOrderLine.Any(x=>x.ImportQuantity > 0))
                validationErrors.Add(new ValidationResult(
                     ErrorMessages.SaleOrder.CanNotDeleteStatusConfirmed,
                     new List<string> { ErrorMessages.SaleOrder.CanNotDeleteStatusConfirmed }));

            if (validationErrors.Any())
                throw new AbpValidationException(validationErrors);

        }

        public async Task<object> GetListAsync(SearchSaleOrderRequest input)
        {
            var result = (await GetListSaleOrderAsync(input)).OrderByDescending(x => x.CreationTime).ToList().DistinctBy(x => x.Code);
            var datas = result.Skip(input.Offset).Take(input.PageSize).ToList();
            var attachments = (await _attachmentService.ListAttachmentByObjectIdAsync(datas.Select(x => x.Id).ToList())).OrderBy(x => x.CreationTime).ToList();
            foreach(var data in datas)
            {
                data.Attachments = attachments.Where(x=>x.ObjectId == data.Id).ToList() ?? new List<DTOs.Attachment.DetailAttachmentDto>();
            }    
            var currentUser = (await _userRepository.FindAsync(_currentUser.GetId()));
            return new { isVtech = currentUser.GetProperty("IsVTech", false), Total = result.Count(), data = datas };
        }

        public async Task<object> GetByIdAsync(Guid id)
        {
            var saleOrder = await _saleOrderRepository.FirstOrDefaultAsync(x => x.Id == id);
            if (saleOrder == null)
                return null;

            var result = _objectMapper.Map<SaleOrders, SaleOrderDetailDto>(saleOrder);
            result.PackageRes = saleOrder.Package;
            result.SupplierName = (await _supplierService.GetByIdAsync(result.SupplierId)).Name;
            result.StoreName = (await _storeService.GetByIdAsync(result.StoreId)).Name;
            result.SaleOrderLineDetailDtos = await _saleOrderLineService.GetByOrderIdAsync(id);
            result.Attachments = await _attachmentService.GetAttachmentByObjectIdAsync(id);
            return result;
        }

        public async Task<byte[]> ExportAsync(SearchSaleOrderRequest request)
        {
            var saleOrders = (await GetListSaleOrderAsync(request)).DistinctBy(x => x.Code).ToList();
            var firstDate = saleOrders.Min(p => p.OrderDate);
            var orderIds = saleOrders.Select(p => p.Id).ToList();
            var supplierIds = saleOrders.Select(p => p.SupplierId).Distinct().ToList();
            var saleOrderLines = await _saleOrderLineRepository.GetListAsync(p => orderIds.Contains(p.OrderId));
            var suppliers = await _supplierService.GetIdCodeNameAsync();
            var workbook = new CustomWorkBook();
            var supplierSaleOrderReport = await _supplierOrderReportRepository.GetListAsync(p => supplierIds.Contains(p.SupplierId) && p.Date >= firstDate);

            var orderTranportSales = await _orderTransportSaleRepository.GetListAsync(p => orderIds.Contains(p.OrderSaleId));
            var tranportIds = orderTranportSales.Select(p => p.OrderTransportId).ToList();
            var allOrderTransports = await _orderTransportRepository.GetListAsync(p => tranportIds.Contains(p.Id));
            var transporterIds = allOrderTransports.Select(p => p.TransporterId).ToList();
            var allTransporters = suppliers.Where(p => transporterIds.Contains(p.Id)).ToList();
            foreach (var supplierId in supplierIds)
            {
                var supplier = suppliers.FirstOrDefault(p => p.Id == supplierId) ?? new MasterDataDTO();
                var supplierSaleOrders = saleOrders.Where(p => p.SupplierId == supplierId).ToList();
                var supplierSaleOrderIds = supplierSaleOrders.Select(p => p.Id).ToList();
                var orderProducts = saleOrderLines.Where(p => supplierSaleOrderIds.Contains(p.OrderId)).ToList();
                var productIds = orderProducts.Select(p => p.ProductId).Distinct().ToList();
                var allProducts = await _productService.GetByIdsAsync(productIds);

                supplierSaleOrders = supplierSaleOrders.OrderBy(p => p.CreationTime.Value.Date).ToList();
                foreach (var item in supplierSaleOrders)
                {
                    item.DeliveryFinalCNY = item.TotalPriceNDT.Value - item.ConfirmedFinalCNY + item.DeliveryBeforeCNY;
                }

                var sheet = CreateSupplierOrderSheet(supplier, supplierSaleOrders, orderProducts, allProducts, allOrderTransports, orderTranportSales, allTransporters);
                workbook.Sheets.Add(sheet);
            }

            return ExcelHelper.ExportExcel(workbook);
        }

        private CustomSheet CreateSupplierOrderSheet(
            MasterDataDTO supplier,
            List<SaleOrderDTO> allSaleOrders,
            List<SaleOrderLines> orderProducts,
            List<ProductDetailDto> allProducts,
            List<OrderTransport> allOrderTransports,
            List<OrderTransportSale> allOrderTransportSales,
            List<MasterDataDTO> allTransporters)
        {
            // Tên sheet tối đa 32 ký tự
            var supplierShortName = supplier.Name.Substring(0, supplier.Name.Length > 32 ? 32 : supplier.Name.Length);
            var sheet = new CustomSheet(supplierShortName);

            // Tên nhà cung cấp
            var supplierNameTable = new CustomDataTable()
            {
                StartCell = "A1",
                RowDirection = Directions.Vertical,
                Rows = new List<DataRow>
                {
                    new DataRow(new HeaderCell("Nhà cung cấp: ")),
                    new DataRow(new Cell(supplier.Name)),
                }
            };
            sheet.Tables.Add(supplierNameTable);
            
            var firstOrder = allSaleOrders.OrderBy(p => p.OrderDate.Value.Date).ThenBy(p => p.Code).FirstOrDefault().Clone();
            var finalOrder = allSaleOrders.OrderByDescending(p => p.OrderDate.Value.Date).ThenByDescending(p => p.Code).FirstOrDefault().Clone();
            var totalPaid = allSaleOrders.Sum(p => p.PaidCNY);
            // Nợ đầu kỳ
            var beginOfPeriodDebtTable = new CustomDataTable()
            {
                StartCell = "B3",
                RowDirection = Directions.Horizontal,
                Rows = new List<DataRow>
                {
                    new DataRow(new HeaderCell("Nợ đầu kỳ: ")),
                    new DataRow(new Cell(firstOrder.DebtBeforeCNY)),
                }
            };
            sheet.Tables.Add(beginOfPeriodDebtTable);
            // Đã thanh toán
            var paidTable = new CustomDataTable()
            {
                StartCell = "D3",
                RowDirection = Directions.Horizontal,
                Rows = new List<DataRow>
                {
                    new DataRow(new HeaderCell("Đã thanh toán: ")),
                    new DataRow(new Cell(totalPaid)),
                }
            };
            sheet.Tables.Add(paidTable);
            // Tổng nợ
            var totalDebtTable = new CustomDataTable()
            {
                StartCell = "F3",
                RowDirection = Directions.Horizontal,
                Rows = new List<DataRow>
                {
                    new DataRow(new HeaderCell("Tổng nợ: ")),
                    new DataRow(new Cell(finalOrder.DebtFinalCNY)),
                }
            };
            sheet.Tables.Add(totalDebtTable);
            // Đã về
            var importedAmountTable = new CustomDataTable()
            {
                StartCell = "H3",
                RowDirection = Directions.Horizontal,
                Rows = new List<DataRow>
                {
                    new DataRow(new HeaderCell("Đã về: ")),
                    new DataRow(new Cell(finalOrder.ConfirmedFinalCNY)),
                }
            };
            sheet.Tables.Add(importedAmountTable);
            // Đang trên đường
            var inDeliveryAmountTable = new CustomDataTable()
            {
                StartCell = "J3",
                RowDirection = Directions.Horizontal,
                Rows = new List<DataRow>
                {
                    new DataRow(new HeaderCell("Đang trên đường: ")),
                    new DataRow(new Cell(finalOrder.DeliveryFinalCNY)),
                }
            };
            sheet.Tables.Add(inDeliveryAmountTable);

            // các bảng đơn hàng
            var row = 6;
            allSaleOrders = allSaleOrders.OrderByDescending(p => p.OrderDate.Value.Date).ThenByDescending(p => p.Code).ToList();
            foreach (var order in allSaleOrders)
            {
                var currentOrderProducts = orderProducts.Where(p => p.OrderId == order.Id).ToList();
                var productCount = currentOrderProducts.Count;
                var orderTransportId = allOrderTransportSales.FirstOrDefault(p => p.OrderSaleId == order.Id)?.OrderTransportId;
                var orderTransport = allOrderTransports.FirstOrDefault(p => p.Id == orderTransportId);
                var transporter = allTransporters.FirstOrDefault(p => p.Id == orderTransport?.TransporterId);
                var orderInfoTable = new CustomDataTable
                {
                    StartColumnIndex = 1,
                    StartRowIndex = row,
                    RowDirection = Directions.Horizontal,
                    Rows = new List<DataRow>()
                    {
                        new DataRow(
                            new HeaderCell("Ngày đặt"),
                            new HeaderCell("Số hóa đơn"),
                            new HeaderCell("Mã vận đơn"),
                            new HeaderCell("Nhà vận chuyển"),
                            new HeaderCell("Tên sản phẩm"),
                            new HeaderCell("SL theo đơn"),
                            new HeaderCell("SL đã về"),
                            new HeaderCell("SL chưa về"),
                            new HeaderCell("Giá tệ"),
                            new HeaderCell("Tổng tiền")
                        ),
                        new DataRow(
                            // ngày đặt
                            new Cell(order.OrderDate.Value.ToString("dd-MM-yyyy"))
                            {
                                RowSpan = productCount,
                                Format = "@"
                            },
                            // số hóa đơn
                            new Cell(order.InvoiceNumber){
                                RowSpan = productCount
                            },
                            // Mã vận đơn
                            new Cell(orderTransport?.TransportCode){
                                RowSpan = productCount
                            },
                            // Nhà vận chuyển
                            new Cell(transporter?.Name){
                                RowSpan = productCount
                            }
                        )
                    }
                };
                sheet.Tables.Add(orderInfoTable);
                var productTable = new CustomDataTable()
                {
                    StartRowIndex = row + 1,
                    StartColumnIndex = 5,
                    RowDirection = Directions.Horizontal,
                };
                foreach (var orderProduct in currentOrderProducts)
                {
                    var productDetail = allProducts.FirstOrDefault(p => p.Id == orderProduct.ProductId);
                    productTable.Rows.Add(
                        new DataRow(
                            // tên sp
                            new Cell(productDetail.Name),
                            // SL theo đơn
                            new Cell(orderProduct.RequestQuantity),
                            // SL đã về
                            new Cell(orderProduct.ImportQuantity),
                            // SL chưa về
                            new Cell(orderProduct.RequestQuantity - orderProduct.ImportQuantity < 0 ? 0 : orderProduct.RequestQuantity - orderProduct.ImportQuantity),
                            // giá tệ
                            new Cell(orderProduct.RequestPrice),
                            // tổng tiền
                            new Cell(orderProduct.RequestPrice * orderProduct.RequestQuantity)
                            )
                    );
                }
                sheet.Tables.Add(productTable);
                row += productCount + 1;
                // thông tin tổng hợp đơn
                var orderTotalInfoTable = new CustomDataTable()
                {
                    StartRowIndex = row,
                    StartColumnIndex = 1,
                    RowDirection = Directions.Vertical,
                    Rows = new List<DataRow>
                    {
                        new DataRow(
                            new HeaderCell("Nợ đến đơn"),
                            new HeaderCell("Đã thanh toán"),
                            new HeaderCell("Tổng nợ"),
                            new HeaderCell("Đã về"),
                            new HeaderCell("Đang trên đường")
                            ),
                        new DataRow(
                            // nợ đến đơn
                            new Cell(order.DebtBeforeCNY),
                            // đã thanh toán
                            new Cell(order.PaidCNY),
                            // tổng nợ
                            new Cell(order.DebtFinalCNY),
                            // đã về
                            new Cell(order.ConfirmedFinalCNY),
                            // đang trên đường
                            new Cell(order.DeliveryFinalCNY)
                            ),
                    }
                };

                sheet.Tables.Add(orderTotalInfoTable);
                row += 6;
            }

            return sheet;
        }

        private async Task ValidateAdd(SaleOrderCreateRequest request)
        {
            var validationErrors = new List<ValidationResult>();
            var supplierExist = await _supplierService.Exist(request.SupplierId);
            if (!supplierExist)
                validationErrors.Add(new ValidationResult(
                    ErrorMessages.Supplier.NotExist,
                    new List<string> { "SupplierId", request.SupplierId.ToString() }));

            var storeExist = await _storeService.Exist(request.StoreId);
            if (!storeExist)
                validationErrors.Add(new ValidationResult(
                    ErrorMessages.Store.NotExist,
                    new List<string> { "StoreId", request.StoreId.ToString() }));

            var invoiceNumberExist = await _saleOrderRepository.FindAsync(x => x.InvoiceNumber == request.InvoiceNumber && x.SupplierId == request.SupplierId);
            if (invoiceNumberExist != null)
                validationErrors.Add(new ValidationResult(
                    ErrorMessages.SaleOrder.Existed,
                    new List<string> { "SupplierId => InvoiceNumber", string.Format("{0} => {1}", request.SupplierId, request.InvoiceNumber) }));

            if (!request.Rate.HasValue || request.Rate < 0)
                validationErrors.Add(new ValidationResult(
                    ErrorMessages.SaleOrder.NumericLessThanZero,
                    new List<string> { "Rate", request.Rate.ToString() }));

            if (request.InvoiceNumber.IsNullOrWhiteSpace())
                validationErrors.Add(new ValidationResult(
                    ErrorMessages.SaleOrder.NullOrWhiteSpace,
                    new List<string> { "InvoiceNumber", request.InvoiceNumber }));

            if (!request.SaleOrderlines.Any())
                validationErrors.Add(new ValidationResult(
                    ErrorMessages.SaleOrderLine.Null,
                    new List<string> { "SaleOrderLine", "Null" }));
            else
            {

                foreach (var item in request.SaleOrderlines)
                {
                    var productExist = await _productService.Exist(item.ProductId);
                    if (!productExist)
                        validationErrors.Add(new ValidationResult(
                            ErrorMessages.Product.NotExist,
                            new List<string> { "ProductId", item.ProductId.ToString() }));

                    if (!item.RequestQuantity.HasValue || item.RequestQuantity < 0 || !item.RequestPrice.HasValue || item.RequestPrice < 0 || !item.SuggestedPrice.HasValue || item.SuggestedPrice < 0)
                        validationErrors.Add(new ValidationResult(
                            ErrorMessages.SaleOrderLine.NumericLessThanZero,
                            new List<string> { "ProductId", item.ProductId.ToString() }));
                }
            }


            if (validationErrors.Any())
                throw new AbpValidationException(validationErrors);
        }

        private async Task ValidateUpdate(SaleOrderUpdateRequest request)
        {
            var validationErrors = new List<ValidationResult>();
            var saleOrder = await _saleOrderRepository.FindAsync(x => x.Id == request.Id);
            if (saleOrder == null)
                throw new AbpValidationException(ErrorMessages.SaleOrder.NotExist + ": SaleOrderId => " + request.Id.ToString());

            if (saleOrder.Status == Enums.SaleOrder.Status.Finished)
                validationErrors.Add(new ValidationResult(
                    ErrorMessages.SaleOrder.IsComplete,
                    new List<string> { ErrorMessages.SaleOrder.IsComplete, request.Id.ToString() }));

            if (request.Package < 0)
                validationErrors.Add(new ValidationResult(
                    ErrorMessages.SaleOrder.NumericLessThanZero,
                    new List<string> { "Package", request.Id.ToString() }));

            if (request.InvoiceNumber.IsNullOrWhiteSpace())
                validationErrors.Add(new ValidationResult(
                    ErrorMessages.SaleOrder.NullOrWhiteSpace,
                    new List<string> { "InvoiceNumber", request.InvoiceNumber }));

            if (saleOrder != null)
            {
                var invoiceNumberExist = await _saleOrderRepository.FindAsync(x => x.InvoiceNumber == request.InvoiceNumber && x.SupplierId == saleOrder.SupplierId && x.Id != request.Id);
                if (invoiceNumberExist != null)
                    validationErrors.Add(new ValidationResult(
                        ErrorMessages.SaleOrder.Existed,
                        new List<string> { "SupplierId => InvoiceNumber", string.Format("{0} => {1}", invoiceNumberExist.SupplierId, request.InvoiceNumber) }));
            }

            var saleOrderLines = await _saleOrderLineService.ListByOrderIdAsync(request.Id);
            var isDeleteCount = request.SaleOrderLines.Where(x => x.IsDelete).Count();
            if (saleOrderLines.Count <= isDeleteCount && isDeleteCount == request.SaleOrderLines.Count)
                validationErrors.Add(new ValidationResult(
                    ErrorMessages.SaleOrderLine.CanNotDeleteAll,
                    new List<string> { "SaleOrderLineIds", saleOrderLines.Select(x => x.Id).JoinAsString("; ") }));

            if (request.SaleOrderLines.Any())
            {

                foreach (var item in request.SaleOrderLines)
                {

                    if (item.Id.HasValue)
                    {
                        var saleOrderLine = saleOrderLines.FirstOrDefault(p => p.Id == item.Id);
                        if (saleOrderLine == null)
                            validationErrors.Add(new ValidationResult(
                            ErrorMessages.SaleOrderLine.NotExist,
                            new List<string> { "SaleOrderLineId", item.Id.ToString() }));

                        if (item.IsDelete && saleOrderLine.ImportQuantity.Value != 0)
                            validationErrors.Add(new ValidationResult(
                            ErrorMessages.SaleOrderLine.CanNotDelete,
                            new List<string> { "SaleOrderLineId", item.Id.ToString() }));

                        //if (!item.IsDelete && saleOrderLine.ImportQuantity > item.RequestQuantity)
                        //    validationErrors.Add(new ValidationResult(
                        //    ErrorMessages.SaleOrderLine.CanNotChange,
                        //    new List<string> { "SaleOrderLineId", item.Id.ToString() }));

                        if (!item.IsDelete && (!item.RequestQuantity.HasValue || item.RequestQuantity < 0))
                            validationErrors.Add(new ValidationResult(
                                ErrorMessages.SaleOrderLine.NumericLessThanZero,
                                new List<string> { "ProductId", item.ProductId.ToString() }));
                    }
                    else if (item.ProductId.HasValue)
                    {

                        var productExist = await _productService.Exist(item.ProductId ?? Guid.NewGuid());
                        if (!productExist)
                            validationErrors.Add(new ValidationResult(
                                ErrorMessages.Product.NotExist,
                                new List<string> { "ProductId", item.ProductId.ToString() }));

                        if (!item.RequestQuantity.HasValue || item.RequestQuantity < 0 || !item.RequestPrice.HasValue || item.RequestPrice < 0)
                            validationErrors.Add(new ValidationResult(
                                ErrorMessages.SaleOrderLine.NumericLessThanZero,
                                new List<string> { "ProductId", item.ProductId.ToString() }));
                    }
                    else
                        validationErrors.Add(new ValidationResult($"{ErrorMessages.SaleOrderLine.Base}.Id And {ErrorMessages.Product.Base}.Id contemporary null"));
                }
            }

            if (validationErrors.Any())
                throw new AbpValidationException(validationErrors);
        }

        private async Task<IQueryable<SaleOrderDTO>> GetListSaleOrderAsync(SearchSaleOrderRequest input)
        {
            try
            {
                var userStores = (await _userStoreRepository.GetQueryableAsync()).Where(x => x.UserId == _currentUser.GetId());
                var users = await _identityUserRepository.GetListAsync();
                var saleOrders = (await _saleOrderRepository.GetQueryableAsync()).Where(x=> userStores.Any(uS=>uS.StoreId == x.StoreId));
                var saleOrderLines = await _saleOrderLineRepository.GetQueryableAsync();
                var suppliers = await _supplierRepository.GetQueryableAsync();
                var stores = await _storeRepository.GetQueryableAsync();

                var list = from so in saleOrders

                           join sol in saleOrderLines
                           on so.Id equals sol.OrderId
                           into solGr
                           from sol in solGr.DefaultIfEmpty()

                           join sup in suppliers 
                           on so.SupplierId equals sup.Id
                           into supGr
                           from sup in supGr.DefaultIfEmpty()

                           join sto in stores 
                           on so.StoreId equals sto.Id
                           into stoGr
                           from sto in stoGr.DefaultIfEmpty()

                           orderby so.CreationTime descending
                           select new SaleOrderDTO
                           {
                               Id = so.Id,
                               Code = so.Code,
                               StoreId = so.StoreId,
                               StoreName = sto.Name,
                               SupplierId = so.SupplierId,
                               SupplierName = sup.Name,
                               InvoiceNumber = so.InvoiceNumber,
                               OrderDate = so.OrderDate,
                               CreatorId = so.CreatorId,
                               IsConfirm = so.Status == Enums.SaleOrder.Status.Finished,
                               TotalPrice = sol.TotalYuan * Convert.ToDecimal(so.Rate),
                               TotalQuantity = sol.RequestQuantity,
                               TotalApprove = sol.ImportQuantity,
                               Rate = so.Rate,
                               TotalProduct = 1,
                               TotalPriceNDT = sol.TotalYuan,
                               Status = so.Status,
                               Note = so.Note,
                               CreationTime = so.CreationTime,
                               ConfirmedBeforeCNY = so.ConfirmedBeforeCNY,
                               ConfirmedFinalCNY = so.ConfirmedFinalCNY,
                               DebtBeforeCNY = so.DebtBeforeCNY,
                               DebtFinalCNY = so.DebtFinalCNY,
                               DeliveryBeforeCNY = so.DeliveryBeforeCNY,
                               DeliveryFinalCNY = so.DeliveryFinalCNY,
                               PaidCNY = so.PaidCNY,
                           };


                list = list.WhereIf(!string.IsNullOrEmpty(input.InvoiceNumber), x => x.InvoiceNumber.Contains(input.InvoiceNumber));

                if (!string.IsNullOrEmpty(input.Code))
                {
                    list = list.Where(x => x.Code.Contains(input.Code));
                }
                if (!string.IsNullOrEmpty(input.SupplierName))
                {
                    list = list.Where(result => result.SupplierName.Contains(input.SupplierName));
                }
                if (input.StoreId != null && input.StoreId.Count > 0)
                {
                    list = list.Where(result => input.StoreId.Contains(result.StoreId));
                }
                if (input.Status != null)
                {
                    list = list.Where(result => result.Status == input.Status);
                }
                if (input.FromDate != null)
                {
                    list = list.Where(result => result.OrderDate.HasValue && result.OrderDate.Value.Date >= input.FromDate.Value.Date);
                }
                if (input.ToDate != null)
                {
                    list = list.Where(result => result.OrderDate.HasValue && result.OrderDate.Value.Date <= input.ToDate.Value.Date);
                }
                var data = list.ToList();
                var respon = data.GroupBy(x => x.Code)
                               .SelectMany(sol => 
                                   sol.Select(
                                    solLine => new SaleOrderDTO
                                    {
                                        Id = solLine.Id,
                                        Code = solLine.Code,
                                        StoreId = solLine.StoreId,
                                        StoreName = solLine.StoreName,
                                        SupplierId = solLine.SupplierId,
                                        SupplierName = solLine.SupplierName,
                                        InvoiceNumber = solLine.InvoiceNumber,
                                        Status = solLine.Status,
                                        OrderDate = solLine.OrderDate,
                                        CreatorId = solLine.CreatorId,
                                        IsConfirm = solLine.IsConfirm,
                                        Note = solLine.Note,
                                        Rate = solLine.Rate,
                                        TotalProduct = sol.Count(),
                                        TotalApprove = sol.Sum(c => c.TotalApprove),
                                        TotalQuantity = sol.Sum(c => c.TotalQuantity),
                                        TotalPriceNDT = sol.Sum(c => c.TotalPriceNDT),
                                        TotalPrice = sol.Sum(c => c.TotalPrice),
                                        ConfirmedBeforeCNY = solLine.ConfirmedBeforeCNY,
                                        ConfirmedFinalCNY = solLine.ConfirmedFinalCNY,
                                        DebtBeforeCNY = solLine.DebtBeforeCNY,
                                        DebtFinalCNY = solLine.DebtFinalCNY,
                                        DeliveryBeforeCNY = solLine.DeliveryBeforeCNY,
                                        DeliveryFinalCNY = solLine.DeliveryFinalCNY,
                                        PaidCNY = solLine.PaidCNY,
                                        CreationTime = solLine.CreationTime,
                                        CreatorName = users.FirstOrDefault(u => u.Id == solLine.CreatorId)?.Name
                                    }))
                            .ToList<SaleOrderDTO>();
                return respon.AsQueryable();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private async Task<IQueryable<SaleOrderDTO>> GetDetailSaleOrder(Guid id)
        {
            try
            {
                var users = await _identityUserRepository.GetQueryableAsync();
                var saleOrders = await _saleOrderRepository.GetQueryableAsync();
                var saleOrderLines = await _saleOrderLineRepository.GetQueryableAsync();
                var suppliers = await _supplierRepository.GetQueryableAsync();
                var stores = await _storeRepository.GetQueryableAsync();

                var list = from so in saleOrders
                           join sol in saleOrderLines
                           on so.Id equals sol.OrderId
                           join sup in suppliers
                           on so.SupplierId equals sup.Id
                           join sto in stores
                           on so.StoreId equals sto.Id
                           join u in users
                           on so.CreatorId equals u.Id
                           into uu
                           from u in uu.DefaultIfEmpty()
                           select new SaleOrderDTO
                           {
                               Id = so.Id,
                               Code = so.Code,
                               StoreId = so.StoreId,
                               StoreName = sto.Name,
                               SupplierId = so.SupplierId,
                               SupplierName = sup.Name,
                               InvoiceNumber = so.InvoiceNumber,
                               OrderDate = so.OrderDate,
                               CreatorId = so.CreatorId,
                               CreatorName = u == null ? "" : u.Name,
                               TotalPrice = sol.TotalYuan * Convert.ToDecimal(so.Rate),
                               TotalQuantity = sol.RequestQuantity,
                               TotalApprove = sol.ImportQuantity,
                               Rate = so.Rate,
                               TotalProduct = 1,
                               TotalPriceNDT = sol.TotalYuan,
                               Status = so.Status,
                               Note = so.Note,

                           };
                var respon = list.ToList().GroupBy(x => x.Code)
                               .SelectMany(sol => sol.Select(
                                solLine => new SaleOrderDTO
                                {
                                    Id = solLine.Id,
                                    Code = solLine.Code,
                                    StoreId = solLine.StoreId,
                                    StoreName = solLine.StoreName,
                                    SupplierId = solLine.SupplierId,
                                    SupplierName = solLine.SupplierName,
                                    InvoiceNumber = solLine.InvoiceNumber,
                                    Status = solLine.Status,
                                    OrderDate = solLine.OrderDate,
                                    CreatorId = solLine.CreatorId,
                                    CreatorName = solLine.CreatorName,
                                    Note = solLine.Note,
                                    Rate = solLine.Rate,
                                    TotalProduct = sol.Count(),
                                    TotalApprove = sol.Sum(c => c.TotalApprove),
                                    TotalQuantity = sol.Sum(c => c.TotalQuantity),
                                    TotalPriceNDT = sol.Sum(c => c.TotalPriceNDT),
                                    TotalPrice = sol.Sum(c => c.TotalPrice),
                                })).ToList<SaleOrderDTO>();
                return respon.AsQueryable();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}