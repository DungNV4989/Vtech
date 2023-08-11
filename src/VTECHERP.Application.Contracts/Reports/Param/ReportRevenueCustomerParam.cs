using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums;

namespace VTECHERP.Reports.Param
{
    public class ReportRevenueCustomerParam
    {
        /// <summary>
        /// Ngày hóa đơn từ
        /// </summary>
        public DateTime? BillCustomerDateFrom { get; set; }

        /// <summary>
        /// Ngày hóa đơn đến
        /// </summary>
        public DateTime? BillCustomerDateTo { get; set; }
        /// <summary>
        /// Id cửa hàng
        /// </summary>
        public List<Guid> StoreId { get; set; } = new List<Guid>();
        /// <summary>
        /// Loại khách hàng
        /// </summary>
        public CustomerType? CustomerType { get; set; }
        /// <summary>
        /// Id khách hàng
        /// </summary>
        public Guid? CustomerId { get; set; }
        /// <summary>
        /// Id tỉnh/thành phố
        /// </summary>
        public Guid? ProvinceId { get; set; }
        /// <summary>
        /// Id nhân viên phụ trách
        /// </summary>
        public Guid? HandleEmployee { get; set; }
        /// <summary>
        /// Id doanh nghiệp
        /// </summary>
        public List<Guid> TenantId { get; set; } = new List<Guid>();
    }
}
