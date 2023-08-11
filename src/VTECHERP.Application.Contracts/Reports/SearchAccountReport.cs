using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Reports
{
    public class SearchAccountReport
    {
        /// <summary>
        /// từ ngày
        /// </summary>
        public DateTime? DateFrom { get; set; }
        /// <summary>
        /// đến ngày
        /// </summary>
        public DateTime? DateTo { get; set; }
        /// <summary>
        /// danh sách Id doanh nghiệp
        /// </summary>
        public List<Guid>? LstEnterpriseId { get; set; }
        /// <summary>
        /// danh sách Id cửa hàng 
        /// </summary>
        public string Code { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}
