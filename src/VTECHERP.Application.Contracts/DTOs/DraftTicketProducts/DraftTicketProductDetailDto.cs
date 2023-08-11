using System;

namespace VTECHERP.DTOs.DraftTicketProducts
{
    public class DraftTicketProductDetailDto
    {
        public Guid Id { get; set; }

        public Guid DraftTicketId { get; set; }

        /// <summary>
        /// Mã sản phẩm
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Mã vạch sản phẩm
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// Tên sản phẩm
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Số lượng xác nhận
        /// </summary>
        public int? ConfirmQuatity { get; set; }
        /// <summary>
        /// Giá vốn / Tại thời điểm tạo phiếu chuyển kho or tạo phiếu nháp
        /// </summary>
        public decimal? CostPrice { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Số lượng có thể chuyển tại thời điểm tạo mới/chỉnh sửa
        /// </summary>
        public int CanTransfer { get; set; }
    }
}