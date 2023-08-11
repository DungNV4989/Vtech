using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.BestSaleByProductReport;

namespace VTECHERP.ServiceInterfaces
{
    public interface IBestSaleByProductReportService : IScopedDependency
    {
        Task<IActionResult> SearchReportAsync(BestSaleByProductReportRequest input, CancellationToken cancellationToken = default);
        Task<byte[]> ExportBestSaleByProductAsync(BestSaleByProductReportRequest request);
    }
}
