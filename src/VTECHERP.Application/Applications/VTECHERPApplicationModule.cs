using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using VTECHERP.BackgroundWorker;

namespace VTECHERP;

[DependsOn(
    typeof(VTECHERPDomainModule),
    typeof(AbpAccountApplicationModule),
    typeof(VTECHERPApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    (typeof(AbpBackgroundWorkersModule))
    )]
public class VTECHERPApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<VTECHERPApplicationModule>();
        });

        Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = false;
        });
    }

    public override async Task OnApplicationInitializationAsync(
       ApplicationInitializationContext context)
    {
        await context.AddBackgroundWorkerAsync<HandlerWorker>();
    }
}
