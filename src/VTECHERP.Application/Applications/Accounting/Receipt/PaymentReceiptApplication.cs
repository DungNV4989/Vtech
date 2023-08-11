using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VTECHERP.Constants;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.PaymentReceipt;
using VTECHERP.Services;

namespace VTECHERP
{
    [Route("api/app/payment-receipt/[action]")]
    [Authorize]
    public class PaymentReceiptApplication: ApplicationService
    {
        private readonly IPaymentReceiptService _paymentReceiptService;
        public PaymentReceiptApplication(IPaymentReceiptService paymentReceiptService)
        {
            _paymentReceiptService = paymentReceiptService;
        }

        [HttpPost]
        public async Task<Guid> Create(CreatePaymentReceiptRequest request)
        {
            return await _paymentReceiptService.CreatePaymentReceipt(request);
        }

        [HttpPut]
        public async Task<bool> Update(UpdatePaymentReceiptRequest request)
        {
            await _paymentReceiptService.UpdatePaymentReceipt(request);
            return true;
        }

        [HttpPost]
        public async Task<PagingResponse<PaymentReceiptDTO>> Search(SearchPaymentReceiptRequest request)
        {
            return await _paymentReceiptService.SearchPaymentReceipt(request);
        }

        [HttpGet]
        public async Task<PaymentReceiptDTO> Get(Guid id)
        {
            return await _paymentReceiptService.GetPaymentReceipt(id);
        }

        [HttpDelete]
        public async Task<bool> Delete(Guid id)
        {
            await _paymentReceiptService.DeletePaymentReceipt(id);
            return true;
        }

        [HttpPost]
        public async Task<bool> ChangeLiquidity(Guid id)
        {
            return await _paymentReceiptService.ChangeLiquidity(id);
        }
        [HttpPost]
        public async Task<FileResult> ExportPaymentReceiptAsync(SearchPaymentReceiptRequest request)
        {
            try
            {
                var file = await _paymentReceiptService.ExportPaymentReceipt(request);
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"DanhSach_Lịch sử sửa xóa phiếu XNK_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
                };
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                throw;
            }
        }
    }
}
