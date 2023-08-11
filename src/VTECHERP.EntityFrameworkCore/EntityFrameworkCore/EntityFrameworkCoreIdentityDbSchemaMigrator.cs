using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VTECHERP.Data;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace VTECHERP.EntityFrameworkCore;

public class EntityFrameworkCoreIdentityDbSchemaMigrator
    : IIdentityDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreIdentityDbSchemaMigrator(
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
            .GetRequiredService<IdentityDbContext>()
            .Database
            .MigrateAsync();
    }
}
