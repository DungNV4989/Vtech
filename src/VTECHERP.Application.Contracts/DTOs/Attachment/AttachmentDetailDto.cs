using System;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.Attachment
{
    public class AttachmentDetailDto
    {
        public Guid Id { get; set; }
        public Guid? ObjectId { get; set; }
        public AttachmentObjectType? ObjectType { get; set; }
        public string Name { get; set; }
        private string _Url;
        public string Url
        {
            get
            {
                return Uri.UnescapeDataString(_Url);
            }
            set
            {
                _Url = value;
            }
        }

        public string Path { get; set; }

        public string Extensions { get; set; }
    }
}