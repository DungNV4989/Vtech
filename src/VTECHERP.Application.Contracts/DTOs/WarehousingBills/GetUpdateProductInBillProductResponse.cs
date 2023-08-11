using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.WarehousingBills
{
    public class GetUpdateProductInBillProductResponse
    {
        public Guid WarehousingBillId { get; set; }
        public string StoreName { get; set; }
        public string ProductName { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public string Note { get; set; }
    }
}
