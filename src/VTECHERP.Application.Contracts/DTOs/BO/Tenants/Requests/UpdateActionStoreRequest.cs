using System;

namespace VTECHERP.DTOs.BO.Tenants.Requests
{
    public class UpdateActionStoreRequest
    {
        public Guid TenantId { get; set; }
        public Guid StoreId { get; set; }
    }
}