using System;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.Customer
{
    public class CustomerDTO: BaseDTO
    {
        /// <summary>
        /// Tên KH
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Mã KH
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Loại khách
        /// </summary>
        public CustomerType CustomerType { get; set; }
        public string CustomerTypeName { get; set; }
        /// <summary>
        /// Tỉnh/thành
        /// </summary>
        public Guid? ProvinceId { get; set; }
        public string? ProvinceName { get; set; }
        /// <summary>
        /// Quận/huyện
        /// </summary>
        public Guid? DistrictId { get; set; }
        public string? DistrictName { get; set; }
        /// <summary>
        /// Phường/xã
        /// </summary>
        public Guid? WardId { get; set; }
        public string? WardName { get; set; }
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
        public string GenderName { get; set; }
        /// <summary>
        /// Phân loại công nợ
        /// </summary>
        public DebtGroup? DebtGroup { get; set; }
        public string DebtGroupName { get; set; }
        /// <summary>
        /// Giới hạn công nợ
        /// </summary>
        public decimal? DebtLimit { get; set; }

        /// <summary>
        /// Nhân viên phụ trách
        /// </summary>
        public Guid? HandlerEmployeeId { get; set; }
        public Guid? HandlerEmployeeName { get; set; }
        /// <summary>
        /// Nhân viên hỗ trợ
        /// </summary>
        public Guid? SupportEmployeeId { get; set; }
        public Guid? SupportEmployeeName { get; set; }
        /// <summary>
        /// Cửa hàng phụ trách
        /// </summary>
        public Guid? HandlerStoreId { get; set; }
        public string? HandlerStoreName { get; set; }
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
        /// Ngày mua đầu tiên
        /// </summary>
        public DateTime? FirstPurchaseDate { get; set; }
        /// <summary>
        /// Ngày mua gần nhất
        /// </summary>
        public DateTime? LastPurchaseDate { get; set; }
        /// <summary>
        /// Tổng tiền mua
        /// </summary>
        public decimal? TotalPurchaseAmount { get; set; } = 0;
        /// <summary>
        /// Tổng số lần mua
        /// </summary>
        public int? NumberOfPurchaseTime { get; set; } = 0;
        /// <summary>
        /// Tổng số lượng sản phẩm mua
        /// </summary>
        public int? PurchaseQuantity { get; set; } = 0;
        /// <summary>
        /// Chu kỳ mua
        /// </summary>
        public int? PurchaseCycle { get; set; } = 0;
        /// <summary>
        /// Số ngày chưa mua
        /// </summary>
        public int? NonPurchaseDays { get; set; } = 0;
        public string AddressDetail { get => $"{ProvinceName?? String.Empty} {Address ?? String.Empty}"; }
        /// <summary>
        /// Cửa hàng phụ trách
        /// </summary>
        public string HandlerStoreIds { get; set; }
    }
}
