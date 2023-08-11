using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Timing;
using Volo.Abp.Users;
using VTECHERP.Constants;
using VTECHERP.DebtReports;
using VTECHERP.Debts;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.DebtCustomer;
using VTECHERP.Entities;
using VTECHERP.Enums;
using static VTECHERP.Constants.EntryConfig.PaymentReceipt;
using static VTECHERP.Constants.EntryConfig.ReturnProduct;
using static VTECHERP.Constants.EntryConfig.WarehousingImport;
using Customer = VTECHERP.Entities.Customer;

namespace VTECHERP.Services
{
    public class DebtCustomerService : IDebtCustomerService
    {
        private readonly IRepository<Debt> _debtRepository;
        private readonly IRepository<DebtReport> _debtReportRepository;
        private readonly IRepository<Suppliers> _supplierRepository;
        private readonly IRepository<SaleOrders> _saleOrderRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<BillCustomer> _billCustomerRepository;
        private readonly IRepository<PaymentReceipt> _paymentReceiptRepository;
        private readonly IRepository<DebtReminderLog> _debtReminderRepository;
        private readonly IRepository<Entry> _entryRepository;
        private readonly IRepository<EntryAccount> _entryAccountRepository;
        private readonly ICommonService _commonService;
        private readonly IClock _clock;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly ICurrentUser _userManager;
        public DebtCustomerService(IRepository<Debt> debtRepository,
            IRepository<Suppliers> supplierRepository,
            IRepository<DebtReport> debtReportRepository,
            IRepository<Customer> customerRepository,
            IRepository<SaleOrders> saleOrderRepository,
            IRepository<BillCustomer> billCustomerRepository,
            IRepository<PaymentReceipt> paymentReceiptRepository,
            IRepository<DebtReminderLog> debtReminderRepository,
            IRepository<Entry> entryRepository,
            IRepository<EntryAccount> entryAccountRepository,
            ICommonService commonService,
            IClock clock,
            IRepository<UserStore> userStoreRepository,
            ICurrentUser userManager
            )
        {
            _debtRepository = debtRepository;
            _supplierRepository = supplierRepository;
            _saleOrderRepository = saleOrderRepository;
            _customerRepository = customerRepository;
            _debtReportRepository = debtReportRepository;
            _billCustomerRepository = billCustomerRepository;
            _paymentReceiptRepository = paymentReceiptRepository;
            _debtReminderRepository = debtReminderRepository;
            _entryRepository = entryRepository;
            _entryAccountRepository = entryAccountRepository;
            _commonService = commonService;
            _clock = clock;
            _userStoreRepository = userStoreRepository;
            _userManager = userManager;
        }

        public async Task<PagingResponse<SearchDebtCustomerResponse>> Search(SearchDebtCustomerRequest request)
        {
            return await SearchFilter(request, true);
        }

        private async Task<PagingResponse<SearchDebtCustomerResponse>> SearchFilter(SearchDebtCustomerRequest request, bool ignorePageing = false)
        {
            try
            {
                if (request.FromDate.HasValue)
                {
                    request.FromDate = _clock.Normalize(request.FromDate.Value);
                }
                if (request.ToDate.HasValue)
                {
                    request.ToDate = _clock.Normalize(request.ToDate.Value).AddDays(1);
                }

                if (!request.FromDate.HasValue && !request.ToDate.HasValue)
                {
                    return new PagingResponse<SearchDebtCustomerResponse>();
                }


                if (request.FromDate.HasValue && !request.ToDate.HasValue)
                {
                    request.ToDate = DateTime.UtcNow.AddDays(1);
                }


                var customerIdDaysOwedFilter = new List<Guid?>();
                if (request.NumberOfDaysOwed != null)
                {
                    var dateNew = DateTime.UtcNow.AddDays(-request.NumberOfDaysOwed.Value);
                    customerIdDaysOwedFilter = _paymentReceiptRepository.GetQueryableAsync().Result.Where(x => x.AudienceType == Enums.AudienceTypes.Customer &&
                             (x.TicketType == Enums.TicketTypes.CreditNote || x.TicketType == Enums.TicketTypes.Receipt) && x.TransactionDate < dateNew).Select(x => x.AudienceId).ToList();
                }

                var customerIdLastOrderFilter = new List<Guid?>();
                if (request.LastOrder != null)
                {
                    var dateNew = DateTime.UtcNow.AddDays(-request.LastOrder.Value);
                    customerIdLastOrderFilter = _billCustomerRepository.GetQueryableAsync().Result.Where(x => x.CreationTime < dateNew).Select(x => x.CustomerId).ToList();
                }

                var customerIdEmployeeFilter = new List<Guid?>();
                if (request.EmployeeId != null)
                {
                    customerIdEmployeeFilter = _billCustomerRepository.GetQueryableAsync().Result.Where(x => x.EmployeeCare == request.EmployeeId).Select(x => x.CustomerId).ToList();
                }

                var date = DateTime.Now;
                if (request.FromDate != null)
                {
                    date = request.FromDate.Value;
                }

                var firstDayMonth = new DateTime(date.Year, date.Month, 01);
                var firstDayMonthUtc = firstDayMonth.ToUniversalTime();

                //công nợ giữa kì của T
                var debtQuery = await _debtRepository.GetQueryableAsync();
                if (request.FromDate.HasValue && request.ToDate.HasValue)
                {
                    debtQuery = debtQuery.Where(x => x.TransactionDate >= request.FromDate && x.TransactionDate < request.ToDate);
                }
                else if (!request.FromDate.HasValue && request.ToDate.HasValue)
                {
                    debtQuery = debtQuery.Where(x => x.TransactionDate >= firstDayMonthUtc && x.TransactionDate < request.ToDate);
                }


                debtQuery = debtQuery.WhereIf(request.StoreId != null, x => x.StoreId == request.StoreId)
                .WhereIf(request.CustomerId != null, x => x.CustomerId == request.CustomerId)
                .WhereIf(request.EmployeeId != null, x => x.EmployeeId == request.EmployeeId)
                .WhereIf(request.HasCod != null && request.HasCod == Enums.Debt.DebtHasCodeEnums.NotCod, x => x.HasCod == false)
                .WhereIf(request.HasCod != null && request.HasCod == Enums.Debt.DebtHasCodeEnums.Cod, x => x.HasCod == true);

                var customerQuery = await _customerRepository.GetQueryableAsync();
                var customerSelect = customerQuery.WhereIf(request.CustomerType != null, x => x.CustomerType == request.CustomerType)
                    .WhereIf(request.ProvinceId != null && request.ProvinceId.Count > 0, x => request.ProvinceId.Any(z => z == (x.ProvinceId ?? Guid.Empty)))
                    .WhereIf(request.NumberOfDaysOwed != null && customerIdDaysOwedFilter.Count > 0, x => customerIdDaysOwedFilter.Any(z => z == x.Id))
                    .WhereIf(request.LastOrder != null && customerIdLastOrderFilter.Count > 0, x => customerIdLastOrderFilter.Any(z => z == x.Id))
                    .WhereIf(request.EmployeeId != null && customerIdEmployeeFilter.Count > 0, x => customerIdEmployeeFilter.Any(z => z == x.Id))
                    .WhereIf(request.CustomerId != null, x => x.Id == request.CustomerId)
                .Select(x => new { x.Id, x.Name, x.Code, x.DebtLimit, x.PhoneNumber });
                //.ToList();
                var userId = _userManager.Id;
                var userStores = (await _userStoreRepository.GetQueryableAsync()).Where(x => x.UserId == userId);

                var query = from customer in customerSelect
                            join debt in debtQuery on customer.Id equals (debt.CustomerId) into ct
                            from debtDe in ct.DefaultIfEmpty()
                            join userStore in userStores on debtDe.StoreId equals userStore.StoreId
                            select new
                            {
                                CustomerId = customer.Id,
                                Customer = customer,
                                Debt = debtDe
                            };

                var customerDebts = query.GroupBy(x => x.CustomerId).Select(x => new
                {
                    CustomerId = x.Key,
                    Customer = x.First().Customer,
                    TransactionDate = x.First().Debt != null ? x.First().Debt.TransactionDate : new DateTime(),
                    Debts = x.Select(z => z.Debt)
                }).ToList();

                //Get công nợ đầu kì trong reportDebt của tháng T
                var queryDebtReport = await _debtReportRepository.GetQueryableAsync();
                var debtReport = queryDebtReport.Where(x => x.Year == date.Year &&
                x.Month == request.ToDate.Value.Month
                && x.AudienceType == Enums.AudienceTypes.Customer).ToList();

                var customers = customerDebts.Select(x => x.Customer);


                var result = new List<SearchDebtCustomerResponse>();
                if (request.FromDate.HasValue && request.ToDate.HasValue)
                {
                    //if (request.FromDate > request.ToDate) return new PagingResponse<SearchDebtCustomerResponse>();

                    var customerDebtsMonth = new List<Debt>();
                    if (request.FromDate.Value.Day != 1)
                    {
                        //Get công nợ từ đầu tháng đến ngày filter
                        var congNoDauKyQuery = await _debtRepository.GetQueryableAsync();
                        customerDebtsMonth = congNoDauKyQuery.Where(x => x.TransactionDate >= firstDayMonthUtc && x.TransactionDate < request.FromDate).ToList();
                    }

                    foreach (var item in customers)
                    {

                        var midterm = customerDebts.FirstOrDefault(x => x.Customer.Id == item.Id);
                        var begin = customerDebtsMonth.Where(x => x.CustomerId == item.Id).ToList();
                        var reportCurrrentMonth = debtReport.FirstOrDefault(x => x.AudienceId == item.Id);
                        //3
                        var BeginDebt = begin.Sum(x => x.Debts) + (reportCurrrentMonth?.BeginOfPeriodDebt ?? 0);
                        //4
                        var BeginCredit = begin.Sum(x => x.Credits) + (reportCurrrentMonth?.BeginOfPeriodCredit ?? 0);
                        //5
                        var Debt = midterm?.Debts?.Sum(x => x?.Debts) ?? 0;
                        //6
                        var Credit = midterm?.Debts?.Sum(x => x?.Credits) ?? 0;


                        result.Add(new SearchDebtCustomerResponse()
                        {
                            CustomerId = item.Id,
                            CustomerName = item.Name,
                            TransactionDate = midterm != null ? midterm.TransactionDate : begin.Max(x => x.TransactionDate),
                            Credit = Credit,
                            Debt = Debt,
                            BeginCredit = BeginCredit,
                            BeginDebt = BeginDebt,
                            EndDebt = BeginDebt + Debt - BeginCredit - Credit, //Nợ [Phải thu] = 3 + 5 - 4 - 6
                            EndCredit = BeginCredit + Credit - BeginDebt - Debt, //Có [Phải trả] = 4 + 6 - 3 - 5
                            DebtLimit = item.DebtLimit,
                            CustomerPhone = item.PhoneNumber
                        });
                    }

                }
                else if (!request.FromDate.HasValue && request.ToDate.HasValue)
                {
                    foreach (var item in customers)
                    {

                        var midterm = customerDebts.FirstOrDefault(x => x.Customer.Id == item.Id);
                        var reportCurrrentMonth = debtReport.FirstOrDefault(x => x.AudienceId == item.Id);
                        //3
                        var BeginDebt = 0;
                        //4
                        var BeginCredit = 0;
                        //5
                        var Debt = (midterm?.Debts?.Sum(x => x?.Debts) ?? 0) + (reportCurrrentMonth?.BeginOfPeriodDebt ?? 0);
                        //6
                        var Credit = (midterm?.Debts?.Sum(x => x?.Credits) ?? 0) + (reportCurrrentMonth?.BeginOfPeriodCredit ?? 0);


                        result.Add(new SearchDebtCustomerResponse()
                        {
                            CustomerId = item.Id,
                            CustomerName = item.Name,
                            TransactionDate = midterm != null ? midterm.TransactionDate : new DateTime(),
                            Credit = Credit,
                            Debt = Debt,
                            BeginCredit = BeginCredit,
                            BeginDebt = BeginDebt,
                            EndDebt = BeginDebt + Debt - BeginCredit - Credit, //Nợ [Phải thu] = 3 + 5 - 4 - 6
                            EndCredit = BeginCredit + Credit - BeginDebt - Debt, //Có [Phải trả] = 4 + 6 - 3 - 5
                            DebtLimit = item.DebtLimit,
                            CustomerPhone = item.PhoneNumber
                        });
                    }
                }

                result = result.WhereIf(request.DebtType == null && request.DebtType == Enums.DebtTypes.Debt, x => x.EndDebt > 0)
               .WhereIf(request.DebtType != null && request.DebtType == Enums.DebtTypes.Credit, x => x.EndCredit > 0)
               .WhereIf(request.DebtLimit != null && request.DebtLimit == Enums.Debt.DebtLimitEnums.DebtEqLimit, x => x.EndCredit == x.DebtLimit)
               .WhereIf(request.DebtLimit != null && request.DebtLimit == Enums.Debt.DebtLimitEnums.DebtGtLimit, x => x.EndCredit > x.DebtLimit)
               .WhereIf(request.DebtLimit != null && request.DebtLimit == Enums.Debt.DebtLimitEnums.DebtLtLimit, x => x.EndCredit < x.DebtLimit)
               .WhereIf(request.DebtFrom != null, x => x.EndCredit > request.DebtFrom)
               .WhereIf(request.DebtFrom != null, x => x.EndDebt > request.DebtFrom)
               .WhereIf(request.DebtTo != null, x => x.EndCredit > request.DebtTo)
               .WhereIf(request.DebtTo != null, x => x.EndDebt > request.DebtTo)
                    .ToList();

                result = result.Where(x => x.Credit > 0 || x.Debt > 0 || x.BeginDebt > 0 || x.BeginCredit > 0)
                   .OrderByDescending(z => z.TransactionDate).ToList();

                if (!ignorePageing)
                {
                    result = result.Skip(request.Offset).Take(request.PageSize).ToList();
                }

                if (result.Count == 0)
                    return new PagingResponse<SearchDebtCustomerResponse>(result.Count, result);

                var listCustomerPagingIds = result.Select(x => x.CustomerId).ToList();
                var billCustomers = _billCustomerRepository.GetQueryableAsync().Result.Where(x => listCustomerPagingIds.Any(z => z == (x.CustomerId ?? Guid.Empty)))
                    .GroupBy(x => x.CustomerId).Select(x => new { CustomerId = x.Key, LastDate = x.Max(z => z.CreationTime) }).ToList();

                var reminders = _debtReminderRepository.GetQueryableAsync().Result.Where(x => listCustomerPagingIds.Any(z => z == (x.CustomerId ?? Guid.Empty)))
                    .GroupBy(x => x.CustomerId).Select(x => new { CustomerId = x.Key, LastDate = x.Max(z => z.CreationTime) }).ToList();

                var paymentReceipts = _paymentReceiptRepository.GetQueryableAsync().Result.Where(x => listCustomerPagingIds.Any(z => z == (x.AudienceId ?? Guid.Empty)) && x.AudienceType == Enums.AudienceTypes.Customer && (x.TicketType == Enums.TicketTypes.CreditNote || x.TicketType == Enums.TicketTypes.Receipt))
                   .GroupBy(x => x.AudienceId).Select(x => new { CustomerId = x.Key, LastDate = x.Max(z => z.TransactionDate) }).ToList();


                foreach (var item in result)
                {
                    item.LastOrderDate = billCustomers.FirstOrDefault(x => x.CustomerId == item.CustomerId)?.LastDate;
                    item.DebtReminderDate = reminders.FirstOrDefault(x => x.CustomerId == item.CustomerId)?.LastDate;
                    item.LatestDebtCollectionDate = paymentReceipts.FirstOrDefault(x => x.CustomerId == item.CustomerId)?.LastDate;
                    if (item.EndDebt <= 0) item.EndDebt = 0;
                    if (item.EndCredit <= 0) item.EndCredit = 0;
                }

                return new PagingResponse<SearchDebtCustomerResponse>()
                {
                    Total = result.Count(),
                    Data = result,
                };

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<TotalDebtCustomerResponse> TotalDebtCustomer(SearchDebtCustomerRequest request)
        {
            var searchData = await SearchFilter(request, false);

            if (searchData == null) return new TotalDebtCustomerResponse();

            return new TotalDebtCustomerResponse()
            {
                Credit = searchData.Data?.Sum(x => x.EndCredit) ?? 0,
                Debt = searchData.Data?.Sum(x => x.EndDebt) ?? 0,
            };
        }

        public async Task<PagingResponse<DebtDetailDto>> DetailDebtCustomer(SearchDebtCustomerDetailRequest request)
        {
            try
            {
                var result = new List<DebtDetailDto>();
                var debts = await _debtRepository.GetQueryableAsync();
                var debtByCustomerId = debts.Where(x => x.CustomerId == request.CustomerId);
                if (!debtByCustomerId.Any())
                {
                    return null;
                }
                var entrys = (await _entryRepository.GetQueryableAsync())
                    .Where(x => debtByCustomerId.Select(x => x.EntryId).Any(id => id == x.Id))
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
                            case TicketTypes.Return:
                                isDebt = false;
                                break;
                            case TicketTypes.Export:
                            case TicketTypes.PaymentVoucher:
                            case TicketTypes.DebitNote:
                            case TicketTypes.Sales:
                                isDebt = true;
                                break;
                        }
                        //entryDetailResponse.DebtAmount = isDebt ? entry.Amount : 0;
                        //entryDetailResponse.CreditAmount = isDebt ? 0 : entry.Amount;
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
                    //result = result.Where(x => x.AmountCny != null && (x.AmountCny.HasValue && x.AmountCny.Value > 0)).ToList();
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

        public async Task<byte[]> ExportDebtCustomer(SearchDebtCustomerRequest request)
        {
            var data = await SearchFilter(request, false);
            var result = data.Data.ToList();
            var exportData = new List<ExportDebtCustomerResponse>();
            var stt = 0;
            foreach (var item in result)
            {
                exportData.Add(new ExportDebtCustomerResponse
                {
                    STT = stt + 1,
                    CustomerName = item.CustomerName,
                    CustomerPhone = item.CustomerPhone,
                    DebtReminderDate = item.DebtReminderDate,
                    LastOrderDate = item.LastOrderDate,
                    LatestDebtCollectionDate = item.LatestDebtCollectionDate,
                    BeginCredit = item.BeginCredit,
                    BeginDebt = item.BeginDebt,
                    Credit = item.Credit,
                    Debt = item.Debt,
                    EndCredit = item.EndCredit,
                    EndDebt = item.EndDebt,
                    DebtLimit = item.DebtLimit
                }) ;
            }
            return ExcelHelper.ExportExcel(exportData);
        }
    }
}
