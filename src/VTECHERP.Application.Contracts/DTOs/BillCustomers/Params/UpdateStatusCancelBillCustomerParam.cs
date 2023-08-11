using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums.Bills;

namespace VTECHERP.DTOs.BillCustomers.Params
{
    public class UpdateStatusCancelBillCustomerParam
    {
        public Guid BillCustomerId { get; set; }
        public string ReasonCancel { get; set; }
        public CustomerBillPayStatus Status { get; set; }
    }
}
