using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.Reports;

namespace VTECHERP.ServiceInterfaces
{
    public interface IAccountReportService : IScopedDependency
    {
        IActionResult SearchAccountReportAsync(SearchAccountReport input, CancellationToken cancellationToken = default);
    }
}
