using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.PriceTableProduct.Respon
{
    public class PriceTableProductImportRespon
    {
        public Guid TablePriceId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public bool Status { get; set; } = true;
        public string Message { get; set; }
    }
}
