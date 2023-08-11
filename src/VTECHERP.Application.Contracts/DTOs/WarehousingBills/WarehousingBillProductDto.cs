using System;
using System.Collections.Generic;
using VTECHERP.Enums;
using VTECHERP.Enums.Product;

namespace VTECHERP.DTOs.WarehousingBills
{
    public class WarehousingBillProductDto
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
        /// Tên SP
        /// </summary>
        public string? ProductName { get; set; }
        /// <summary>
        /// Mã SP
        /// </summary>
        public string? ProductCode { get; set; }
        /// <summary>
        /// Tồn kho hiện tại
        /// </summary>
        public int ProductStockQuantity { get; set; }
        /// <summary>
        /// Số tồn tại thời điểm xuất/nhập
        /// </summary>
        public int CurrentStockQuantity { get; set; }
        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// Tên đơn vị tính
        /// </summary>
        public string UnitName { get; set; }
        /// <summary>
        /// Số lượng nhập/xuất
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// Giá
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Giá vốn hiện tại
        /// </summary>
        public decimal CurrentStockPrice { get; set; }
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
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// Số tồn sau phiếu
        /// </summary>
        public int UpdatedStockQuantity { get; set; }

        public override bool Equals(object obj)
        {
            return obj is WarehousingBillProductDto dto &&
                   WarehousingBillId.Equals(dto.WarehousingBillId) &&
                   ProductId.Equals(dto.ProductId) &&
                   ProductName == dto.ProductName &&
                   ProductCode == dto.ProductCode &&
                   Unit == dto.Unit &&
                   UnitName == dto.UnitName &&
                   Quantity == dto.Quantity &&
                   Price == dto.Price &&
                   TotalPriceBeforeDiscount == dto.TotalPriceBeforeDiscount &&
                   DiscountType == dto.DiscountType &&
                   DiscountAmount == dto.DiscountAmount &&
                   TotalPrice == dto.TotalPrice &&
                   Note == dto.Note;
        }
    }
}
