using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace VTECHERP.Data;

/* This is used if database provider does't define
 * IVTECHERPDbSchemaMigrator implementation.
 */
public class NullVTECHERPDbSchemaMigrator : IIdentityDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}

public class NullVTechDbSchemaMigrator : IVTechDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}

public class NullAgencyDbSchemaMigrator : IAgencyDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}

