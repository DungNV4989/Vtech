using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace VTECHERP.Controllers;

public class HomeController : AbpController
{

    public ActionResult Index()
    {
        return Redirect("~/swagger");
    }
}
