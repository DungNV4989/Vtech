using System;

namespace VTECHERP.DTOs.PriceTableProduct.Respon
{
    public class PriceTableProductPriceDetail
    {
        public Guid? PriceTableId { get; set; }
        public Guid? PriceTableProductId { get; set; }
        public Guid ProductId { get; set; }
        public decimal Price { get; set; }
    }
}
