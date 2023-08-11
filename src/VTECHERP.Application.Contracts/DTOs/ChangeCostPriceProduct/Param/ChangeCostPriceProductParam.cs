using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.ChangeCostPriceProduct.Param
{
    public class ChangeCostPriceProductParam
    {
        public Guid ProductId { get; set; }
        public decimal PriceNew { get; set; }
        public ChangeCostPriceProductType Type { get; set; }
    }
}
