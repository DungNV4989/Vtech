using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;

namespace VTECHERP.DTOs.Permissions
{
    public class SearchRoleDTO : BasePagingRequest
    {
        public string RoleName { get; set; }
    }
}
