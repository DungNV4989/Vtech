using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VTECHERP.Entities;
using VTECHERP.Reports;
using VTECHERP.ServiceInterfaces;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VTECHERP.Helper;
using Vinpearl.Modelling.Library.Utility.Excel;
using VTECHERP.DTOs.Stores;
using VTECHERP.Enums;
using VTECHERP.DTOs.Base;
using Volo.Abp;
using VTECHERP.Constants;
using Volo.Abp.Uow;

namespace VTECHERP.Services.ReportService
{
    public class StoreReportService : IStoreReportService
    {
        private readonly IRepository<Entry> _entryRepository;
        private readonly IRepository<EntryAccount> _entryAccountRepository;
        private readonly IRepository<PaymentReceipt> _paymentReceipttRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICommonService _commonService;
        public StoreReportService(IRepository<Entry> entryRepository, IRepository<EntryAccount> entryAccountRepository, IRepository<PaymentReceipt> paymentReceipttRepository, IUnitOfWorkManager unitOfWorkManager, ICommonService commonService
            )
        {
            _entryRepository = entryRepository;
            _entryAccountRepository = entryAccountRepository;
            _paymentReceipttRepository = paymentReceipttRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _commonService = commonService;
        }

        public async Task<List<StoreReportDetailDto>> GetListStoreReportDetail(RequestDetail request)
        {
            var result = new List<StoreReportDetailDto>();
            var storeReports = await _paymentReceipttRepository.GetQueryableAsync();
            var data = storeReports
                .Where(x => x.AccountCode.ToLower() == request.Code.ToLower())
                .WhereIf(request.Type != null, x => request.Type == TicketTypesSearch.PaymentVoucher ? (x.TicketType == TicketTypes.PaymentVoucher || x.TicketType == TicketTypes.DebitNote) : (x.TicketType == TicketTypes.Receipt || x.TicketType == TicketTypes.CreditNote))
                .ToList();

            if (data.Any())
            {
                var allEntrys = await _entryRepository.GetQueryableAsync();
                var lstDataIds = data.Select(x => x.Id).ToList();
                var entrys = allEntrys.Where(x => lstDataIds.Contains(x.SourceId.Value)).ToList();

                var entryAccounts = (await _entryAccountRepository.GetQueryableAsync())
                    .Where(x => entrys.Select(x => x.Id).Any(id => id == x.EntryId));

                if (entryAccounts == null)
                    return null;

                var audienceRequests = data.Where(p => p.AudienceId != null).Select(p =>
                {
                    (AudienceTypes, Guid?) item = (p.AudienceType, p.AudienceId);
                    return item;
                }).Distinct().ToArray();

                var audiences = await _commonService.GetAudiences(audienceRequests);

                foreach (var item in entryAccounts)
                {
                    var isDebt = false;
                    var entryDetailResponse = new StoreReportDetailDto();
                    var entry = entrys.FirstOrDefault(x => x.Id == item.EntryId);
                    var audience = audiences.FirstOrDefault(x => x.Id == entry.AudienceId);

                    entryDetailResponse.ParentId = entry.Id;
                    entryDetailResponse.ParentCode = entry.Code;
                    entryDetailResponse.Id = item.Id;
                    entryDetailResponse.Code = item.Code;
                    entryDetailResponse.Date = entry.TransactionDate;


                    switch (entry.TicketType)
                    {
                        case TicketTypes.Receipt:
                        case TicketTypes.CreditNote:
                            isDebt = false;
                            break;
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
                    entryDetailResponse.DocumentCode = entry.DocumentCode;
                    entryDetailResponse.DocumentType = entry.DocumentType;
                    entryDetailResponse.DocumentDetailType = entry.DocumentDetailType;
                    entryDetailResponse.DebtAmount = isDebt ? (entry.AudienceType == AudienceTypes.SupplierCN ? entryDetailResponse.AmountCny : entryDetailResponse.AmountVnd) : 0;
                    entryDetailResponse.CreditAmount = isDebt ? 0 : (entry.AudienceType == AudienceTypes.SupplierCN ? entryDetailResponse.AmountCny : entryDetailResponse.AmountVnd);
                    entryDetailResponse.Note = item.Note;
                    entryDetailResponse.TenantId = entry.TenantId;
                    entryDetailResponse.StoreId = entry.StoreId;
                    entryDetailResponse.AudienceType = entry.AudienceType;
                    if (audience != null)
                    {
                        entryDetailResponse.AudienceName = audience.Name;
                        entryDetailResponse.AudienceCode = audience.Code;
                        entryDetailResponse.AudiencePhone = audience.Phone;
                    }

                    result.Add(entryDetailResponse);
                }
                if (!string.IsNullOrEmpty(request.Code))
                {
                    result = result.Where(x => (x.DebtAccountCode.Contains(request.Code) || (x.CreditAccountCode.Contains(request.Code)))).ToList();
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        public async Task<StoreReportListDetailDto> GetStoreReporDetail(SearchStoreReportDetailRequest request)
        {
            if (request == null)
            {
                return null;
            }
            var result = new List<StoreReportDetailDto>();
            var paymentReceipts = await _paymentReceipttRepository.GetQueryableAsync();
            var data = paymentReceipts
                .Where(x => (!string.IsNullOrEmpty(request.AccountCode) && x.AccountCode.ToLower() == request.AccountCode.ToLower()) 
                && ( request.Type.HasValue ? (request.Type == TicketTypesSearch.PaymentVoucher ? (x.TicketType == TicketTypes.PaymentVoucher || x.TicketType == TicketTypes.DebitNote) : request.Type == TicketTypesSearch.Receipt ? (x.TicketType == TicketTypes.Receipt || x.TicketType == TicketTypes.CreditNote) 
                : (x.TicketType == TicketTypes.CreditNote || x.TicketType == TicketTypes.DebitNote || x.TicketType == TicketTypes.PaymentVoucher || x.TicketType == TicketTypes.Receipt))
                : (x.TicketType == TicketTypes.CreditNote || x.TicketType == TicketTypes.DebitNote || x.TicketType == TicketTypes.PaymentVoucher || x.TicketType == TicketTypes.Receipt)) )
                .ToList();
            var b = paymentReceipts.Where(x =>x.AccountCode.Contains(request.AccountCode)).ToList();
            var c = paymentReceipts.Select(x => x.AccountCode).ToList();
            if (data.Any())
            {
                var allEntrys = await _entryRepository.GetQueryableAsync();
                var lstDataIds = data.Select(x => x.Id).ToList();
                var entrys = allEntrys.Where(x => lstDataIds.Contains(x.SourceId.Value)).ToList();

                var entryAccounts = (await _entryAccountRepository.GetQueryableAsync())
                    .Where(x => entrys.Select(x => x.Id).Any(id => id == x.EntryId));

                if (entryAccounts == null)
                    return null;

                var audienceRequests = data.Where(p => p.AudienceId != null).Select(p =>
                {
                    (AudienceTypes, Guid?) item = (p.AudienceType, p.AudienceId);
                    return item;
                }).Distinct().ToArray();

                var audiences = await _commonService.GetAudiences(audienceRequests);

                foreach (var item in entryAccounts)
                {
                    var audienceTypeName = "";
                    var isDebt = false;
                    var entryDetailResponse = new StoreReportDetailDto();
                    var entry = entrys.FirstOrDefault(x => x.Id == item.EntryId);
                    var audience = audiences.FirstOrDefault(x => x.Id == entry.AudienceId);
                    entryDetailResponse.ParentId = entry.Id;
                    entryDetailResponse.ParentCode = entry.Code;
                    entryDetailResponse.Id = item.Id;
                    entryDetailResponse.Code = item.Code;
                    entryDetailResponse.Date = entry.TransactionDate;


                    switch (entry.TicketType)
                    {
                        case TicketTypes.Receipt:
                        case TicketTypes.CreditNote:
                            isDebt = false;
                            break;
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
                    entryDetailResponse.DocumentCode = entry.DocumentCode;
                    entryDetailResponse.DocumentType = entry.DocumentType;
                    entryDetailResponse.DocumentDetailType = entry.DocumentDetailType;
                    entryDetailResponse.DebtAmount = isDebt ? (entry.AudienceType == AudienceTypes.SupplierCN ? entryDetailResponse.AmountCny : entryDetailResponse.AmountVnd) : 0;
                    entryDetailResponse.CreditAmount = isDebt ? 0 : (entry.AudienceType == AudienceTypes.SupplierCN ? entryDetailResponse.AmountCny : entryDetailResponse.AmountVnd);
                    entryDetailResponse.Note = item.Note;
                    entryDetailResponse.TenantId = entry.TenantId;
                    entryDetailResponse.StoreId = entry.StoreId;
                    entryDetailResponse.AudienceType = entry.AudienceType;

                    switch (entry.AudienceType)
                    {
                        case AudienceTypes.SupplierCN:
                            audienceTypeName =  " NCC TQ";
                            break;
                        case AudienceTypes.SupplierVN:
                            audienceTypeName = "NCC VN ";
                            break;
                        case AudienceTypes.Other:
                            audienceTypeName = " Khác  ";
                            break;
                        case AudienceTypes.Customer:
                            audienceTypeName = "Khách hành ";
                            break;
                        case AudienceTypes.Employee:
                            audienceTypeName = "Nhân viên ";
                            break;
                    }
                    if (audience != null)
                    {
                        entryDetailResponse.AudienceName = audience.Name;
                        entryDetailResponse.AudienceCode = audience.Code;
                        entryDetailResponse.AudiencePhone = audience.Phone;
                        entryDetailResponse.Audience = audienceTypeName + " - " +  audience.Code + " - " + audience.Name + " - " + audience.Phone;
                    }
                    result.Add(entryDetailResponse);
                }
                if (!string.IsNullOrEmpty(request.Code))
                {
                    result = result.Where(x => (x.DebtAccountCode.Contains(request.AccountCode) || (x.CreditAccountCode.Contains(request.AccountCode)))).ToList();
                }
            }
            var listStoreReportDetailForSearch = result.WhereIf(request.TenantId != null && request.TenantId.Count > 0, x => request.TenantId.Contains(x.TenantId.Value))
                .WhereIf(request.StoreId != null && request.StoreId.Count > 0, x => request.StoreId.Contains(x.StoreId.Value))
                .WhereIf(!string.IsNullOrEmpty(request.ParentCode), x => x.ParentCode.Contains(request.ParentCode))
                .WhereIf(!string.IsNullOrEmpty(request.Code), x => x.Code.Contains(request.Code))
                .WhereIf(request.TicketType != null, x => x.TicketType == request.TicketType)
                .WhereIf(!string.IsNullOrEmpty(request.DocumentCode), x => x.DocumentCode.Contains(request.DocumentCode))
                .WhereIf(request.Start != null, x => x.Date >= request.Start)
                .WhereIf(request.End != null, x => x.Date <= request.End)
                .WhereIf(request.AudienceType != null, x => x.AudienceType == request.AudienceType)
                .WhereIf(!string.IsNullOrEmpty(request.Audience), x => (!string.IsNullOrEmpty(x.AudienceName) ? x.AudienceName.ToLower().Contains(request.Audience, StringComparison.OrdinalIgnoreCase) : x.AudienceName.ToLower().Contains(""))
                || (!string.IsNullOrEmpty(x.AudiencePhone) ? x.AudiencePhone.ToLower().Contains(request.Audience, StringComparison.OrdinalIgnoreCase) : x.AudiencePhone.ToLower().Contains(""))
                || (!string.IsNullOrEmpty(x.AudienceCode) ? x.AudienceCode.ToLower().Contains(request.Audience, StringComparison.OrdinalIgnoreCase) : x.AudienceCode.ToLower().Contains("")))
                .ToList();
            var responseDto = new StoreReportListDetailDto();
            responseDto.storeReportDetailDto = listStoreReportDetailForSearch;
            if (listStoreReportDetailForSearch.Any())
            {
                responseDto.TotalCredit = listStoreReportDetailForSearch.Sum(x => x.CreditAmount.HasValue ? x.CreditAmount.Value : 0);
                responseDto.TotalDebt = listStoreReportDetailForSearch.Sum(x => x.DebtAmount.HasValue ? x.DebtAmount.Value : 0);
            }
            return responseDto;
        }

        public async Task<IActionResult> SearchStoreReportAsync(SearchRequest input, CancellationToken cancellationToken = default)
        {
            try
            {
                var retVal = new List<StoreReportDto>();
                var dataTableStoreId = new DataTable();
                dataTableStoreId.Columns.Add("StoreId", typeof(Guid));
                if (input.LstStoreId != null && input.LstStoreId.Count > 0)
                {
                    foreach (var guidValue in input.LstStoreId)
                    {
                        dataTableStoreId.Rows.Add(guidValue);
                    }
                }

                var dataTableEnterpriseId = new DataTable();
                dataTableEnterpriseId.Columns.Add("EnterpriseId", typeof(Guid));
                if (input.LstEnterpriseId != null && input.LstEnterpriseId.Count > 0)
                {
                    foreach (var guidValue in input.LstEnterpriseId)
                    {
                        dataTableEnterpriseId.Rows.Add(guidValue);
                    }
                }
                if (input.DateFrom == null || input.DateTo == null)
                {
                    return new GenericActionResult(200, true, "No Data", null);
                }

                using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false))
                {
                    var query = $"EXEC sp_store_report @DateFrom, @DateTo, @EnterpriseIds, @StoreIds";
                    var dateFrm = new SqlParameter("@DateFrom", SqlDbType.DateTime);
                    var dateTo = new SqlParameter("@DateTo", SqlDbType.DateTime);
                    var epIds = new SqlParameter("@EnterpriseIds", SqlDbType.Structured);
                    var sIds = new SqlParameter("@StoreIds", SqlDbType.Structured);

                    dateFrm.Value = new DateTime(input.DateFrom.Value.Year, input.DateFrom.Value.Month, input.DateFrom.Value.Day, 0, 0, 0);
                    dateTo.Value = new DateTime(input.DateTo.Value.Year, input.DateTo.Value.Month, input.DateTo.Value.Day, 23, 59, 59);

                    epIds.TypeName = "dbo.GuidListEnterpriseId";
                    epIds.Value = dataTableEnterpriseId;
                    sIds.TypeName = "dbo.GuidListStoreId";
                    sIds.Value = dataTableStoreId;

                    //var data = await _entryRepository.GetDbContext().Database.ExecuteSqlRawAsync(query, listParam, cancellationToken);
                    var connection = _entryRepository.GetDbContext().Database.GetDbConnection();
                    connection.Close();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = "sp_store_report";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(dateFrm);
                    cmd.Parameters.Add(dateTo);
                    cmd.Parameters.Add(epIds);
                    cmd.Parameters.Add(sIds);
                    connection.Open();

                    var reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var val = new StoreReportDto()
                            {
                                amountBeginPay = Convert.ToDecimal(reader["amountBeginPay"].ToString()),
                                amountBeginReceive = Convert.ToDecimal(reader["amountBeginReceive"].ToString()),
                                totalReceive = Convert.ToDecimal(reader["totalReceive"].ToString()),
                                totalReceiveCNY = Convert.ToDecimal(reader["totalReceiveCNY"].ToString()),
                                totalPay = Convert.ToDecimal(reader["totalPay"].ToString()),
                                totalPayCNY = decimal.Parse(reader["totalPayCNY"].ToString()),
                                AccountCode = reader["AccountCode"].ToString(),
                                AccountName = reader["AccountName"].ToString(),
                                StoreId = reader["StoreId"] == null ? null : Guid.Parse(reader["StoreId"].ToString()),
                                TenantId = reader["TenantId"] == null ? null : Guid.Parse(reader["TenantId"].ToString()),
                                Lvl = reader["Lvl"] == null ? null : Int32.Parse(reader["Lvl"].ToString()),
                                EnterpriseName = reader["EnterpriseName"].ToString(),
                                StoreName = reader["StoreName"].ToString()
                            };
                            retVal.Add(val);
                        }
                    }
                    connection.Close();
                }

                if (retVal.Count > 0)
                {
                    foreach (var val in retVal)
                    {
                        val.amountBegin = val.amountBeginReceive - val.amountBeginPay;
                        val.totalEnd = val.amountBegin == null ? 0 : val.amountBegin.Value + val.totalReceive - val.totalPay;
                    }

                    var result = retVal.GroupBy(x => x.StoreId);
                    List<StoreReportGroupDto> storeReportGroupDto = result.Select(x => new StoreReportGroupDto { StoreId = x.Key,
                        StoreName = x.FirstOrDefault()?.StoreName,
                        EnterpriseName = x.FirstOrDefault()?.EnterpriseName,
                        TenantId = x.FirstOrDefault()?.TenantId,
                    StoreReportDto = x.ToList()}).ToList();
                    return new GenericActionResult(storeReportGroupDto.Count(), storeReportGroupDto);
                }
                else
                {
                    return new GenericActionResult(200, true, "No Data", null);
                }

            }
            catch (Exception ex)
            {
                var errM = ex.Message;
                return new GenericActionResult(400, false, "Lỗi xảy ra", null);
            }

        }

        public async Task<List<StoreReportDto>> ExportStoreReportAsync(SearchRequest input, CancellationToken cancellationToken = default)
        {
            var retVal = new List<StoreReportDto>();
            var dataTableStoreId = new DataTable();
            dataTableStoreId.Columns.Add("StoreId", typeof(Guid));
            if (input.LstStoreId != null && input.LstStoreId.Count > 0)
            {
                foreach (var guidValue in input.LstStoreId)
                {
                    dataTableStoreId.Rows.Add(guidValue);
                }
            }

            var dataTableEnterpriseId = new DataTable();
            dataTableEnterpriseId.Columns.Add("EnterpriseId", typeof(Guid));
            if (input.LstEnterpriseId != null && input.LstEnterpriseId.Count > 0)
            {
                foreach (var guidValue in input.LstEnterpriseId)
                {
                    dataTableEnterpriseId.Rows.Add(guidValue);
                }
            }

            using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false))
            {
                var query = $"EXEC sp_store_report @DateFrom, @DateTo, @EnterpriseIds, @StoreIds";
                var dateFrm = new SqlParameter("@DateFrom", SqlDbType.DateTime);
                var dateTo = new SqlParameter("@DateTo", SqlDbType.DateTime);
                var epIds = new SqlParameter("@EnterpriseIds", SqlDbType.Structured);
                var sIds = new SqlParameter("@StoreIds", SqlDbType.Structured);

                dateFrm.Value = new DateTime(input.DateFrom.Value.Year, input.DateFrom.Value.Month, input.DateFrom.Value.Day, 0, 0, 0);
                dateTo.Value = new DateTime(input.DateTo.Value.Year, input.DateTo.Value.Month, input.DateTo.Value.Day, 23, 59, 59);

                epIds.TypeName = "dbo.GuidListEnterpriseId";
                epIds.Value = dataTableEnterpriseId;
                sIds.TypeName = "dbo.GuidListStoreId";
                sIds.Value = dataTableStoreId;

                //var data = await _entryRepository.GetDbContext().Database.ExecuteSqlRawAsync(query, listParam, cancellationToken);
                var connection = _entryRepository.GetDbContext().Database.GetDbConnection();
                connection.Close();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "sp_store_report";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(dateFrm);
                cmd.Parameters.Add(dateTo);
                cmd.Parameters.Add(epIds);
                cmd.Parameters.Add(sIds);
                connection.Open();

                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var val = new StoreReportDto()
                        {
                            amountBeginPay = Convert.ToDecimal(reader["amountBeginPay"].ToString()),
                            amountBeginReceive = Convert.ToDecimal(reader["amountBeginReceive"].ToString()),
                            totalReceive = Convert.ToDecimal(reader["totalReceive"].ToString()),
                            totalReceiveCNY = Convert.ToDecimal(reader["totalReceiveCNY"].ToString()),
                            totalPay = Convert.ToDecimal(reader["totalPay"].ToString()),
                            totalPayCNY = decimal.Parse(reader["totalPayCNY"].ToString()),
                            AccountCode = reader["AccountCode"].ToString(),
                            AccountName = reader["AccountName"].ToString(),
                            StoreId = reader["StoreId"] == null ? null : Guid.Parse(reader["StoreId"].ToString()),
                            TenantId = reader["TenantId"] == null ? null : Guid.Parse(reader["TenantId"].ToString()),
                            Lvl = reader["Lvl"] == null ? null : Int32.Parse(reader["Lvl"].ToString()),
                            EnterpriseName = reader["EnterpriseName"].ToString(),
                            StoreName = reader["StoreName"].ToString()
                        };
                        retVal.Add(val);
                    }
                }
                connection.Close();
            }

            if (retVal.Count > 0)
            {
                foreach (var val in retVal)
                {
                    val.amountBegin = val.amountBeginReceive - val.amountBeginPay;
                    val.totalEnd = val.amountBegin == null ? 0 : val.amountBegin.Value + val.totalReceive - val.totalPay;
                }
                return retVal;
            }
            else
            {
                return null;
            }
        }

        public async Task<byte[]> ExportStoreReportDetailAsync(SearchStoreReportDetailRequest input)
        {
            var reports = await GetStoreReporDetail(input);
            var exportData = new List<ExportStoreDetailDto>();
            if (reports != null && reports.storeReportDetailDto != null && reports.storeReportDetailDto.Any())
            {
                foreach (var item in reports.storeReportDetailDto)
                {
                    exportData.Add(new ExportStoreDetailDto
                    {
                        Code = item?.Code,
                        ParentCode = item?.ParentCode,
                        CreationTime = item?.Date.Value.ToString("dd-MM-yyyy"),
                        AudienceName = item?.Audience,
                        DebtAccountCode = item?.DebtAccountCode,
                        CreditAccountCode = item?.CreditAccountCode,
                        DebtAmount = item?.DebtAmount,
                        CreditAmount = item?.CreditAmount,
                        DocumentName = item.DocumentCode,
                        Note = item?.Note
                    });
                }
            }
            return ExcelHelper.ExportExcel(exportData);
        }
    }
}
