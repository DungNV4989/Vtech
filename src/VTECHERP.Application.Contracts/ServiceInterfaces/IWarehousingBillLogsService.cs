using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.WarehousingBillLogs;

namespace VTECHERP.Services
{
    public interface IWarehousingBillLogsService : IScopedDependency
    {
        Task<PagingResponse<WarehousingBillLogsDTO>> SearchBillLogs(SearchWarehousingBillLogsRequest request);

        Task<WarehousingBillLogsDTO> GetById(Guid id);

        Task<byte[]> ExportWarehousingBillLog(SearchWarehousingBillLogsRequest request);
    }
}
