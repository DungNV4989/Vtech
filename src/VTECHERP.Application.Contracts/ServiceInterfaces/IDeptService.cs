using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.Debts;

namespace VTECHERP.Services
{
    public interface IDebtService: IScopedDependency
    {
        Task<List<DebtDto>> GetListDebtAsync(SearchDebtRequest input);
        Task<List<DebtDto>> GetListDebtAsync_old(SearchDebtRequest input);
        Task<List<DebtDto>> GetListDebtByMonthAsync(DateTime input);
        Task<List<DebtDto>> GetAllByMonthAsync(DateTime input);
        Task<List<DebtDto>> GetAllAsync(DateTime input);
        Task<List<DebtDto>> GetListDebtBySupplierIdAsync(Guid supplierId);
    }
}
