using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.MultiTenancy;
using VTECHERP.Enums;

namespace VTECHERP.Entities
{
    [Table("ProductLogss")]
    public class ProductLog : BaseEntity<Guid>, IMultiTenant
    {
        public string Code { get; set; }
        public Guid ActionId { get; set; }
        public Guid ProductId { get; set; }
        public string FromValue { get; set; }
        public string ToValue { get; set; }
        public EntityActions Action { get; set; }
    }
}