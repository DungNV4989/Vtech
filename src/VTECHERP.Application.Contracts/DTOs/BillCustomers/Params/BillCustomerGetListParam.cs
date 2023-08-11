using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums.Bills;

namespace VTECHERP.DTOs.BillCustomers.Params
{
    public class BillCustomerGetListParam : BasePagingRequest
    {
        public List<Guid>? StoreIds { get; set; }
        public string BillCustomerCode { get; set; }
        public DateTime? CreateTimeFrom { get; set; }
        public DateTime? CreateTimeTo { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public string EmployeeCashier { get; set; }
        public string CouponCode { get; set; }
        public string IMei { get; set; }
        public Guid? ProductCategory { get; set; }
        public string EmployeeSell { get; set; }
        public string Description { get; set; }
        public string EmployeeTech { get; set; }
        public bool IsCheckData { get; set; } = false;
        public CustomerBillPayStatus? CustomerBillPayStatus { get; set; }
    }
}
