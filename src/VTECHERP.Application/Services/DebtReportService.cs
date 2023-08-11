using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using VTECHERP.DebtReports;
using VTECHERP.Entities;
using VTECHERP.Enums;
using VTECHERP.ServiceInterfaces;
using Volo.Abp.MultiTenancy;

namespace VTECHERP.Services
{
    public class DebtReportService : IDebtReportService
    {
        private readonly IRepository<Debt> _debtRepository;
        private readonly IRepository<DebtReport> _debtReportRepository;
        private readonly IRepository<Suppliers> _supplierRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<SaleOrders> _saleOrderRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IDataFilter _dataFilter;
        public DebtReportService(
            IRepository<Debt> debtRepository,
            IRepository<Suppliers> supplierRepository,
            IRepository<Customer> customerRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IDataFilter dataFilter,
            IRepository<SaleOrders> saleOrderRepository,
            IRepository<DebtReport> debtReportRepository)
        {
            _debtRepository = debtRepository;
            _supplierRepository = supplierRepository;
            _saleOrderRepository = saleOrderRepository;
            _customerRepository = customerRepository;
            _debtReportRepository = debtReportRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _dataFilter = dataFilter;
        }
        /// <summary>
        /// CaculateDebtMonthly
        /// T là tháng tính công nợ. tháng trước tháng hiện tại
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task CaculateDebtMonthly()
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
                try
                {

                    var now = DateTime.Now;
                    var firstDayMonth = new DateTime(now.Year, now.Month, 01);
                    var firstDayLastMonth = new DateTime(now.Year, now.Month, 01).AddMonths(-1);

                    //convert to Utc
                    var firstDayMonthUtc = firstDayMonth.ToUniversalTime();
                    var firstDayLastMonthUtc = firstDayLastMonth.ToUniversalTime();


                    #region Tính công nợ giữa kì của tháng trước (T)
                    var debtQuery = await _debtRepository.GetQueryableAsync();
                    debtQuery = debtQuery.Where(x => x.TransactionDate >= firstDayLastMonthUtc && x.TransactionDate < firstDayMonthUtc);

                    var supplierDebts = debtQuery.GroupBy(x => x.SupplierId).Select(x => new { SupplierId = x.Key, Credit = x.Sum(c => c.Credits), Debt = x.Sum(d => d.Debts) }).ToList();
                    var customerDebt = debtQuery.GroupBy(x => x.CustomerId).Select(x => new { CustomerId = x.Key, Credit = x.Sum(c => c.Credits), Debt = x.Sum(d => d.Debts) }).ToList();

                    #endregion


                    //DateTime tháng trước của T - 1
                    var previousPeriodDate = firstDayLastMonth.AddDays(-15);

                    #region Get công nợ tháng T và T+1 : nếu tồn tại rồi thì xóa tính lại

                    var periodT = _debtReportRepository.GetQueryableAsync().Result.Where(x => (x.Year.Equals(firstDayLastMonth.Year) && x.Month.Equals(firstDayLastMonth.Month)) || (x.Year.Equals(firstDayMonth.Year) && x.Month.Equals(firstDayMonth.Month))).ToList();
                    if (periodT.Any())
                    {
                        await _debtReportRepository.HardDeleteAsync(periodT);
                        await uow.SaveChangesAsync();
                    }

                    #endregion

                    //Get công của tháng T - 1
                    var previousPeriodDebt = _debtReportRepository.GetQueryableAsync().Result.Where(x => x.Year.Equals(previousPeriodDate.Year) && x.Month.Equals(previousPeriodDate.Month)).ToList();

                    var supplierIds = supplierDebts.Select(x => x.SupplierId).ToList();
                    var suppliers = _supplierRepository.GetDbSetAsync().Result.AsNoTracking().Where(x => supplierIds.Any(z => z == x.Id));

                    var debtReports = new List<DebtReport>();
                    foreach (var item in supplierDebts)
                    {
                        if (item.SupplierId == null) continue;
                        var pre = previousPeriodDebt.FirstOrDefault(x => x.AudienceId == item.SupplierId && (x.AudienceType == Enums.AudienceTypes.SupplierVN || x.AudienceType == Enums.AudienceTypes.SupplierCN));
                        if (pre == null) pre = new DebtReport();
                        var supplier = suppliers.FirstOrDefault(x => x.Id == item.SupplierId);
                        if (supplier == null) continue;

                        //3
                        var beginOfPeriodCredit = pre?.EndOfPeriodCredit ?? 0;
                        //4
                        var beginOfPeriodDebt = pre?.EndOfPeriodDebt ?? 0;
                        //5
                        var periodCredit = item.Credit;
                        //6
                        var periodDebt = item.Debt;
                        var endOfPeriodCredit = (pre?.EndOfPeriodCredit ?? 0) + item.Credit;
                        var endOfPeriodDebt = (pre?.EndOfPeriodDebt ?? 0) + item.Debt;
                        //var endOfPeriodCredit = (pre?.EndOfPeriodCredit ?? 0) + item.Credit - beginOfPeriodDebt - periodDebt;
                        //var endOfPeriodDebt = (pre?.EndOfPeriodDebt ?? 0) + item.Debt - beginOfPeriodCredit - periodCredit;
                        //debtReport Tháng T
                        var debtReport = new DebtReport()
                        {
                            Year = firstDayLastMonth.Year,
                            Month = firstDayLastMonth.Month,
                            AudienceType = supplier.Origin == SupplierOrigin.CN ? AudienceTypes.SupplierCN : AudienceTypes.SupplierVN,
                            AudienceId = item.SupplierId,
                            BeginOfPeriodCredit = beginOfPeriodCredit,
                            BeginOfPeriodDebt = beginOfPeriodDebt,
                            PeriodCredit = item.Credit,
                            PeriodDebt = item.Debt,
                            EndOfPeriodCredit = endOfPeriodCredit <= 0 ? 0 : endOfPeriodCredit,
                            EndOfPeriodDebt = endOfPeriodDebt <= 0 ? 0 : endOfPeriodDebt,
                            IsAutoGenerated = true,
                        };
                        debtReports.Add(debtReport);
                        //debtReport Tháng T + 1
                        debtReports.Add(new DebtReport()
                        {
                            Year = firstDayMonth.Year,
                            Month = firstDayMonth.Month,
                            AudienceType = debtReport.AudienceType,
                            AudienceId = debtReport.AudienceId,
                            BeginOfPeriodCredit = debtReport.EndOfPeriodCredit,
                            BeginOfPeriodDebt = debtReport.EndOfPeriodDebt,
                            PeriodCredit = 0,
                            PeriodDebt = 0,
                            EndOfPeriodCredit = 0,
                            EndOfPeriodDebt = 0,
                            IsAutoGenerated = true,
                        });
                    }
                    foreach (var item in customerDebt)
                    {
                        if (item.CustomerId == null) continue;
                        var pre = previousPeriodDebt.FirstOrDefault(x => x.AudienceId == item.CustomerId && (x.AudienceType == Enums.AudienceTypes.Customer));
                        if (pre == null) pre = new DebtReport();

                        var BeginOfPeriodCredit = pre?.EndOfPeriodCredit ?? 0;
                        var BeginOfPeriodDebt = pre?.EndOfPeriodDebt ?? 0;
                        var PeriodCredit = item.Credit;
                        var PeriodDebt = item.Debt;
                        var endOfPeriodCredit = (pre?.EndOfPeriodCredit ?? 0) + item.Credit;
                        var endOfPeriodDebt = (pre?.EndOfPeriodDebt ?? 0) + item.Debt;
                        //debtReport Tháng T
                        var debtReport = new DebtReport()
                        {
                            Year = firstDayLastMonth.Year,
                            Month = firstDayLastMonth.Month,
                            AudienceType = AudienceTypes.Customer,
                            AudienceId = item.CustomerId,
                            BeginOfPeriodCredit = BeginOfPeriodCredit,
                            BeginOfPeriodDebt = BeginOfPeriodDebt,
                            PeriodCredit = item.Credit,
                            PeriodDebt = item.Debt,
                            EndOfPeriodCredit = endOfPeriodCredit <= 0 ? 0 : endOfPeriodCredit,
                            EndOfPeriodDebt = endOfPeriodDebt <= 0 ? 0 : endOfPeriodDebt,
                            IsAutoGenerated = true,
                        };
                        debtReports.Add(debtReport);
                        //debtReport Tháng T + 1
                        debtReports.Add(new DebtReport()
                        {
                            Year = firstDayMonth.Year,
                            Month = firstDayMonth.Month,
                            AudienceType = AudienceTypes.Customer,
                            AudienceId = debtReport.AudienceId,
                            BeginOfPeriodCredit = debtReport.EndOfPeriodCredit,
                            BeginOfPeriodDebt = debtReport.EndOfPeriodDebt,
                            PeriodCredit = 0,
                            PeriodDebt = 0,
                            EndOfPeriodCredit = 0,
                            EndOfPeriodDebt = 0,
                            IsAutoGenerated = true,
                        });

                    }
                    await _debtReportRepository.InsertManyAsync(debtReports);
                    await uow.SaveChangesAsync();
                    await uow.CompleteAsync();

                }
                catch (Exception ex)
                {
                    await uow.RollbackAsync();
                    throw;
                }
            }
           
        }

    }
}
