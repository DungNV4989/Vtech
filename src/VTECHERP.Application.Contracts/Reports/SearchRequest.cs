using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;

namespace VTECHERP.Reports
{
    public class SearchRequest : BasePagingRequest
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
        public  List<Guid>? LstStoreId { get; set; }
    }
}
