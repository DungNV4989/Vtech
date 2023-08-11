using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Attribute;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.Attachment
{
    public class UploadAttachmentRequest
    {
        public Guid? ObjectId { get; set; }
        public AttachmentObjectType ObjectType { get; set; }

        [MaxFileSize(50 * 1024 * 1024)]
        [AllowedExtensions(".xls", ".xlsx", ".doc", ".docx", ".ppt", ".pptx", ".csv", ".pdf", ".bmp", ".jpg", ".jpeg", ".png")]
        public IEnumerable<IFormFile> formFiles { get; set; }
    }
}
