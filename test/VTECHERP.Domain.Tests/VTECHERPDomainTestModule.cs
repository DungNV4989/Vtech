using VTECHERP.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace VTECHERP;

[DependsOn(
    typeof(VTECHERPEntityFrameworkCoreTestModule)
    )]
public class VTECHERPDomainTestModule : AbpModule
{

}
