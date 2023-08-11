using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VTECHERP.DebtReports;
using VTECHERP.Entities;

namespace VTECHERP.EntityFrameworkCore.EfCoreRepository
{
    public class EFCoreDebtReportRepository : EfCoreRepository<VTechDbContext, DebtReport, Guid>, 
          IDebtReportRepository
    {
        public EFCoreDebtReportRepository(IDbContextProvider<VTechDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<DebtReport>> GetAllAsync(DateTime input)
        {
            var debtReports = await GetDbSetAsync();
            var listDebtReports = debtReports.Where(x => x.Year > input.Year || (x.Year == input.Year && x.Month >= input.Month)).AsQueryable();
            return listDebtReports.ToList();
        }

        public async Task<List<DebtReport>> GetDebtReportLastMonthAsync(DateTime input)
        {
            var debtReports = await GetDbSetAsync();
            var listDebtReports = debtReports.Where(x =>x.Year == input.AddMonths(-1).Year && x.Month == input.AddMonths(-1).Month).AsQueryable();
            return listDebtReports.ToList();
        }
    }
}
