using System;
using System.Collections.Generic;

namespace VTECHERP.DTOs.PriceTableProduct.Param
{
    public class ListAllPriceTableRequest
    {
        public List<Guid> IgnoredPriceTables { get; set; } = new();
        public List<Guid> CustomerIds { get; set; } = new();
        public List<Guid> StoreIds { get; set; } = new();
        public string IdOrName { get; set; }
    }
}
