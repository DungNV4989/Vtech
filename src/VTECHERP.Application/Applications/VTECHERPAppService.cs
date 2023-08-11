using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Localization;
using Volo.Abp.Application.Services;

namespace VTECHERP;

/* Inherit your application services from this class.
 */
public abstract class VTECHERPAppService : ApplicationService
{
    protected VTECHERPAppService()
    {
        LocalizationResource = typeof(VTECHERPResource);
    }
}
