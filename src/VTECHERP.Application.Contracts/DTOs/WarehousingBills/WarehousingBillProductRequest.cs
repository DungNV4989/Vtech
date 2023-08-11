using System;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.WarehousingBills
{
    public class WarehousingBillProductRequest
    {
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
        /// Loại chiết khấu
        /// </summary>
        public MoneyModificationType? DiscountType { get; set; }
        /// <summary>
        /// Số tiền chiết khấu
        /// </summary>
        public decimal? DiscountAmount { get; set; }
        /// <summary>
        /// Giá cước
        /// </summary>
        public decimal? TransportPrice { get; set; }
        public string? Note { get; set; }
        
    }
}
