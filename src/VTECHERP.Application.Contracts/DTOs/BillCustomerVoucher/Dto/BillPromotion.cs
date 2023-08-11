using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums.Bills;

namespace VTECHERP.DTOs.BillCustomerVoucher.Dto
{
    public class BillPromotion
    {
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
    }
}
