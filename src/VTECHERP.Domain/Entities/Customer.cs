using System;
using System.ComponentModel.DataAnnotations.Schema;
using VTECHERP.Enums;

namespace VTECHERP.Entities
{
    [Table("Customers")]
    public class Customer: BaseEntity<Guid>
    {
        /// <summary>
        /// Mã KH
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Tên KH
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Loại khách
        /// </summary>
        public CustomerType CustomerType { get; set; }
        /// <summary>
        /// Tỉnh/thành
        /// </summary>
        public Guid? ProvinceId { get; set; } 
        /// <summary>
        /// Quận/huyện
        /// </summary>
        public Guid? DistrictId { get; set; } 
        /// <summary>
        /// Phường/xã
        /// </summary>
        public Guid? WardId { get; set; } 
        /// <summary>
        /// Đia chỉ
        /// </summary>
        public string? Address { get; set; } 
        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string PhoneNumber { get; set; } 
        /// <summary>
        /// Ngày sinh
        /// </summary>
        public DateTime? DateOfBirth { get; set; } 
        /// <summary>
        /// Giới tính
        /// </summary>
        public Gender? Gender { get; set; }
        /// <summary>
        /// Phân loại công nợ
        /// </summary>
        public DebtGroup? DebtGroup { get; set; }
        /// <summary>
        /// Giới hạn công nợ
        /// </summary>
        public decimal? DebtLimit { get; set; }
        /// <summary>
        /// Điểm thưởng
        /// </summary>
        public decimal? BonusPoint { get; set; }
        /// <summary>
        /// Ngày mua cuối cùng
        /// </summary>
        public DateTime? LastPurchaseDate { get; set; }
        /// <summary>
        /// Nhân viên phụ trách
        /// </summary>
        public Guid? HandlerEmployeeId { get; set; }
        /// <summary>
        /// Nhân viên hỗ trợ
        /// </summary>
        public Guid? SupportEmployeeId { get; set; }
        /// <summary>
        /// Cửa hàng phụ trách
        /// </summary>
        public Guid? HandlerStoreId { get; set; }
        /// <summary>
        /// Số Zalo
        /// </summary>
        public string? Zalo { get; set; }
        /// <summary>
        /// Link Facebook
        /// </summary>
        public string? Facebook { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Note { get; set; }
        /// <summary>
        /// Danh sách cửa hàng phụ trách
        /// </summary>
        public string HandlerStoreIds { get; set; }
        public string? Email { get; set; }
        public string? HandlerEmpName { get; set; }
    }
}
