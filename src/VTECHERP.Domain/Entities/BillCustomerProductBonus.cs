using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace VTECHERP.Entities
{
    public class BillCustomerProductBonus : BaseEntity<Guid>, IMultiTenant
    {
        public string Code { get; set; }
        public Guid? BillCustomerProductId { get; set; }
        public Guid? ProductId { get; set; }
        public int Quantity { get; set; }
        public bool? IsDebt { get; set; }
        /// <summary>
        /// Giá vốn
        /// </summary>
        public decimal? CostPrice { get; set; }
    }
}
