using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using VTECHERP.ServiceInterfaces;
using System.Threading.Tasks;
using VTECHERP.DTOs.Base;
using Microsoft.Extensions.Logging;
using VTECHERP.Reports;
using VTECHERP.Helper;
using System;
using Volo.Abp;
using VTECHERP.Constants;
using System.Collections.Generic;
using Vinpearl.Modelling.Library.Utility.Excel;
using VTECHERP.DTOs.Stores;
using System.Data;
using Microsoft.Data.SqlClient;
using Volo.Abp.Domain.Repositories;
using VTECHERP.Entities;
using Microsoft.EntityFrameworkCore;

namespace VTECHERP.Applications.Reports
{
    [Route("api/app/storeReport/[action]")]
    [Authorize]
    public class StoreReportAppService : ApplicationService
    {
        private readonly IStoreReportService _storeReportService;
        private readonly IRepository<Entry> _entryRepository;
        private readonly ILogger<StoreReportAppService> _logger;

        public StoreReportAppService(IStoreReportService storeReportService, ILogger<StoreReportAppService> logger)
        {
            _storeReportService = storeReportService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Search(SearchRequest request)
        {
            try
            {
                var reports = await _storeReportService.SearchStoreReportAsync(request);
                return  reports;
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
           
        }

        [HttpPost]
        public async Task<StoreReportListDetailDto> SearchStoreReportDetail(SearchStoreReportDetailRequest request)
        {
            try
            {
                var reports = await _storeReportService.GetStoreReporDetail(request);
                return reports;
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
        }

        //[HttpGet]
        //public async Task<List<StoreReportDetailDto>> GetListStoreReportDetail(RequestDetail request)
        //{
        //    try
        //    {
        //        var data = await _storeReportService.GetListStoreReportDetail(request);
        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
        //    }
           
        //}

        [HttpPost]
        public async Task<byte[]> ExportStoreReport(SearchRequest input)
        {
            try
            {
                var data = await _storeReportService.ExportStoreReportAsync(input);
                if (data != null && data.Count > 0)
                {
                    var exportData = ObjectMapper.Map<List<StoreReportDto>, List<ExportStoreDto>>(data);
                    return ExcelHelper.ExportExcel(exportData);
                }
                else
                {
                    return ExcelHelper.ExportExcel(new List<ExportStoreDto>());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
        }

        [HttpPost]
        public async Task<FileResult> ExportStoreReportDetail(SearchStoreReportDetailRequest input)
        {
           
            try
            {
                var file = await _storeReportService.ExportStoreReportDetailAsync(input);
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
