using System.Threading.Tasks;
using System;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.PaymentReceipt;
using static VTECHERP.Constants.EntryConfig;
using System.Collections.Generic;
using VTECHERP.DTOs.Base;

namespace VTECHERP.Services
{
    public interface IPaymentReceiptService: IScopedDependency
    {
        Task<Guid> CreatePaymentReceipt(CreatePaymentReceiptRequest request);
        Task UpdatePaymentReceipt(UpdatePaymentReceiptRequest request);
        Task DeletePaymentReceipt(Guid id);
        Task<PagingResponse<PaymentReceiptDTO>> SearchPaymentReceipt(SearchPaymentReceiptRequest request);
        Task<PaymentReceiptDTO> GetPaymentReceipt(Guid id);
        Task AutoCreatePaymentReceiptOnWarehousingBill(Guid billId);
        Task AutoDeletePaymentReceiptOnWarehousingBill(Guid billId);
        Task AutoCreatePaymentReceiptForReturnProduct(Guid id);
        Task AutoDeletePaymentReceiptForReturnProduct(Guid id);

        /// <summary>
        /// Thay đổi trạng thái thanh khoản của phiếu thu chi
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> ChangeLiquidity(Guid id);
        Task<byte[]> ExportPaymentReceipt(SearchPaymentReceiptRequest request);
    }
}
