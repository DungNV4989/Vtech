using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.EntryLogs;
using VTECHERP.Entities;
using VTECHERP.Enums;

namespace VTECHERP.Services
{
    public class EntryLogService : IEntryLogService
    {
        private readonly IRepository<EntryLog> _entryLogRepository;
        private readonly IRepository<Entry> _entryRepository;
        private readonly IRepository<EntryAccount> _entryAccountRepository;
        private readonly IIdentityUserRepository _userRepository;

        private readonly IEntryService _entryService;
        private readonly ICommonService _commonService;

        private readonly IDataFilter _dataFilter;

        public EntryLogService(
            IRepository<EntryLog> entryLogRepository,
            IRepository<Entry> entryRepository,
            IRepository<EntryAccount> entryAccountRepository,
            IIdentityUserRepository userRepository,

            IEntryService entryService,
            ICommonService commonService,

            IDataFilter dataFilter)
        {
            _entryLogRepository = entryLogRepository;
            _entryRepository = entryRepository;
            _entryAccountRepository = entryAccountRepository;
            _userRepository = userRepository;

            _entryService = entryService;
            _commonService = commonService;

            _dataFilter = dataFilter;
        }

        public async Task<PagingResponse<EntryLogResponse>> SearchEntryLogAsync(SearchEntryLogRequest request)
        {
            using (_dataFilter.Disable<ISoftDelete>())
            {
                var result = new List<EntryLogResponse>();

                var entryLogs = await _entryLogRepository.GetQueryableAsync();

                var entryIds = entryLogs.AsEnumerable().DistinctBy(x => x.EntryId).Select(x => x.EntryId);
                if (!entryIds.Any())
                    return new PagingResponse<EntryLogResponse>(0, result);

                var entrys = (await _entryRepository.GetQueryableAsync()).Where(x => entryIds.Any(entryId => entryId == x.Id));
                if (!entrys.Any())
                    return new PagingResponse<EntryLogResponse>(0, result);

                var entryQueries = entrys
                    .WhereIf(!request.Code.IsNullOrWhiteSpace(), x => x.Code.ToLower().Contains(request.Code.ToLower()))
                    .WhereIf(request.StartCreated.HasValue, x => x.CreationTime >= request.StartCreated.Value)
                    .WhereIf(request.EndCreated.HasValue, x => x.CreationTime <= request.EndCreated.Value)
                    .WhereIf(request.StartTransaction.HasValue, x => x.TransactionDate >= request.StartTransaction.Value)
                    .WhereIf(request.EndTransaction.HasValue, x => x.TransactionDate <= request.EndTransaction.Value)
                    .WhereIf(request.DocumentDetailType.HasValue, x => x.DocumentDetailType == request.DocumentDetailType.Value)
                    .WhereIf(!request.DocumentCode.IsNullOrWhiteSpace(), x => x.DocumentCode.ToLower().Contains(request.DocumentCode.ToLower()))
                    .WhereIf(request.TicketType.HasValue, x => x.TicketType == request.TicketType.Value)
                    .WhereIf(request.AudienceType.HasValue, x => x.AudienceType == request.AudienceType.Value)
                    .ToList();

                if (!entryQueries.Any())
                    return new PagingResponse<EntryLogResponse>(0, result);

                var entryLogQueries = entryLogs.Where(x => entryQueries.Select(x => x.Id).Any(entryId => entryId == x.EntryId));
                if (!entryLogQueries.Any())
                    return new PagingResponse<EntryLogResponse>(0, result);

                var entryAccountQueries = (await _entryAccountRepository.GetQueryableAsync())
                    .Where(x => entryQueries.Select(x => x.Id).Any(entryId => entryId == x.EntryId));

                var users = await _userRepository.GetListAsync();

                var audienceRequests = entryQueries.Where(p => p.AudienceId != null).Select(p =>
                {
                    (AudienceTypes, Guid?) item = (p.AudienceType, p.AudienceId);
                    return item;
                }).Distinct().ToArray();
                var audiences = (await _commonService.GetAudiences(audienceRequests));

                foreach (var entryLog in entryLogQueries)
                {
                    var entry = entryQueries.FirstOrDefault(x => x.Id == entryLog.EntryId) ?? new Entities.Entry();
                    var entryAccounts = entryAccountQueries.Where(x => x.EntryId == entryLog.EntryId);
                    var user = users.FirstOrDefault(x => x.Id == entryLog.CreatorId);
                    var audience = audiences.FirstOrDefault(x => x.Id == entry.AudienceId) ?? new MasterDataDTO();
                    decimal? totalTransactionMoney = null;
                    totalTransactionMoney = (entryAccounts != null && entry.Amount == 0) ? entryAccounts.Where(x=>x.IsDeleted == false).Sum(x=>x.AmountVnd) : entry.Amount;
                    if(entryLog.FromValue != null)
                    {
                        var fromValue = JObject.Parse(entryLog.FromValue);
                        if (fromValue.ContainsKey("Amount"))
                        {
                            totalTransactionMoney = (decimal)fromValue["Amount"];
                        }
                    }

                    var entryLogResponse = new EntryLogResponse()
                    {
                        Id = entryLog.Id,
                        Code = entryLog.Code ?? "",
                        TransactionCode = entry.Code ?? "",
                        TransactionDate = entry.TransactionDate,
                        DocumentType = entry.DocumentType,
                        DocumentCode = entry.DocumentCode ?? "",
                        TicketType = entry.TicketType,
                        TotalTransactionMoney = totalTransactionMoney,
                        UserAction = user != null ? user.Name : "",
                        Action = entryLog.Action,
                        AudienceCode = audience.Code ?? "",
                        AudienceName = audience.Name ?? "",
                    };
                    result.Add(entryLogResponse);
                }

                var resultQueries = result
                    .WhereIf(request.Action.HasValue, x => x.Action == request.Action)
                    .WhereIf(!request.Audience.IsNullOrWhiteSpace(), x =>
                    x.AudienceCode.ToLower().Contains(request.Audience.ToLower()) ||
                    x.AudienceName.ToLower().Contains(request.Audience.ToLower()))
                    .WhereIf(!request.UserAction.IsNullOrWhiteSpace(), x =>
                    x.UserAction.ToLower() == request.UserAction.ToLower());

                var resultPage = resultQueries
                    .OrderByDescending(x => x.Code)
                    .Skip(request.Offset)
                    .Take(request.PageSize)
                    .ToList();

                return new PagingResponse<EntryLogResponse>(resultQueries.Count(), resultPage);
            }
        }

        public async Task<DetailEntryLogResponse> DetailEntryLogAsync(Guid entryLogId)
        {
            using (_dataFilter.Disable<ISoftDelete>())
            {
                var result = new DetailEntryLogResponse();
                var entryLog = await _entryLogRepository.FindAsync(x => x.Id == entryLogId);
                if (entryLog == null)
                    return result;

                result.FromValue = entryLog.FromValue;
                result.ToValue = entryLog.ToValue;

                return result;
            }
        }

        public async Task<byte[]> ExportEntryLog(SearchEntryLogRequest request)
        {
            using (_dataFilter.Disable<ISoftDelete>())
            {
                var result = new List<EntryLogResponse>();

                var entryLogs = await _entryLogRepository.GetQueryableAsync();

                var entryIds = entryLogs.AsEnumerable().DistinctBy(x => x.EntryId).Select(x => x.EntryId);
                if (!entryIds.Any())
                    return ExcelHelper.ExportExcel(new List<ExportEntryLogResponse>());

                var entrys = (await _entryRepository.GetQueryableAsync()).Where(x => entryIds.Any(entryId => entryId == x.Id));
                if (!entrys.Any())
                    return ExcelHelper.ExportExcel(new List<ExportEntryLogResponse>());

                var entryQueries = entrys
                    .WhereIf(!request.Code.IsNullOrWhiteSpace(), x => x.Code.ToLower().Contains(request.Code.ToLower()))
                    .WhereIf(request.StartCreated.HasValue, x => x.CreationTime >= request.StartCreated.Value)
                    .WhereIf(request.EndCreated.HasValue, x => x.CreationTime <= request.EndCreated.Value)
                    .WhereIf(request.StartTransaction.HasValue, x => x.TransactionDate >= request.StartTransaction.Value)
                    .WhereIf(request.EndTransaction.HasValue, x => x.TransactionDate <= request.EndTransaction.Value)
                    .WhereIf(request.DocumentDetailType.HasValue, x => x.DocumentDetailType == request.DocumentDetailType.Value)
                    .WhereIf(!request.DocumentCode.IsNullOrWhiteSpace(), x => x.DocumentCode.ToLower().Contains(request.DocumentCode.ToLower()))
                    .WhereIf(request.TicketType.HasValue, x => x.TicketType == request.TicketType.Value)
                    .WhereIf(request.AudienceType.HasValue, x => x.AudienceType == request.AudienceType.Value)
                    .ToList();

                if (!entryQueries.Any())
                    return ExcelHelper.ExportExcel(new List<ExportEntryLogResponse>());

                var entryLogQueries = entryLogs.Where(x => entryQueries.Select(x => x.Id).Any(entryId => entryId == x.EntryId));
                if (!entryLogQueries.Any())
                    return ExcelHelper.ExportExcel(new List<ExportEntryLogResponse>());

                var entryAccountQueries = (await _entryAccountRepository.GetQueryableAsync())
                    .Where(x => entryQueries.Select(x => x.Id).Any(entryId => entryId == x.EntryId));

                var users = await _userRepository.GetListAsync();

                var audienceRequests = entryQueries.Where(p => p.AudienceId != null).Select(p =>
                {
                    (AudienceTypes, Guid?) item = (p.AudienceType, p.AudienceId);
                    return item;
                }).Distinct().ToArray();
                var audiences = (await _commonService.GetAudiences(audienceRequests));

                foreach (var entryLog in entryLogQueries)
                {
                    var entry = entryQueries.FirstOrDefault(x => x.Id == entryLog.EntryId) ?? new Entities.Entry();
                    var entryAccounts = entryAccountQueries.Where(x => x.EntryId == entryLog.EntryId);
                    var user = users.FirstOrDefault(x => x.Id == entryLog.CreatorId);
                    var audience = audiences.FirstOrDefault(x => x.Id == entry.AudienceId) ?? new MasterDataDTO();
                    decimal? totalTransactionMoney = null;
                    totalTransactionMoney = (entryAccounts != null && entry.Amount == 0) ? entryAccounts.Where(x => x.IsDeleted == false).Sum(x => x.AmountVnd) : entry.Amount;
                    if (entryLog.FromValue != null)
                    {
                        var fromValue = JObject.Parse(entryLog.FromValue);
                        if (fromValue.ContainsKey("Amount"))
                        {
                            totalTransactionMoney = (decimal)fromValue["Amount"];
                        }
                    }

                    var entryLogResponse = new EntryLogResponse()
                    {
                        Id = entryLog.Id,
                        Code = entryLog.Code ?? "",
                        TransactionCode = entry.Code ?? "",
                        TransactionDate = entry.TransactionDate,
                        DocumentType = entry.DocumentType,
                        DocumentCode = entry.DocumentCode ?? "",
                        TicketType = entry.TicketType,
                        TotalTransactionMoney = totalTransactionMoney,
                        UserAction = user != null ? user.Name : "",
                        Action = entryLog.Action,
                        AudienceCode = audience.Code ?? "",
                        AudienceName = audience.Name ?? "",
                    };
                    result.Add(entryLogResponse);
                }

                var resultQueries = result
                    .WhereIf(request.Action.HasValue, x => x.Action == request.Action)
                    .WhereIf(!request.Audience.IsNullOrWhiteSpace(), x =>
                    x.AudienceCode.ToLower().Contains(request.Audience.ToLower()) ||
                    x.AudienceName.ToLower().Contains(request.Audience.ToLower()))
                    .WhereIf(!request.UserAction.IsNullOrWhiteSpace(), x =>
                    x.UserAction.ToLower() == request.UserAction.ToLower());
                var exportData = new List<ExportEntryLogResponse>();

                foreach (var item in resultQueries)
                {
                    exportData.Add(new ExportEntryLogResponse
                    {
                        Code = item.Code,
                        TransactionCode = item.Code ,
                        TransactionDate = item.TransactionDate,
                        DocumentTypeName = _commonService.GetDocumentTypeName(item.DocumentType),
                        DocumentCode = item.DocumentCode,
                        TicketType = _commonService.GetTicketTypeName(item.TicketType),
                        TotalTransactionMoney = item.TotalTransactionMoney,
                        UserAction = item.UserAction,
                        Action = item.Action,
                    });
                }
                return ExcelHelper.ExportExcel(exportData);
            }
            
        }
    }
}