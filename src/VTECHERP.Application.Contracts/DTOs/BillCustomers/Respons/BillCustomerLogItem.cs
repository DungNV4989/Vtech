using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.BillCustomers.Respons
{
    public class BillCustomerLogItem
    {
        public Guid Id { get; set; }
        public string CreatorText { get; set; }
        public Guid? CreatorId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
