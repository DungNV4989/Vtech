using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.Stores
{
    public class StoreDetailToUpdate
    {
        public Guid? StoreId { get; set; }
        public string StoreName { get; set; }
        /// <summary>
        /// Id Thành phố
        /// </summary>
        public Guid ProvinceId { get; set; }

        /// <summary>
        /// Id huyện
        /// </summary>
        public Guid DistricId { get; set; }

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
        /// Thứ tự
        /// </summary>
        public uint Order { get; set; }
        public string Note { get; set; }
        public bool? IsActive { get; set; }
        public string Email { get; set; }
        public List<StoreShippingInformationDto> StoreShippingInformations { get; set; }
        /// <summary>
        /// Id Xã
        /// </summary>
        public Guid WardId { get; set; }

    }
}
