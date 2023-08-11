using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Reports
{
    public class InventoryByProductReportDto
    {
        /// <summary>
        /// Mã sản phẩm
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Mã vạch
        /// </summary>
        public string BarCode { get; set; }
        /// <summary>
        /// Sản phẩm
        /// </summary>
        public string ProductName { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? StoreId { get; set; }
        public Guid? TenantId { get; set; }
        public decimal StockPrice { get; set; }
        /// <summary>
        /// Tổng tồn
        /// </summary>
        public decimal TotalInventory { get; set; }
        /// <summary>
        /// Tồn hiện tại
        /// </summary>
        public decimal Inventory { get; set; }
        /// <summary>
        /// SL tồn đầu kỳ
        /// </summary>
        public decimal SLBegin { get; set; }
        /// <summary>
        /// Giá vốn tồn đầu kỳ
        /// </summary>
        public decimal StockPriceBegin { get; set; }
        /// <summary>
        /// thành tiền tồn đầu kỳ
        /// </summary>
        public decimal MoneyBegin { get; set; }
        /// <summary>
        /// Số lượng nhập trong kỳ 
        /// </summary>
        public decimal SLImportPeriod { get; set; }
        /// <summary>
        /// Giá vốn nhập trong kỳ
        /// </summary>
        public decimal StockPriceImportPeriod { get; set; }
        /// <summary>
        /// thành tiền nhập trong kỳ 
        /// </summary>
        public decimal MoneyImportPeriod { get; set; }
        /// <summary>
        /// SL xuất trong kỳ 
        /// </summary>
        public decimal SLExportPeriod { get; set; }
        /// <summary>
        /// giá vốn xuất trong kỳ
        /// </summary>
        public decimal StockPriceExportPeriod { get; set; }
        /// <summary>
        /// thành tiền xuất trong kỳ
        /// </summary>
        public decimal MoneyExportPeriod { get; set; }
        /// <summary>
        /// SL tồn cuối kỳ
        /// </summary>
        public decimal SLEnd { get; set; }
        /// <summary>
        /// giá vốn cuối kỳ 
        /// </summary>
        public decimal StockPriceEnd { get; set; }
        /// <summary>
        /// thành tiền cuối kỳ
        /// </summary>
        public decimal MoneyEnd { get; set; }
    }
    public class InventoryByProductReportResponse
    {
        public List<InventoryByProductReportDto> inventoryByProductReports { get; set; }
        public decimal TotalBegin { get; set; }
        public decimal PriceBegin { get; set; }
        public decimal MoneyBegin { get; set; }
        public decimal TotalImportPeriod { get; set; }
        public decimal PriceImportPeriod { get; set; }
        public decimal MoneyImportPeriod { get; set; }
        public decimal TotalExportPeriod { get; set; }
        public decimal PriceExportPeriod { get; set; }
        public decimal MoneyExportPeriod { get; set; }
        public decimal TotalEnd { get; set; }
        public decimal PriceEnd { get; set; }
        public decimal MoneyEnd { get; set; }
    }
}
