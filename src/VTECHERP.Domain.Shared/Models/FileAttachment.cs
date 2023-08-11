using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Models
{
    public class FileAttachment
    {
        public string Base64 { get; set; }
        public string FileName { get; set; }
        public string? ContentType { get; set; }
        public string? Extensions { get; set; }
    }
}
