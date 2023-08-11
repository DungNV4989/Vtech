using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace VTECHERP.Entities
{
    public class TransporstBills : BaseEntity<Guid>, IMultiTenant
    {
        /// <summary>
        /// Id bảng TransportInformation
        /// </summary>
        public Guid? TransportInformationId { get; set; }
        /// <summary>
        /// Id bảng BillCustomers
        /// </summary>
        public Guid? BillCustomerId { get; set; }

    }
}
