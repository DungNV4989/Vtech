using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.SaleOrderLines
{
    public class SaleOrderListItem
    {
        public Guid Id { get; set; }
        public string SaleOrderLineCode { get; set; }
        public string SaleOrderCode { get; set; }
        public string ProductName { get; set; }
        public string SuplierText { get; set; }
        public string InvoiceNumber { get; set; }
        /// <summary>
        /// Tỉ giá NDT - VND
        /// </summary>
        public decimal? Rate { get; set; } = 1;
        /// <summary>
        /// Giá yêu cầu
        /// </summary>
        public decimal? RequestPrice { get; set; }

        /// <summary>
        /// Giá đề xuất
        /// </summary>
        public decimal? SuggestedPrice { get; set; }

        /// <summary>
        /// Số lượng yêu cầu
        /// </summary>
        public int? RequestQuantity { get; set; }

        /// <summary>
        /// Số lượng đã nhập
        /// </summary>
        public int? ImportQuantity { get; set; }

        /// <summary>
        /// Tổng tiền tệ
        /// </summary>
        public decimal? TotalYuan { get; set; }
        public decimal? TotalPrice { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
        public string ChargerName { get; set; }
        public Guid? StoreId { get; set; }
        /// <summary>
        /// Loại sản phẩm
        /// </summary>
        public Guid? ProductType { get; set; }
        /// <summary>
        /// Trạng thái phiếu đặt hàng
        /// </summary>
        public SaleOrder.Status SaleOrderStatus { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
