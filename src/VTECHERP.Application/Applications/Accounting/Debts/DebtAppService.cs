using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Timing;
using VTECHERP.Constants;
using VTECHERP.DebtReports;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.DebtReports;
using VTECHERP.DTOs.Suppliers;
using VTECHERP.Entities;
using VTECHERP.Enums;
using VTECHERP.Services;

namespace VTECHERP.Debts
{
    [Route("api/app/debt/[action]")]
    [Authorize]
    public class DebtAppService : VTECHERPAppService, IDebtAppService
    {
        private readonly IDebtService _debtService;
        private readonly IRepository<Debt> _debtRepository;
        private readonly IDebtReportRepository _debtReportRepository;
        private readonly IRepository<SaleOrders> _saleOrderRepository;
        private readonly IRepository<Entry> _entryRepository;
        private readonly IClock _clock;
        private readonly IRepository<EntryAccount> _entryAccountRepository;
        private readonly IRepository<Suppliers> _supplierRepository;
        private readonly ICommonService _commonService;

        private readonly ILogger<DebtAppService> _logger;
        public DebtAppService(
            IDebtService debtService,
            IRepository<Debt> debtRepository,
            IDebtReportRepository debtReportRepository,
            IRepository<Entry> entryRepository,
            IRepository<SaleOrders> saleOrderRepository,
            IRepository<EntryAccount> entryAccountRepository, 
            IClock clock, 
            ICommonService commonService, 
            IRepository<Suppliers> supplierRepository,
            ILogger<DebtAppService> logger)
        {
            _debtService = debtService;
            _debtRepository = debtRepository;
            _supplierRepository = supplierRepository;
            _saleOrderRepository = saleOrderRepository;
            _entryRepository = entryRepository;
            _entryAccountRepository = entryAccountRepository;
            _commonService = commonService;
            _clock = clock;
            _debtReportRepository = debtReportRepository;
            _logger = logger;

        }
        [HttpPost]
        public async Task<DebtDto> Create(CreateOrUpdateDebtDto input)
        {
            try
            {
                var debt = ObjectMapper.Map<CreateOrUpdateDebtDto, Debt>(input);
                await _debtRepository.InsertAsync(debt);
                await CurrentUnitOfWork.SaveChangesAsync();
                var debtDto = ObjectMapper.Map<Debt, DebtDto>(debt);
                return debtDto;
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
        }

        [HttpPost]
        public async Task<PagingResponse<DebtDto>> Search(SearchDebtRequest input)
        {

            
            try
            {
                if (input != null && input.SearchDateFrom != null)
                {
                    input.SearchDateFrom = _clock.Normalize(input.SearchDateFrom.GetValueOrDefault());
                }
                if (input != null && input.SearchDateTo != null)
                {
                    input.SearchDateTo = _clock.Normalize(input.SearchDateTo.GetValueOrDefault());
                }
                if ((input != null && input.SearchDateFrom != null) && input.SearchDateTo == null && input.SearchDateFrom.Value.Date <= DateTime.Now.Date)
                {
                    input.SearchDateTo = _clock.Normalize(DateTime.Now);
                }
                if ((input != null && input.SearchDateFrom != null) && input.SearchDateTo == null && input.SearchDateFrom.Value.Date > DateTime.Now.Date)
                {
                    return new PagingResponse<DebtDto>(0, new List<DebtDto>());
                }
                var result = await _debtService.GetListDebtAsync(input);
                if (result == null)
                {
                    return new PagingResponse<DebtDto>(0, new List<DebtDto>());
                }
                //var result = await _debtRepository.GetListDebtAsync_old(input);
                result = result.Where(x => x.BeginCredit != 0 || x.BeginDebt != 0 || x.Debt != 0 || x.Credit != 0).ToList();
                if (result == null)
                {
                    return new PagingResponse<DebtDto>();
                }
                if (input.DebtType != null && result != null)
                {
                    if (input.DebtType == DebtTypes.Credit)
                    {
                        result = result.Where(x => x.Credit != 0).ToList();
                    }
                    else if (input.DebtType == DebtTypes.Debt)
                    {
                        result = result.Where(x => x.Debt != 0).ToList();
                    }
                    else
                    {
                        result = result.Where(x => x.Debt != 0 && x.Credit != 0).ToList();
                    }
                }
                var data = result.Skip(input.Offset).Take(input.PageSize).ToList();
                foreach (var item in data)
                {
                    if (item.BeginDebt > item.BeginCredit)
                    {
                        item.BeginDebt = item.BeginDebt - item.BeginCredit;
                        item.BeginCredit = 0;
                    }
                    else
                    {
                        item.BeginCredit = item.BeginCredit - item.BeginDebt;
                        item.BeginDebt = 0;
                    }
                    item.EndDebt = (item.Debt + item.BeginDebt) - (item.BeginCredit + item.Credit);
                    if (item.EndDebt <= 0)
                    {
                        item.EndDebt = 0;
                    }
                    item.EndCredit = (item.BeginCredit + item.Credit) - (item.Debt + item.BeginDebt);
                    if (item.EndCredit <= 0)
                    {
                        item.EndCredit = 0;
                    }
                }
                return new PagingResponse<DebtDto>(result.Count(), data);
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }

        }

        [HttpGet]
        public async Task<PagingResponse<DebtDto>> SearchByMonth(DateTime input)
        {
            try
            {
                var result = await _debtService.GetListDebtByMonthAsync(input);
                if (result == null)
                {
                    return new PagingResponse<DebtDto>();
                }
                var data = result.ToList();
                return new PagingResponse<DebtDto>(result.Count(), data);
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
        }
        [HttpGet]
        public async Task<DebtDto> GetDetail(Guid id)
        {
            try
            {
                var result = await _debtRepository.FindAsync(p => p.Id == id);
                var debtDto = ObjectMapper.Map<Debt, DebtDto>(result);
                return debtDto;
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
        }
        [HttpPost]
        public async Task<PagingResponse<SupplierDetailDto>> GetListSuppliers(SupplierOrigin? type, string phoneNumber)
        {
            try
            {
                var result = await _supplierRepository.GetQueryableAsync();
                var data = result.WhereIf(type != null, x => x.Origin == type)
                    .WhereIf(!string.IsNullOrEmpty(phoneNumber), x => x.PhoneNumber.Contains(phoneNumber)).ToList();
                var debtDto = ObjectMapper.Map<List<Suppliers>, List<SupplierDetailDto>>(data);
                return new PagingResponse<SupplierDetailDto>(result.Count(), debtDto);
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
        }

        private async Task<List<DebtReportDto>> StatisticalDebt(DateTime input)
        {
            try
            {
                var lstDebt = await _debtService.GetAllByMonthAsync(input);
                var lstDebtReport = await _debtReportRepository.GetAllAsync(input);
                var lstDebtReportLastMonth = await _debtReportRepository.GetDebtReportLastMonthAsync(input);

                var lstSupplierIdLastMonth = lstDebtReportLastMonth.Select(x => x.Id).ToList();


                if (lstDebtReport.Any())
                {
                    await _debtReportRepository.DeleteManyAsync(lstDebtReport.Select(x => x.Id));
                }
                //var yesterday = input.AddDays(-1);
                var lstDebtBeginOfPeriod = lstDebt.Where(x => x.TransactionDate.Date.Month == input.Month && x.TransactionDate.Date.Year == input.Year);
                var lstStatic = lstDebtBeginOfPeriod.GroupBy(x => x.SupplierId);

                var lstSupplierIdThisMonth = lstDebtBeginOfPeriod.Select(x => x.Id).ToList();

                var lstSupplierId = new List<Guid>();
                lstSupplierId.AddRange(lstSupplierIdLastMonth);
                lstSupplierId.AddRange(lstSupplierIdThisMonth);

                lstSupplierId = lstSupplierId.Distinct().ToList();

                List<DebtReport> debtReports = new List<DebtReport>();
                foreach (var id in lstSupplierId)
                {
                    DebtReport debtReport = new DebtReport();

                    var sumReport = lstDebtReportLastMonth.Where(x => x.AudienceId == id).Sum(x => x.BeginOfPeriodDebt);
                    var sumCreditReport = lstDebtReportLastMonth.Where(x => x.AudienceId == id).Sum(x => x.BeginOfPeriodCredit);
                    var sumDebt = lstDebt.Where(x => (x.TicketType == TicketTypes.Import || x.TicketType == TicketTypes.CreditNote || x.TicketType == TicketTypes.Receipt) && x.SupplierId == id).Sum(x => x.Debt);
                    var sumCredit = lstDebt.Where(x => (x.TicketType == TicketTypes.PaymentVoucher || x.TicketType == TicketTypes.DebitNote) && x.SupplierId == id).Sum(x => x.Credit);

                    debtReport.BeginOfPeriodDebt = sumReport + sumDebt;
                    debtReport.BeginOfPeriodCredit = sumCreditReport + sumCredit;
                    debtReport.Month = input.AddMonths(1).Month;
                    debtReport.Year = input.AddMonths(1).Year;
                    debtReport.AudienceId = id;
                    debtReports.Add(debtReport);
                }


                //foreach (var item in lstStatic)
                //{
                //    DebtReport debtReport = new DebtReport();
                //    var beginOfPeriodDebt = item.Where(x => x.TicketType == Enums.TicketTypes.Import || x.TicketType == Enums.TicketTypes.CreditNote || x.TicketType == Enums.TicketTypes.Receipt).Sum(x => x.Debts);
                //    var beginOfPeriodCredit = item.Where(x => x.TicketType == Enums.TicketTypes.PaymentVoucher || x.TicketType == Enums.TicketTypes.DebitNote).Sum(x => x.Credits);
                //    debtReport.BeginOfPeriodDebt = beginOfPeriodDebt;
                //    debtReport.BeginOfPeriodCredit = beginOfPeriodCredit;
                //    debtReport.Month = input.AddMonths(1).Month;
                //    debtReport.Year = input.AddMonths(1).Year;
                //    debtReport.AudienceId = item.Key;
                //    debtReports.Add(debtReport);
                //}
                await _debtReportRepository.InsertManyAsync(debtReports);
                var lstDebtReportDto = ObjectMapper.Map<List<DebtReport>, List<DebtReportDto>>(debtReports);
                return lstDebtReportDto;
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
        }
        [HttpPost]
        public async Task<bool> StatisticalMonthDebt(DateTime input)
        {
            try
            {
                DateTime firstDayOfMonth = DateTime.Now.AddDays((-DateTime.Now.Day) + 1);
                while (input.Date <= firstDayOfMonth.Date)
                {
                    await StatisticalDebt(input);
                    input = input.AddMonths(1);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
        }

        [HttpPost]
        public async Task<PagingResponse<DebtDetailDto>> GetDebtDetail(SearchDebtDetailRequest request)
        {
            try
            {
                var result = new List<DebtDetailDto>();
                var debts = await _debtRepository.GetQueryableAsync();
                var debtBySupplierId = debts.Where(x => x.SupplierId == request.SupplierId);
                var entrys = (await _entryRepository.GetQueryableAsync())
                    .Where(x => debtBySupplierId.Select(x => x.EntryId).Any(id => id == x.Id))
                    .WhereIf(request.ParentId.HasValue, x => x.Id == request.ParentId)
                    .WhereIf(request.TicketType.HasValue, x => x.TicketType == request.TicketType)
                    .WhereIf(!request.ParentCode.IsNullOrWhiteSpace(), x => x.Code.ToLower().Contains(request.ParentCode.ToLower()))
                    .WhereIf(!request.DocumentCode.IsNullOrWhiteSpace(), x => x.DocumentCode.ToLower().Contains(request.DocumentCode.ToLower()))
                    .WhereIf(request.Start.HasValue, x => x.TransactionDate >= request.Start.Value)
                    .WhereIf(request.End.HasValue, x => x.TransactionDate <= request.End.Value)
                    .ToList();

                if (entrys.Any())
                {
                    var entryAccounts = (await _entryAccountRepository.GetQueryableAsync())
                        .Where(x => entrys.Select(x => x.Id).Any(id => id == x.EntryId))
                        .WhereIf(!request.Code.IsNullOrWhiteSpace(), x => x.Code.ToLower().Contains(request.Code.ToLower()));

                    if (entryAccounts == null)
                        return new PagingResponse<DebtDetailDto>(0, result);

                    var audienceRequests = entrys.Where(p => p.AudienceId != null).Select(p =>
                    {
                        (AudienceTypes, Guid?) item = (p.AudienceType, p.AudienceId);
                        return item;
                    }).Distinct().ToArray();

                    var audiences = await _commonService.GetAudiences(audienceRequests);

                    foreach (var item in entryAccounts)
                    {
                        var isDebt = false;
                        var entryDetailResponse = new DebtDetailDto();
                        var entry = entrys.FirstOrDefault(x => x.Id == item.EntryId);
                        var audience = audiences.FirstOrDefault(x => x.Id == entry.AudienceId);

                        entryDetailResponse.ParentId = entry.Id;
                        entryDetailResponse.ParentCode = entry.Code;
                        entryDetailResponse.Id = item.Id;
                        entryDetailResponse.Code = item.Code;
                        entryDetailResponse.Date = entry.TransactionDate;

                        if (audience != null)
                        {
                            entryDetailResponse.AudienceName = audience.Name;
                            entryDetailResponse.AudienceCode = audience.Code;
                            entryDetailResponse.AudiencePhone = audience.Phone;
                        }
                        switch (entry.TicketType)
                        {
                            case TicketTypes.Import:
                            case TicketTypes.Receipt:
                            case TicketTypes.CreditNote:
                                isDebt = false;
                                break;
                            case TicketTypes.Export:
                            case TicketTypes.PaymentVoucher:
                            case TicketTypes.DebitNote:
                                isDebt = true;
                                break;
                        }
                        entryDetailResponse.DebtAccountCode = item.DebtAccountCode;
                        entryDetailResponse.CreditAccountCode = item.CreditAccountCode;
                        entryDetailResponse.AmountVnd = item.AmountVnd;
                        entryDetailResponse.AmountCny = item.AmountCny;
                        entryDetailResponse.TicketType = entry.TicketType;
                        entryDetailResponse.DocumentId = entry.DocumentId;
                        entryDetailResponse.DocumentCode = entry.DocumentCode;
                        entryDetailResponse.DocumentType = entry.DocumentType;
                        entryDetailResponse.DocumentDetailType = entry.DocumentDetailType;
                        entryDetailResponse.DebtAmount = isDebt ? (entry.AudienceType == AudienceTypes.SupplierCN ? entryDetailResponse.AmountCny : entryDetailResponse.AmountVnd) : 0;
                        entryDetailResponse.CreditAmount = isDebt ? 0 : (entry.AudienceType == AudienceTypes.SupplierCN ? entryDetailResponse.AmountCny : entryDetailResponse.AmountVnd);
                        entryDetailResponse.Note = item.Note;

                        result.Add(entryDetailResponse);
                    }
                    if (!string.IsNullOrEmpty(request.Code))
                    {
                        result = result.Where(x => x.Code.Contains(request.Code)).ToList();
                    }
                    result = result.Where(x => x.AmountCny != null && (x.AmountCny.HasValue  && x.AmountCny.Value > 0)).ToList();
                    return new PagingResponse<DebtDetailDto>(result.Count(), result
                        .OrderByDescending(x => x.ParentCode)
                        .Skip(request.Offset)
                        .Take(request.PageSize)
                        .ToList());
                }

                return new PagingResponse<DebtDetailDto>(0, result);
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
        }

        [HttpPost]
        public async Task<FileResult> ExportDebt(SearchDebtRequest input)
        {
            if (input != null && input.SearchDateFrom != null)
            {
                input.SearchDateFrom = _clock.Normalize(input.SearchDateFrom.GetValueOrDefault());
            }
            if (input != null && input.SearchDateTo != null)
            {
                input.SearchDateTo = _clock.Normalize(input.SearchDateTo.GetValueOrDefault());
            }
            var result = await _debtService.GetListDebtAsync(input);
            result = result.Where(x => x.BeginCredit != 0 || x.BeginDebt != 0 || x.Debt != 0 || x.Credit != 0).ToList();
            if (result == null)
            {
                return null;
            }
            if (input.DebtType != null && result != null)
            {
                if (input.DebtType == DebtTypes.Credit)
                {
                    result = result.Where(x => x.Credit != 0).ToList();
                }
                else if (input.DebtType == DebtTypes.Debt)
                {
                    result = result.Where(x => x.Debt != 0).ToList();
                }
                else
                {
                    result = result.Where(x => x.Debt != 0 && x.Credit != 0).ToList();
                }
            }
            var data = result.ToList();
            foreach (var item in data)
            {
                if (item.BeginDebt > item.BeginCredit)
                {
                    item.BeginDebt = item.BeginDebt - item.BeginCredit;
                    item.BeginCredit = 0;
                }
                else
                {
                    item.BeginCredit = item.BeginCredit - item.BeginDebt;
                    item.BeginDebt = 0;
                }
                item.EndDebt = (item.Debt + item.BeginDebt) - (item.BeginCredit + item.Credit);
                if (item.EndDebt <= 0)
                {
                    item.EndDebt = 0;
                }
                item.EndCredit = (item.BeginCredit + item.Credit) - (item.Debt + item.BeginDebt);
                if (item.EndCredit <= 0)
                {
                    item.EndCredit = 0;
                }
            }
            
            var exportData = from da in data
                             select new ExportDebtDto
                             {
                                 SupplierCode = da.SupplierCode,
                                 SupplierName = da.SupplierName,
                                 PhoneNumber = da.PhoneNumber,
                                 BeginDebt = da.SupplierType == SupplierOrigin.CN ? Math.Round(da.BeginDebt, 0).ToString("N1", CultureInfo.InvariantCulture) + " NDT" :  Math.Round(da.BeginDebt, 0).ToString("N1", CultureInfo.InvariantCulture) + " VND",
                                 BeginCredit = da.SupplierType == SupplierOrigin.CN ?  Math.Round(da.BeginCredit, 0).ToString("N1", CultureInfo.InvariantCulture) + " NDT" : Math.Round(da.BeginCredit, 0).ToString("N1", CultureInfo.InvariantCulture) + " VND",
                                 Debt = da.SupplierType == SupplierOrigin.CN ? Math.Round(da.Debt, 0).ToString("N1", CultureInfo.InvariantCulture) + " NDT" : Math.Round(da.Debt, 0).ToString("N1", CultureInfo.InvariantCulture) + " VND",
                                 Credit = da.SupplierType == SupplierOrigin.CN ?  Math.Round(da.Credit, 0).ToString("N1", CultureInfo.InvariantCulture) + " NDT" : Math.Round(da.Credit, 0).ToString("N1", CultureInfo.InvariantCulture) + " VND",
                                 EndCredit = da.SupplierType == SupplierOrigin.CN ? Math.Round(da.EndCredit, 0).ToString("N1", CultureInfo.InvariantCulture) + " NDT" : Math.Round(da.EndCredit, 0).ToString("N1", CultureInfo.InvariantCulture) + " VND",
                                 EndDebt = da.SupplierType == SupplierOrigin.CN ? Math.Round(da.EndDebt, 0).ToString("N1", CultureInfo.InvariantCulture) + " NDT" :  Math.Round(da.EndDebt, 0).ToString("N1", CultureInfo.InvariantCulture) + " VND"
                             };
            var exportResult  = exportData.ToList();
            var file = ExcelHelper.ExportExcel(exportResult);
            try
            {
                _logger.LogWarning("[VTECH] Start Export");
                _logger.LogWarning($"[VTECH] Get Export File: File Length - {file.Length}");
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"DanhSach_CongNoNhaCungCap_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
                };
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                _logger.LogError($"[VTECH]{e.Message}");
                throw;
            }
        }

        [HttpPost]
        public async Task<byte[]> ExportDebtDetail(SearchDebtDetailRequest request)
        {
            var data = await GetDebtDetail(request);
            if (data != null && data.Data.Count() > 0 )
            {
                var result = data.Data.ToList();
                var exportData = ObjectMapper.Map<List<DebtDetailDto>, List<ExportDebtDetailDto>>(result);
                return ExcelHelper.ExportExcel(exportData);
            }
            else
            {
                return ExcelHelper.ExportExcel(null);
            }
            
        }
    }
}
