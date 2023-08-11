using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;

namespace VTECHERP.DTOs.ProductByCustomerReport
{
    public class ProductByCustomerReportRequest : BasePagingRequest
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
        public string ProducName { get; set; }
        /// <summary>
        /// khách hàng
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// Danh mục
        /// </summary>
        public List<Guid>? LstCategoryId { get; set; }
        /// <summary>
        /// Thành phố 
        /// </summary>
        public Guid? City { get; set; }
        
    }
}
