using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.Permissions
{
    public class GrantPermissionRequest
    {
        public bool IsGrant { get; set; }
        public string PermissionName { get; set; }
    }
}
