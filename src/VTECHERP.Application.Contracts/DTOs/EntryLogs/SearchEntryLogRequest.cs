using System;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.EntryLogs
{
    public class SearchEntryLogRequest : BasePagingRequest
    {
        /// <summary>
        /// Id Giao dịch (Id bút toán cha)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Ngày bắt đầu ghi log
        /// </summary>
        public DateTime? StartCreated { get; set; }

        /// <summary>
        /// Ngày kết thúc ghi log
        /// </summary>
        public DateTime? EndCreated { get; set; }

        /// <summary>
        /// Ngày bắt đầu giao dịch
        /// </summary>
        public DateTime? StartTransaction { get; set; }

        /// <summary>
        /// Ngày kết thúc giao dịch
        /// </summary>
        public DateTime? EndTransaction { get; set; }

        /// <summary>
        /// Thao tác
        /// </summary>
        public EntityActions? Action { get; set; }

        /// <summary>
        /// Kiểu
        /// </summary>
        public DocumentDetailType? DocumentDetailType { get; set; }

        /// <summary>
        /// Id chứng từ
        /// </summary>
        public string DocumentCode { get; set; }

        /// <summary>
        /// Loại phiếu
        /// </summary>
        public TicketTypes? TicketType { get; set; }

        /// <summary>
        /// Loại đối tượng
        /// </summary>
        public AudienceTypes? AudienceType { get; set; }

        /// <summary>
        /// Đối tượng
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Người thao tác
        /// </summary>
        public string UserAction { get; set; }
    }
}