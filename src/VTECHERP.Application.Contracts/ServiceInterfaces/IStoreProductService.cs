using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.StoreProducts;

namespace VTECHERP.Services
{
    public interface IStoreProductService : IScopedDependency
    {
        Task<List<StoreProductDto>> GetByStoreId(Guid storeId);
    }
}