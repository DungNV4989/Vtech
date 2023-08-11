using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.Product;
using VTECHERP.Enums.Product;
using VTECHERP.ServiceInterfaces;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Users;
using VTECHERP.Entities;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp;
using VTECHERP.Constants;

namespace VTECHERP.Services
{
    public class ProductStockService : IProductStockService
    {

        private readonly IRepository<Stores> _storeRepository;
        private readonly IRepository<Products> _productRepository;
        private readonly IRepository<ProductCategories> _productCategoriesRepository;
        private readonly IRepository<StoreProduct> _storeProductRepository;
        private readonly IRepository<SaleOrderLines> _saleOrderLineRepository;
        private readonly IRepository<SaleOrders> _saleOrderRepository;
        private readonly IRepository<BillCustomer> _billCustomerRepository;
        private readonly IRepository<BillCustomerProduct> _billCustomerProductRepository;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly IRepository<ProductStockView> _productStockViewRepository;
        private readonly ICurrentUser _currentUser;
        public ProductStockService(IRepository<Stores> storeRepository,
            IRepository<Products> productRepository,
            IRepository<ProductCategories> productCategoriesRepository,
            IRepository<StoreProduct> storeProductRepository,
            IRepository<SaleOrders> saleOrderRepository,
            IRepository<SaleOrderLines> saleOrderLineRepository,
            IRepository<UserStore> userStoreRepository,
            IRepository<BillCustomer> billCustomerRepository,
            IRepository<BillCustomerProduct> billCustomerProductRepository,
            IRepository<ProductStockView> productStockViewRepository,
            ICurrentUser currentUser)
        {
            _storeRepository = storeRepository;
            _productRepository = productRepository;
            _productCategoriesRepository = productCategoriesRepository;
            _storeProductRepository = storeProductRepository;
            _saleOrderRepository = saleOrderRepository;
            _saleOrderLineRepository = saleOrderLineRepository;
            _userStoreRepository = userStoreRepository;
            _billCustomerRepository = billCustomerRepository;
            _billCustomerProductRepository = billCustomerProductRepository;
            _currentUser = currentUser;
            _productStockViewRepository = productStockViewRepository;
        }
        public async Task<byte[]> ExportProductStockAsync(SearchProductExcelRequest request)
        {
            try
            {
                var paramSearch = new SearchProductStockRequest();
                paramSearch.StoreIds = request.StoreIds;
                paramSearch.ProductCategoryIds = request.ProductCategoryIds;
                paramSearch.ProductCode = request.ProductCode;
                paramSearch.ProductName = request.ProductName;
                paramSearch.InventoryFilter = request.InventoryFilter;
                paramSearch.InventoryStatus = request.InventoryStatus;
                paramSearch.PageIndex = 1;
                paramSearch.PageSize = 10000000;
                var data = await Search(paramSearch);
                if (data != null && data.Data.Count() > 0)
                {
                    var response = data.Data.ToList();
                    if (request.ReponseIds != null && request.ReponseIds.Count > 0)
                    {
                        response = response.Where(x => request.ReponseIds.Contains(x.Id)).ToList();
                    }
                    var exportData = new List<ExportProductStockResponse>();
                    foreach (var item in response)
                    {
                        exportData.Add(new ExportProductStockResponse()
                        {

                        });
                    }
                    return ExcelHelper.ExportExcel(response);
                }
                else
                {
                    return ExcelHelper.ExportExcel(new List<StoreProductResponse>());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
            
        }

        public async Task<SearchProductStockResponse> Search(SearchProductStockRequest request)
        {
            try
            {
                var storeQuery = await _storeRepository.GetQueryableAsync();
                var productQuery = await _productRepository.GetQueryableAsync();
                var categoryIdQuery = await _productCategoriesRepository.GetQueryableAsync();
                var storeProductQuery = await _storeProductRepository.GetQueryableAsync();
                var userStoreQuery = await _userStoreRepository.GetQueryableAsync();
                var productStockViewQuery = (await _productStockViewRepository.GetQueryableAsync())
                    .WhereIf(request.StoreIds != null && request.StoreIds.Count > 0, p => p.StoreId != null && request.StoreIds.Contains(p.StoreId.Value))
                    .GroupBy(p => p.Id)
                    .Select(grp => new
                    {
                        ProductId = grp.Key,
                        StockQuantity = grp.Sum(p => p.StockQuantity),
                        ComingQuantity = grp.Sum(p => p.ComingQuantity),
                        CustomerOrderQuantity = grp.Sum(p => p.CustomerOrderQuantity),
                        DeliveryQuantity = grp.Sum(p => p.DeliveryQuantity),
                        HoldQuantity = grp.Sum(p => p.HoldQuantity),
                        CanSellQuantity = grp.Sum(p => p.CanSellQuantity),
                    }).ToList();

                var stores = storeQuery
                    .WhereIf(request.StoreIds != null && request.StoreIds.Count > 0, p => request.StoreIds.Contains(p.Id)).ToList();

                var products = productQuery
                    .WhereIf(!request.ProductCode.IsNullOrEmpty(), p => p.Code.Contains(request.ProductCode))
                    .WhereIf(!request.ProductName.IsNullOrEmpty(), p => p.Code.Contains(request.ProductName) || p.Name.Contains(request.ProductName)).ToList();
                var categoryId = categoryIdQuery
                    .WhereIf(request.ProductCategoryIds != null, p => p.Id == request.ProductCategoryIds).ToList();
                var productStockQuery =
                    (
                        from p in products
                        join c in categoryId on p.CategoryId equals c.Id
                        join pStock in productStockViewQuery on p.Id equals pStock.ProductId into pStockLeftJoin
                        from pStock in pStockLeftJoin.DefaultIfEmpty()
                        select new
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Code = p.Code,
                            StockQuantity = pStock != null ? pStock.StockQuantity : 0,
                            DeliveryQuantity = pStock != null ? pStock.DeliveryQuantity : 0,
                            HoldQuantity = pStock != null ? pStock.HoldQuantity : 0,
                            CanSellQuantity = pStock != null ? pStock.CanSellQuantity : 0,
                            ComingQuantity = pStock != null ? pStock.ComingQuantity : 0,
                            PreOrderQuantity = pStock != null ? pStock.CustomerOrderQuantity : 0
                        }
                    )
                    .WhereIf(request.InventoryFilter == ProductInventoryFilter.OnHold, p => p.StockQuantity > 0)
                    .WhereIf(request.InventoryFilter == ProductInventoryFilter.OnSale, p => p.CanSellQuantity > 0)
                    .WhereIf(request.InventoryStatus == ProductInventoryStatus.DeliveryInProgressLte0, p => p.DeliveryQuantity <= 0)
                    .WhereIf(request.InventoryStatus == ProductInventoryStatus.DeliveryInProgressGt0, p => p.DeliveryQuantity > 0)
                    .WhereIf(request.InventoryStatus == ProductInventoryStatus.ComingLte0, p => p.ComingQuantity <= 0)
                    .WhereIf(request.InventoryStatus == ProductInventoryStatus.ComingGt0, p => p.ComingQuantity > 0)
                    .WhereIf(request.InventoryStatus == ProductInventoryStatus.OnHoldLte0, p => p.HoldQuantity <= 0)
                    .WhereIf(request.InventoryStatus == ProductInventoryStatus.OnHoldGt0, p => p.HoldQuantity > 0)
                    .WhereIf(request.InventoryStatus == ProductInventoryStatus.OnSaleLte0, p => p.CanSellQuantity <= 0)
                    .WhereIf(request.InventoryStatus == ProductInventoryStatus.OnSaleGt0, p => p.CanSellQuantity > 0)
                    .WhereIf(request.InventoryStatus == ProductInventoryStatus.PreOrderLte0, p => p.PreOrderQuantity <= 0)
                    .WhereIf(request.InventoryStatus == ProductInventoryStatus.PreOrderGt0, p => p.PreOrderQuantity > 0)
                    .ToList();

                var count = productStockQuery.Count();
                var productResult = productStockQuery.Skip(request.Offset).Take(request.PageSize).ToList();

                var storeIds = stores.Select(p => p.Id);
                var productsIds = productResult.Select(p => p.Id).ToList();

                var storeProducts = storeProductQuery
                    .Where(p => storeIds.Contains(p.StoreId) && productsIds.Contains(p.ProductId))
                    .ToList();

                var productResponse = productResult.Select(product => new StoreProductResponse
                {
                    Id = product.Id,
                    Code = product.Code,
                    Name = product.Name,
                    Stocks = stores.Select(store =>
                    {
                        var storeProduct = storeProducts.FirstOrDefault(p => p.StoreId == store.Id && p.ProductId == product.Id);

                        return new StoreProductStock
                        {
                            StoreId = store.Id,
                            ProductId = product.Id,
                            Quantity = storeProduct != null ? storeProduct.StockQuantity : 0,
                        };
                    }).Append(new StoreProductStock
                    {
                        StoreId = Guid.Empty,
                        ProductId = product.Id,
                        Quantity = product.DeliveryQuantity
                    }).ToList()
                }).ToList();

                var response = new SearchProductStockResponse
                {
                    Stores = stores.Select(store => new MasterDataDTO
                    {
                        Code = store.Code,
                        Name = store.Name,
                        Id = store.Id
                    }).Append(new MasterDataDTO
                    {
                        Code = "",
                        Name = "Đang di chuyển",
                        Id = Guid.Empty
                    }).ToList(),
                    Total = count,
                    Data = productResponse
                };

                return response;
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
            
        }
    }
}
