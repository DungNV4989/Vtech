using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums.Bills;

namespace VTECHERP.DTOs.Promotions
{
    public class VoucherDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public Guid PromotionId { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? MaxDiscountValue { get; set; }
        public VoucherStatus Status { get; set; }

    }
}
