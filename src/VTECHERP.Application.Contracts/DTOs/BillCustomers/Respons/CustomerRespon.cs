using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.BillCustomers.Respons
{
    public class CustomerRespon
    {
        public Guid Id { get; set; }
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
        /// Đia chỉ
        /// </summary>
        public string? Address { get; set; }
        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string PhoneNumber { get; set; }
        
        /// <summary>
        /// Nhân viên phụ trách
        /// </summary>
        public Guid? HandlerEmployeeId { get; set; }
        /// <summary>
        /// Nhân viên hỗ trợ
        /// </summary>
        public Guid? SupportEmployeeId { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Note { get; set; }
        /// <summary>
        /// Giới hạn công nợ
        /// </summary>
        public decimal? DebtLimit { get; set; }
        public decimal DebtTotal { get; set; } = 0;
    }
}
