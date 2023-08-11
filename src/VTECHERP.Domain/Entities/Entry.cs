using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.MultiTenancy;
using VTECHERP.Enums;

namespace VTECHERP.Entities
{
    /// <summary>
    /// Bút toán
    /// </summary>
    [Table("Entries")]
    public class Entry : BaseEntity<Guid>, IMultiTenant
    {
        public string Code { get; set; }
        public Guid StoreId { get; set; }
        public ActionSources EntrySource { get; set; }
        /// <summary>
        /// Mã tham chiếu đến nguồn gốc tạo bút toán (đơn hàng,...)
        /// </summary>
        public Guid? SourceId { get; set; }
        public string? SourceCode { get; set; }
        /// <summary>
        /// tạo thủ công hoặc tự động
        /// </summary>
        public AccountingTypes AccountingType { get; set; }
        public TicketTypes TicketType { get; set; }
        /// <summary>
        /// Mã tham chiếu đến chứng từ ngoài
        /// </summary>
        public string DocumentCode { get; set; }
        /// <summary>
        /// Id chứng từ
        /// </summary>
        public Guid? DocumentId { get; set; }
        /// <summary>
        /// Loại tham chiếu chi tiết
        /// nguồn tạo đơn hàng và nguồn hoàn thành đơn hàng thì là ID đơn hàng
        /// nguồn xác nhận đơn hàng là ID phiếu nhập kho
        /// </summary>
        public DocumentTypes DocumentType { get; set; }
        /// <summary>
        /// Đối tượng
        /// </summary>
        public AudienceTypes AudienceType { get; set; }
        /// <summary>
        /// Id tham chiếu Đối tượng
        /// </summary>
        public Guid? AudienceId { get; set; }
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
        public string? Attachments { get; set; }
        public DocumentDetailType? DocumentDetailType { get; set; }
        public string ConfigCode { get; set; }
    }
}
