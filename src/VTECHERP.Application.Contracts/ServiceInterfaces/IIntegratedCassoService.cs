using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.IntegratedCasso;

namespace VTECHERP.ServiceInterfaces
{
    public interface IIntegratedCassoService : IScopedDependency
    {
        public Task<IActionResult> HandlerWebhookCasso(CassoWebhookDTO request);
    }
}
