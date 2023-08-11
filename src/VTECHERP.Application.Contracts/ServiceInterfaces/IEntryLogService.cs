using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.EntryLogs;

namespace VTECHERP.Services
{
    public interface IEntryLogService : IScopedDependency
    {
        Task<PagingResponse<EntryLogResponse>> SearchEntryLogAsync(SearchEntryLogRequest request);
        Task<DetailEntryLogResponse> DetailEntryLogAsync(Guid entryLogId);
        Task<byte[]> ExportEntryLog(SearchEntryLogRequest request);
    }
}