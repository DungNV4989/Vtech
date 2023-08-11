using System;

namespace VTECHERP.DTOs.BO.Tenants.Responses
{
    public class StoreByTenantResponse
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Mã cửa hàng
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Tên cửa hàng
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

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
        /// Ngày hết hạn
        /// </summary>
        public DateTime? ExpriDate { get; set; }

        /// <summary>
        /// Trạng thái (1: hoạt động; 0: không hoạt động)
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Sử dụng để thay đổi trạng thái cửa hàng.
        /// </summary>
        public Guid TenantId { get; set; }

        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}