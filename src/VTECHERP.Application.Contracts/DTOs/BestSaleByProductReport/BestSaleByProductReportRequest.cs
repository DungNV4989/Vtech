using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.BestSaleByProductReport
{
    public class BestSaleByProductReportRequest : BasePagingRequest
    {
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
        public List<Guid>? LstStoreId { get; set; }
        /// <summary>
        /// Sản  phẩm
        /// </summary>
        public string ProductName { get; set; }
        public DocumentDetailType? DocumentDetailType { get; set; }
    }
}
