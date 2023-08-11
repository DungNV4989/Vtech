using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Domain.Shared.Helper.Excel.Attributes;

namespace VTECHERP.DTOs.PriceTable
{
    public class ExportPriceProductResponse
    {
        [Header("Mã sản phẩm")]
        public string ProductCode { get; set; }
        [Header("Tên sản phẩm")]
        public string ProductName { get; set; }
        [Header("Danh mục sản phẩm")]
        public string ProductCategoryName { get; set; }
        [Header("SL tồn")]
        public decimal StockQuantity { get; set; }
        [Header("Giá nhập")]
        public decimal EntryPrice { get; set; }
        [Header("Giá bán")]
        public decimal SalePrice { get; set; }
        [Header("Giá SPA")]
        public decimal SPAPrice { get; set; }
        [Header("Giá vốn")]
        public decimal StockPrice { get; set; }
        [Header("Bảng giá 1")]
        public string PriceTable1 { get; set; }
        [Header("Bảng giá 2")]
        public string PriceTable2 { get; set; }
        [Header("Bảng giá 3")]
        public string PriceTable3 { get; set; }
        [Header("Bảng giá 4")]
        public string PriceTable4 { get; set; }
        [Header("Bảng giá 5")]
        public string PriceTable5 { get; set; }
    }
}
