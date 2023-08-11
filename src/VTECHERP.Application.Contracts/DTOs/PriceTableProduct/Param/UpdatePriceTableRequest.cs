using System;

namespace VTECHERP.DTOs.PriceTableProduct.Param
{
    public class UpdatePriceTableRequest: CreatePriceTableRequest
    {
        public Guid Id { get; set; }
    }
}
