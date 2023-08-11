using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.TransportInformation
{
    public class SearchTransportInformationRequest : BasePagingRequest
    {
        public DateTime? SearchDateFrom { get; set; }
        public DateTime? SearchDateTo { get; set; }
        public string? TransportInformationCode { get; set; }
        public string CustomerName { get; set; }
        public string  TransportName { get; set; }
        public string  PhoneNumber { get; set; }
        public string? Shipper { get; set; }
        public TransportStatus? Status { get; set; }
        public Guid? FromStoreId { get; set; }
    }
}
