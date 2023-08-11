using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Domain.Shared.Helper.Excel.Attributes;

namespace VTECHERP.DTOs.WarehousingBills
{
    public class ExportInventoryByProductResponse
    {
        [Header("Mã sản phẩm")]
        public string Code { get; set; }
        [Header("Mã vạch")]
        public string BarCode { get; set; }
        [Header("Sản phẩm")]
        public string ProductName { get; set; }
        [Header("Tồn hiện tại")]
        public decimal Inventory { get; set; }
        [Header("Tổng tồn")]
        public decimal TotalInventory { get; set; }
        [Header("SL tồn đầu kỳ")]
        public decimal SLBegin { get; set; }
        [Header("Giá vốn đầu kỳ")]
        public decimal StockPriceBegin { get; set; }
        [Header("Thành tiền đầu kỳ")]
        public decimal MoneyBegin { get; set; }
        [Header("SL nhập trong kỳ")]
        public decimal SLImportPeriod { get; set; }
        [Header("Giá vốn nhập trong kỳ")]
        public decimal StockPriceImportPeriod { get; set; }
        [Header("Thành tiền nhập trong kỳ")]
        public decimal MoneyImportPeriod { get; set; }
        [Header("SL xuất trong kỳ")]
        public decimal SLExportPeriod { get; set; }
        [Header("Giá vốn xuất trong kỳ")]
        public decimal StockPriceExportPeriod { get; set; }
        [Header("Thành tiền xuất trong kỳ")]
        public decimal MoneyExportPeriod { get; set; }
        [Header("SL tồn cuối kỳ")]
        public decimal SLEnd { get; set; }
        [Header("Giá vốn cuối kỳ")]
        public decimal StockPriceEnd { get; set; }
        [Header("Thành tiền cuối kỳ")]
        public decimal MoneyEnd { get; set; }
    }
}
