using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Reports.Param
{
    public class ReportRevenueCategoryProductParam
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public List<Guid> EnterpriseIds { get; set; } = new List<Guid>();
        public List<Guid> StoreIds { get; set; } = new List<Guid>();
        public List<Guid> CategoryIds { get; set; } = new List<Guid>();
        public List<SaleType> SaleType { get; set; } = new List<SaleType>();
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public enum SaleType
    {
        Retail = 0,
        Wholesale = 1,
        SPA = 2
    }
}
