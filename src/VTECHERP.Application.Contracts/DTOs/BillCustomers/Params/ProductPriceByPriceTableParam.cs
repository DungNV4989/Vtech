using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.BillCustomers.Params
{
    public class ProductPriceByPriceTableParam
    {
        public List<Guid> ProductId { get; set; }
        public Guid PriceTableId { get; set; }
    }
}
