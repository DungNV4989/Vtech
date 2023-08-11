using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Reports
{
    public class StoreReportRevenueProduct
    {
        public Guid ProductId { get; set; }
        public Guid? TenantId { get; set; }
        public string TenantName { get; set; }
        public string BarCode { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal StockPrice { get; set; }
        /// <summary>
        /// Giá lẻ
        /// </summary>
        public decimal SalePrice { get; set; }
        /// <summary>
        /// Giá bán sỉ
        /// </summary>
        public decimal WholeSalePrice { get; set; }
        /// <summary>
        /// Giá spa
        /// </summary>
        public decimal SPAPrice { get; set; }
        /// <summary>
        /// Tên danh mục
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// Tổng bán - số lượng
        /// </summary>
        public decimal QuantitySell { get; set; }
        /// <summary>
        /// Tổng bán - tổng
        /// </summary>
        public decimal TotalAmountSell { get; set; }
        /// <summary>
        /// Tổng trả - sl
        /// </summary>
        public decimal QuantityReturn { get; set; }
        /// <summary>
        /// Tổng trả - tổng
        /// </summary>
        public decimal TotalAmountReturn { get; set; }
        /// <summary>
        /// Doanh số - số lượng
        /// </summary>
        public decimal QuantityReveue { get; set; }
        /// <summary>
        /// Doanh số - tổng
        /// </summary>
        public decimal TotalAmountReveue { get; set; }
        /// <summary>
        /// Triết khấu
        /// </summary>
        public decimal Discount { get; set; }
        /// <summary>
        /// Doanh thu
        /// </summary>
        public decimal Revenue { get; set; }
        /// <summary>
        /// Giá vốn
        /// </summary>
        public decimal CostPrice { get; set; }
        /// <summary>
        /// Lợi nhuận - tổng
        /// </summary>
        public decimal TotalProfit { get; set; }
        /// <summary>
        /// Lợi nhuận - %
        /// </summary>
        public decimal PrecentProfit { get; set; }
        public string UrlImg { get; set; }
    }
}
