using System;

namespace VTECHERP.DTOs.Permissions
{
    public class RoleDTO
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string CreatorName { get; set; }
        public string Description { get; set; }
    }
}
