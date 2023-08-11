using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;

namespace VTECHERP.DTOs.TransportInformation
{
    public class SearchTransportInformationBy3RDRequest : BasePagingRequest
    {
        public DateTime? SearchDateFrom { get; set; }
        public DateTime? SearchDateTo { get; set; }
        public string? TransportInformationCode { get; set; }
        public string CustomerName { get; set; }
        public List<Guid?>? FromStoreId { get; set; }
    }
}
