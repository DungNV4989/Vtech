using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.Permissions
{
    public class PermissionGroupDTO
    {
        public Guid Id {  get; set; }
        public Guid ModuleId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameVN { get; set; }
        public List<PermissionDTO> Permissions { get; set; }
    }
}
