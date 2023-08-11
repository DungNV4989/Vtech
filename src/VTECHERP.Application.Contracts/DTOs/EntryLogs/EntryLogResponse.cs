using System;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.EntryLogs
{
    public class EntryLogResponse
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Id giao dịch
        /// </summary>
        public string TransactionCode { get; set; }

        /// <summary>
        /// Ngày giao dịch
        /// </summary>
        public DateTime? TransactionDate { get; set; }

        /// <summary>
        /// Loại chứng từ
        /// </summary>
        public DocumentTypes DocumentType { get; set; }

        /// <summary>
        /// Chứng từ
        /// </summary>
        public string DocumentCode { get; set; }

        /// <summary>
        /// Loại giao dịch (Loại phiếu)
        /// </summary>
        public TicketTypes TicketType { get; set; }

        /// <summary>
        /// Tổng tiền giao dịch
        /// </summary>
        public decimal? TotalTransactionMoney { get; set; }

        /// <summary>
        /// Người thao tác
        /// </summary>
        public string UserAction { get; set; }

        /// <summary>
        /// Hành động
        /// </summary>
        public EntityActions Action { get; set; }

        /// <summary>
        /// Mã đối tượng
        /// </summary>
        public string AudienceCode { get; set; }

        /// <summary>
        /// Tên đối tượng
        /// </summary>
        public string AudienceName { get; set; }
    }
}