using System;
using System.Collections.Generic;
using VTECHERP.DTOs.SaleOrderLines;

namespace VTECHERP.DTOs.SaleOrders
{
    /// <summary>
    /// Phiếu đặt hàng
    /// </summary>
    public class SaleOrderCreateRequest
    {
        /// <summary>
        /// Id nhà cung cấp
        /// </summary>
        public Guid SupplierId { get; set; }

        /// <summary>
        /// Id cửa hàng
        /// </summary>
        public Guid StoreId { get; set; }

        /// <summary>
        /// Số hóa đơn
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Ngày đặt hàng
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Tỉ giá NDT - VND
        /// </summary>
        public decimal? Rate { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Danh sách sản phẩm của phiếu đặt hàng từ nhà cung cấp
        /// </summary>
        public IList<SaleOrderLineCreateRequest> SaleOrderlines { get; set; }
    }
}