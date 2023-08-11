using System;

namespace VTECHERP.Entities
{
    /// <summary>
    /// Danh sách sản phẩm trong phiếu nháp
    /// </summary>
    public class DraftTicketProduct : BaseEntity<Guid>
    {
        /// <summary>
        /// Mã phiếu nháp
        /// </summary>
        public Guid DraftTicketId { get; set; }
        /// <summary>
        /// Mã sản phẩm
        /// </summary>
        public Guid ProductId { get; set; }
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