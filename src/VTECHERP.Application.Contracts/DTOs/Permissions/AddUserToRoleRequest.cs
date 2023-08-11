using System;
using System.Collections.Generic;

namespace VTECHERP.DTOs.Permissions
{
    public class AddUserToRoleRequest
    {
        public Guid UserId { get; set; }
        public List<string> Roles { get; set; }
    }
}
