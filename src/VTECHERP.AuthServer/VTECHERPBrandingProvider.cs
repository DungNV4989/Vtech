using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace VTECHERP;

[Dependency(ReplaceServices = true)]
public class VTECHERPBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "VTECHERP";
}
