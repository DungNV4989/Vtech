using System;

namespace VTECHERP.DTOs.StoreProducts
{
    public class StoreProductDto
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
        /// <summary>
        /// Giá vốn
        /// </summary>
        public decimal StockPrice { get; set; } = 0;
        /// <summary>
        /// Giá bán lẻ
        /// </summary>
        public decimal? RetailPrice { get; set; }
        /// <summary>
        /// Giá bán sỉ
        /// </summary>
        public decimal? WholesalePrice { get; set; }
        /// <summary>
        /// Giá đồng thuận SPA
        /// </summary>
        public decimal SPAPrice { get; set; }
    }
}