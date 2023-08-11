using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.ViettelPost.Places
{
    public class CityViettelPostRespon : BaseResponViettelPost
    {
        public List<CityViettelPost> data { get; set; }
    }
    public class CityViettelPost
    {
        public int PROVINCE_ID { get; set; }
        public string PROVINCE_CODE { get; set; }
        public string PROVINCE_NAME { get; set; }
    }
}
