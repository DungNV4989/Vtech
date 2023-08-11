using System;
using System.Collections.Generic;
using VTECHERP.DTOs.Base;

namespace VTECHERP.DTOs.DraftTickets
{
    public class SearchDraftTicketRequest : BasePagingRequest
    {
        public List<Guid> SourceStoreIds { get; set; }
        public List<Guid> DestinationStoreIds { get; set; }
        public string Code { get; set; }
        public string WarehousingBillCode { get; set; }
    }
}