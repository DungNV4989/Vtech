using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Reports
{
    public class SpGetRevenueAllTenant
    {
        public Guid TenantId { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
