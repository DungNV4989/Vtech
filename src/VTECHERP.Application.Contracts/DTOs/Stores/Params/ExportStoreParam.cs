using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace VTECHERP.DTOs.Stores.Params
{
    public class ExportStoreParam
    {
        public SearchListStoreParam Search { get; set; } = new SearchListStoreParam();
        public List<Guid> ListStoreIds { get; set; } = new List<Guid>();
    }
}
