using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace VTECHERP.DTOs.MasterDatas
{
    public class SearchMasterAccountRequest: SearchTextRequest
    {
        public List<string>? AccountCode { get; set; } = new();
        public List<string>? ParentAccountCode { get; set; } = new();
    }
}
