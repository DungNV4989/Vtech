using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.BillCustomers.Params
{
    public class BillCustomerProductCheckValid
    {
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}
