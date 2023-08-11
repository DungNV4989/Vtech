using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using VTECHERP.Enums;

namespace VTECHERP.Entities
{
    public class BillCustomerLog : BaseEntity<Guid>, IMultiTenant
    {
        public Guid BillCustomerId { get; set; }
        public string FromValue { get; set; }
        public string ToValue { get; set; }
        public EntityActions Action { get; set; }
    }
}
