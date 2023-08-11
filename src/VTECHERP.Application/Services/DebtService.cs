
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;
using VTECHERP.DebtReports;
using VTECHERP.Debts;
using VTECHERP.DTOs.Base;
using VTECHERP.Entities;
using VTECHERP.Enums;

namespace VTECHERP.Services
{
    public class DebtService: IDebtService
    {
        private readonly IRepository<Debt> _debtRepository;
        private readonly IRepository<DebtReport> _debtReportRepository;
        private readonly IRepository<Suppliers> _supplierRepository;
        private readonly IRepository<SaleOrders> _saleOrderRepository;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly ICurrentUser _userManager;
        private readonly IObjectMapper _mapper;
        public DebtService(
            IRepository<Debt> debtRepository,
            IRepository<DebtReport> debtReportRepository,
            IRepository<Suppliers> supplierRepository,
            IRepository<SaleOrders> saleOrderRepository,
            IRepository<UserStore> userStoreRepository,
            ICurrentUser userManager,
            IObjectMapper mapper)
        {
            _debtRepository = debtRepository;
            _supplierRepository = supplierRepository;
            _debtReportRepository = debtReportRepository;
            _saleOrderRepository = saleOrderRepository;
            _debtReportRepository = debtReportRepository;
            _mapper = mapper;
            _userStoreRepository = userStoreRepository;
            _userManager = userManager;
        }

        public async Task<List<DebtDto>> GetAllByMonthAsync(DateTime input)
        {
            var debts = await _debtRepository.GetQueryableAsync();
            var listDebts = debts.Where(x => x.TransactionDate.Date.Month == input.Month && x.TransactionDate.Date.Year == input.Year).ToList();
            return _mapper.Map<List<Debt>, List<DebtDto>>(listDebts);
        }
        public async Task<List<DebtDto>> GetAllAsync(DateTime input)
        {
            var debts = await _debtRepository.GetQueryableAsync();
            var listDebts = debts.Where(x => x.TransactionDate.Date >= input.Date).OrderBy(x => x.TransactionDate).ToList();
            return _mapper.Map<List<Debt>, List<DebtDto>>(listDebts);
        }

        public async Task<List<DebtDto>> GetListDebtAsync(SearchDebtRequest input)
        {
            var suppliers = await _supplierRepository.GetQueryableAsync();
            var debts = await _debtRepository.GetQueryableAsync();
            var debtReport = await _debtReportRepository.GetQueryableAsync();
            var userId = _userManager.Id;
            var userStores = (await _userStoreRepository.GetQueryableAsync()).Where(x=>x.UserId==userId);

            if (input.SearchDateFrom != null && input.SearchDateTo != null)
            {
                DateTime firstDayOfMonth = input.SearchDateFrom.Value.AddDays((-input.SearchDateFrom.Value.Day) + 1);
                DateTime dayBeforeSearchFrom = input.SearchDateFrom.Value.AddDays(-1);
                DateTime afterDayBeforeSearchFrom = dayBeforeSearchFrom.AddDays(1);

                // lấy tất cả công nợ từ đầu tháng đến cuối thời gian search
                debts = debts.Where(x => x.SupplierId != null
                    && x.TransactionDate.Date >= firstDayOfMonth.Date
                    && x.TransactionDate.Date <= input.SearchDateTo.Value.Date);

                // lấy tất cả báo cáo công nợ các NCC trong tháng được chọn
                debtReport = debtReport.Where(x =>
                    (x.AudienceType == Enums.AudienceTypes.SupplierCN || x.AudienceType == Enums.AudienceTypes.SupplierVN)
                    && x.AudienceId != null
                    && x.Month == input.SearchDateFrom.Value.Month
                    && x.Year == input.SearchDateFrom.Value.Year);

                // Tổng hợp công nợ theo NCC
                // Lấy tất cả dữ liệu công nợ + NCC (flat)
                var query = from sup in suppliers
                            join debt in debts on sup.Id equals debt.SupplierId
                            join dr in debtReport on debt.SupplierId equals dr.AudienceId
                            into debtReports
                            from drGrp in debtReports.DefaultIfEmpty()
                            join userStore in userStores on debt.StoreId equals userStore.StoreId
                            select new
                            {
                                SupplierId = sup.Id,
                                SupplierCode = sup.Code,
                                SupplierType = sup.Origin,
                                SupplierName = sup.Name,
                                SupplierPhoneNumber = sup.PhoneNumber,
                                SupplierDebtReport = drGrp,
                                SupplierDebts = debt
                            };
                
                var datas = query.ToList();
                // group lại dữ liệu flat theo supplier id
                var list = datas
                    .GroupBy(p => p.SupplierId)
                    .Select(grp =>
                    {
                        var sup = grp.First();
                        var supDebts = grp.Select(p => p.SupplierDebts).ToList();
                        return new
                        {

                            sup.SupplierId,
                            sup.SupplierCode,
                            sup.SupplierType,
                            sup.SupplierName,
                            sup.SupplierPhoneNumber,
                            sup.SupplierDebtReport,
                            SupplierDebts = supDebts
                        };
                    }
                    )
                  // Tính tổng công nợ theo list công nợ MCC
                    .Select(sup =>
                    new DebtDto
                    {
                        SupplierId = sup.SupplierId,
                        SupplierCode = sup.SupplierCode,
                        SupplierName = sup.SupplierName,
                        SupplierType = sup.SupplierType,
                        PhoneNumber = sup.SupplierPhoneNumber,
                        // Tính tổng Nợ từ (Đầu tháng đến T - 1)
                        BeginDebt = SumDebtIn(sup.SupplierDebts, sup.SupplierDebtReport, firstDayOfMonth, dayBeforeSearchFrom, true),
                        // Tính tổng Có từ (Đầu tháng đến T - 1)
                        BeginCredit = SumCreditIn(sup.SupplierDebts, sup.SupplierDebtReport, firstDayOfMonth, dayBeforeSearchFrom, true),
                        // Tính tổng Nợ trong kỳ T
                        Debt = SumDebtIn(sup.SupplierDebts, sup.SupplierDebtReport, afterDayBeforeSearchFrom, input.SearchDateTo.Value, false),
                        // Tính tổng Có trong kỳ T
                        Credit = SumCreditIn(sup.SupplierDebts, sup.SupplierDebtReport, afterDayBeforeSearchFrom, input.SearchDateTo.Value, false)
                    });
                // Apply điều kiện filter
                list = list.WhereIf(input.SupplierId != null, x => x.SupplierId == input.SupplierId)
                .WhereIf(!string.IsNullOrEmpty(input.PhoneNumber), x => x.PhoneNumber.ToLower().Contains(input.PhoneNumber.ToLower()))
                .WhereIf(!string.IsNullOrEmpty(input.SupplierCode), x => x.SupplierCode.ToLower().Contains(input.SupplierCode.ToLower()))
                .WhereIf(input.Type != null, x => x.SupplierType == input.Type)
                .WhereIf(input.DebtType != null && input.DebtType == Enums.DebtTypes.Credit, x => x.Credit != 0)
                .WhereIf(input.DebtType != null && input.DebtType == Enums.DebtTypes.Debt, x => x.Debt != 0);

                return list.ToList();
            }
            else if (input.SearchDateFrom == null && input.SearchDateTo != null)
            {
                DateTime dayBeforeSearchFrom = input.SearchDateTo.Value.AddDays(-1);

                // lấy tất cả công nợ từ đầu tháng đến cuối thời gian search
                debts = debts.Where(x => x.SupplierId != null
                    && x.TransactionDate.Date <= input.SearchDateTo.Value.Date);

                // Tổng hợp công nợ theo NCC
                // Lấy tất cả dữ liệu công nợ + NCC (flat)
                var query = from sup in suppliers
                            join debt in debts on sup.Id equals debt.SupplierId
                            join userStore in userStores on debt.StoreId equals userStore.StoreId
                            select new
                            {
                                SupplierId = sup.Id,
                                SupplierCode = sup.Code,
                                SupplierType = sup.Origin,
                                SupplierName = sup.Name,
                                SupplierPhoneNumber = sup.PhoneNumber,
                                SupplierDebts = debt
                            };
                var datas = query.ToList();

                // group lại dữ liệu flat theo supplier id
                var list = datas
                    .GroupBy(p => p.SupplierId)
                    .Select(grp =>
                    {
                        var sup = grp.First();
                        var supDebts = grp.Select(p => p.SupplierDebts).ToList();
                        return new
                        {
                            sup.SupplierId,
                            sup.SupplierCode,
                            sup.SupplierType,
                            sup.SupplierName,
                            sup.SupplierPhoneNumber,
                            SupplierDebts = supDebts
                        };
                    }
                    )
                // Tính tổng công nợ theo list công nợ MCC
                    .Select(sup =>
                    new DebtDto
                    {
                        SupplierId = sup.SupplierId,
                        SupplierCode = sup.SupplierCode,
                        SupplierName = sup.SupplierName,
                        SupplierType = sup.SupplierType,
                        PhoneNumber = sup.SupplierPhoneNumber,
                        // Tính tổng Nợ từ (Đầu tháng đến T - 1)
                        BeginDebt = 0,
                        // Tính tổng Có từ (Đầu tháng đến T - 1)
                        BeginCredit = 0,
                        // Tính tổng Nợ trong kỳ T
                        Debt = SumDebtIn(sup.SupplierDebts, null, null, input.SearchDateTo.Value, false),
                        // Tính tổng Có trong kỳ T
                        Credit = SumCreditIn(sup.SupplierDebts, null, null, input.SearchDateTo.Value, false)
                    });
                // Apply điều kiện filter
                list = list.WhereIf(input.SupplierId != null, x => x.SupplierId == input.SupplierId)
                .WhereIf(!string.IsNullOrEmpty(input.PhoneNumber), x => x.PhoneNumber.ToLower().Contains(input.PhoneNumber.ToLower()))
                .WhereIf(!string.IsNullOrEmpty(input.SupplierCode), x => x.SupplierCode.ToLower().Contains(input.SupplierCode.ToLower()))
                .WhereIf(input.Type != null, x => x.SupplierType == input.Type)
                .WhereIf(input.DebtType != null && input.DebtType == Enums.DebtTypes.Credit, x => x.Credit != 0)
                .WhereIf(input.DebtType != null && input.DebtType == Enums.DebtTypes.Debt, x => x.Debt != 0);

                return list.ToList();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Tính tổng công nợ NCC - Nợ
        /// </summary>
        /// <param name="debts">Tất cả công nợ của NCC trong kỳ</param>
        /// <param name="report">Công nợ đầu kỳ của NCC</param>
        /// <param name="dateFrom">Tính từ ngày</param>
        /// <param name="dateTo">Tính đến ngày</param>
        /// <param name="useReport">Có cộng dữ liệu đầu kỳ không</param>
        /// <returns></returns>
        private decimal SumDebtIn(
            List<Debt> debts,
            DebtReport report,
            DateTime? dateFrom,
            DateTime? dateTo,
            bool useReport)
        {
            var reportNum = 0m;
            if (useReport)
            {
                reportNum = report == null ? 0 : report.BeginOfPeriodDebt;
            }
            var currentNum =
                debts.Where(debt =>
                    (dateFrom == null || debt.TransactionDate.Date >= dateFrom.Value.Date)
                    && (dateTo == null || debt.TransactionDate.Date <= dateTo.Value.Date)
                    &&
                    (
                        (
                            (
                                debt.TicketType == Enums.TicketTypes.PaymentVoucher
                                || debt.TicketType == Enums.TicketTypes.DebitNote
                            )
                            && debt.AudienceType == Enums.AudienceTypes.SupplierCN
                        )
                        ||
                        (
                            debt.AudienceType == Enums.AudienceTypes.SupplierVN
                            &&
                            (
                                debt.TicketType == Enums.TicketTypes.PaymentVoucher
                                || debt.TicketType == Enums.TicketTypes.DebitNote
                                || debt.TicketType == Enums.TicketTypes.Export
                            )
                        )
                    )
                ).Sum(x => x.Debts);
            return reportNum + currentNum;
        }

        /// <summary>
        /// Tính tổng công nợ NCC - Có
        /// </summary>
        /// <param name="debts">Tất cả công nợ của NCC trong kỳ</param>
        /// <param name="report">Công nợ đầu kỳ của NCC</param>
        /// <param name="dateFrom">Tính từ ngày</param>
        /// <param name="dateTo">Tính đến ngày</param>
        /// <param name="useReport">Có cộng dữ liệu đầu kỳ không</param>
        /// <returns></returns>
        private decimal SumCreditIn(
            List<Debt> debts,
            DebtReport report,
            DateTime? dateFrom,
            DateTime? dateTo,
            bool useReport)
        {
            var reportNum = 0m;
            if (useReport)
            {
                reportNum = report == null ? 0 : report.BeginOfPeriodCredit;
            }
            var currentNum =
                debts.Where(debt =>
                    (dateFrom == null || debt.TransactionDate.Date >= dateFrom.Value.Date)
                    && (dateTo == null || debt.TransactionDate.Date <= dateTo.Value.Date)
                    &&
                    (
                        (
                            (
                                debt.TicketType == Enums.TicketTypes.Import
                                || debt.TicketType == Enums.TicketTypes.Receipt
                                || debt.TicketType == Enums.TicketTypes.CreditNote
                                || debt.TicketType == Enums.TicketTypes.Other
                            )
                            && debt.AudienceType == Enums.AudienceTypes.SupplierCN
                        )
                        ||
                        (
                            debt.AudienceType == Enums.AudienceTypes.SupplierVN
                            &&
                            (
                                debt.TicketType == Enums.TicketTypes.Import
                                || debt.TicketType == Enums.TicketTypes.Receipt
                                || debt.TicketType == Enums.TicketTypes.CreditNote
                                || debt.TicketType == Enums.TicketTypes.Other
                            )
                        )
                    )
                ).Sum(x => x.Credits);
            return reportNum + currentNum;
        }

        [Obsolete]
        public async Task<List<DebtDto>> GetListDebtAsync_old(SearchDebtRequest input)
        {
            var suppliers = await _supplierRepository.GetQueryableAsync();
            var debts = await _debtRepository.GetQueryableAsync();
            var debtReport = await _debtReportRepository.GetQueryableAsync();

            if (input.SearchDateFrom != null && input.SearchDateTo != null)
            {
                debts = debts.Where(x => x.SupplierId != null && x.TransactionDate.Date <= input.SearchDateTo.Value.Date);
                debtReport = debtReport.Where(x => x.AudienceId != null && x.Month == input.SearchDateFrom.Value.Month && x.Year == input.SearchDateFrom.Value.Year);
                DateTime firstDayOfMonth = input.SearchDateFrom.Value.AddDays((-input.SearchDateFrom.Value.Day) + 1);
                var list = from de in debts.ToList()
                           group de by de.SupplierId into degr
                           join su in suppliers.ToList()
                           on degr.Key equals su.Id
                           into gr
                           from a in gr.DefaultIfEmpty()
                           //join sa in saleOrders.ToList() on degr.Key equals sa.Id
                           //into re
                           //from b in re.DefaultIfEmpty()
                           join bR in debtReport.ToList() on degr.Key equals bR.AudienceId
                           into br
                           from brGrj in br.DefaultIfEmpty()
                           select new DebtDto
                           {
                               Id = new Guid(),
                               Code = a == null ? "" : a.Code,
                               //DocumentType = de.DocumentType,
                               //Document = b?.Code,
                               //TransactionId = a.Id,
                               SupplierId = a == null ? null : a.Id,
                               SupplierCode = a == null ? "" : a.Code,
                               SupplierType = a == null ? null : a.Origin,
                               SupplierName = a == null ? "" : a.Name,
                               PhoneNumber = a == null ? "" : a.PhoneNumber,
                               BeginCredit = brGrj == null ? degr.Where(x => x.TransactionDate.Date >= firstDayOfMonth.Date && x.TransactionDate.Date < input.SearchDateFrom.Value.Date &&
                               (((x.TicketType == Enums.TicketTypes.Import || x.TicketType == Enums.TicketTypes.Receipt || x.TicketType == Enums.TicketTypes.CreditNote) && x.AudienceType == Enums.AudienceTypes.SupplierCN) || (x.AudienceType == Enums.AudienceTypes.SupplierVN && (x.TicketType == Enums.TicketTypes.Import || x.TicketType == Enums.TicketTypes.Receipt || x.TicketType == Enums.TicketTypes.CreditNote))
                               )).Sum(x => x.Credits) : brGrj.BeginOfPeriodCredit + degr.Where(x => x.TransactionDate.Date >= firstDayOfMonth.Date && x.TransactionDate.Date < input.SearchDateFrom.Value.Date &&
                               (((x.TicketType == Enums.TicketTypes.Import || x.TicketType == Enums.TicketTypes.Receipt || x.TicketType == Enums.TicketTypes.CreditNote) && x.AudienceType == Enums.AudienceTypes.SupplierCN) || (x.AudienceType == Enums.AudienceTypes.SupplierVN && (x.TicketType == Enums.TicketTypes.Import || x.TicketType == Enums.TicketTypes.Receipt || x.TicketType == Enums.TicketTypes.CreditNote))
                               )).Sum(x => x.Credits),
                               BeginDebt = brGrj == null ? degr.Where(x => x.TransactionDate.Date >= firstDayOfMonth.Date && x.TransactionDate.Date < input.SearchDateFrom.Value.Date && (((x.TicketType == Enums.TicketTypes.PaymentVoucher || x.TicketType == Enums.TicketTypes.DebitNote) && x.AudienceType == Enums.AudienceTypes.SupplierCN)
                               ||
                               (x.AudienceType == Enums.AudienceTypes.SupplierVN && (x.TicketType == Enums.TicketTypes.PaymentVoucher || x.TicketType == Enums.TicketTypes.DebitNote || x.TicketType == Enums.TicketTypes.Export))
                               )).Sum(x => x.Debts) : brGrj.BeginOfPeriodDebt + degr.Where(x => x.TransactionDate.Date >= firstDayOfMonth.Date && x.TransactionDate.Date < input.SearchDateFrom.Value.Date && (((x.TicketType == Enums.TicketTypes.PaymentVoucher || x.TicketType == Enums.TicketTypes.DebitNote) && x.AudienceType == Enums.AudienceTypes.SupplierCN)
                               ||
                               (x.AudienceType == Enums.AudienceTypes.SupplierVN && (x.TicketType == Enums.TicketTypes.PaymentVoucher || x.TicketType == Enums.TicketTypes.DebitNote || x.TicketType == Enums.TicketTypes.Export))
                               )).Sum(x => x.Debts),
                               Debt = degr == null ? 0 : degr.Where(x => x.TransactionDate.Date >= input.SearchDateFrom.Value.Date && x.TransactionDate.Date <= input.SearchDateTo.Value.Date &&
                               (((x.TicketType == Enums.TicketTypes.PaymentVoucher || x.TicketType == Enums.TicketTypes.DebitNote) && x.AudienceType == Enums.AudienceTypes.SupplierCN)
                               ||
                               (x.AudienceType == Enums.AudienceTypes.SupplierVN && (x.TicketType == Enums.TicketTypes.PaymentVoucher || x.TicketType == Enums.TicketTypes.DebitNote || x.TicketType == Enums.TicketTypes.Export))
                               )).Sum(x => x.Debts),
                               Credit = degr == null ? 0 : degr.Where(x => x.TransactionDate.Date >= input.SearchDateFrom.Value.Date && x.TransactionDate.Date <= input.SearchDateTo.Value.Date && (((x.TicketType == Enums.TicketTypes.Import || x.TicketType == Enums.TicketTypes.Receipt || x.TicketType == Enums.TicketTypes.CreditNote) && x.AudienceType == Enums.AudienceTypes.SupplierCN) || (x.AudienceType == Enums.AudienceTypes.SupplierVN && (x.TicketType == Enums.TicketTypes.Import || x.TicketType == Enums.TicketTypes.Receipt || x.TicketType == Enums.TicketTypes.CreditNote))
                               )).Sum(x => x.Credits),
                               //Rate = b?.Rate
                           };
                list = list.AsQueryable().WhereIf(input.SupplierId != null, x => x.SupplierId == input.SupplierId)
                    .WhereIf(!string.IsNullOrEmpty(input.PhoneNumber), x => x.PhoneNumber.ToLower().Contains(input.PhoneNumber.ToLower()))
                    .WhereIf(!string.IsNullOrEmpty(input.SupplierCode), x => x.SupplierCode.ToLower().Contains(input.SupplierCode.ToLower()))
                    .WhereIf(input.Type != null, x => x.SupplierType == input.Type)
                    .WhereIf(input.DebtType != null && input.DebtType == Enums.DebtTypes.Credit, x => x.Credit != 0)
                    .WhereIf(input.DebtType != null && input.DebtType == Enums.DebtTypes.Debt, x => x.Debt != 0)
                    //.WhereIf(input.NDT != null, x => x.Rate == input.NDT)
                    ;
                return list.ToList();
            }
            else
            {
                return null;
            }
        }

        public async Task<List<DebtDto>> GetListDebtByMonthAsync(DateTime input)
        {
            var suppliers = await _supplierRepository.GetQueryableAsync();
            var debts = await _debtRepository.GetQueryableAsync();
            var debtReport = await _debtReportRepository.GetQueryableAsync();
            var saleOrders = await _saleOrderRepository.GetQueryableAsync();

            var list = from de in debts
                       join su in suppliers
                       on de.SupplierId equals su.Id
                       into gr
                       from a in gr.DefaultIfEmpty()
                       join sa in saleOrders on de.SupplierId equals sa.Id
                       into re
                       from b in re.DefaultIfEmpty()
                       select new DebtDto
                       {
                           Code = a.Code,
                           DocumentCode = b == null ? "" : b.Code,
                           SupplierId = a.Id,
                           SupplierCode = a.Code,
                           SupplierType = a.Origin,
                           SupplierName = a.Name,
                           PhoneNumber = a.PhoneNumber,
                           Credit = de.Credits,
                           Debt = de.Debts,
                           Rate = b == null ? 0 : b.Rate,
                           TransactionDate = de.TransactionDate,
                       };
            list = list.Where(x => x.TransactionDate.Month == input.Month && x.TransactionDate.Year == input.Year);
            return list.ToList();

        }

        public async Task<List<DebtDto>> GetListDebtBySupplierIdAsync(Guid supplierId)
        {
            var debts = await _debtRepository.GetQueryableAsync();
            var listDebts = debts.Where(x => x.SupplierId == supplierId).OrderBy(x => x.TransactionDate).ToList();
            return _mapper.Map<List<Debt>, List<DebtDto>>(listDebts);
        }
    }
}
