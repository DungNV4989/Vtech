using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Uow;
using VTECHERP.Constants;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.BillCustomers.Params;
using VTECHERP.DTOs.CustomerReturnImport.Params;
using VTECHERP.DTOs.ExchangeReturn;
using VTECHERP.Helper;
using VTECHERP.ServiceInterfaces;
using VTECHERP.Services;

namespace VTECHERP.Applications.CustomerReturn
{
    [Route("api/app/customer-return/[action]")]
    [Authorize]
    public class CustomerReturnApplication : ApplicationService
    {
        private readonly ICustomerReturnService _customerReturnService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public CustomerReturnApplication(
            ICustomerReturnService customerReturnService
            , IUnitOfWorkManager unitOfWorkManager
            )
        {
            _customerReturnService = customerReturnService;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateCustomerReturnRequest request)
        {
            if (request == null)
            {
                return new GenericActionResult(400, false);
            }
            CustomerReturnDTO dto = new CustomerReturnDTO();
            List<CustomerReturnProductDTO> lstPRod = new List<CustomerReturnProductDTO>();
            lstPRod.Add(new CustomerReturnProductDTO());
            dto.Products = lstPRod;
            var respon = await _customerReturnService.Create(request);
            var httpStatus = respon.Item3 ? 200 : 400;
            return new GenericActionResult(httpStatus, respon.Item3, respon.Item2, respon.Item1);
        }

        [HttpPost]
        public async Task<IActionResult> GetListAsync(SearchCustomerReturnRequest request)
        {
            var respon = (await _customerReturnService.Search(request));
            var httpStatus = respon.Item3 ? 200 : 400;
            return new GenericActionResult(httpStatus, respon.Item3, respon.Item2, respon.Item1);
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var respon = await _customerReturnService.Get(id);
            var httpStatus = respon.Item3 ? 200 : 400;
            return new GenericActionResult(httpStatus, respon.Item3, respon.Item2, respon.Item1);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(CreateCustomerReturnRequest request)
        {
            var res = await _customerReturnService.Update(request);
            var httpStatus = res.Item3 ? 200 : 400;
            return new GenericActionResult(httpStatus, res.Item3, res.Item2, res.Item1);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmAsync(Guid id)
        {
            var res = await _customerReturnService.ConfirmAsync(id);
            int status = res.Item2 ? 200 : 400;
            return new GenericActionResult(status, res.Item2, res.Item1, null);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateNoteAsync(CreateCustomerReturnRequest request)
        {
            var res = await _customerReturnService.UpdateNoteAsync(request.Id.Value, request.PayNote);
            var httpStatus = res.Item3 ? 200 : 400;
            return new GenericActionResult(httpStatus, res.Item3, res.Item2, res.Item1);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var res = await _customerReturnService.Delete(id);
            var httpStatus = res.Item3 ? 200 : 400;
            return new GenericActionResult(httpStatus, res.Item3, res.Item2, res.Item1);
        }

        [HttpPost]
        public async Task<FileResult> ExportCustomerReturnAsync(SearchCustomerReturnRequest request)
        {
            try
            {
                var file = await _customerReturnService.ExportCustomerReturn(request);
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"DanhSach_Trả hàng_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
                };
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportBillCustomerReturn([FromForm] CustomerReturnImportParam param)
        {
            using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
            {
                if (param == null || param.File.Length <= 0)
                    return new BadRequestObjectResult($"Dữ liệu không hợp lệ");

                // Kiểm tra phần mở rộng tệp
                var fileExtension = Path.GetExtension(param.File.FileName);
                if (fileExtension.ToLower() != ".xlsx" && fileExtension.ToLower() != ".xls")
                    return new BadRequestObjectResult("Sai định dạng file");

                var responCreate = await _customerReturnService.ImportBillCustomerReturn(param);

                if (responCreate.success)
                {
                    await uow.CompleteAsync();
                }

                return new OkObjectResult(new
                {
                    Data = responCreate.data,
                    Success = responCreate.success,
                    Message = responCreate.message,
                    FileReturn = responCreate.fileRespon
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadTemplateImport()
        {
            var file = await _customerReturnService.DownloadTemplateImport();
            return new FileContentResult(file, ContentTypes.SPREADSHEET)
            {
                FileDownloadName = $"Mau_import_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
            };
        }
    }
}
