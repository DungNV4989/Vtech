using System;

namespace VTECHERP.DTOs.DraftTicketProducts
{
    public class DraftTicketProductCreateRequest
    {
        /// <summary>
        /// Mã sản phẩm
        /// </summary>
        public Guid ProductId { get; set; }
        /// <summary>
        /// Số lượng
        /// </summary>
        public int Quantity { get; set; }
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