using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.WarehousingBills
{
    public class UpdateProductInBillProductRequest
    {
        public Guid WarehousingBillProductId { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public string Note { get; set; }
    }
}
