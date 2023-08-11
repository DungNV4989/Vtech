using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;

namespace VTECHERP.DTOs.ExchangeReturn
{
    public class SearchCustomerReturnRequest : BasePagingRequest
    {
        public List<Guid> StoreIds { get; set; }
        public string? BillCode { get; set; }
        public string? ProductName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? CustomerName { get; set; }
        public string? EmployeeName { get; set; }
        public string? CreatorName { get; set;}
        public string? BillCustomerCode { get; set; }
        public int? IsConfirmed { get; set; }
    }
}
