using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.BillCustomers.Params
{
    public class BillCustomerByIdParam
    {
        public Guid BillCustomerId { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
