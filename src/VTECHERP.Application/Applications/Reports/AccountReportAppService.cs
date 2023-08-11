using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTECHERP.Reports;
using Volo.Abp.Application.Services;
using VTECHERP.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using VTECHERP.Constants;

namespace VTECHERP.Applications.Reports
{
    public class AccountReportAppService : ApplicationService
    {
        private readonly IAccountReportService _accountReportService;
        public AccountReportAppService(IAccountReportService accountReportService)
        {
            _accountReportService = accountReportService;
        }
        [HttpGet]
        public IActionResult Search(SearchAccountReport request)
        {
            try
            {
                var reports =_accountReportService.SearchAccountReportAsync(request);
                return reports;
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }

        }
    }
}
