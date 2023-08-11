using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace VTECHERP.ServiceInterfaces
{
    public interface IDebtReportService: IScopedDependency
    {
        Task CaculateDebtMonthly();
    }
}
