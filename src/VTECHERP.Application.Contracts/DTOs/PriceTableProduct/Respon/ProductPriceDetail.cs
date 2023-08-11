using System;
using System.Collections.Generic;

namespace VTECHERP.DTOs.PriceTableProduct.Respon
{
    public class ProductPriceDetail
    {
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; }
        public Guid? ProductCategoryId { get; set; }
        public string? ProductCategoryName { get; set; }
        public string ProductName { get; set; }
        /// <summary>
        /// Giá nhập
        /// </summary>
        public decimal? EntryPrice { get; set; }

        /// <summary>
        /// Giá bán lẻ
        /// </summary>
        public decimal? SalePrice { get; set; }

        /// <summary>
        /// Giá vốn
        /// </summary>
        public decimal StockPrice { get; set; }

        /// <summary>
        /// Giá sỉ
        /// </summary>
        public decimal? WholeSalePrice { get; set; }

        /// <summary>
        /// Giá SPA
        /// </summary>
        public decimal? SPAPrice { get; set; }
        /// <summary>
        /// Số tồn
        /// </summary>
        public int StockQuantity { get; set; }
        public List<PriceTableProductPriceDetail> ProductPrices { get; set; } = new();
    }


}
