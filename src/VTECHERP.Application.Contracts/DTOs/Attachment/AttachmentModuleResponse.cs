using System;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.Attachment
{
    public class AttachmentModuleResponse
    {
        public Guid Id { get; set; }
        public Guid? ObjectId { get; set; }
        public AttachmentObjectType? ObjectType { get; set; }
        public string ObjectTypeName { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string TypeFile { get; set; }
        public Guid? CreatorId { get; set; }
        public string CreatorName { get; set; }
        public DateTime? CreationTime { get; set; }
    }

}