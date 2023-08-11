using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VTECHERP.DTOs.Attachment;
using VTECHERP.DTOs.SaleOrders;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.OrderTransports
{
    public class OrderTransportItemList
    {
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string TransporterText { get; set; }
        public Guid? TransporterId { get; set; }
        public string TransportCode { get; set; }
        public OrderTransportStatus Status { get; set; }
        public List<SaleOrderCommonDto> SaleOrders { get; set; }
        /// <summary>
        /// Tổng tiền
        /// </summary>
        public decimal? TotalPrice { get; set; }
        public DateTime? DateTransport { get; set; }
        public DateTime? DateArrive { get; set; }
        /// <summary>
        /// Link sản phẩm
        /// </summary>
        public List<DetailAttachmentDto> Attachments { get; set; }
    }
}
