using System;

namespace VTECHERP.DTOs.DraftTickets
{
    public class SearchDraftTicketApproveRequest
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
    }
}