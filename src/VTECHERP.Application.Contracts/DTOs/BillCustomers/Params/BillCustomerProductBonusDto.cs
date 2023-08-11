using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.BillCustomers.Params
{
    public class BillCustomerProductBonusDto
    {
        public Guid? BillCustomerProductId { get; set; }
        public Guid? ProductId { get; set; }
        public int Quantity { get; set; }
        public bool? IsDebt { get; set; }
        public string ProductName { get; set; } = "";
        /// <summary>
        /// Số lượng tồn
        /// </summary>
        public int Inventory { get; set; } = 0;
        public decimal Price { get; set; }
        /// <summary>
        /// Giá vốn
        /// </summary>
        public decimal? CostPrice { get; set; }
    }
}
