using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.SaleOrders
{
    public class SaleOrderDropList
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string SuplierText { get; set; }
        public string InvoiceNumber { get; set; }
    }
}
