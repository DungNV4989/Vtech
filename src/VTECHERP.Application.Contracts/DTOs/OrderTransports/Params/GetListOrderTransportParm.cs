using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.OrderTransports.Params
{
    public class GetListOrderTransportParm : BasePagingRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string SaleOrderCode { get; set; }
        public string OrderTransportCode { get; set; }
        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public string Suplier { get; set; }
        /// <summary>
        /// Nhà vận chuyển
        /// </summary>
        public string Transporter { get; set; }
        public OrderTransportStatus? Status { get; set; }
    }
}
