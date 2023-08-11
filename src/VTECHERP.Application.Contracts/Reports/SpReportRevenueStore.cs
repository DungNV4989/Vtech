using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Reports
{
    public class SpReportRevenueStore
    {
        /// <summary>
        /// Id cửa hàng
        /// </summary>
        public Guid StoreId { get; set; }
        /// <summary>
        /// Id doanh nghiệp
        /// </summary>
        public Guid TenantId { get; set; }
        /// <summary>
        /// Tên cửa hàng
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// Tên doanh nghiệp
        /// </summary>
        public string EnterpiseName { get; set; }
        /// <summary>
        /// Doanh số khách lẻ
        /// </summary>
        public decimal RetailSales { get; set; }
        /// <summary>
        /// Doanh thu khách lẻ
        /// </summary>
        public decimal RetailRevenue { get; set; }
        /// <summary>
        /// Giá vốn khách lẻ
        /// </summary>
        public decimal RetailCostPrice { get; set; }
        /// <summary>
        /// Lợi nhuận khách lẻ
        /// </summary>
        public decimal RetailProfit{ get; set; }
        /// <summary>
        /// Doanh số bán sỉ
        /// </summary>
        public decimal AgencySales { get; set; }
        /// <summary>
        /// Doanh thu bán sỉ
        /// </summary>
        public decimal AgencyRevenue { get; set; }
        /// <summary>
        /// Giá vốn bán sỉ
        /// </summary>
        public decimal AgencyCostPrice { get; set; }
        /// <summary>
        /// Lợi nhuận khách sỉ
        /// </summary>
        public decimal AgencyProfit { get; set; }
        /// <summary>
        /// Doanh số bán spa
        /// </summary>
        public decimal SpaSales { get; set; }
        /// <summary>
        /// Doanh thu bán spa
        /// </summary>
        public decimal SpaRevenue { get; set; }
        /// <summary>
        /// Giá vốn bán spa
        /// </summary>
        public decimal SpaCostPrice { get; set; }
        /// <summary>
        /// Lợi nhuận spa
        /// </summary>
        public decimal SpaProfit { get; set; }
        /// <summary>
        /// Tổng doanh số
        /// </summary>
        public decimal TotalSales { get; set; }
        /// <summary>
        /// Tổng doanh thu
        /// </summary>
        public decimal TotalRevenue { get; set; }
        /// <summary>
        /// Tổng giá vốn
        /// </summary>
        public decimal TotalCostPrice { get; set; }
        /// <summary>
        /// Tổng lợi nhuận
        /// </summary>
        public decimal TotalProfit { get; set; }
    }
}
