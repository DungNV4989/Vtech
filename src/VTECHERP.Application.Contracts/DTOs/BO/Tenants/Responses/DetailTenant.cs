using Microsoft.AspNetCore.Routing.Internal;
using System;
using VTECHERP.Enums.TenantExtension;

namespace VTECHERP.DTOs.BO.Tenants.Responses
{
    public class DetailTenant
    {
        public Guid Id { get; set; }

        /// <summary>
        /// (true: doanh nghiệp/false: đại lý)
        /// </summary>
        public bool IsEnterprise { get; set; }

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
        /// (Trạng thái)
        /// </summary>
        public Status? Status { get; set; }

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

        public Guid TenantId { get; set; }
    }
}