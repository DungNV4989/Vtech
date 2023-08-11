using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.ViettelPost
{
    public class PostOfficeRespon
    {
        public bool error { get; set; }
        public string message { get; set; }
        public List<PostOfficeViettelPostDTO> data { get; set; }
    }
    public class PostOfficeViettelPostDTO
    {
        public string TEN_TINH { get; set; }
        public string TEN_QUANHUYEN { get; set; }
        public string TEN_PHUONGXA { get; set; }
        public string MA_BUUCUC { get; set; }
        public string TEN_BUUCUC { get; set; }
        public string DIA_CHI { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public string DIEN_THOAI { get; set; }
        public string PHUTRACH { get; set; }
        public string PHUTRACHPHONE { get; set; }
    }
}
