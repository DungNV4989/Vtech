using System;
using System.ComponentModel.DataAnnotations.Schema;
using VTECHERP.Enums;

namespace VTECHERP.Entities
{
    /// <summary>
    /// Bảng giá
    /// </summary>
    [Table("PriceTables")]
    public class PriceTable: BaseEntity<Guid>
    {
        /// <summary>
        /// Mã tự sinh
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// STT
        /// </summary>
        public string STT { get; set; }
        /// <summary>
        /// ID bảng giá cha
        /// </summary>
        public Guid? ParentPriceTableId { get; set; }
        /// <summary>
        /// ID Doanh nghiệp
        /// </summary>
        public Guid OrganizationId { get; set; }
        /// <summary>
        /// Tên bảng giá
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Ngày áp dụng từ
        /// </summary>
        public DateTime AppliedFrom { get; set; }
        /// <summary>
        /// Ngày áp dụng đến
        /// </summary>
        public DateTime? AppliedTo { get; set; }
        /// <summary>
        /// Trạng thái
        /// </summary>
        public PriceTableStatus Status { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
    }
}
