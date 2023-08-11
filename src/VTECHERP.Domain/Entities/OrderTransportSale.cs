using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace VTECHERP.Entities
{
    public class OrderTransportSale : BaseEntity<Guid>, IMultiTenant
    {
        public Guid OrderTransportId { get; set; }
        public Guid OrderSaleId { get; set; }
    }
}
