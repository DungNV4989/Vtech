using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.MultiTenancy;
using VTECHERP.Enums;

namespace VTECHERP.Entities
{
    [Table("EntryAccounts")]
    public class EntryAccount : BaseEntity<Guid>, IMultiTenant
    {
        public string Code { get; set; }

        public Guid EntryId { get; set; }
        /// <summary>
        /// Tài khoản có
        /// </summary>
        public string? CreditAccountCode { get; set; }
        /// <summary>
        /// Tài khoản nợ
        /// </summary>
        public string? DebtAccountCode { get; set; }
        public decimal? AmountVnd { get; set; }
        public decimal? AmountCny { get; set; } 
        /// <summary>
        /// Loại chứng từ
        /// </summary>
        public DocumentTypes? DocumentType { get; set; }
        /// <summary>
        /// Mã tham chiếu đến chứng từ (phiếu nhập, phiếu xuất,...)
        /// </summary>
        public string? DocumentCode { get; set; }
        public string? Note { get; set; }
    }
}
