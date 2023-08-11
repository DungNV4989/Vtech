using System;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.Attachment
{
    public class DetailAttachmentDto
    {
        public Guid Id { get; set; }
        public Guid? ObjectId { get; set; }
        public AttachmentObjectType? ObjectType { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string TypeFile { get; set; }
        public DateTime? CreationTime { get; set; }
    }
    public class TypeFile
    {
        private TypeFile() { }

        public const string Pdf = "application/pdf";
        public const string Excel = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public const string Word = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
    }

}