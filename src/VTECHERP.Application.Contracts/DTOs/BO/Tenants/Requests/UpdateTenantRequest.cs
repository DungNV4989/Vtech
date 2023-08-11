using System;
using VTECHERP.Enums.TenantExtension;

namespace VTECHERP.DTOs.BO.Tenants.Requests
{
    public class UpdateTenantRequest
    {
        public Guid Id { get; set; }

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
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// (Trạng thái: 0: hoạt động; 1: Không hoạt động)
        /// </summary>
        public Status? Status { get; set; }
    }
}