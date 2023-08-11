using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using VTECHERP.Constants;
using VTECHERP.DTOs.InventoryByProductReport;
using VTECHERP.ServiceInterfaces;

namespace VTECHERP.Applications.Reports
{
    [Route("api/app/inventoryByProductReport/[action]")]
    [Authorize]
    public class InventoryByProductReportAppService : ApplicationService
    {
        private readonly IInventoryByProductReportService _reportService;
        public InventoryByProductReportAppService(IInventoryByProductReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost]
        public async Task<IActionResult> Search(InventoryByProductReportRequest request)
        {
            try
            {
                var reports = await _reportService.SearchReportAsync(request);
                return reports;
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }

        }

        [HttpPost]
        public async Task<FileResult> ExportInventoryByProductAsync(InventoryByProductReportRequest request)
        {
            try
            {
                var file = await _reportService.ExportInventoryByProductAsync(request);
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"DanhSach_Tồn kho theo sản phẩm_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
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
