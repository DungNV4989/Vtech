using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.DebtReminderLogs;

namespace VTECHERP.ServiceInterfaces
{
    public interface IDebtReminderLogService : IScopedDependency
    {
        Task<DebtReminderLogCreateRequest> AddAsync(DebtReminderLogCreateRequest request);
        Task<PagingResponse<DebtReminderLogDto>> GetListAsync(SearchDebtReminderLogRequest request);
        Task<byte[]> ExportDebtReminderLogAsync(SearchDebtReminderLogRequest request);
    }
}