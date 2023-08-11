using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.TransportInformation
{
    public class SearchHistoryTransportInformationRequest : BasePagingRequest
    {
        public DateTime? SearchDateFrom { get; set; }
        public DateTime? SearchDateTo { get; set; }
        public string? TransportInformationCode { get; set; }
        public Guid? CustomerId { get; set; }
        public string? Shipper { get; set; }
        public TransportStatus? Status { get; set; }
        public string TransportName { get; set; }
        public List<Guid> StoreIds { get; set; }
    }
}
