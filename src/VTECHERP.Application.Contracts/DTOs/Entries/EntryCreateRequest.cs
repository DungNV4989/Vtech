using System;
using System.Collections.Generic;
using VTECHERP.Enums;
using VTECHERP.Models;

namespace VTECHERP.DTOs.Entries
{
    public class EntryCreateRequest
    {
        /// <summary>
        /// Id cửa hàng
        /// </summary>
        public Guid? StoreId { get; set; }
        /// <summary>
        /// Ngày thu chi
        /// </summary>
        public DateTime TransactionDate { get; set; }
        /// <summary>
        /// Loại phiếu
        /// </summary>
        public TicketTypes TicketType { get; set; }
        /// <summary>
        /// Id đối tượng
        /// </summary>
        public Guid? AudienceId { get; set; }
        /// <summary>
        /// Loại Đối tượng
        /// </summary>
        public AudienceTypes AudienceType { get; set; }
        /// <summary>
        /// Chứng từ ngoài
        /// </summary>
        public string DocumentCode { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Note { get; set; }
        /// <summary>
        /// File đính kèm
        /// </summary>

        //[MaxFileSize(50 * 1024 * 1024)]
        //[AllowedExtensions(".xls", ".xlsx", ".doc", ".docx", ".ppt", ".pptx", ".csv", ".pdf", ".bmp", ".jpg", ".jpeg", ".png")]
        public IEnumerable<FileAttachment>? Attachments { get; set; }
        /// <summary>
        /// List tài khoản
        /// </summary>
        public List<EntryAccountRequest> EntryAccounts { get; set; }
    }

    public class EntryAccountRequest
    {
        /// <summary>
        /// Số tiền VN
        /// </summary>
        public decimal? AmountVnd { get; set; }
        /// <summary>
        /// Số tiền CN
        /// </summary>
        public decimal? AmountCny { get; set; }
        /// <summary>
        /// Tài khoản có
        /// </summary>
        public string? CreditAccountCode { get; set; }
        /// <summary>
        /// Tài khoản nợ
        /// </summary>
        public string? DebtAccountCode { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Note { get; set; }
    }
}

