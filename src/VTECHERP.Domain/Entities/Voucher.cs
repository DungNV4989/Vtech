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
    [Table("Vouchers")]
    public class Voucher : BaseEntity<Guid>, IMultiTenant
    {
        public string Code { get; set; }
        public Guid PromotionId { get; set; }
        public DiscountUnit DiscountUnit { get; set; }
        public decimal? DiscountValue { get; set; }
        public DiscountUnit MaxDiscountUnit { get; set; }
        public decimal? MaxDiscountValue { get; set; }
        public decimal? BillMinValue { get; set; }
        public decimal? BillMaxValue { get; set; }
        public VoucherStatus Status { get; set; }
    }
}
