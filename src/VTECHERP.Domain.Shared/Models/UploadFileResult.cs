using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Models
{
    public class UploadFileResult
    {
        public string Key { get; set; } = string.Empty;
        public List<UploadFileResultDetail> Files { get; set; } = new();
    }
    public class UploadFileResultDetail
    {
        public string Name { get; set; } = string.Empty;
        public string Extensions { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Path { get; set; }
        public UploadFileResultDetail(string name, string url, string extensions, string path)
        {
            Name = name;
            Url = url;
            Path = path;
            Extensions = extensions;
        }
    }
}
