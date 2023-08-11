using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.BillCustomers.Respons
{
    public class BillCustomerLogDetail
    {
        public string BillCustomerCode { get; set; }
        public string CreatorText { get; set; }
        public Guid? CreatorId { get; set; }
        public DateTime CreatTime { get; set; }
        public BillCustomerLogJson FromValue { get; set; }
        public BillCustomerLogJson ToValue { get; set; }
    }
}
