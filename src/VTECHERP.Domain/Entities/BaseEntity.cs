using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VTECHERP.Entities
{
    public class BaseEntity<TKey>: FullAuditedAggregateRoot<TKey>, IMultiTenant
    {
        public new TKey Id { get => base.Id; set => base.Id = value; }
        public Guid? TenantId { get; set; }
        public bool IsActive { get; set; }
    }
}
