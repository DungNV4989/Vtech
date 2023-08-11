using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System;
using System.Collections.Generic;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.Customer
{
    public class SearchCustomerRequest: BasePagingRequest
    {
        /// <summary>
        /// Tìm theo Code
        /// </summary>
        public string? ID { get; set; }
        /// <summary>
        /// Tìm kiếm theo tên khách hàng hoặc số điện thoại
        /// </summary>
        //public List<Guid>? CustomerIds { get; set; }
        public string NameOrPhone { get; set; }
        // Loại khách hàng (Select)
        public CustomerType? CustomerType { get; set; }
        // Nhân viên chăm sóc
        public string SupportEmployeeName { get; set; }
        // Nhân viên phụ trách
        public string HandlerEmployeeName { get; set; }
        // Phân loại công nợ (Select)
        public DebtGroup? DebtGroup { get; set; }
        // Cửa hàng phụ trách (select)
        public List<Guid>? HandlerStoreId { get; set; } = new List<Guid>();
        // Giới tính (Select)
        public Gender? Gender { get; set; }
        // Id Tỉnh, thành phố (Select)
        public Guid? ProvinceId { get; set; }
        // Ngày mua đầu tiên từ
        public DateTime? FirstPurchaseDateFrom { get; set; }
        // Ngày mua đầu tiên đến
        public DateTime? FirstPurchaseDateTo { get; set; }
        // Ngày mua cuối cùng từ
        public DateTime? LastPurchaseDateFrom { get; set; }
        // Ngày mua cuối cùng đến
        public DateTime? LastPurchaseDateTo { get; set; }
        // Số ngày không mua từ
        public int? NonPurchaseDaysFrom { get; set; }
        // Số ngày không mua đến
        public int? NonPurchaseDaysTo { get; set; }
        // Chu kỳ mua từ
        public int? PurchaseCycleFrom { get; set; }
        // Chu kỳ mua đến
        public int? PurchaseCycleTo { get; set; }
    }
}
