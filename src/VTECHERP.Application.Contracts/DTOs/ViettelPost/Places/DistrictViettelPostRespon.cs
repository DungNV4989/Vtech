using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.ViettelPost.Places
{
    public class DistrictViettelPostRespon : BaseResponViettelPost
    {
        public List<DistrictViettelPost> data { get; set; }
    }    
    public class DistrictViettelPost
    {
        public int DISTRICT_ID { get; set; }
        public string DISTRICT_VALUE { get; set; }
        public string DISTRICT_NAME { get; set; }
        public int PROVINCE_ID { get; set; }
    }
}
