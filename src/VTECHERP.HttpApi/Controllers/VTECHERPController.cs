using VTECHERP.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace VTECHERP.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class VTECHERPController : AbpControllerBase
{
    protected VTECHERPController()
    {
        LocalizationResource = typeof(VTECHERPResource);
    }
}
