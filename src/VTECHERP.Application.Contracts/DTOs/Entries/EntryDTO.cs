using System;
using System.Collections.Generic;
using System.Linq;
using VTECHERP.DTOs.Attachment;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;
using VTECHERP.Models;

namespace VTECHERP.DTOs.Entries
{

    public class EntryDTO: BaseDTO
    {
        public string Code { get; set; }
        public ActionSources EntrySource { get; set; }
        public Guid? StoreId { get; set; }
        /// <summary>
        /// Mã tham chiếu đến nguồn gốc tạo bút toán (đơn hàng,...)
        /// </summary>
        public Guid? SourceId { get; set; }
        public string? SourceCode { get; set; }
        public AccountingTypes AccountingType { get; set; }
        public TicketTypes TicketType { get; set; }
        /// <summary>
        /// Mã tham chiếu đến chứng từ (phiếu nhập, phiếu xuất,...)
        /// </summary>
        public string DocumentCode { get; set; }
        public DocumentTypes DocumentType { get; set; }
        public string DocumentTypeName { get; set; }
        public DocumentDetailType DocumentDetailType { get; set; }

        /// <summary>
        /// Đối tượng
        /// </summary>
        public AudienceTypes AudienceType { get; set; }
        public Guid? AudienceId { get; set; }
        public string AudienceName { get; set; }
        public string AudiencePhone { get; set; }
        public string AudienceCode { get; set; }
        public Currencies Currency { get; set; }
        /// <summary>
        /// Số tiền
        /// </summary>
        public decimal Amount { get; set; }
        public string Note { get; set; }
        /// <summary>
        /// Ngày thu chi
        /// </summary>
        public DateTime TransactionDate { get; set; }
        /// <summary>
        /// Tài khoản có (phải thu)
        /// </summary>
        public List<EntryAccountValue> Accounts { get; set; }

        /// <summary>
        /// Link sản phẩm
        /// </summary>
        public List<DetailAttachmentDto> Attachments { get; set; }
    }

    public class EntryAccountValue
    {
        public string? DebtAccountCode { get; set; }
        public string? CreditAccountCode { get; set; }
        public decimal? AmountVnd { get; set; }
        public decimal? AmountCny { get; set; }
    }
}
