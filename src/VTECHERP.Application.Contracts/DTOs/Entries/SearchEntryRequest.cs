using System;
using System.Collections.Generic;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.Entries
{
    public class SearchEntryRequest: BasePagingRequest
    {
        /// <summary>
        /// Search Id cửa hàng
        /// </summary>
        public List<Guid>? StoreIds { get; set; } = new();
        /// <summary>
        /// Id bút toán
        /// </summary>
        public string? EntryCode { get; set; }
        /// <summary>
        /// Kiểu
        /// </summary>
        public List<DocumentDetailType>? DocumentDetailType { get; set; }
        /// <summary>
        /// Loại ngày: Ngày tạo, ngày giao dịch
        /// </summary>
        public SearchDateType? SearchDateType { get; set; }
        public DateTime? SearchDateFrom { get; set; }
        public DateTime? SearchDateTo { get; set; }
        /// <summary>
        /// Chứng từ: Có/Không
        /// </summary>
        public bool? IsDocument { get; set; }
        /// <summary>
        /// Mã chứng từ
        /// </summary>
        public string? DocumentCode { get; set; }
        /// <summary>
        /// Loại Phiếu
        /// </summary>
        public TicketTypes? TicketType { get; set; }
        /// <summary>
        /// Loại hạch toán
        /// </summary>
        public AccountingTypes? AccountingType { get; set; }
        /// <summary>
        /// Loại đối tượng
        /// </summary>
        public AudienceTypes? AudienceType { get; set; }
        /// <summary>
        /// Id đối tượng
        /// </summary>
        public Guid? AudienceId { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Note { get; set; }
        /// <summary>
        /// Mã tài khoản
        /// </summary>
        public string? AccountCode { get; set; }
        /// <summary>
        /// Số tiền
        /// </summary>
        public decimal? Amount { get; set; }
        /// <summary>
        /// Search người tạo
        /// </summary>
        public string? Creator { get; set; }
    }
}
