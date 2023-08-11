using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.Base;
using VTECHERP.Reports;

namespace VTECHERP.ServiceInterfaces
{
    public interface IStoreReportService : IScopedDependency
    {
        Task<IActionResult> SearchStoreReportAsync(SearchRequest input, CancellationToken cancellationToken = default);
        Task<List<StoreReportDto>> ExportStoreReportAsync(SearchRequest input, CancellationToken cancellationToken = default);
        Task<byte[]> ExportStoreReportDetailAsync(SearchStoreReportDetailRequest request);
        Task<StoreReportListDetailDto> GetStoreReporDetail(SearchStoreReportDetailRequest request);
        Task<List<StoreReportDetailDto>> GetListStoreReportDetail(RequestDetail request);
    }
}
