using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;

namespace VTECHERP.DTOs.Stores.Params
{
    public class SearchListStoreParam : BasePagingRequest
    {
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreationTimeFrom { get; set; }
        public DateTime? CreationTimeTo { get; set; }
    }
}
