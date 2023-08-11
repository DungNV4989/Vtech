using System;
using VTECHERP.Enums.Product;

namespace VTECHERP.DTOs.Base
{
    public class ProductMasterDataDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string BarCode { get; set; }
        public string Unit { get; set; }
        public string UnitName { get; set; }
        public decimal StockPrice { get; set; }
        public string Value
        {
            get
            {
                return $"{Name}";
            }
        }
        public int StockQuantity { get; set; } = 0;
    }
}