using System;
using VTECHERP.DTOs.Base;

namespace VTECHERP.DTOs.BO.Tenants.Requests
{
    public class StoreByTenantRequest : BasePagingRequest
    {
        public Guid TenantId { get; set; }
        public string StoreName { get; set; }
    }
}