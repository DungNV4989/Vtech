using VTECHERP.Enums;

namespace VTECHERP.DTOs.MasterDatas
{
    public class SearchAudienceRequest: SearchTextRequest
    {
        public AudienceTypes? AudienceType { get; set; }
    }
}
