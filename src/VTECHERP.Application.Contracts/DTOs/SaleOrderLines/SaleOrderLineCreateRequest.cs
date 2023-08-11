using System;

namespace VTECHERP.DTOs.SaleOrderLines
{
    /// <summary>
    /// Sản phẩm của phiếu đặt hành từ nhà cung cấp
    /// </summary>
    public class SaleOrderLineCreateRequest
    {
        /// <summary>
        /// Id sản phẩm
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Số lượng yêu cầu
        /// </summary>
        public int? RequestQuantity { get; set; }

        /// <summary>
        /// Giá yêu cầu
        /// </summary>
        public decimal? RequestPrice { get; set; }

        /// <summary>
        /// Giá đề xuất
        /// </summary>
        public decimal? SuggestedPrice { get; set; }
    }
}