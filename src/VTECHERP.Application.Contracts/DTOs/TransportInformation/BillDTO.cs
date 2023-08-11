using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.TransportInformation
{
    public class BillDTO
    {
        public Guid? BillId { get; set; }
        public string BillCode { get; set; }
        public string CustomerName { get; set; }
    }
}
