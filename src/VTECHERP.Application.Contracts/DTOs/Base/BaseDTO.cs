using System;
using VTECHERP.Domain.Shared.Helper.Excel.Attributes;

namespace VTECHERP.DTOs.Base
{
    public class BaseDTO
    {
        public Guid? Id { get; set; }
        public Guid? CreatorId { get; set; }
        public string CreatorName { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? LastModifierId { get; set; }
        public string LastModifierName { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsEditable { get; set; } = true;
        public bool IsDeletable { get; set; } = true;
    }
}