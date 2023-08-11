using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.PriceTableProduct.Respon
{
    public class PriceTableProductItem
    {
        public Guid Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Inventory { get; set; }
        /// <summary>
        /// Giá nhập
        /// </summary>
        public decimal? EntryPrice { get; set; }

        /// <summary>
        /// Giá bán
        /// </summary>
        public decimal? SalePrice { get; set; }
        /// <summary>
        /// Giá vốn
        /// </summary>
        public decimal StockPrice { get; set; }
        /// <summary>
        /// Giá bảng giá
        /// </summary>
        public decimal PriceTable { get; set; }
    }
}
