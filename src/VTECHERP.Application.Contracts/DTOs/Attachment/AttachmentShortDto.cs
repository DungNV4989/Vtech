using System;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.Attachment
{
    public class AttachmentShortDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}