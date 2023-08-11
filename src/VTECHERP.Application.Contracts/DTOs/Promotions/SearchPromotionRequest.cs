using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums.Bills;

namespace VTECHERP.DTOs.Promotions
{
    public class SearchPromotionRequest:BasePagingRequest
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DiscountUnit? DiscountUnit { get; set; }
        public PromotionStatus? Status { get; set; }

    }
}
