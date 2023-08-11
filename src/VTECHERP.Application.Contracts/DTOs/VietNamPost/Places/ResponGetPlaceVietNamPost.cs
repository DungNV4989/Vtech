using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.VietNamPost.Places
{
    public class ResponGetPlaceVietNamPost
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; }
        public List<ProvinceVietNamPost> Provinces { get; set; }
        public List<DistrictVietNamPost> Districts { get; set; }
        public List<WardVietNamPost> Wards { get; set; }
    }
}
