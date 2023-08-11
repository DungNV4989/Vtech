using System;

namespace VTECHERP.DTOs.Stores
{
    public class StoreDto
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
    }
}