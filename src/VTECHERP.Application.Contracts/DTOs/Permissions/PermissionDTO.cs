using System;

namespace VTECHERP.DTOs.Permissions
{
    public class PermissionDTO
    {
        public Guid Id { get; set; }
        public string ModuleCode { get; set; }
        public string GroupCode { get; set; }
        public Guid GroupId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameVN { get; set; }
        public bool IsGranted { get; set; } = false;
    }
}
