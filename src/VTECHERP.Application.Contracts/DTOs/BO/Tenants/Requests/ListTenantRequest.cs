using System;
using System.Collections.Generic;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums.TenantExtension;

namespace VTECHERP.DTOs.BO.Tenants.Requests
{
    public class ListTenantRequest : BasePagingRequest
    {
        /// <summary>
        /// Từ ngày
        /// </summary>
        public DateTime? From { get; set; }

        /// <summary>
        /// Đến ngày
        /// </summary>
        public DateTime? To { get; set; }
        /// <summary>
        /// ID doanh nghiệp/đại lý
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Tên doanh nghiệp/đại lý
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Kiểu (true: doanh nghiệp/ false: đại lý)
        /// </summary>
        public bool? IsEnterprise { get; set; }
        /// <summary>
        /// Trạng thái (0: Hoạt động/ 1: Không hoạt động)
        /// </summary>
        public Status? Status { get; set; }

        public List<Guid> Ids { get; set; }
    }
}