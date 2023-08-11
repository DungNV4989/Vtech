using System;
using System.Collections.Generic;

namespace VTECHERP.DTOs.MasterDatas
{
    public class SearchProductMasterDataRequest: SearchTextRequest
    {
        public bool IsSearchByIMEI { get; set; }
        public Guid? StoreId { get; set; }
        public List<Guid>? ProductIds { get; set; }
    }
}
