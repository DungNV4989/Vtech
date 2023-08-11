using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.BillCustomers.Params;
using VTECHERP.DTOs.WarehouseTransferBills;

namespace VTECHERP.Services
{
    public interface IWarehouseTransferBillService : IScopedDependency
    {

        Task<WarehouseTransferBillDetaildDto> GetByIdAsync(Guid id);

        Task<Guid> AddWarehouseTransferBillAsync(WarehouseTransferBillCreateRequest request);

        Task<Guid> DeleteAsync(Guid id);
        Task DeleteMoving(Guid id);
        Task DeleteComing(Guid id);

        Task DeleteRangeAsync(List<Guid> ids);

        /// <summary>
        /// Xóa phiếu chuyển kho
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Guid> DeleteWarehouseTransferBill(Guid id, bool isManual);
        /// <summary>
        /// Xác nhận phiếu chuyển kho đến nơi
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Guid> AcceptWarehouseTransferBill(Guid id);
        Task<PagingResponse<SearchWarehouseTransferResponse>> SearchWarehouseTransfer(SearchWarehouseTransferRequest requerst);

        /// <summary>
        /// Đang chuyển đi
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<PagingResponse<SearchWarehouseTransferMovingResponse>> SearchWarehouseTransferMoving(SearchWarehouseTransferMovingRequest request);

        /// <summary>
        /// Sắp chuyển đến
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<PagingResponse<SearchWarehouseTransferComingResponse>> SearchWarehouseTransferComing(SearchWarehouseTransferComingRequest request);

        /// <summary>
        /// Xác nhận phiếu chuyển kho
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task ConfirmAsync(WarehouseTransferBillConfirmRequest request);
        /// <summary>
        /// Export DS đang di chuyển
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<byte[]> ExportWarehouseTransferMoving(SearchWarehouseTransferMovingRequest request);
        /// <summary>
        /// Export DS chuyển kho
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<byte[]> ExportWarehouseTransfer(SearchWarehouseTransferRequest request);
        /// <summary>
        /// Export DS sắp chuyển đến
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<byte[]> ExportWarehouseTransferComing(SearchWarehouseTransferComingRequest request);

        Task<byte[]> DownloadTemplateImport();
        Task<(string message, bool success, Guid? data, byte[]? fileRespon)> ImportBillCustomer(WarehouseTransferImportParam param);
    }
}