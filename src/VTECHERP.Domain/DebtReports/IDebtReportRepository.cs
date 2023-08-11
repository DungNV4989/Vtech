using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VTECHERP.Entities;

namespace VTECHERP.DebtReports
{
    public interface IDebtReportRepository :  IRepository<DebtReport, Guid>
    {
        Task<List<DebtReport>> GetAllAsync(DateTime input);
        Task<List<DebtReport>> GetDebtReportLastMonthAsync(DateTime input);
    }
}
