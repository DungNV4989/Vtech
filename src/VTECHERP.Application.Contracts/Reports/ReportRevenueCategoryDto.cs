using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.TransportInformation;

namespace VTECHERP.Reports
{
    public class ReportRevenueCategoryDto
    {
        public Guid? TenantId { get; set; }
        public string TeantName { get; set; }
        public Guid CategoryProductId { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }

        public decimal TotalSaleSL { get; set; }
        public decimal TotalSaleL { get; set; }
        public decimal TotalSaleS { get; set; }
        public decimal TotalSaleSPA { get; set; }

        public decimal TotalReturnSL { get; set; }
        public decimal TotalReturnL { get; set; }
        public decimal TotalReturnS { get; set; }
        public decimal TotalReturnSPA { get; set; }

        public decimal RevenueSL { get; set; }
        public decimal RevenueL { get; set; }
        public decimal RevenueS { get; set; }
        public decimal RevenueSPA { get; set; }
        public decimal RevenueTotal { get; set; }
        public decimal RevenueTB { get; set; }

        public decimal CostPrice { get; set; }
        public decimal Profit { get; set; }
        public decimal ProportionSL { get; set; }
        public decimal ProportionDT { get; set; }
        public decimal ProportionCostPrice { get; set; }
        public decimal ProportionProfit { get; set; }
    }
}
