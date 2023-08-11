using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.ViettelPost.Places
{
    public class WardViettelPostRespon : BaseResponViettelPost
    {
        public List<WardViettelPost> data { get; set; }
    }
    public class WardViettelPost
    {
        public int WARDS_ID { get; set; }
        public string WARDS_NAME { get; set; }
        public int DISTRICT_ID { get; set; }
    }
}
