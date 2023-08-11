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
using VTECHERP.DTOs.ProductByCustomerReport;
using VTECHERP.ServiceInterfaces;

namespace VTECHERP.Applications.Reports
{
    [Route("api/app/productByCustomerReport/[action]")]
    [Authorize]
    public class ProductByCustomerReportAppService : ApplicationService
    {
        private readonly IProductByCustomerReportService _reportService;
        public ProductByCustomerReportAppService(IProductByCustomerReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost]
        public async Task<IActionResult> Search(ProductByCustomerReportRequest request)
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
        public async Task<FileResult> ExportProductByCustomerAsync(ProductByCustomerReportRequest request)
        {
            try
            {
                var file = await _reportService.ExportProductByCustomerAsync(request);
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"DanhSach_sản phẩm theo khách hàng_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
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
