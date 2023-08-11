using System;
using VTECHERP.Enums.TenantExtension;

namespace VTECHERP.DTOs.BO.Tenants.Responses
{
    public class ListTenantResponse
    {
        public Guid Id { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Tên doanh nghiệp/ đại lý
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Kiểu (true: doanh nghiệp/ false: đại lý)
        /// </summary>
        public bool IsEnterprise { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Id Thành Phố
        /// </summary>
        public Guid? ProvinceId { get; set; }

        /// <summary>
        /// Id quận huyện
        /// </summary>
        public Guid? DistrictId { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Cửa hàng (số lượng)
        /// </summary>
        public int StoreCount { get; set; }

        /// <summary>
        /// Ngày hết hạn
        /// </summary>
        public DateTime? Expiration { get; set; }

        /// <summary>
        /// Trạng thái (0: hoạt động/ 1: không hoạt động)
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// Ngày tại
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// Sử dụng để lấy danh sách cửa hàng.
        /// </summary>
        public Guid TenantId { get; set; }
    }
}