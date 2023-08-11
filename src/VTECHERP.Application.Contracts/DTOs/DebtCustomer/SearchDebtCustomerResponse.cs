using System;
using System.Collections.Generic;
using System.Text;
using static VTECHERP.Constants.EntryConfig.PaymentReceipt;

namespace VTECHERP.DTOs.DebtCustomer
{
    public class SearchDebtCustomerResponse
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Ngày thu chi
        /// </summary>
        public DateTime TransactionDate { get; set; }
        public string CustomerName { get; set; }
        /// <summary>
        /// Ngày nhắc nợ
        /// </summary>
        public DateTime? DebtReminderDate { get; set; } 

        /// <summary>
        /// Ngày đơn hàng gần nhất
        /// </summary>
        public DateTime? LastOrderDate { get; set; }

        /// <summary>
        /// Ngày thu nợ gần nhất
        /// </summary>
        public DateTime? LatestDebtCollectionDate { get; set; }

        /// <summary>
        /// Có (phải thu) / Đầu kỳ
        /// </summary>
        public decimal BeginCredit { get; set; }
        /// <summary>
        /// Nợ (phải trả) / Đầu kỳ
        /// </summary>
        public decimal BeginDebt { get; set; }
        /// <summary>
        /// Có (phải thu) / Giữa kỳ
        /// </summary>
        public decimal Credit { get; set; }
        /// <summary>
        /// Nợ (phải trả)  / Giữa kỳ
        /// </summary>
        public decimal Debt { get; set; }
        /// <summary>
        /// Có (phải thu) / Cuối kỳ
        /// </summary>
        public decimal EndCredit { get; set; }
        /// <summary>
        /// Nợ (phải trả) / Cuối kỳ
        /// </summary>
        public decimal EndDebt { get; set; }

        /// <summary>
        /// Giới hạn
        /// </summary>
        public decimal? DebtLimit { get; set; }
        public string CustomerPhone { get; set; }

    }
}
