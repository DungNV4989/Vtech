using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Domain.Shared.Helper.Excel.Attributes;

namespace VTECHERP.DTOs.BestSaleByProductReport
{
    public class ExportBestSaleByProductResponse
    {
        [Header("Mã sản phẩm")]
        public string Code { get; set; }
        [Header("Sản phẩm")]
        public string ProductName { get; set; }
        [Header("Giá bán lẻ")]
        public decimal Price { get; set; }
        [Header("Tồn đầu kỳ")]
        public decimal SLBegin { get; set; }
        [Header("SL nhập")]
        public decimal ImportQuatity { get; set; }
        [Header("Tồn đầu kỳ + nhập")]
        public decimal TotalSLBeginAndImportQuatity { get; set; }
        [Header("Bán lẻ")]
        public decimal RetailQuantity { get; set; }
        [Header("Bán sỉ")]
        public decimal AgencyQuantity { get; set; }
        [Header("Bán spa")]
        public decimal SpaQuantity { get; set; }
        [Header("SL bán ")]
        public decimal SaleQuantity { get; set; }
        [Header("Tồn cuối kỳ")]
        public decimal SLEnd { get; set; }
        [Header("Tỉ lệ bán ra")]
        public decimal SaleRate { get; set; }
    }
}
