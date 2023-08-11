using System.Collections.Generic;

namespace VTECHERP.DTOs.Permissions
{
    public class CreateRoleRequest
    {
        public string RoleName { get; set; }
        public string? Description { get; set; }

        //public List<string> Permissions { get; set; }
    }
}
