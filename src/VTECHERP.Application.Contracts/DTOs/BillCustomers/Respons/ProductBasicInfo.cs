using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.BillCustomers.Respons
{
    public class ProductBasicInfo
    {
        public Guid ProductId { get; set; }
      
        public decimal? SalePrice { get; set; }
        /// <summary>
        /// Giá vốn
        /// </summary>
        public decimal? CostPrice { get; set; }
        /// <summary>
        /// Giá sỉ
        /// </summary>
        public decimal? WholeSalePrice { get; set; }

        /// <summary>
        /// Giá SPA
        /// </summary>
        public decimal? SPAPrice { get; set; }

        /// <summary>
        /// Giá bán
        /// </summary>
        public decimal? SalePriceDto { get; set; }
    }
}
