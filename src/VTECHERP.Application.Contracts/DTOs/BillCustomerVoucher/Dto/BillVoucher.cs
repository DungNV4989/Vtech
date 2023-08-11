using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums.Bills;

namespace VTECHERP.DTOs.BillCustomerVoucher.Dto
{
    public class BillVoucher
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public DiscountUnit DiscountUnit { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? MaxDiscountValue { get; set; }
        public decimal? BillMinValue { get; set; }
        public decimal? BillMaxValue { get; set; }
        public bool NotApplyWithDiscount { get; set; }
        public ApplyFor? ApplyFor { get; set; }
        public Guid PromotionId { get; set; }
        public List<Guid> Products { get; set; }
    }
}
