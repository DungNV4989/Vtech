using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VTECHERP.Constants;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.WarehousingBillLogs;
using VTECHERP.Services;

namespace VTECHERP
{
    [Route("api/app/warehousing-bill-logs/[action]")]
    [Authorize]
    public class WarehousingBillLogsAppication : ApplicationService
    {
        private readonly IWarehousingBillLogsService _warehousingBillLogsService;
        public WarehousingBillLogsAppication(
            IWarehousingBillLogsService warehousingBillLogsService
            )
        {
            _warehousingBillLogsService = warehousingBillLogsService;
        }
        [HttpPost]
        public async Task<PagingResponse<WarehousingBillLogsDTO>> Search(SearchWarehousingBillLogsRequest request)
        {
            return await _warehousingBillLogsService.SearchBillLogs(request);
        }

        [HttpGet]
        public async Task<WarehousingBillLogsDTO> Get(Guid id)
        {
            return await _warehousingBillLogsService.GetById(id);
        }

        [HttpPost]
        public async Task<FileResult> ExportWarehousingBillLogAsync(SearchWarehousingBillLogsRequest request)
        {
            try
            {
                var file = await _warehousingBillLogsService.ExportWarehousingBillLog(request);
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"DanhSach_Lịch sử sửa xóa phiếu XNK_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
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
