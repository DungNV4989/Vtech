using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Domain.Shared.Helper.Excel.Attributes;

namespace VTECHERP.DTOs.OrderTransports
{
    public class ExportOrderTransportDto
    {
        [Header("Mã phiếu đặt hàng")]
        public string SaleOrderCode { get; set; }
        [Header("Nhà vận chuyển")]
        public string Transporter { get; set; }
        [Header("Mã vận chuyển")]
        public string TransportCode { get; set; }
        [Header("Nhà cung cấp")]
        public string Suplier { get; set; }
        [Header("Tổng tiền")]
        public string TotalPrice { get; set; }
        [Header("Số hóa đơn")]
        public string InvoiceNumber { get; set; }
    }
}
