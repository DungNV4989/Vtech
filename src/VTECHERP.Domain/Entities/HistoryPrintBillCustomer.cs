using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace VTECHERP.Entities
{
    public class HistoryPrintBillCustomer : BaseEntity<Guid>, IMultiTenant
    {
        public Guid BillCustomerId { get; set; }
    }
}
