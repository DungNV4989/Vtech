using System;
using System.Collections.Generic;

namespace VTECHERP.DTOs.BO.Tenants.Requests
{
    public class ModuleManagerRequest
    {
        public Guid TenantId { get; set; }
        public List<Guid> Ids { get; set; }
    }
}