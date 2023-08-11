using Volo.Abp.Modularity;

namespace VTECHERP;

[DependsOn(
    typeof(VTECHERPApplicationModule),
    typeof(VTECHERPDomainTestModule)
    )]
public class VTECHERPApplicationTestModule : AbpModule
{

}
