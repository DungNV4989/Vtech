using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.OrderTransports.Params
{
    public class UpdateOrderTransportParam
    {
        public Guid? Transporter { get; set; }
        public string TransportCode { get; set; }
        public OrderTransportStatus? Status { get; set; }
        public DateTime? DateTransport { get; set; }
        public DateTime? DateArrive { get; set; }
        public decimal? TotalPrice { get; set; }
        public List<Guid> SaleOrdersId { get; set; }
    }
}
