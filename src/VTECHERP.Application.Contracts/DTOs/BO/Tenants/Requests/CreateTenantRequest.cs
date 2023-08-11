using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System;
using System.Collections.Generic;
using VTECHERP.Enums.TenantExtension;

namespace VTECHERP.DTOs.BO.Tenants.Requests
{
    public class CreateTenantRequest
    {
        /// <summary>
        /// (true: doanh nghiệp/false: đại lý)
        /// </summary>
        public bool IsEnterprise { get; set; } = true;

        /// <summary>
        /// (Tên doanh nghiệp/Đại lý)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// (Số điện thoại)
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// (Mail)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Id (Thành phố)
        /// </summary>
        public Guid? ProvinceId { get; set; }

        /// <summary>
        /// Id (Quận huyện)
        /// </summary>
        public Guid? DistrictId { get; set; }

        /// <summary>
        /// (Địa chỉ)
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// (Ngày hết hạn)
        /// </summary>
        public DateTime? Expiration { get; set; }

        /// <summary>
        /// (Doanh nghiệp liên kết)
        /// </summary>
        public Guid? EnterpriseId { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// (Trạng thái: 0: hoạt động; 1: Không hoạt động)
        /// </summary>
        public Status? Status { get; set; } = Enums.TenantExtension.Status.InActive;

        /// <summary>
        /// (Tổ chức)
        /// </summary>
        public string Institute { get; set; }

        /// <summary>
        /// (Tên đăng nhập)
        /// </summary>
        public string UserName { get; set; }
        
        /// <summary>
        /// Mật khẩu
        /// </summary>
        public string Password { get; set; }

        public List<Guid> Ids { get; set; } = new List<Guid>();
    }
}