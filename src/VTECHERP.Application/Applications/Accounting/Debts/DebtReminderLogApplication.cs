using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VTECHERP.Constants;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.DebtReminderLogs;
using VTECHERP.ServiceInterfaces;
using VTECHERP.Services;

namespace VTECHERP.Applications.Accounting.Debts
{
    [Route("api/app/DebtReminderLog/[action]")]
    [Authorize]
    public class DebtReminderLogApplication : ApplicationService
    {
        private readonly IDebtReminderLogService _debtReminderLogService;
        private readonly ILogger<DebtReminderLogApplication> _logger;

        public DebtReminderLogApplication(
            IDebtReminderLogService debtReminderLogService,
            ILogger<DebtReminderLogApplication> logger)
        {
            _debtReminderLogService = debtReminderLogService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<DebtReminderLogCreateRequest> AddAsync(DebtReminderLogCreateRequest request)
        {

            return await _debtReminderLogService.AddAsync(request);
        }

        [HttpPost]
        public async Task<PagingResponse<DebtReminderLogDto>> GetListAsync(SearchDebtReminderLogRequest request)
        {
            return await _debtReminderLogService.GetListAsync(request);
        }

        [HttpPost]
        public async Task<FileResult> ExportDebtReminderLogAsync(SearchDebtReminderLogRequest request)
        {
            try
            {
                _logger.LogWarning("[VTECH] Start Export");
                var file = await _debtReminderLogService.ExportDebtReminderLogAsync(request);
                _logger.LogWarning($"[VTECH] Get Export File: File Length - {file.Length}");
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"DanhSach_LichSuNhacNo_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
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