using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Domain.Shared.Helper.Excel.Attributes;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.EntryLogs
{
    public class ExportEntryLogResponse
    {
        [Header("ID")]
        public string Code { get; set; }
        [Header(" ID giao dịch")]
        public string TransactionCode { get; set; }
        [Header("Ngày giao dịch")]
        public DateTime? TransactionDate { get; set; }
        [Header("Chứng từ")]
        public string DocumentTypeName { get; set; }
        [Header("Id chứng từ")]
        public string DocumentCode { get; set; }
        [Header("Loại giao dịch")]
        public string TicketType { get; set; }
        [Header("Tổng tiền giao dịch")]
        public decimal? TotalTransactionMoney { get; set; }
        [Header("Người thao tác")]
        public string UserAction { get; set; }
        [Header("Hành động")]
        public EntityActions Action { get; set; }

    }
}
