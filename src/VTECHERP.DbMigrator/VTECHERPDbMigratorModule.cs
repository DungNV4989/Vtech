using VTECHERP.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace VTECHERP.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(VTECHERPEntityFrameworkCoreModule),
    typeof(VTECHERPApplicationContractsModule)
    )]
public class VTECHERPDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
    }
}
