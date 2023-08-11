using System;

namespace VTECHERP.DTOs.SaleOrderLines
{
    /// <summary>
    /// Cập nhật thông tin về sản phẩm trong đơn hàng từ nhà cung cấp
    /// </summary>
    public class SaleOrderLineUpdateRequest
    {
        /// <summary>
        /// Id SaleOrderLine
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Id sản phẩm
        /// </summary>
        public Guid? ProductId { get; set; }

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

        /// <summary>
        /// Xác nhận xóa sản phẩm khỏi đơn hàng;
        /// TRUE: Sẽ xóa;
        /// FALSE: Không xóa;
        /// </summary>
        public bool IsDelete { get; set; }
    }
}