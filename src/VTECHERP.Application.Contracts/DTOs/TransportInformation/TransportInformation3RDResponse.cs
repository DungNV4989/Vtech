using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.TransportInformation
{
    public class TransportInformation3RDResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string? FromStoreName { get; set; }
        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// Tên đơn vị vận chuyển 
        /// </summary>
        public Carrier TransportName { get; set; }
        /// <summary>
        /// Tạng thái vận đơn
        /// </summary>
        public TransportStatus Status { get; set; }
        public string CarrierShippingCode { get; set; }

        public decimal? TotalAmount { get; set; }
        public DateTime? CreatetionTime { get; set; }

    }
}
