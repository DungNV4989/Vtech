using System;
using Volo.Abp.MultiTenancy;

namespace VTECHERP.Entities
{
    /// <summary>
    /// Cửa hàng
    /// </summary>
    public class Stores : BaseEntity<Guid>, IMultiTenant
    {
        /// <summary>
        /// ID Doanh nghiệp
        /// </summary>
        public Guid OrganizationId { get; set; }
        /// <summary>
        /// Mã cửa hàng
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Tên cửa hàng
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Id Thành phố
        /// </summary>
        public Guid ProvinceId { get; set; }

        /// <summary>
        /// Id huyện
        /// </summary>
        public Guid DistricId { get; set; }

        /// <summary>
        /// Id Xã
        /// </summary>
        public Guid WardId { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Ngày hết hạn
        /// </summary>
        public DateTime? ExpriDate { get; set; }
        /// <summary>
        /// Là cửa hàng chính của doanh nghiệp
        /// </summary>
        public bool IsMainStore { get; set; } = false;
        /// <summary>
        /// Thứ tự
        /// </summary>
        public uint Order { get; set; }
        public string Note { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// Đã được duyệt chưa ?
        /// </summary>
        public bool? IsApprove { get; set; }
        public string? GHTKApiToken { get; set; }
        public string? VTECHApiToken { get; set; }

    }
}