using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.ProductXnk;
using VTECHERP.DTOs.WarehousingBills;
using VTECHERP.Enums;

namespace VTECHERP.Services
{
    public interface IWarehousingBillService : IScopedDependency
    {
        Task<PagingResponse<WarehousingBillDto>> SearchBills(SearchWarehousingBillRequest request);

        Task<PagingResponse<SearchProductXnkResponse>> SearchProductXnk(SearchProductXnkRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="calculateStockPrice"></param>
        /// <returns></returns>
        Task<Guid> CreateBill(CreateWarehousingBillRequest request, bool calculateStockPrice = true, bool confirm = false);

        Task UpdateBill(UpdateWarehousingBillRequest request);

        Task DeleteBill(Guid id);

        Task<WarehousingBillDto> GetBill(Guid id);

        Task<List<WarehousingBillDto>> GetContainCodeAsync(string code);

        Task<List<WarehousingBillDto>> GetBySourceIdsAsync(List<Guid?> sourceIds);

        Task DeleteProductInBillProduct(Guid warehousingBillId);
        Task DeleteListProductInBillProduct(DeleteListProductInBillProduct requert);

        Task UpdateProductInBillProduct(UpdateProductInBillProductRequest request);

        Task<GetUpdateProductInBillProductResponse> GetUpdateProductInBillProduct(Guid warehousingBillId);

        Task AutoDeleteBillByWarehouseTransferBill(Guid warehouseTransferBill);
        Task<WarehousingBillDto> GetByWarehouseTransferBillId(Guid warehouseTransferId, WarehousingBillType billType);
        Task AutoCreateWearhousingBillForReturnProduct(Guid id);
        Task AutoDeleteWearhousingBillForReturnProduct(Guid id);
        Task RebuildProductStock();
        Task<byte[]> ExportWarehousingBill(SearchWarehousingBillRequest request);
        Task<byte[]> ExportProductXnk(SearchProductXnkRequest request);
    }
}