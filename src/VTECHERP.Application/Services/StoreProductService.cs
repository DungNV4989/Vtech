using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;
using VTECHERP.DTOs.StoreProducts;
using VTECHERP.Entities;

namespace VTECHERP.Services
{
    public class StoreProductService : IStoreProductService
    {
        private readonly IRepository<StoreProduct> _storeProductRepository;
        private readonly IObjectMapper _objectMapper;
        private readonly ICurrentUser _currentUser;

        public StoreProductService(
            IRepository<StoreProduct> storeProductRepository,
            IObjectMapper objectMapper,
            ICurrentUser currentUser)
        {
            _storeProductRepository = storeProductRepository;
            _objectMapper = objectMapper;
            _currentUser = currentUser;
        }

        public async Task<List<StoreProductDto>> GetByStoreId(Guid storeId)
        {
            var result = new List<StoreProductDto>();

            var storeProducts = (await _storeProductRepository.GetQueryableAsync()).Where(x => x.StoreId == storeId);

            if (!storeProducts.Any())
                return result;

            result = _objectMapper.Map<List<StoreProduct>, List<StoreProductDto>>(storeProducts.ToList());
            return result;
        }
    }
}