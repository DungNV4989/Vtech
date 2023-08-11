using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.ProductByCustomerReport;

namespace VTECHERP.ServiceInterfaces
{
    public interface IProductByCustomerReportService : IScopedDependency
    {
        Task<IActionResult> SearchReportAsync(ProductByCustomerReportRequest input, CancellationToken cancellationToken = default);
        Task<byte[]> ExportProductByCustomerAsync(ProductByCustomerReportRequest request);
    }
}
