using System;
using System.ComponentModel.DataAnnotations.Schema;
using VTECHERP.Enums;

namespace VTECHERP.Entities
{
    /// <summary>
    /// Danh sách SP trong phiếu xuất nhập kho
    /// </summary>
    [Table("WarehousingBillProducts")]
    public class WarehousingBillProduct:BaseEntity<Guid>
    {
        /// <summary>
        /// Mã phiếu XNK
        /// </summary>
        public Guid WarehousingBillId { get; set; }
        /// <summary>
        /// Mã SP
        /// </summary>
        public Guid ProductId { get; set; }
        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// Số lượng nhập/xuất
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// Giá
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Tổng tiền trước chiết khấu
        /// </summary>
        public decimal TotalPriceBeforeDiscount { get; set; }
        /// <summary>
        /// Loại chiết khấu
        /// </summary>
        public MoneyModificationType? DiscountType { get; set; }
        /// <summary>
        /// Số tiền chiết khấu
        /// </summary>
        public decimal? DiscountAmount { get; set; }
        /// <summary>
        /// Tổng tiền cuối cùng
        /// </summary>
        public decimal TotalPrice { get; set; }
        /// <summary>
        /// Số tồn tại thời điểm xuất/nhập
        /// </summary>
        public int CurrentStockQuantity { get; set; }
        /// <summary>
        /// Số tồn sau khi xuất/nhập
        /// </summary>
        public int UpdatedStockQuantity { get; set; }
        /// <summary>
        /// Giá vốn tại thời điểm xuất/nhập
        /// </summary>
        public decimal CurrentStockPrice { get; set; }
        /// <summary>
        /// Giá vốn sau khi xuất/nhập
        /// </summary>
        public decimal UpdatedStockPrice { get; set; }
        /// <summary>
        /// Giá cước
        /// </summary>
        public decimal? TransportPrice { get; set; } = 0;
        /// <summary>
        /// Tổng tiền sau cước
        /// </summary>
        public decimal TotalPriceAfterTransport { get; set; }
        public string Note { get; set; }
    }
}
