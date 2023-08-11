using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Models
{
    public class FileModel
    {
        public string Name { get; set; } = string.Empty;
        public string ContentType { get; set; }
        public string Extensions { get; set; }
        public byte[] Content { get; set; } = new byte[0];
    }
}
