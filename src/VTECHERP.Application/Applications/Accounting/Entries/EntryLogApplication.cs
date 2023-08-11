using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VTECHERP.Constants;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.EntryLogs;
using VTECHERP.Services;

namespace VTECHERP
{
    [Route("api/app/EntryLog/[action]")]
    [Authorize]
    public class EntryLogApplication : ApplicationService
    {
        private readonly IEntryLogService _entryLogService;
        public EntryLogApplication(IEntryLogService entryLogService)
        {
            _entryLogService = entryLogService;
        }

        [HttpPost]
        public async Task<PagingResponse<EntryLogResponse>> Search(SearchEntryLogRequest request)
        {
            return await _entryLogService.SearchEntryLogAsync(request);
        }

        [HttpGet]
        //Task<DetailEntryLogResponse> DetailEntryLogAsync(Guid entryLogId)
        public async Task<DetailEntryLogResponse> DetailEntryLogAsync(Guid entryLogId)
        {
            return await _entryLogService.DetailEntryLogAsync(entryLogId);
        }

        [HttpPost]
        public async Task<FileResult> ExportEntryLogAsync(SearchEntryLogRequest request)
        {
            try
            {
                var file = await _entryLogService.ExportEntryLog(request);
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"DanhSach_Lịch sử bút toán_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
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