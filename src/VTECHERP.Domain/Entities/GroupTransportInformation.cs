using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace VTECHERP.Entities
{
    public class GroupTransportInformation:BaseEntity<Guid>, IMultiTenant
    {
        /// <summary>
        /// Id đơn vận chuyển cha
        /// </summary>
        public Guid? ParentTransportInformationId { get; set; }
        /// <summary>
        /// Id đơn vận chuyển con
        /// </summary>
        public Guid? ChildTransportInformationId { get; set; }
    }
}
