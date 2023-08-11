using System;
using Volo.Abp.MultiTenancy;
using VTECHERP.Enums;

namespace VTECHERP.Entities
{
    public class EntryLog : BaseEntity<Guid>, IMultiTenant
    {
        public string Code { get; set; }
        public Guid ActionId { get; set; }
        /// <summary>
        /// Mã bút toán
        /// </summary>
        public Guid EntryId { get; set; }
        public string FromValue { get; set; }
        public string ToValue { get; set; }
        public EntityActions Action { get; set; }
    }
}
