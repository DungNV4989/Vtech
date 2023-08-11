using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.Permissions
{
    public class PermissionModuleDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameVN { get; set; }
        /// <summary>
        /// Có thể xóa hay không (false: không thể; true: có thể)
        /// </summary>
        public bool IsDeleted { get; set; }
        public List<PermissionGroupDTO> Groups { get; set; }
    }
}
