using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums.Bills;

namespace VTECHERP.DTOs.Promotions
{
    public class SearchProductInPromotionRequest: BasePagingRequest
    {
        public string SequenceId { get; set; }
        public string Name { get; set; }
    }
}
