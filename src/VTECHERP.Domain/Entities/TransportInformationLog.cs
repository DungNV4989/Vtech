using System;
using Volo.Abp.MultiTenancy;
using VTECHERP.Enums;

namespace VTECHERP.Entities
{
    public class TransportInformationLog : BaseEntity<Guid>,IMultiTenant
    {
        public Guid? ShipperId { get; set; }
        public TransportStatus Status { get; set; }
        public string TransportInformationCode { get; set; }
        /// <summary>
        /// Thời gian giao hàng
        /// </summary>
        public DateTime? ShipTime { get; set; }
    }
}
