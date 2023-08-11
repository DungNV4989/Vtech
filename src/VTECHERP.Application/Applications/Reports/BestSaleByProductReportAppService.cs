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
using VTECHERP.DTOs.BestSaleByProductReport;
using VTECHERP.ServiceInterfaces;

namespace VTECHERP.Applications.Reports
{
    [Route("api/app/bestSaleByProductReport/[action]")]
    [Authorize]
    public class BestSaleByProductReportAppService : ApplicationService
    {
        private readonly IBestSaleByProductReportService _bestSaleReportService;
        public BestSaleByProductReportAppService(IBestSaleByProductReportService bestSaleReportService)
        {
            _bestSaleReportService = bestSaleReportService;
        }

        [HttpPost]
        public async Task<IActionResult> Search(BestSaleByProductReportRequest request)
        {
            try
            {
                var reports = await _bestSaleReportService.SearchReportAsync(request);
                return reports;
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }

        }

        [HttpPost]
        public async Task<FileResult> ExportBestSaleByProductAsync(BestSaleByProductReportRequest request)
        {
            try
            {
                var file = await _bestSaleReportService.ExportBestSaleByProductAsync(request);
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"DanhSach_BestSale_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
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
