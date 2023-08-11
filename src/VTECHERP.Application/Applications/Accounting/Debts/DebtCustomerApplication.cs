using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VTECHERP.Constants;
using VTECHERP.Debts;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.DebtCustomer;
using VTECHERP.DTOs.Entries;
using VTECHERP.ServiceInterfaces;
using VTECHERP.Services;

namespace VTECHERP
{
    [Route("api/app/DebtCustomer/[action]")]
    [Authorize]
    public class DebtCustomerApplication : ApplicationService
    {
        private readonly IDebtCustomerService _service;
        private readonly IDebtReportService _debtReportService;
        private readonly IConfiguration _configuration;

        public DebtCustomerApplication(
            IDebtCustomerService service,
            IDebtReportService debtReportService,
            IConfiguration configuration
            )
        {
            _service = service;
            _configuration = configuration;
            _debtReportService = debtReportService;
        }

        [HttpPost]
        public async Task<PagingResponse<SearchDebtCustomerResponse>> Search(SearchDebtCustomerRequest request)
        {

            return await _service.Search(request);
        }

        [HttpPost]
        public async Task<TotalDebtCustomerResponse> TotalDebtCustomer(SearchDebtCustomerRequest request)
        {
            return await _service.TotalDebtCustomer(request);
        }
        
        [HttpGet]
        public async Task<bool> CaculateDebtMonthly()
        {
            await _debtReportService.CaculateDebtMonthly();
            return true;
        }

        [HttpPost]
        public async Task<PagingResponse<DebtDetailDto>> DetailDebtCustomer(SearchDebtCustomerDetailRequest request)
        {
            return await _service.DetailDebtCustomer(request);
        }

        [HttpPost]
        public async Task<FileResult> ExportDebtCustomerAsync(SearchDebtCustomerRequest request)
        {
            try
            {
                var file = await _service.ExportDebtCustomer(request);
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"DanhSach_Công nợ khách hàng_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
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
