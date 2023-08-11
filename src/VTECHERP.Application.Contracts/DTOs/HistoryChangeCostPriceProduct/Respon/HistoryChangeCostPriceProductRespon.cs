using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.HistoryChangeCostPriceProduct.Respon
{
    public class HistoryChangeCostPriceProductRespon
    {
        public DateTime CreatTime { get; set; }
        public decimal CostPriceNew { get; set; }
        public decimal ProfitDecrease { get; set; }
        public Guid? CreatorId { get; set; }
        public string CreatorText { get; set; }
        public string TypeText { get; set; }
        public ChangeCostPriceProductType? Type { get; set; }
    }
}
