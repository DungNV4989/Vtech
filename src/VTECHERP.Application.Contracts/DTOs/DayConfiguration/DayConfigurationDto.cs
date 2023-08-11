using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums.TenantExtension;

namespace VTECHERP.DTOs.DayConfiguration
{
    public class DayConfigurationDto 
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
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// Link website
        /// </summary>
        public string WebsiteLink { get; set; }
        /// <summary>
        /// Link ảnh
        /// </summary>
        public string PictureLink { get; set; }
        /// <summary>
        /// Gói phần mềm
        /// </summary>
        public string SoftwarePackage { get; set; }

        /// <summary>
        /// (Trạng thái)
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// Id tài khoản được khởi tạo khi tạo mới
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// số ngày cho phép nhân viên sửa bút toán
        /// </summary>
        public int DayNumbers { get; set; } = 0;
        /// <summary>
        /// số ngày cho phép nhân viên xóa bút toán
        /// </summary>
        public int NumberOfDayAllowDeleteEntry { get; set; } = 0;
        /// <summary>
        /// số ngày quá khứ nhân viên được chọn khi lập phiếu thu chi
        /// </summary>
        public int NumberOfDayAllowCreatePayRecieve { get; set; } = 0;
    }
}
