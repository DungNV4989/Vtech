using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.PriceTableProduct.Param
{
    public class PriceProduductUpdateParam
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
    }
}
