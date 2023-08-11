using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.ViettelPost
{
    public class BaseResponViettelPost
    {
        public int status { get; set; }
        public bool error { get; set; }
        public string message { get; set; }
    }
}
