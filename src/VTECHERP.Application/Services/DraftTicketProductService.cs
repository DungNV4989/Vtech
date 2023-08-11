using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;
using VTECHERP.DTOs.DraftTicketProducts;
using VTECHERP.Entities;

namespace VTECHERP.Services
{
    public class DraftTicketProductService : IDraftTicketProductService
    {
        private readonly IRepository<DraftTicketProduct> _draftTicketProductRepository;
        private readonly IProductService _productService;
        private readonly IObjectMapper _objectMapper;
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<Products> _productRepository;

        public DraftTicketProductService(
            IRepository<DraftTicketProduct> draftTicketProductRepository,
            IProductService productService,
            IRepository<Products> productRepository,
            IObjectMapper objectMapper,
            ICurrentUser currentUser)
        {
            _draftTicketProductRepository = draftTicketProductRepository;
            _productService = productService;
            _objectMapper = objectMapper;
            _currentUser = currentUser;
            _productRepository = productRepository;
        }

        public async Task AddRangeAsync(Guid DraftTicketId, List<DraftTicketProductCreateRequest> requests)
        {
            var draftTicketProducts = _objectMapper.Map<List<DraftTicketProductCreateRequest>, List<DraftTicketProduct>>(requests);

            var productIds = requests.Select(x => x.ProductId).ToList();
            var products = _productRepository.GetDbSetAsync().Result.AsNoTracking().Where(x => productIds.Contains(x.Id)).ToList();

            draftTicketProducts.ForEach(x =>
            {
                x.DraftTicketId = DraftTicketId;
                x.CreatorId = _currentUser.Id;
                x.CostPrice = products.FirstOrDefault(z => z.Id == x.ProductId)?.StockPrice ?? 0;
            });

            await _draftTicketProductRepository.InsertManyAsync(draftTicketProducts);
        }

        public async Task<List<DraftTicketProductDetailDto>> GetByIdsAsync(List<Guid> ids)
        {
            var draftTicketProducts = (await _draftTicketProductRepository.GetQueryableAsync()).Where(x => ids.Any(id => id == x.Id)).ToList();
            var result = _objectMapper.Map<List<DraftTicketProduct>, List<DraftTicketProductDetailDto>>(draftTicketProducts);
            return result;
        }

        public async Task<List<DraftTicketProductDetailDto>> GetByDraftTicketId(Guid draftTicketId)
        {
            var result = new List<DraftTicketProductDetailDto>();
            var draftTicketProducts = (await _draftTicketProductRepository.GetQueryableAsync()).Where(x => x.DraftTicketId == draftTicketId);
            if (draftTicketProducts == null)
                return result;
            result = _objectMapper.Map<List<DraftTicketProduct>, List<DraftTicketProductDetailDto>>(draftTicketProducts.ToList());
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

        public async Task<List<DraftTicketProductDetailDto>> GetByDraftTicketIds(List<Guid> draftTicketIds)
        {
            var result = new List<DraftTicketProductDetailDto>();
            var draftTicketProducts = (await _draftTicketProductRepository.GetQueryableAsync()).Where(x => draftTicketIds.Any(draftTicketId => draftTicketId == x.DraftTicketId));
            if (draftTicketProducts == null)
                return result;
            result = _objectMapper.Map<List<DraftTicketProduct>, List<DraftTicketProductDetailDto>>(draftTicketProducts.ToList());
            return result;
        }

        public async Task<List<DraftTicketProductApproveDto>> SetApproveByDraftTicketIdAsync(Guid draftTicketId, Guid storeId, string productName)
        {
            var result = new List<DraftTicketProductApproveDto>();
            var draftTicketProducts = (await _draftTicketProductRepository.GetQueryableAsync())
                .Where(x => x.DraftTicketId == draftTicketId);
            if (draftTicketProducts == null)
                return result;
            result = _objectMapper.Map<List<DraftTicketProduct>, List<DraftTicketProductApproveDto>>(draftTicketProducts.ToList());
            var products = await _productService.MapStockQuantityAsync(storeId, draftTicketProducts.Select(x => x.ProductId).ToList());
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

        public async Task ApprovesAsync(List<DraftTicketProductApproveRequest> requests)
        {
            var draftTicketProducts = (await _draftTicketProductRepository.GetListAsync()).Where(x => requests.Any(r => r.Id == x.Id)).ToList();
            if (!draftTicketProducts.Any())
                return;
            draftTicketProducts.ForEach(x =>
            {
                var approveQuantity = requests.FirstOrDefault(r => r.Id == x.Id).ApproveQuantity;
                x.Quantity = approveQuantity;
            });

            await _draftTicketProductRepository.UpdateManyAsync(draftTicketProducts);
        }

        public async Task DeleteRangeAsync(Guid draftTicketId)
        {
            var draftTicketProducts = (await _draftTicketProductRepository.GetQueryableAsync()).Where(x => x.DraftTicketId == draftTicketId).ToList();

            draftTicketProducts.ForEach(x =>
            {
                x.IsDeleted = true;
                x.DeleterId = _currentUser.Id;
            });

            await _draftTicketProductRepository.UpdateManyAsync(draftTicketProducts);
        }
    }
}