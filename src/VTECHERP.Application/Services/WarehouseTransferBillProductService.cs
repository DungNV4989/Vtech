using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;
using VTECHERP.DTOs.WarehouseTransferBillProducts;
using VTECHERP.Entities;

namespace VTECHERP.Services
{
    public class WarehouseTransferBillProductService : IWarehouseTransferBillProductService
    {
        private readonly IRepository<WarehouseTransferBillProduct> _warehouseTransferBillProductRepository;
        private readonly IProductService _productService;
        private readonly IObjectMapper _objectMapper;
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<Products> _productRepository;

        public WarehouseTransferBillProductService(
            IRepository<WarehouseTransferBillProduct> warehouseTransferBillProductRepository,
            IProductService productService,
            IRepository<Products> productRepository,
            IObjectMapper objectMapper,
            ICurrentUser currentUser)
        {
            _warehouseTransferBillProductRepository = warehouseTransferBillProductRepository;
            _productService = productService;
            _objectMapper = objectMapper;
            _currentUser = currentUser;
            _productRepository = productRepository;
        }

        public async Task AddRangeAsync(Guid warehouseTransferBillId, List<WarehouseTransferBillProductCreateRequest> requests)
        {
            var warehouseTransferBillProducts = _objectMapper.Map<List<WarehouseTransferBillProductCreateRequest>, List<WarehouseTransferBillProduct>>(requests);

            var productIds = requests.Select(x =>x.ProductId).ToList();
            var products = _productRepository.GetDbSetAsync().Result.AsNoTracking().Where(x => productIds.Contains(x.Id)).ToList();

            warehouseTransferBillProducts.ForEach(x =>
            {
                x.WarehouseTransferBillId = warehouseTransferBillId;
                x.CreatorId = _currentUser.Id;
                x.CostPrice = products.FirstOrDefault(z => z.Id == x.ProductId)?.StockPrice ?? 0;
            });

            await _warehouseTransferBillProductRepository.InsertManyAsync(warehouseTransferBillProducts);
        }

        public async Task<List<WarehouseTransferBillProductDetailDto>> GetByIdsAsync(List<Guid> ids)
        {
            var warehouseTransferBillProducts = (await _warehouseTransferBillProductRepository.GetQueryableAsync()).Where(x => ids.Any(id => id == x.Id)).ToList();
            var result = _objectMapper.Map<List<WarehouseTransferBillProduct>, List<WarehouseTransferBillProductDetailDto>>(warehouseTransferBillProducts);
            return result;
        }

        public async Task<List<WarehouseTransferBillProductDetailDto>> GetByWarehouseTransferBillId(Guid warehouseTransferBillId)
        {
            var result = new List<WarehouseTransferBillProductDetailDto>();
            var warehouseTransferBillProducts = (await _warehouseTransferBillProductRepository.GetQueryableAsync()).Where(x => x.WarehouseTransferBillId == warehouseTransferBillId);
            if (warehouseTransferBillProducts == null)
                return result;
            result = _objectMapper.Map<List<WarehouseTransferBillProduct>, List<WarehouseTransferBillProductDetailDto>>(warehouseTransferBillProducts.ToList());
            var products = await _productService.GetByIdsAsync(result.Select(x => x.ProductId).ToList());
            result.ForEach(x =>
            {
                var product = products.FirstOrDefault(p => p.Id == x.ProductId);
                if (product != null)
                {
                    x.ProductName = product.Name;
                    x.BarCode = product.BarCode;
                }
            });
            return result;
        }

        public async Task<List<WarehouseTransferBillProductDetailDto>> GetByWarehouseTransferBillIds(List<Guid> warehouseTransferBillIds)
        {
            var result = new List<WarehouseTransferBillProductDetailDto>();
            var warehouseTransferBillProducts = (await _warehouseTransferBillProductRepository.GetQueryableAsync()).Where(x => warehouseTransferBillIds.Any(warehouseTransferBillId => warehouseTransferBillId == x.WarehouseTransferBillId));
            if (warehouseTransferBillProducts == null)
                return result;
            result = _objectMapper.Map<List<WarehouseTransferBillProduct>, List<WarehouseTransferBillProductDetailDto>>(warehouseTransferBillProducts.ToList());
            return result;
        }

        public async Task<List<WarehouseTransferBillProductApproveDto>> SetApproveByWarehouseTransferBillIdAsync(Guid warehouseTransferBillId, Guid storeId, string productName)
        {
            var result = new List<WarehouseTransferBillProductApproveDto>();
            var warehouseTransferBillProducts = (await _warehouseTransferBillProductRepository.GetQueryableAsync())
                .Where(x => x.WarehouseTransferBillId == warehouseTransferBillId);
            if (warehouseTransferBillProducts == null)
                return result;
            result = _objectMapper.Map<List<WarehouseTransferBillProduct>, List<WarehouseTransferBillProductApproveDto>>(warehouseTransferBillProducts.ToList());
            var products = await _productService.MapStockQuantityAsync(storeId, warehouseTransferBillProducts.Select(x => x.ProductId).ToList());
            result.ForEach(x =>
            {
                var product = products.FirstOrDefault(p => p.Id == x.ProductId);
                if (product != null)
                {
                    x.ProductName = product.Name;
                    x.BarCode = product.BarCode;
                    x.StockQuantity = product.StockQuantity;
                }
            });
            return result.WhereIf(!productName.IsNullOrWhiteSpace(), x => x.ProductName.ToLower().Contains(productName.ToLower())).ToList();
        }

        public async Task ApprovesAsync(List<WarehouseTransferBillProductApproveRequest> requests)
        {
            var warehouseTransferBillProducts = (await _warehouseTransferBillProductRepository.GetListAsync()).Where(x => requests.Any(r => r.Id == x.Id)).ToList();
            if (!warehouseTransferBillProducts.Any())
                return;
            warehouseTransferBillProducts.ForEach(x =>
            {
                var approveQuantity = requests.FirstOrDefault(r => r.Id == x.Id).ApproveQuantity;
                x.Quantity = approveQuantity;
            });

            await _warehouseTransferBillProductRepository.UpdateManyAsync(warehouseTransferBillProducts);
        }

        public async Task DeleteRangeAsync(Guid warehouseTransferBillId)
        {
            var warehouseTransferBillProducts = (await _warehouseTransferBillProductRepository.GetQueryableAsync()).Where(x => x.WarehouseTransferBillId == warehouseTransferBillId).ToList();
           
            warehouseTransferBillProducts.ForEach(x =>
            {
                x.IsDeleted = true;
                x.DeleterId = _currentUser.Id;
            });

            await _warehouseTransferBillProductRepository.UpdateManyAsync(warehouseTransferBillProducts);
        }
    }
}