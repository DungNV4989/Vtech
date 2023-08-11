using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VTECHERP.Entities;

namespace VTECHERP.Batch
{
    public interface IBatchStatusRepository : IRepository<BatchStatus, Guid>
    {
        Task<BatchStatus> GetBy(string batchName);
    }
}
