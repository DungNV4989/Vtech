using System;
using System.Collections.Generic;

namespace VTECHERP.DTOs.PriceTableProduct.Param
{
    public class SearchPriceProductNotInPriceTableRequest
    {
        public Guid PriceTableId { get; set; }
        public string ProductCodeName { get; set; }
    }
    public class UpdatePriceProductRequest
    {
        public Guid PriceTableProductId { get; set; }
        public decimal Price { get; set; }
    }

    public class AddPriceProductRequest
    {
        public Guid PriceTableId { get; set; }
        public List<AddPriceProduct> Products { get; set; }
    }

    public class AddPriceProduct
    {
        public Guid ProductId { get; set; }
        public decimal Price { get; set; }
    }
    
    public class DeletePriceProductRequest
    {
        public List<Guid> PriceTableProductIds { get; set; }
    }

    public class DeleteMultiplePriceProductRequest
    {
        public List<Guid> ProductIds { get; set; }
    }
}
