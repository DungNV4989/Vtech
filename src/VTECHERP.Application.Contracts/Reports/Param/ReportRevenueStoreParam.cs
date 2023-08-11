using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Reports.Param
{
    public class ReportRevenueStoreParam
    {
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// Ngày hóa đơn đến
        /// </summary>
        public DateTime? DateTo { get; set; }
        /// <summary>
        /// Id cửa hàng
        /// </summary>
        public List<Guid> StoreId { get; set; } = new List<Guid>();
        public List<Guid> TenantId { get; set; } = new List<Guid>();
        public List<Guid> CategoryIds { get; set; } = new List<Guid>();
    }
}
