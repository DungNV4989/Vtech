using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.WarehouseTransferBillProducts;

namespace VTECHERP.Services
{
    public interface IWarehouseTransferBillProductService : IScopedDependency
    {
        Task AddRangeAsync(Guid warehouseTransferBillId, List<WarehouseTransferBillProductCreateRequest> requests);

        Task<List<WarehouseTransferBillProductDetailDto>> GetByIdsAsync(List<Guid> ids);

        Task<List<WarehouseTransferBillProductDetailDto>> GetByWarehouseTransferBillId(Guid warehouseTransferBillId);

        Task<List<WarehouseTransferBillProductDetailDto>> GetByWarehouseTransferBillIds(List<Guid> warehouseTransferBillIds);

        Task<List<WarehouseTransferBillProductApproveDto>> SetApproveByWarehouseTransferBillIdAsync(Guid warehouseTransferBillId, Guid storeId, string productName);

        Task ApprovesAsync(List<WarehouseTransferBillProductApproveRequest> requests);

        Task DeleteRangeAsync(Guid warehouseTransferBillId);
    }
}