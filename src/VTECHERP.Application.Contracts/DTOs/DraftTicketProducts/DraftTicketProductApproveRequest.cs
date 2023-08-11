using System;

namespace VTECHERP.DTOs.DraftTicketProducts
{
    public class DraftTicketProductApproveRequest
    {
        public Guid Id { get; set; }
        public int ApproveQuantity { get; set; }
    }
}