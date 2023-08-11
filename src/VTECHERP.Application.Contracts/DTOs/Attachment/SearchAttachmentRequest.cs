using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.Attachment
{
    public class SearchAttachmentRequest : BasePagingRequest
    {
        public Guid ObjectId { get; set; }
        public AttachmentObjectType ObjectType { get; set; }
        public string? Keyword { get; set; }
    }
}
