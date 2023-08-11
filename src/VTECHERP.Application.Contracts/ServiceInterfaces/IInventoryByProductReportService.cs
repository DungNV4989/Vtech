using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.InventoryByProductReport;

namespace VTECHERP.ServiceInterfaces
{
    public interface IInventoryByProductReportService : IScopedDependency
    {

        Task<IActionResult> SearchReportAsync(InventoryByProductReportRequest input, CancellationToken cancellationToken = default);
        Task<byte[]> ExportInventoryByProductAsync(InventoryByProductReportRequest request);
    }
}
