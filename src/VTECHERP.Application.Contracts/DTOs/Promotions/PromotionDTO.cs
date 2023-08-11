using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums.Bills;

namespace VTECHERP.DTOs.Promotions
{
    public class PromotionDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DiscountUnit DiscountUnit { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? MaxDiscountValue { get; set; }
        public Guid? CreatorId { get; set; }
        public string CreatorName { get; set; }
        public PromotionStatus Status { get; set; }
        public int VoucherUsed { get; set; }
        public int VoucherNum { get; set; }
    }
}
