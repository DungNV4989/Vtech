using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VTECHERP.Constants;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.Entries;
using VTECHERP.Helper;
using VTECHERP.Services;

namespace VTECHERP
{
    [Route("api/app/entry/[action]")]
    [Authorize]
    public class EntryApplication : ApplicationService
    {
        private readonly IEntryService _entryService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EntryApplication> _logger;

        public EntryApplication(
            IEntryService entryService, 
            IHostingEnvironment hostingEnvironment, 
            IConfiguration configuration,
            ILogger<EntryApplication> logger)
        {
            _entryService = entryService;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public async Task<PagingResponse<EntryDTO>> Search(SearchEntryRequest request)
        {
            return await _entryService.SearchEntry(request);
        }

        [HttpGet]
        public async Task<EntryResponse> GetById(Guid id)
        {
            return await _entryService.GetEntryById(id);
        }

        [HttpDelete]
        public async Task Delete(Guid id)
        {
            await _entryService.Delete(id);
        }

        [HttpPost]
        public async Task<Guid> Create([FromBody] EntryCreateRequest request)
        {

            string apiUrl = _configuration.GetValue<string>("HostSetting:API");

            
            var fileModels = await FileUploadHelper.GetFilesFromForm(request.Attachments);
            var uploadFiles = await FileUploadHelper.UploadFile(fileModels, _hostingEnvironment.ContentRootPath, apiUrl);

            return await _entryService.ManualCreateEntry(request, uploadFiles);
        }

        [HttpPost]
        public async Task<bool> Update([FromBody] EntryUpdateRequest request)
        {

            string apiUrl = _configuration.GetValue<string>("HostSetting:API");

            var fileModels = await FileUploadHelper.GetFilesFromForm(request.Attachments);
            var uploadFiles = await FileUploadHelper.UploadFile(fileModels, _hostingEnvironment.ContentRootPath, apiUrl);

            await _entryService.ManualUpdateEntry(request, uploadFiles);
            return true;
        }

        [HttpPost]
        public async Task<bool> EditNoteEntry(EditNoteEntryRequest request)
        {
            await _entryService.ManualEditNoteEntry(request);
            return true;
        }

        [HttpPost]
        public async Task<PagingResponse<EntryDetailResponse>> EntryDetailAsync(SearchEntryDetailRequest request)
        {
            return await _entryService.EntryDetailAsync(request);
        }

        [HttpPost]
        public async Task<FileResult> ExportEntryAsync(SearchEntryRequest request)
        {
            try
            {
                _logger.LogWarning("[VTECH] Start Export");
                var file = await _entryService.ExportEntryAsync(request);
                _logger.LogWarning($"[VTECH] Get Export File: File Length - {file.Length}");
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"DanhSach_ButToan_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
                };
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                _logger.LogError($"[VTECH]{e.Message}");
                throw;
            }
        }
    }
}
