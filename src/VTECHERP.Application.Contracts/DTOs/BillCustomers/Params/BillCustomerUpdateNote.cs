using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.BillCustomers.Params
{
    public class BillCustomerUpdateNote
    {
        public Guid BillCustomerId { get; set; }
        public string Note { get; set; }
    }
}
