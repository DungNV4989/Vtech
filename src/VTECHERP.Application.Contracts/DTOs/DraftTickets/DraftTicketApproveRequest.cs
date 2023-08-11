using System.Collections.Generic;
using VTECHERP.DTOs.DraftTicketProducts;

namespace VTECHERP.DTOs.DraftTickets
{
    public class DraftTicketApproveRequest
    {
        public List<DraftTicketProductApproveRequest> DraftTicketProducts { get; set; }
        public string Note { get; set; }
    }
}