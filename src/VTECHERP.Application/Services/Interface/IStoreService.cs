using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.Stores;
using VTECHERP.DTOs.Stores.Params;
using VTECHERP.Entities;

namespace VTECHERP.Services
{
    public interface IStoreService : IScopedDependency
    {
        Task<List<MasterDataDTO>> GetUserManagedStoresAsync();
        Task<List<MasterDataDTO>> GetTenantStoresAsync();
        Task<StoreDetailDto> GetByIdAsync(Guid id);
        Task<List<StoreDto>> GetByIdsAsync(List<Guid> ids);
        Task<bool> Exist(Guid id);
        Task<bool> SetFlagStore(Guid storeId);
        Task<(Stores data, string message, bool success)> Create(CreateStoreParam param);
        Task<(StoreDetailToUpdate data, string message, bool success)> GetDetail(Guid StoreId);
        Task<(List<StoreListItem> data, int count)> Search(SearchListStoreParam param);
        Task<byte[]> ExportExcel(ExportStoreParam param);
    }
}