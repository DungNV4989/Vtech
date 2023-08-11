using VTECHERP.Enums;

namespace VTECHERP.DTOs.MasterDatas
{
    public class SearchDocumentDetailTypeRequest
    {
        public WarehousingBillType? WarehousingBillType { get; set; }
        public AudienceTypes? AudienceType { get; set; }
        public TicketTypes? TicketType { get; set; }
        public DocumentTypes? DocumentType { get; set; }
        public bool IsWarehousingBillForm { get; set; }
    }
}
