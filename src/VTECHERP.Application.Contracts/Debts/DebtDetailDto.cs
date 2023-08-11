using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums;

namespace VTECHERP.Debts
{
    public class DebtDetailDto
    {
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        public string ParentCode { get; set; }

        public Guid? Id { get; set; }
        /// <summary>
        /// Id bút toán
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Ngày
        /// </summary>
        public DateTime? Date { get; set; }

        public string AudienceName { get; set; }
        public string AudiencePhone { get; set; }
        public string AudienceCode { get; set; }
        /// <summary>
        /// Tài khoản ghi nợ
        /// </summary>
        public string DebtAccountCode { get; set; }
        /// <summary>
        /// Tài khoản ghi có
        /// </summary>
        public string CreditAccountCode { get; set; }

        /// <summary>
        /// ghi nợ
        /// </summary>
        public decimal? DebtAmount { get; set; }
        /// <summary>
        /// ghi có
        /// </summary>
        public decimal? CreditAmount { get; set; }

        public decimal? AmountVnd { get; set; }
        public decimal? AmountCny { get; set; }
        public Guid? DocumentId { get; set; }
        public string DocumentCode { get; set; }
        public DocumentTypes? DocumentType { get; set; }
        public DocumentDetailType? DocumentDetailType { get; set; }
        /// <summary>
        /// Loại phiếu
        /// </summary>
        public TicketTypes TicketType { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
    }
}
