using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Threading;
using Volo.Abp.Uow;
using VTECHERP.Batch;
using VTECHERP.Constants;
using VTECHERP.Debts;
using VTECHERP.Entities;
using VTECHERP.ServiceInterfaces;
using static VTECHERP.Constants.BatchConstant;

namespace VTECHERP.BackgroundWorker
{
    public class HandlerWorker : BaseWorker
    {
        private readonly IRepository<Debt> _debtRepository;
        private readonly IRepository<DebtReport, Guid> _debtReportRepository;
        private readonly IBatchStatusRepository _batchStatusRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IDebtReportService _debtReportService;
        public HandlerWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory, IRepository<Debt> debtRepository,
            IRepository<DebtReport, Guid> debtReportRepository,
            IUnitOfWorkManager unitOfWorkManager, IBatchStatusRepository batchStatusRepository, IDebtReportService debtReportService) : base(timer, serviceScopeFactory)
        {
            _debtRepository = debtRepository;
            _debtReportRepository = debtReportRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _batchStatusRepository = batchStatusRepository;
            _debtReportService = debtReportService;
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            var day = DateTime.Now.Day;
            
            if (Timer.Period == 30000)
            {
            }
            else if (Timer.Period == 86400000)
            {
                if (day == 1)
                {
                    await _debtReportService.CaculateDebtMonthly();
                   // RuningJobStaticAsync(DateTime.Now);
                }
            }
        }

        /// <summary>
        /// Lấy ra ngày đầu tiên trong tháng có chứa 
        /// 1 ngày bất kỳ được truyền vào
        /// </summary>
        /// <param name="dtDate">Ngày nhập vào</param>
        /// <returns>Ngày đầu tiên trong tháng</returns>
        public static DateTime GetFirstDayOfMonth(DateTime dtInput)
        {
            DateTime dtResult = dtInput;
            DateTime dtResult1 = dtResult.AddDays(-dtResult.Day);
            dtResult = dtResult.AddDays((-dtResult.Day) + 1);
            return dtResult;
        }
        public async void RuningJobStaticAsync(DateTime dtInput)
        {
            try
            {
                var lstDebt = await _debtRepository.GetListAsync();
                var yesterday = dtInput.AddDays(-1);
                var lstDebtBeginOfPeriod = lstDebt.Where(x => x.TransactionDate.Date <= yesterday.Date);
                //var lstDebtBeginOfPeriod = lstDebt.Where(x => x.TransactionDate.Date <= yesterday.Date && x.TransactionDate.Month == yesterday.Month && x.TransactionDate.Year == yesterday.Year);
                var lstStatic = lstDebtBeginOfPeriod.GroupBy(x => x.SupplierId);
                var existDebtReports = await _debtReportRepository.GetListAsync();
                List<DebtReport> debtReports = new List<DebtReport>();
                var exist = existDebtReports.Where(x => x.Year == dtInput.Year && x.Month == dtInput.Month).ToList();
                if (exist.Any())
                {
                    await _debtReportRepository.DeleteManyAsync(exist.Select(x => x.Id));
                }
                foreach (var item in lstStatic)
                {
                    DebtReport debtReport = new DebtReport();
                    var beginOfPeriodCredit = item.Where(x =>  ((x.TicketType == Enums.TicketTypes.Import || x.TicketType == Enums.TicketTypes.CreditNote || x.TicketType == Enums.TicketTypes.Receipt) && x.AudienceType == Enums.AudienceTypes.SupplierCN) || (x.AudienceType == Enums.AudienceTypes.SupplierVN && (x.TicketType == Enums.TicketTypes.Export))
                    ).Sum(x => x.Credits);
                    var beginOfPeriodDebt = item.Where(x => x.TicketType == Enums.TicketTypes.PaymentVoucher || x.TicketType == Enums.TicketTypes.DebitNote).Sum(x => x.Debts);
                    if (beginOfPeriodDebt > beginOfPeriodCredit)
                    {
                        debtReport.BeginOfPeriodDebt = beginOfPeriodDebt - beginOfPeriodCredit;
                        debtReport.BeginOfPeriodCredit = 0;
                    }else
                    {
                        debtReport.BeginOfPeriodDebt = 0;
                        debtReport.BeginOfPeriodCredit = beginOfPeriodCredit - beginOfPeriodDebt;
                    }
                    debtReport.Month = dtInput.Month;
                    debtReport.Year = dtInput.Year;
                    debtReport.AudienceId = item.Key;
                    debtReport.AudienceType = item.FirstOrDefault().AudienceType;
                    debtReports.Add(debtReport);
                }
                await _debtReportRepository.InsertManyAsync(debtReports);
                Logger.LogInformation("Completed");
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }

        }
        public async Task FisrtRuningJobStaticAsync()
        {
            try
            {
                var fisrtday = DateTime.Now;
                if (fisrtday.Day != 1)
                {
                    fisrtday = DateTime.Now.AddDays((-DateTime.Now.Day) + 1);
                }
                var lstDebt = await _debtRepository.GetListAsync();
                lstDebt = lstDebt.OrderBy(x => x.TransactionDate).Where(x => x.SupplierId != null && x.TransactionDate.Date < fisrtday.Date).ToList();
                var lstStatic = lstDebt.GroupBy(x => x.SupplierId);
                var existDebtReports = await _debtReportRepository.GetListAsync();
                List<DebtReport> debtReports = new List<DebtReport>();
                var exist = existDebtReports.Where(x => x.Year == fisrtday.Year && x.Month == fisrtday.Month).ToList();
                if (exist == null || exist.Count == 0)
                {
                    foreach (var item in lstStatic)
                    {
                        DebtReport debtReport = new DebtReport();
                        var beginOfPeriodCredit = item.Where(x => (((x.TicketType == Enums.TicketTypes.Import || x.TicketType == Enums.TicketTypes.Receipt || x.TicketType == Enums.TicketTypes.CreditNote) && x.AudienceType == Enums.AudienceTypes.SupplierCN) || (x.AudienceType == Enums.AudienceTypes.SupplierVN && (x.TicketType == Enums.TicketTypes.Import || x.TicketType == Enums.TicketTypes.Receipt || x.TicketType == Enums.TicketTypes.CreditNote))
                               )
                    ).Sum(x => x.Credits);
                        var beginOfPeriodDebt = item.Where(x => (((x.TicketType == Enums.TicketTypes.PaymentVoucher || x.TicketType == Enums.TicketTypes.DebitNote) && x.AudienceType == Enums.AudienceTypes.SupplierCN)
                               ||
                               (x.AudienceType == Enums.AudienceTypes.SupplierVN && (x.TicketType == Enums.TicketTypes.PaymentVoucher || x.TicketType == Enums.TicketTypes.DebitNote || x.TicketType == Enums.TicketTypes.Export))
                               )).Sum(x => x.Debts);
                        if (beginOfPeriodDebt > beginOfPeriodCredit)
                        {
                            debtReport.BeginOfPeriodDebt = beginOfPeriodDebt - beginOfPeriodCredit;
                            debtReport.BeginOfPeriodCredit = 0;
                        }
                        else
                        {
                            debtReport.BeginOfPeriodDebt = 0;
                            debtReport.BeginOfPeriodCredit = beginOfPeriodCredit - beginOfPeriodDebt;
                        }
                        debtReport.Month = fisrtday.Month;
                        debtReport.Year = fisrtday.Year;
                        debtReport.AudienceId = item.Key;
                        debtReport.AudienceType = item.FirstOrDefault().AudienceType;
                        debtReports.Add(debtReport);
                    }
                    await _debtReportRepository.InsertManyAsync(debtReports);
                }
                await LoadSettings();
                Logger.LogInformation("Completed");
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }

        }
    }
}
