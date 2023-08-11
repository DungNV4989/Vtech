using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VTECHERP.Entities
{
    [Table("StoreProducts")]
    /// <summary>
    /// Hàng tồn trong kho cửa hàng
    /// </summary>
    public class StoreProduct : BaseEntity<Guid>
    {
        /// <summary>
        /// Id Cửa hàng
        /// </summary>
        public Guid StoreId { get; set; }
        /// <summary>
        /// Id sản phẩm
        /// </summary>
        public Guid ProductId { get; set; }
        /// <summary>
        /// Số lượng tồn
        /// </summary>
        public int StockQuantity { get; set; } = 0;
        //public decimal StockPrice { get; set; } = 0;
        ///// <summary>
        ///// Phiếu nhập kho cuối cùng ảnh hưởng đến giá vốn
        ///// </summary>
        //public Guid? LatestWarehousingBillId { get; set; }
        ///// <summary>
        ///// Giá vốn trước phiếu nhập kho cuối cùng
        ///// </summary>
        //public decimal StockPriceBeforeLatest { get; set; }
        /// <summary>
        /// số lượng sản phẩm trước phiếu nhập kho cuối cùng
        /// </summary>
        public int QuantityBeforeLatest { get; set; }
    }
}
