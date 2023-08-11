using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Domain.Shared.Helper.Excel.Attributes;

namespace VTECHERP.DTOs.PaymentReceipt
{
    public class ExportPaymentReceiptResponse
    {
        [Header("ID")]
        public string Code { get; set; }
        [Header("Ngày")]
        public string TransactionDate { get; set; }
        [Header("Mã tài khoản")]
        public string AccountCode { get; set; }
        [Header("Tên tài khoản")]
        public string AccountName { get; set; }
        [Header("Mã tài khoản đối ứng")]
        public string ReciprocalAccountCode { get; set; }
        [Header("Tên tài khoản đối ứng")]
        public string ReciprocalAccountName { get; set; }
        [Header("Loại phiếu")]
        public string? TicketType { get; set; }
        [Header("Đối tượng")]
        public string Audience { get; set; }
        [Header("Loại chứng từ")]
        public string DocumentTypeName { get; set; }
        [Header("ID chứng từ")]
        public string? DocumentType { get; set; }
        [Header("Thu")]
        public decimal RecieveAmount { get; set; }
        [Header("Chi")]
        public decimal PaymentAmount { get; set; }
        [Header("Ghi chú")]
        public string? Note { get; set; }
    }
}
