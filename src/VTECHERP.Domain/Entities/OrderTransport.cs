using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using VTECHERP.Enums;

namespace VTECHERP.Entities
{
    /// <summary>
    /// Đơn vận chuyển
    /// </summary>
    public class OrderTransport : BaseEntity<Guid>, IMultiTenant
    {
        /// <summary>
        /// Nhà vận chuyển
        /// </summary>
        public Guid? TransporterId { get; set; }
       /// <summary>
       /// Mã vận đơn
       /// </summary>
        public string TransportCode { get; set; }
        /// <summary>
        /// Trạng thái
        /// </summary>
        public OrderTransportStatus Status { get; set; }
        public DateTime? DateTransport { get; set; }
        public DateTime? DateArrive { get; set; }
        public decimal TotalPrice { get; set; }
      
    }
}
