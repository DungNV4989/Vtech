using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Reports
{
    public class SpReportRevenueCustomer
    {
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Hóa đơn mua
        /// </summary>
        public decimal TotalBillCustomerBuy { get; set; }
        /// <summary>
        /// Tổng mua
        /// </summary>
        public decimal TotalBuyAmount { get; set; }
        /// <summary>
        /// Hóa đơn trả
        /// </summary>
        public decimal TotalBillCustomerReturn { get; set; }
        /// <summary>
        /// Tổng trả
        /// </summary>
        public decimal TotalReturnAmount { get; set; }
        /// <summary>
        /// Sản phẩm mua
        /// </summary>
        public decimal TotalProductBuy { get; set; }
        /// <summary>
        /// Sản phẩm trả
        /// </summary>
        public decimal TotalProductReturn { get; set; }
        /// <summary>
        /// Triết khấu
        /// </summary>
        public decimal DiscountAmount { get; set; }
        /// <summary>
        /// Doanh THu
        /// </summary>
        public decimal Revenue { get; set; }
        /// <summary>
        /// Giá vốn
        /// </summary>
        public decimal CostPrice { get; set; }
        /// <summary>
        /// Lợi nhuận
        /// </summary>
        public decimal Profit { get; set; }
        /// <summary>
        /// Ngày mua cuối 
        /// </summary>
        public DateTime LastDayBuy { get; set; }
        public Guid? TenantId { get; set; }
        /// <summary>
        /// Tên doanh nghiệp
        /// </summary>
        public string TenantName { get; set; }
    }
}
