using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Reports
{
    public class StoreReportDto
    {
        public decimal? amountBegin { get; set; }
        public decimal? amountBeginReceive { get; set; }
        public decimal? amountBeginPay { get; set; }
        public decimal totalReceive { get; set; }
        public decimal totalReceiveCNY { get; set; }
        public decimal totalPay { get; set; }
        public decimal totalPayCNY { get; set; }
        public decimal totalEnd { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public Guid? StoreId { get; set; }
        public Guid? TenantId { get; set; }
        public int? Lvl { get; set; }
        public string EnterpriseName { get; set; }
        public string StoreName { get; set; }
    }

    public class StoreReportGroupDto
    {
        public List<StoreReportDto> StoreReportDto { get; set; }
        public string EnterpriseName { get; set; }
        public string StoreName { get; set; }
        public Guid? StoreId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
