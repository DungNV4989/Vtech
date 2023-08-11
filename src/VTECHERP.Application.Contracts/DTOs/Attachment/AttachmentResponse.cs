using System;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.Attachment
{
    public class AttachmentResponse
    {

        public Guid? Id { get; set; }
        public Guid? ObjectId { get; set; }
        public AttachmentObjectType? ObjectType { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Path { get; set; }
        public string Extensions { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}