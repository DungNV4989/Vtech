using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.Entries;
using VTECHERP.DTOs.SaleOrders;
using VTECHERP.Models;

namespace VTECHERP.Services
{
    public interface IEntryService : IScopedDependency
    {
        Task<PagingResponse<EntryDTO>> SearchEntry(SearchEntryRequest request);

        /// <summary>
        /// Tự động tạo bút toán khi tạo đơn hàng
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task AutoCreateEntryByCreateSupplierOrder(Guid orderId);

        /// <summary>
        /// Tự động cập nhật bút toán khi cập nhật đơn hàng
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task AutoUpdateEntryByUpdateSupplierOrder(Guid orderId);
        /// <summary>
        /// Tự động tạo bút toán khi xác nhận đơn hàng
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="warehousingBillId"></param>
        /// <param name="confirm">True nếu đc gọi tại func xác nhận đơn đặt hàng ncc</param>
        /// <returns></returns>
        Task AutoCreateEntryByConfirmSupplierOrder(Guid orderId, Guid warehousingBillId, decimal? saleAmount, bool confirm = false);

        Task AutoCreateEntryByConfirmOrderSurplus(Guid orderId, Guid warehousingBillId, decimal? saleAmount);

        /// <summary>
        /// Tự động tạo bút toán khi hoàn thành đơn hàng
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task AutoCreateEntryByCompleteOrder(Guid orderId);

        /// <summary>
        /// Tự động tạo bút toán khi tạo phiếu xuất nhập kho
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        Task AutoCreateEntryByCreateWarehousingBill(Guid billId, bool autoInsertDebt = true);

        /// <summary>
        /// Tự động cập nhật bút toán khi cập nhật phiếu xuất nhập kho
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        Task AutoUpdateEntryByUpdateWarehousingBill(Guid billId);

        /// <summary>
        /// Tự động xóa bút toán khi xóa phiếu xuất nhập kho
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        Task AutoDeleteEntryByDeleteWarehousingBill(Guid billId);

        /// <summary>
        /// Tự động tạo bút toán khi tạo phiếu thu chi
        /// </summary>
        /// <param name="receiptId"></param>
        /// <returns></returns>
        Task AutoCreateEntryOnCreatePaymentReceipt(Guid receiptId);

        /// <summary>
        /// Tự động cập nhật bút toán khi cập nhật phiếu thu chi
        /// </summary>
        /// <param name="receiptId"></param>
        /// <returns></returns>
        Task AutoUpdateEntryOnUpdatePaymentReceipt(Guid receiptId);

        /// <summary>
        /// Tự động xóa bút toán khi xóa phiếu thu chi
        /// </summary>
        /// <param name="receiptId"></param>
        /// <returns></returns>
        Task AutoDeleteEntryOnDeletePaymentReceipt(Guid receiptId);

        /// <summary>
        /// Tạo bút toán thủ công
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uploadFile"></param>
        /// <returns></returns>
        Task<Guid> ManualCreateEntry(EntryCreateRequest request, UploadFileResult uploadFile);

        /// <summary>
        /// cập nhật bút toán thủ công
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uploadFile"></param>
        /// <returns></returns>
        Task ManualUpdateEntry(EntryUpdateRequest request, UploadFileResult uploadFile);

        Task<EntryResponse> GetEntryById(Guid id);

        /// <summary>
        /// Xóa bút toán thủ công
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Delete(Guid id);

        /// <summary>
        /// Sửa ghi chú
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task ManualEditNoteEntry(EditNoteEntryRequest request);

        /// <summary>
        /// Chi tiết bút toán
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<PagingResponse<EntryDetailResponse>> EntryDetailAsync(SearchEntryDetailRequest request);

        /// <summary>
        /// Tự động xóa bút toán khi xóa đơn đặt hàng
        /// </summary>
        /// <param name="saleOrderId"></param>
        /// <returns></returns>
        Task AutoDeleteEntryOnDeleteSaleOrder(Guid saleOrderId);

        /// <summary>
        /// Danh sách bút toán - Xuất Excel
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<byte[]> ExportEntryAsync(SearchEntryRequest request);
        /// <summary>
        /// Tạo bút toán khi tạo hóa đơn bán hàng
        /// </summary>
        /// <param name="BillCustomerId"></param>
        /// <param name="amountAfterDiscount"></param>
        /// <returns></returns>
        Task AutoCreateEntryForCreatBillCustomer(Guid BillCustomerId, decimal amountAfterDiscount);
        /// <summary>
        /// Tạo bút toán khi tạo hóa đơn bán hàng có vat
        /// </summary>
        /// <param name="BillCustomerId"></param>
        /// <param name="amountAfterDiscount"></param>
        /// <returns></returns>
        Task AutoCreateEntryForCreatBillCustomerHasVAT(Guid BillCustomerId, decimal Amount);
        Task AutoDeleteEntryForBillCustomer(Guid BillCustomerId);
        Task AutoUpdateEntryForBillCustomer(Guid BillCustomerId);
        Task AutoUpdateEntryVatForBillCustomer(Guid BillCustomerId, decimal amount);

        Task AutoCreateEntryForReturnProduct(Guid CustomerReturnId, decimal amount);
        Task AutoDeleteEntryForReturnProduct(Guid BillCustomerId);

    }
}