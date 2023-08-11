using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VTECHERP.Batch;
using VTECHERP.Debts;
using VTECHERP.Entities;

namespace VTECHERP.EntityFrameworkCore.EfCoreRepository
{
    public class EFCoreBatchStatusRepository : EfCoreRepository<VTechDbContext, BatchStatus, Guid>,
          IBatchStatusRepository
    {
        public EFCoreBatchStatusRepository(IDbContextProvider<VTechDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<BatchStatus> GetBy(string batchName)
        {
            var context = await GetDbContextAsync();
            var result = context.BatchStatus.FirstOrDefault(x => x.BatchName == batchName);
            return result;
        }
    }
}
