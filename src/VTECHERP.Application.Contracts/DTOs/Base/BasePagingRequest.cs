using System.Text.Json.Serialization;

namespace VTECHERP.DTOs.Base
{
    public class BasePagingRequest//: BaseSearchRequest
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        [JsonIgnore]
        public int Offset { get
            {
                var index = PageIndex - 1 < 0 ? 0 : PageIndex - 1;
                return index * PageSize;
            }
        }
    }
}
