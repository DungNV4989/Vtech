using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VTECHERP.Data;
using Volo.Abp.DependencyInjection;

namespace VTECHERP.EntityFrameworkCore;

public class EntityFrameworkCoreVTechDbSchemaMigrator
    : IVTechDbSchemaMigrator,ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreVTechDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the VTECHERPDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<VTechDbContext>()
            .Database
            .MigrateAsync();
    }
}
