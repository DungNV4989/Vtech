using System;
using System.Collections.Generic;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.PaymentReceipt
{
    public class SearchPaymentReceiptRequest: BasePagingRequest
    {
        public List<Guid>? StoreIds { get; set; }
        public List<TicketTypes> TicketType { get; set; }
        public string? AccountCode { get; set; }
        public DateTime? SearchDateFrom { get; set; }
        public DateTime? SearchDateTo { get; set; }
        public bool? IsDocument { get; set; }
        public string? DocumentCode { get; set; }
        public string? AudienceName { get; set; }
        public AudienceTypes? AudienceType { get; set; }
        public Guid? AudienceId { get; set; }
        public AccountingTypes? AccountingType { get; set; }
        public DocumentDetailType? DocumentDetailType { get; set; }
        public string? Note { get; set; }
        public string? Creator { get; set; }
        public string? PaymentReceiptCode { get; set; }
    }
}
