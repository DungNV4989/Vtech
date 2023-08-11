using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VTECHERP.Constants;
using VTECHERP.DTOs.BillCustomers.Params;
using VTECHERP.Entities;
using VTECHERP.ServiceInterfaces;
using VTECHERP.Services;

namespace VTECHERP.Applications.BillCustomerImport
{
    public class BillCustomerImportApplication : ApplicationService
    {
        private readonly IBillCustomerService _billCustomerService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public BillCustomerImportApplication(
             IBillCustomerService billCustomerService
            , IUnitOfWorkManager unitOfWorkManager
            )
        {
            _billCustomerService = billCustomerService;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [HttpPost]
        public async Task<IActionResult> ImportBillCustomer([FromForm] BillCustomerImportParam param)
        {
            using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
            {
                if (param == null || param.File.Length <= 0)
                    return new BadRequestObjectResult($"Dữ liệu không hợp lệ");

                // Kiểm tra phần mở rộng tệp
                var fileExtension = Path.GetExtension(param.File.FileName);
                if (fileExtension.ToLower() != ".xlsx" && fileExtension.ToLower() != ".xls")
                    return new BadRequestObjectResult("Sai định dạng file");

                var responCreate = await _billCustomerService.ImportBillCustomer(param);

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
            var file = await _billCustomerService.DownloadTemplateImport();
            return new FileContentResult(file, ContentTypes.SPREADSHEET)
            {
                FileDownloadName = $"Mau_import_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
            };
        }
    }
}
