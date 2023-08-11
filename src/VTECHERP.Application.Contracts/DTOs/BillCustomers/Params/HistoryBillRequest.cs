using System;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.BillCustomers.Respons;

namespace VTECHERP.DTOs.BillCustomers.Params
{
    public class HistoryBillRequest : BasePagingRequest
    {
        public Guid CustomerId { get; set; }
        public string Code { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public BillLogType? BillLogType { get; set; }

    }
}
