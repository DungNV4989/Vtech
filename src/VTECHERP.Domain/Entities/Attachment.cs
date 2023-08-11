using System;
using Volo.Abp.MultiTenancy;
using VTECHERP.Enums;

namespace VTECHERP.Entities
{
    public class Attachment : BaseEntity<Guid>, IMultiTenant
    {
        public Guid? ObjectId { get; set; }
        public AttachmentObjectType? ObjectType { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Path { get; set; }    
        public string Extensions { get; set; }
    }
}
