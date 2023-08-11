using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using System.Threading.Tasks;
using Vinpearl.Modelling.Library.Utility.Excel;
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
using VTECHERP.DTOs.Product;
using VTECHERP.DTOs.ProductXnk;
using VTECHERP.DTOs.Stores;
using VTECHERP.DTOs.TransportInformation;
using VTECHERP.Entities;
using VTECHERP.Enums;
using VTECHERP.Enums.Bills;
using VTECHERP.Enums.Product;
using VTECHERP.Helper;
using VTECHERP.Models;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;


namespace VTECHERP.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Products> _productRepository;
        private readonly IStoreProductService _storeProductService;
        private readonly IRepository<StoreProduct> _storeProductRepository;
        private readonly IRepository<ProductCategories> _productCategoryRepository;
        private readonly IRepository<BillCustomer> _billCustomerRepository;
        private readonly IRepository<BillCustomerProduct> _billCustomerProductRepository;
        private readonly IRepository<SaleOrders> _saleOrderRepository;
        private readonly IRepository<SaleOrderLines> _saleOrderLineRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IObjectMapper _objectMapper;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IAttachmentService _attachmentService;
        private readonly ICurrentUser _userManager;
        private readonly IdentityUserManager _user;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly IRepository<Stores> _storeRepository;
        private readonly IRepository<Suppliers> _supplierRepository;
        private readonly IRepository<ProductView> _productViewRepository;
        private readonly IRepository<WarehouseTransferBill> _wareHouseTransferRepository;
        private readonly IRepository<WarehouseTransferBillProduct> _wareHouseTransferProductRepository;
        private readonly IRepository<WarehousingBill> _wareHouseRepository;
        private readonly IRepository<WarehousingBillProduct> _wareHouseProductRepository;
        private readonly IClock _clock;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IAuthorizationService _authorizationService;
        public ProductService(
            IRepository<Products> productRepository,
            IStoreProductService storeProductService,
            IRepository<StoreProduct> storeProductRepository,
            IRepository<ProductCategories> productCategoryRepository,
            IRepository<BillCustomer> billCustomerRepository,
            IRepository<BillCustomerProduct> billCustomerProductRepository,
            IRepository<SaleOrders> saleOrderRepository,
            IRepository<SaleOrderLines> saleOrderLineRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IObjectMapper objectMapper, IHostingEnvironment hostingEnvironment,
            IConfiguration configuration, IAttachmentService attachmentService,
            ICurrentUser userManager,
            IRepository<UserStore> userStoreRepository,
            IRepository<Stores> storeRepository,
            IdentityUserManager user,
            IRepository<ProductView> productViewRepository,
            IRepository<WarehouseTransferBill> wareHouseTransferRepository,
            IRepository<WarehouseTransferBillProduct> wareHouseTransferProductRepository,
            IRepository<WarehousingBill> wareHouseRepository,
            IRepository<WarehousingBillProduct> wareHouseProductRepository,
            IRepository<Suppliers> supplierRepository,
            IClock clock,
            IIdentityUserRepository userRepository,
            IAuthorizationService authorizationService
            )
        {
            _productRepository = productRepository;
            _storeProductService = storeProductService;
            _objectMapper = objectMapper;
            _storeProductRepository = storeProductRepository;
            _productCategoryRepository = productCategoryRepository;
            _billCustomerRepository = billCustomerRepository;
            _billCustomerProductRepository = billCustomerProductRepository;
            _saleOrderLineRepository = saleOrderLineRepository;
            _saleOrderRepository = saleOrderRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _attachmentService = attachmentService;
            _userManager = userManager;
            _userStoreRepository = userStoreRepository;
            _storeRepository = storeRepository;
            _user = user;
            _productViewRepository = productViewRepository;
            _wareHouseTransferRepository = wareHouseTransferRepository;
            _wareHouseTransferProductRepository = wareHouseTransferProductRepository;
            _wareHouseRepository = wareHouseRepository;
            _wareHouseProductRepository = wareHouseProductRepository;
            _clock = clock;
            _userRepository = userRepository;
            _authorizationService = authorizationService;
            _supplierRepository = supplierRepository;
        }

        public async Task<List<MasterDataDTO>> GetIdCodeNameAsync()
        {
            var products = await _productRepository.GetListAsync();
            var result = _objectMapper.Map<List<Products>, List<MasterDataDTO>>(products);
            return result;
        }

        public async Task<List<ProductMasterDataDto>> MapStockQuantityAsync(Guid storeId, List<Guid> productIds = null)
        {
            var results = new List<ProductMasterDataDto>();

            var products = await _productRepository.GetListAsync();
            if (products == null)
                return results;

            results = _objectMapper.Map<List<Products>, List<ProductMasterDataDto>>(products);
            var storeProducts = (await _storeProductService.GetByStoreId(storeId))
                .WhereIf(productIds != null, x => productIds.Any(productId => productId == x.ProductId))
                .ToList();
            if (storeProducts.Count > 0)
                results.ForEach(x =>
                {
                    var storeProduct = storeProducts.Where(s => s.ProductId == x.Id);
                    if (storeProduct != null)
                        x.StockQuantity = storeProduct.Sum(x => x.StockQuantity);
                });

            return results;
        }

        public async Task<List<ProductMasterDataDto>> GetProductByStordId(Guid storeId)
        {
            var results = new List<ProductMasterDataDto>();

            var storeProducts = _storeProductRepository.GetQueryableAsync().Result.Where(x => x.StoreId == storeId).Select(x => new
            {
                x.ProductId,
                x.StockQuantity
            }).ToList();

            if (storeProducts == null || storeProducts.Count == 0) return results;

            var productIds = storeProducts.Select(x => x.ProductId).ToList();

            var products = await _productRepository.GetListAsync(x => productIds.Any(z => z == x.Id));

            if (products == null)
                return results;

            results = _objectMapper.Map<List<Products>, List<ProductMasterDataDto>>(products);

            foreach (var item in results)
            {
                item.StockQuantity = storeProducts.FirstOrDefault(x => x.ProductId == item.Id).StockQuantity;
            }

            return results;
        }

        public async Task<ProductDetailDto> GetByIdAsync(Guid id)
        {
            var product = await _productRepository.FindAsync(x => x.Id == id);
            var parentProduct = await _productRepository.FindAsync(x => x.Id == product.ParentId);
            var categories= await _productCategoryRepository.FindAsync(x => x.Id == product.CategoryId);
            if (product == null)
                throw new AbpValidationException(ErrorMessages.Product.NotExist + ": ProductId => " + id.ToString());
            var result = _objectMapper.Map<Products, ProductDetailDto>(product);

            result.Attachments = await _attachmentService.GetAttachmentByObjectIdAsync(id);

            if (parentProduct != null)
            {
                result.ParentName = parentProduct.Name;
            }
            if (result.EntryPrice == null)
            {
                result.EntryPrice = 0;
            }
            if (result!= null && result.SalePrice > 0)
            {
                result.SalePriceInterest = ((result.SalePrice - result.StockPrice) / result.SalePrice)*100;
            }
            if (result != null && result.SPAPrice > 0)
            {
                result.SPAPriceInterest = ((result.SPAPrice - result.StockPrice) / result.SalePrice) *100;
            }
            if (result != null && result.WholeSalePrice > 0)
            {
                result.WholeSalePriceInterest = ((result.WholeSalePrice - result.StockPrice) / result.SalePrice) *100;
            }
            if (categories != null)
            {
                result.CategoryName = categories.Name;
            }
            if (product.CreatorId != null)
            {
                var user = await _user.FindByIdAsync(product.CreatorId.ToString());
                result.CreatorName = user == null ? "" : user.Name;
            }
            return result;
        }

        public async Task<List<ProductDetailDto>> GetByIdsAsync(List<Guid> ids)
        {
            var product = await _productRepository.GetListAsync(x => ids.Any(id => id == x.Id));
            if (product == null)
                return null;
            var result = _objectMapper.Map<List<Products>, List<ProductDetailDto>>(product);
            return result;
        }

        public async Task<bool> Exist(Guid id)
        {
            var product = await _productRepository.FindAsync(x => x.Id == id);

            if (product == null)
                return false;
            return true;
        }

        public async Task<PagingResponse<SearchProductResponse>> Search(SearchProductRequest request)
        {
            try
            {
                var productQuery = await _productRepository.GetQueryableAsync();
                var products = productQuery.ToList();
                var user = _userManager.Id;
                var userStores = (await _userStoreRepository.GetListAsync(x => x.UserId == user));
                var storePro = (await _storeProductRepository.GetListAsync());
                var userStoreId = userStores.Select(x => x.StoreId).ToList();
                //var x = request.StoreId.Intersect(userStoreId).ToList();
                if (request.StoreId != null && request.StoreId.Count > 0)
                {
                    storePro = storePro.Where(x => request.StoreId.Contains(x.StoreId)).ToList();
                }
                var query1 = from product in products
                             join storeProduct in storePro on product.Id equals storeProduct?.ProductId
                             into joinedTableStoreProduct
                             from joinItemStoreProduct in joinedTableStoreProduct.DefaultIfEmpty()
                             join userStore in userStores on joinItemStoreProduct?.StoreId equals userStore.StoreId
                             into joinedTableUserStore
                             select product;
                query1 = query1.Distinct();
                productQuery = query1.AsQueryable();

                productQuery = productQuery
                    //.WhereIf(!string.IsNullOrEmpty(request.ProductName) || !string.IsNullOrEmpty(request.ProductCode)
                    //    , x => EF.Functions.Collate(x.Name, "SQL_Latin1_General_CP1_CI_AI").Contains(EF.Functions.Collate(request.ProductName, "SQL_Latin1_General_CP1_CI_AI"))
                    //    || x.Code.Contains(request.ProductCode)
                    //    )
                    .WhereIf(!String.IsNullOrEmpty(request.ProductName), x => (!string.IsNullOrEmpty(x.Name) && x.Name.ToLower().Contains(request.ProductName.ToLower().Trim())) || (!string.IsNullOrEmpty(x.Code) && x.Code.ToLower().Contains(request.ProductName.ToLower().Trim())))
                    .WhereIf(request.Status != null, x => x.Status == request.Status)
                    .WhereIf(!String.IsNullOrEmpty(request.SequenceId), x => x.SequenceId.Contains(request.SequenceId))
                    .WhereIf(request.ProductCategoryIds != null && request.ProductCategoryIds.Count > 0, x => request.ProductCategoryIds.Any(z => z == x.CategoryId));
                productQuery = productQuery.OrderByDescending(x => x.SequenceId);
                var productQueryTest = productQuery.OrderByDescending(x => x.SequenceId).ToList();
                var billCustomer = await _billCustomerRepository.GetQueryableAsync();
                var billCustomerProduct = await _billCustomerProductRepository.GetQueryableAsync();
                var saleOrder = await _saleOrderRepository.GetQueryableAsync();
                var saleOrderLine = await _saleOrderLineRepository.GetQueryableAsync();

                //lấy số lượng đặt trước
                var billBookingIds = request.StoreId != null ? billCustomer.WhereIf(request.StoreId != null, x => request.StoreId.Contains(x.StoreId ?? Guid.Empty) && x.CustomerBillPayStatus == Enums.Bills.CustomerBillPayStatus.CustomerOrder).Select(x => x.Id).ToList() : billCustomer.Where( x => x.CustomerBillPayStatus == Enums.Bills.CustomerBillPayStatus.CustomerOrder).Select(x => x.Id).ToList();
                var billBooking = billCustomerProduct.WhereIf(billBookingIds != null, x => billBookingIds.Contains(x.BillCustomerId.Value));

                //lấy số lượng đang giao
                
                var billDeliveringIds = request.StoreId == null ? billCustomer.Where(x => x.CustomerBillPayStatus == Enums.Bills.CustomerBillPayStatus.Delivery).Select(x => x.Id).ToList() : billCustomer.WhereIf(request.StoreId != null, x => request.StoreId.Contains(x.StoreId ?? Guid.Empty) && x.CustomerBillPayStatus == Enums.Bills.CustomerBillPayStatus.Delivery).Select(x => x.Id).ToList();
                var billDelivering = billCustomerProduct.WhereIf(billDeliveringIds != null, x => billDeliveringIds.Contains(x.BillCustomerId.Value));

                //lấy số lượng tạm giữ
                var billTemporarilyHoldIds = request.StoreId != null ? billCustomer.WhereIf(request.StoreId != null, x => request.StoreId.Contains(x.StoreId ?? Guid.Empty) && x.CustomerBillPayStatus == Enums.Bills.CustomerBillPayStatus.WaitCall).Select(x => x.Id).ToList() : billCustomer.Where( x => x.CustomerBillPayStatus == Enums.Bills.CustomerBillPayStatus.WaitCall).Select(x => x.Id).ToList();
                var billTemporarilyHold = billCustomerProduct.WhereIf(billTemporarilyHoldIds != null, x => billTemporarilyHoldIds.Contains(x.BillCustomerId.Value));

                //lấy số lượng đang về
                var saleOrderIds = request.StoreId != null 
                    ? saleOrder.WhereIf(request.StoreId != null, x => request.StoreId.Contains(x.StoreId) && x.Status == SaleOrder.Status.Unfinished).Select(x => x.Id).ToList() 
                    : saleOrder.Where( x => x.Status == SaleOrder.Status.Unfinished).Select(x => x.Id).ToList();
                var billComing = saleOrderLine.WhereIf(saleOrderIds != null, x => saleOrderIds.Contains(x.OrderId));
                var response = productQuery
                    .Skip(request.Offset)
                    .Take(request.PageSize)
                    .Select(x => new SearchProductResponse
                    {
                        Id = x.Id,
                        SequenceId = x.SequenceId,
                        StockPrice = x.StockPrice,
                        ProudctName = x.Name,
                        ProudctCode = x.Code,
                        BarCode = x.BarCode,
                        SalePrice = x.SalePrice,
                        SpaPrice = x.SPAPrice,
                        EntryPrice = x.EntryPrice,
                        Inventory = request.StoreId == null ? _storeProductRepository.GetQueryableAsync().Result.Where(a => a.ProductId == x.Id && userStoreId.Contains(a.StoreId)).Sum(i => i.StockQuantity) : _storeProductRepository.GetQueryableAsync().Result.Where(a => request.StoreId.Contains(a.StoreId) && a.ProductId == x.Id).Sum(i => i.StockQuantity),
                        TotalInventory = _storeProductRepository.GetQueryableAsync().Result.Where(a => a.ProductId == x.Id).Sum(i => i.StockQuantity),
                        Delivery = billDelivering.Where(a => a.ProductId == x.Id).Sum(i => i.Quantity),
                        TemporarilyHold = billTemporarilyHold.Where(a => a.ProductId == x.Id).Sum(i => i.Quantity),
                        SellNumber = request.StoreId == null ? 0 :
                            (_storeProductRepository.GetQueryableAsync().Result.Where(a => request.StoreId.Contains(a.StoreId) && a.ProductId == x.Id).Sum(i => i.StockQuantity) - billTemporarilyHold.Where(a => a.ProductId == x.Id).Sum(i => i.Quantity)) > 0 ? _storeProductRepository.GetQueryableAsync().Result.Where(a => request.StoreId.Contains(a.StoreId) && a.ProductId == x.Id).Sum(i => i.StockQuantity) - billTemporarilyHold.Where(a => a.ProductId == x.Id).Sum(i => i.Quantity) : 0,
                        Coming = (billComing.Where(a => a.ProductId == x.Id).Sum(i => i.RequestQuantity) - billComing.Where(a => a.ProductId == x.Id).Sum(i => i.ImportQuantity)) > 0 ? (billComing.Where(a => a.ProductId == x.Id).Sum(i => i.RequestQuantity) - billComing.Where(a => a.ProductId == x.Id).Sum(i => i.ImportQuantity)) : 0,
                        Booking = billBooking.Where(a => a.ProductId == x.Id).Sum(i => i.Quantity),
                        Status = x.Status
                    });
                return new PagingResponse<SearchProductResponse>()
                {
                    Data = response.ToList(),
                    Total = productQuery.Count(),
                };
            }
            catch (Exception ex)
            {
                var x = ex.Message;
                return null;
            }

        }

        public async Task<PagingResponse<SearchProductResponse>> SearchClone(SearchProductRequest request)
        {
            try
            {
                bool checkEntryPrice = false;
                var authResult = await _authorizationService.AuthorizeAsync("VTECHERP.Product.Management.ViewEntryPrice");
                if (authResult.Succeeded)
                    checkEntryPrice = true;

                var userStores = (await _userStoreRepository.GetListAsync(x => x.UserId == _userManager.Id));
                var userStoreId = userStores.Select(x => x.StoreId).ToList();
                if (request.StoreId == null || request.StoreId.Count() == 0)
                {
                    request.StoreId = userStoreId;
                }
                var productQuery = await _productRepository.GetQueryableAsync();
                var productView = (await _productViewRepository.GetQueryableAsync())
                    .WhereIf(request.StoreId != null && request.StoreId.Count > 0, p => p.StoreId != null && request.StoreId.Contains(p.StoreId.Value))
                        .GroupBy(x => new
                        {
                            x.Id,
                            x.TotalInventory
                        })
                        .Select(grp => new
                        {
                            ProductId = grp.Key.Id,
                            TotalInventory = grp.Key.TotalInventory,
                            Inventory = grp.Sum(p => p.Inventory),
                            Coming = grp.Sum(p => p.Coming),
                            Booking = grp.Sum(p => p.Booking),
                            Delivery = grp.Sum(p => p.Delivery),
                            TemporarilyHold = grp.Sum(p => p.TemporarilyHold),
                            SellNumber = grp.Sum(p => p.SellNumber),
                        }).ToList();
                ;
                var products = productQuery
                        .WhereIf(!String.IsNullOrEmpty(request.ProductName), x => (!string.IsNullOrEmpty(x.Name) && x.Name.ToLower().Contains(request.ProductName.ToLower().Trim())) || (!string.IsNullOrEmpty(x.Code) && x.Code.ToLower().Contains(request.ProductName.ToLower().Trim())))
                        .WhereIf(request.Status != null, x => x.Status == request.Status)
                        .WhereIf(!String.IsNullOrEmpty(request.SequenceId), x => x.SequenceId.Contains(request.SequenceId))
                        .WhereIf(request.ProductCategoryIds != null && request.ProductCategoryIds.Count > 0, x => request.ProductCategoryIds.Any(z => z == x.CategoryId))
                        .OrderByDescending(x => x.SequenceId)
                        .ToList();

                var productStockQuery =
                        (
                            from p in products
                            join pStock in productView on p.Id equals pStock.ProductId into pStockLeftJoin
                            from pStock in pStockLeftJoin.DefaultIfEmpty()
                            orderby p.CreationTime descending, p.LastModificationTime descending
                            select new SearchProductResponse
                            {
                                Id = p.Id,
                                Enterprise = p.Enterprise,
                                ParentId = p.ParentId,
                                ParentCode = p.ParentCode,
                                ParentName = p.ParentName,
                                SequenceId = p.SequenceId,
                                ProudctName = p.Name,
                                ProudctCode = p.Code,
                                OtherName = p.OtherName,
                                BarCode = p.Code,
                                Unit = p.Unit,
                                Description = p.Description,
                                WholeSalePrice = p.WholeSalePrice,
                                SalePrice = p.SalePrice,
                                SpaPrice = p.SPAPrice,
                                RatePrice = p.RatePrice,
                                CategoryId = p.CategoryId,
                                SupplierId = p.SupplierId,
                                EntryPrice = checkEntryPrice ? p.EntryPrice : null,
                                StockPrice = p.StockPrice,
                                Status = p.Status,
                                Inventory = pStock != null ? pStock.Inventory : 0,
                                TotalInventory = pStock != null ? pStock.TotalInventory : 0,
                                Delivery = pStock != null ? pStock.Delivery : 0,
                                TemporarilyHold = pStock != null ? pStock.TemporarilyHold : 0,
                                SellNumber = pStock != null ? pStock.SellNumber : 0,
                                Coming = pStock != null ? pStock.Coming : 0,
                                Booking = pStock != null ? pStock.Booking : 0,
                                CreationTime = p.CreationTime
                            }
                        )
                        .ToList()
                        .WhereIf(request.InventoryFilter == ProductInventoryFilter.OnHold, p => p.Inventory > 0)
                        .WhereIf(request.InventoryFilter == ProductInventoryFilter.OnSale, p => p.SellNumber > 0)
                        .WhereIf(request.InventoryStatus == ProductInventoryStatus.DeliveryInProgressLte0, p => p.Delivery <= 0)
                        .WhereIf(request.InventoryStatus == ProductInventoryStatus.DeliveryInProgressGt0, p => p.Delivery > 0)
                        .WhereIf(request.InventoryStatus == ProductInventoryStatus.ComingLte0, p => p.Coming <= 0)
                        .WhereIf(request.InventoryStatus == ProductInventoryStatus.ComingGt0, p => p.Coming > 0)
                        .WhereIf(request.InventoryStatus == ProductInventoryStatus.OnHoldLte0, p => p.TemporarilyHold <= 0)
                        .WhereIf(request.InventoryStatus == ProductInventoryStatus.OnHoldGt0, p => p.TemporarilyHold > 0)
                        .WhereIf(request.InventoryStatus == ProductInventoryStatus.OnSaleLte0, p => p.SellNumber <= 0)
                        .WhereIf(request.InventoryStatus == ProductInventoryStatus.OnSaleGt0, p => p.SellNumber > 0)
                        .WhereIf(request.InventoryStatus == ProductInventoryStatus.PreOrderLte0, p => p.Booking <= 0)
                        .WhereIf(request.InventoryStatus == ProductInventoryStatus.PreOrderGt0, p => p.Booking > 0)
                        .ToList()
                        ;
                var result = productStockQuery
                        .Skip(request.Offset)
                        .Take(request.PageSize).ToList();
                var attachments = (await _attachmentService.ListAttachmentByObjectIdAsync(result.Select(x => x.Id).ToList())).OrderBy(x => x.CreationTime).ToList();
                foreach (var item in result)
                {
                    item.TotalInventory = _storeProductRepository.GetQueryableAsync().Result.Where(a => a.ProductId == item.Id).Sum(i => i.StockQuantity);
                    item.Attachments = attachments.Where(x => x.ObjectId == item.Id).ToList() ?? new List<DetailAttachmentDto>();
                }
                return new PagingResponse<SearchProductResponse>()
                {
                    Data = result,
                    Total = productStockQuery.Count(),
                };
            }
            catch(Exception ex)
            {
                return new PagingResponse<SearchProductResponse>()
                {
                    Data =null,
                    Total = 0,
                };
            }
            
            
        }

        public async Task<IActionResult> Create([FromForm] CreateProductDto input)
        {
            var lstProduct = await _productRepository.GetQueryableAsync();
            if (!string.IsNullOrEmpty(input.ProductName))
            {
                var existName = lstProduct.Where(x => x.Name == input.ProductName);
                if (existName.Any())
                {
                    return new GenericActionResult(400, false, "Không được nhập trùng tên sản phẩm", null);
                }
            }
            if (!string.IsNullOrEmpty(input.Code))
            {
                var existCode = lstProduct.Where(x => x.Code == input.Code);
                if (existCode.Any())
                {
                    return new GenericActionResult(400, false, "Không được nhập trùng mã sản phẩm", null);
                }
            }
            if (!string.IsNullOrEmpty(input.OtherName))
            {
                var existOtherName = lstProduct.Where(x => x.OtherName == input.OtherName);
                if (existOtherName.Any())
                {
                    return new GenericActionResult(400, false, "Không được nhập trùng tên khác của sản phẩm", null);
                }
            }

            var uow = _unitOfWorkManager.Current;
            var attachments = "";
            if (input.formFiles != null)
            {
                UploadAttachmentRequest uploadAttachmentRequest = new UploadAttachmentRequest();
                uploadAttachmentRequest.ObjectType = AttachmentObjectType.Product;
                uploadAttachmentRequest.formFiles = input.formFiles;
                string apiUrl = _configuration.GetValue<string>("HostSetting:API");
                var fileModels = await FileUploadHelper.GetFilesFromForm(input.formFiles);
                var uploadFiles = await FileUploadHelper.UploadFile(fileModels, _hostingEnvironment.ContentRootPath, apiUrl);

                var attachmentResponse = await _attachmentService.Save(uploadAttachmentRequest, uploadFiles);
                var lst = attachmentResponse.Data.Select(x => x.Path).ToList();
                attachments = string.Join(",", lst);
            }

            var product = await _productRepository.InsertAsync(new Products()
            {
                Name = input.ProductName,
                OtherName = input.ProductName,
                ParentId = input.ParentId,
                CategoryId = input.CategoryId,
                Code = input.Code,
                Unit = input.Unit,
                SalePrice = input.SalePrice,
                SPAPrice = input.SPAPrice,
                RatePrice = input.RatePrice,
                Status = input.Status,
                WholeSalePrice = input.WholeSalePrice,
                Description = input.Description,
                Question = input.Question,
                Attachments = attachments,
                CreatorId = _userManager.Id
            });
            await uow.SaveChangesAsync();
            var result = _objectMapper.Map<Products, ProductDetailDto>(product);
            return new GenericActionResult(200, true, "", result);
        }

        public async Task<byte[]> ExportProductAsync(SearchProductRequest request)
        {
            request.PageIndex = 1;
            request.PageSize = int.MaxValue;
            var data = (await SearchClone(request)).Data;
            var query = data.ToList();
            var exportData = new List<ExportProductResponse>();
            foreach (var item in query)
            {
                var parentProduct = await _productRepository.FindAsync(x => x.Id == item.ParentId);
                var category = await _productCategoryRepository.FindAsync(x => x.Id == item.CategoryId);
                var supplier = await _supplierRepository.FindAsync(x => x.Id == item.SupplierId);
                exportData.Add(new ExportProductResponse()
                {
                    SequenceId = item.SequenceId,
                    ParentID = parentProduct?.SequenceId,
                    ParentCode = item.ParentCode,
                    ParentName = item.ParentName,
                    BarCode = item.BarCode,
                    Enterprise = item.Enterprise,
                    Code = item.ProudctCode,
                    Name = item.ProudctName,
                    OtherName = item.OtherName,
                    Unit = item.Unit,
                    StockPrice = item.StockPrice,
                    EntryPrice = item.EntryPrice.HasValue ? item.EntryPrice.Value : 0,
                    SalePrice = item.SalePrice.HasValue ? item.SalePrice.Value : 0,
                    SalePricePercent = 0,
                    WholeSalePrice = item.WholeSalePrice.HasValue ? item.WholeSalePrice.Value : 0,
                    WholeSalePricePercent = 0,
                    SpaPrice = item.SalePrice.HasValue ? item.SalePrice.Value : 0,
                    SpaPricePercent = 0,
                    RatePrice = item.RatePrice,
                    Inventory = item.Inventory,
                    TotalInventory = item.TotalInventory,
                    Description = item.Description,
                    Delivery= item.Delivery,
                    TemporarilyHold = item.TemporarilyHold,
                    SellNumber = item.SellNumber,
                    Coming = item.Coming,
                    Booking = item.Booking,
                    Status = item.Status == ProductStatus.New ? "Đang bán" : item.Status == ProductStatus.Liquidation? "Thanh lý" : item.Status == ProductStatus.StopSelling ? "Ngừng bán" : item.Status == ProductStatus.OnSale ? "Đang bán" : "Hết hàng",
                    CategoryCode = category?.CategoryCode,
                    CategoryName = category?.Name,
                    SupplierName = supplier?.Name,
                    CreationTime = item.CreationTime.ToString("dd-MM-yyyy")
                });
            }
            return ExcelHelper.ExportExcel(exportData);
        }

        public async Task<string> ValidateCreateDto(CreateProductDto input)
        {
            var error = "";
            if (String.IsNullOrEmpty(input.Code))
            {
                return error = "Mã sản phẩm";
            }
            return error;
        }

        public async Task<List<ProductDetailInventory>> GetProductDetailInventories(Guid ProductId)
        {
            var result = new List<ProductDetailInventory>();

            var userStoreQueryable = await _userStoreRepository.GetQueryableAsync();

            var storeUsers = userStoreQueryable.Where(x => x.UserId == _userManager.Id).Select(x => x.StoreId).ToList();

            var storeQueryable = (await _storeRepository.GetQueryableAsync()).Where(x => storeUsers.Contains(x.Id));
            var storeProductQueryable = (await _storeProductRepository.GetQueryableAsync()).Where(x => storeUsers.Contains(x.StoreId) 
                                                                                                    && x.ProductId == ProductId);

            var wareHouseTranferQueryable = await _wareHouseTransferRepository.GetQueryableAsync();
            var wareHouseTranferProductQueryable = await _wareHouseTransferProductRepository.GetQueryableAsync();

            var billProductQueryable = await _billCustomerProductRepository.GetQueryableAsync();
            var billCustomerQueryable = await _billCustomerRepository.GetQueryableAsync();

            var saleOrderQueryable = await _saleOrderRepository.GetQueryableAsync();
            var saleOrderLineQueryable = await _saleOrderLineRepository.GetQueryableAsync();

            // Lấy ra danh sách cửa hàng của user và tồn của sản phẩm trên từng cửa hàng
            var query = (from store in storeQueryable

                         join storePro in storeProductQueryable
                         on store.Id equals storePro.StoreId
                         into storeProGr
                         from storePro in storeProGr.DefaultIfEmpty()

                         select new ProductDetailInventory
                         {
                             StoreId = store.Id,
                             StoreName = store.Name,
                             Inventory = storePro == null ? 0 : storePro.StockQuantity
                         })
                       .ToList();

            // Lấy ra danh sách hóa đơn và số lượng sản phẩm của từng hóa đơn 
            var queryBill = (from bill in billCustomerQueryable

                            join billPro in billProductQueryable
                            on bill.Id equals billPro.BillCustomerId

                            where (bill.CustomerBillPayStatus == CustomerBillPayStatus.CustomerOrder
                            || bill.CustomerBillPayStatus == CustomerBillPayStatus.Delivery
                            || bill.CustomerBillPayStatus == CustomerBillPayStatus.WaitCall)
                            && billPro.ProductId == ProductId
                            && storeUsers.Contains(bill.StoreId ?? Guid.Empty)
                             select new
                            {
                                Store = bill.StoreId,
                                Status = bill.CustomerBillPayStatus,
                                Quantity = billPro.Quantity
                            })
                            .ToList();

            // Lấy ra danh sách phiếu chuyển kho và số lượng sản phẩm trên từng phiếu chuyển kho
            var queryWareBill = (from ware in wareHouseTranferQueryable

                                 join warePro in wareHouseTranferProductQueryable
                                 on ware.Id equals warePro.WarehouseTransferBillId

                                 where warePro.ProductId == ProductId
                                 && ware.TransferStatus != Enums.WarehouseTransferBill.TransferStatuses.DeliveryCompleted
                                 && ware.TransferBillType == Enums.WarehouseTransferBill.TransferBillType.Export
                                 && (storeUsers.Contains(ware.SourceStoreId) || storeUsers.Contains(ware.DestinationStoreId))
                                 select new
                                 {
                                     SourceStore = ware.SourceStoreId,
                                     DestinationStore = ware.DestinationStoreId,
                                     Quantity = warePro.Quantity
                                 }
                                ).ToList();

            // Lấy ra danh sách phiếu đặt hàng và số lượng sản phẩm trên từng phiếu đặt hàng
            var querySaleOrderLine = (from saleOrder in saleOrderQueryable

                                      join saleOrderLine in saleOrderLineQueryable
                                      on saleOrder.Id equals saleOrderLine.OrderId

                                      where saleOrderLine.ProductId == ProductId
                                      && storeUsers.Contains(saleOrder.StoreId)

                                      select new
                                      {
                                          Store = saleOrder.StoreId,
                                          RequestQuantity = saleOrderLine.RequestQuantity,
                                          ImportQuantity = saleOrderLine.ImportQuantity
                                      })
                                     .ToList();

            foreach (var item in query)
            {
                item.Delivery = queryBill.Where(x => x.Status == CustomerBillPayStatus.Delivery && x.Store == item.StoreId).Sum(x => x.Quantity);
                item.Hold = queryBill.Where(x => x.Status == CustomerBillPayStatus.WaitCall && x.Store == item.StoreId).Sum(x => x.Quantity);
                item.Ordered = queryBill.Where(x => x.Status == CustomerBillPayStatus.CustomerOrder && x.Store == item.StoreId).Sum(x => x.Quantity);
                item.AbleSell = item.Inventory - item.Hold;
                item.Exported = queryWareBill.Where(x => x.SourceStore == item.StoreId).Sum(x => x.Quantity);
                item.WaitImport = queryWareBill.Where(x => x.DestinationStore == item.StoreId).Sum(x => x.Quantity);
                var requestQuantity = querySaleOrderLine.Where(x => x.Store == item.StoreId).Sum(x => x.RequestQuantity);
                var importQuantity = querySaleOrderLine.Where(x => x.Store == item.StoreId).Sum(x => x.ImportQuantity);
                var remainder = requestQuantity < importQuantity ? importQuantity - requestQuantity : 0;
                item.Comming = Convert.ToDecimal(requestQuantity - importQuantity + remainder);
            }

            return query;
        }

        public async Task<(int count, List<ProductDetailXnk> data)> GetProductDetailXnk(SearchProductDetailXnk request)
        {
            var result = new List<ProductDetailXnk>();

            var userStores = (await _userStoreRepository.GetQueryableAsync()).Where(x => x.UserId == _userManager.GetId());

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
                draftTransferBillIds = _wareHouseTransferRepository
                    .GetQueryableAsync()
                    .Result
                    .Where(p => !p.IsDeleted && p.Code.Contains(request.DraftTransferBillCode))
                    .Select(p => p.Id)
                    .ToList();
            }

            var queryBill = (await _wareHouseRepository.GetQueryableAsync()).Where(x => userStores.Any(uS => uS.StoreId == x.StoreId));
            var queryBillProduct = (await _wareHouseProductRepository.GetQueryableAsync()).Where(x => queryBill.Any(qB => qB.Id == x.WarehousingBillId));

            var query = from billProduct in queryBillProduct
                        join b in queryBill on billProduct.WarehousingBillId equals b.Id into bp
                        from bill in bp.DefaultIfEmpty()
                        join p in _productRepository.GetQueryableAsync().Result on billProduct.ProductId equals p.Id into pd
                        from product in pd.DefaultIfEmpty()

                        where billProduct.ProductId == request.ProductId

                        select new { billProduct, bill, product };

            query = query.WhereIf(!string.IsNullOrEmpty(request.ProductName), x => x.product.Name.Contains(request.ProductName) || x.product.Code.Contains(request.ProductName))
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
                        .WhereIf(request.CustomerIds.HasValue, x => request.CustomerIds == x.bill.AudienceId && (x.bill.AudienceType == AudienceTypes.Customer))
                        .OrderByDescending(p => p.bill.Code);

            var resPage = query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).Select(x => new ProductDetailXnk()
            {
                Id = x.billProduct.Id,
                Code = x.bill.Code,
                StoreId = x.bill.StoreId,
                ProductId = x.product.Id,
                ProductCode = x.product.Code,
                ProductName = x.product.Name,
                Price = x.billProduct.Price,
                CostPrice = x.product.StockPrice,
                Unit = x.product.Unit,
                BillType = x.bill.BillType,
                TotalMoney = x.billProduct.TotalPrice,
                CreatorId = x.billProduct.CreatorId,
                CreationTime = x.bill.CreationTime,
                DiscountAmount = x.billProduct.DiscountAmount,
                Money = x.billProduct.Quantity * x.billProduct.Price,
                Quantity = x.billProduct.Quantity,
                IsEditable = !(x.bill.IsFromWarehouseTransfer == true),
                IsDeletable = !(x.bill.IsFromWarehouseTransfer == true),
                DocumentDetailType = x.bill.DocumentDetailType,
                Note = x.bill.Note
            }).ToList();

            var storedIds = resPage.Select(x => x.StoreId).ToList();

            var productIds = resPage.Select(x => x.ProductId).ToList();

            var listStordProduct = (await _storeProductRepository.GetQueryableAsync())
                .Where(x => productIds.Contains(x.ProductId) && userStores.Any(us => us.StoreId == x.StoreId));

            var listStores = await _storeRepository.GetListAsync(x => storedIds.Contains(x.Id));

            foreach (var item in resPage)
            {
                item.Inventory = listStordProduct.Where(x => item.StoreId == x.StoreId && item.ProductId == x.ProductId).Sum(x => x.StockQuantity);
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

            return (count: query.Count(), data: resPage);
        }
    }
}