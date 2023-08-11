using System;
using VTECHERP.Enums.TenantExtension;

namespace VTECHERP.Entities
{
    public class Agency : BaseEntity<Guid>
    {
        /// <summary>
        /// Mã tự tăng (ID)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// (Tên doanh nghiệp)
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
        public Status Status { get; set; }

        /// <summary>
        /// Id tài khoản được khởi tạo khi tạo mới
        /// </summary>
        public Guid UserId { get; set; }
       
    }
}