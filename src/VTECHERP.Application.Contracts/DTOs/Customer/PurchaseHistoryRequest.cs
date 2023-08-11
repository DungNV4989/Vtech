using System;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.BillCustomers.Respons;

namespace VTECHERP.DTOs.Customer
{
    public class PurchaseHistoryRequest : BasePagingRequest
    {
        public Guid CustomerId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public BillLogType? BillLogType { get; set; }
        public string ProductName { get; set; }
    }
}
