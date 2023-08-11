using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Domain.Shared.Helper.Excel.Attributes;

namespace VTECHERP.DTOs.DebtCustomer
{
    public class ExportDebtCustomerResponse
    {
        [Header("STT")]
        public int STT { get; set; }
        [Header("Khách Hàng")]
        public string CustomerName { get; set; }
        [Header("Số Điện Thoại")]
        public string CustomerPhone { get; set; }
        [Header("Ngày nhắc nợ")]
        public DateTime? DebtReminderDate { get; set; }
        [Header("Ngày đơn hàng gần nhất")]
        public DateTime? LastOrderDate { get; set; }
        [Header("Ngày thu nợ gần nhất")]
        public DateTime? LatestDebtCollectionDate { get; set; }
        [Header("Có (phải thu) / Đầu kỳ")]
        public decimal BeginCredit { get; set; }
        [Header("Nợ (phải trả) / Đầu kỳ")]
        public decimal BeginDebt { get; set; }
        [Header("Có (phải thu) / Giữa kỳ")]
        public decimal Credit { get; set; }
        [Header("Nợ (phải trả)  / Giữa kỳ")]
        public decimal Debt { get; set; }
        [Header("Có (phải thu) / Cuối kỳ")]
        public decimal EndCredit { get; set; }
        [Header("Nợ (phải trả) / Cuối kỳ")]
        public decimal EndDebt { get; set; }
        [Header("Giới hạn")]
        public decimal? DebtLimit { get; set; }
        
    }
}
