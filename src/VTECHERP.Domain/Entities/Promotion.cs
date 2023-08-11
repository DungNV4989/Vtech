using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using VTECHERP.Enums.Bills;

namespace VTECHERP.Entities
{
    [Table("Promotions")]
    public class Promotion : BaseEntity<Guid>, IMultiTenant
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string? ApplyStoreIds { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal? BillMinValue { get; set; }
        public decimal? BillMaxValue { get; set; }
        public ApplyFor? ApplyFor { get; set; }
        public string? ApplyProductCategoryIds { get; set; }
        public string? ApplyProductIds { get; set; }
        public PromotionStatus Status { get; set; }
        public bool NotApplyWithDiscount { get; set; }
        public string? Prefix { get; set; }
        public string? Suffix { get; set; }
        public int GenCodeNum { get; set; }
        public int VoucherNum { get; set; }
        public DiscountUnit DiscountUnit { get; set; }
        public decimal? DiscountValue { get; set; }
        public DiscountUnit MaxDiscountUnit { get; set; }
        public decimal? MaxDiscountValue { get; set; }
        public string? Note { get; set; }
        public string CreatorName { get; set; }
        public string LastModifierName { get; set; }

    }
}
