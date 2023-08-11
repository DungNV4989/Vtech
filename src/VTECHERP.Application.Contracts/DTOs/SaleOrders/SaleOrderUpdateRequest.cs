using System;
using System.Collections.Generic;
using VTECHERP.DTOs.SaleOrderLines;

namespace VTECHERP.DTOs.SaleOrders
{
    /// <summary>
    /// Cập nhật đơn hàng từ nhà cung cấp
    /// </summary>
    public class SaleOrderUpdateRequest
    {
        /// <summary>
        /// Id SaleOrder
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Số kiện
        /// </summary>
        public int Package { get; set; }

        /// <summary>
        /// Số hóa đơn
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Ngày đặt hàng
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Danh sách sản phẩm cần cập nhật của đơn hàng
        /// </summary>
        public IList<SaleOrderLineUpdateRequest> SaleOrderLines { get; set; }
    }
}