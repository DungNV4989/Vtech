using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.Users.Dto
{
    public class UserItemList
    {
        public Guid UserId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string GroupPermission { get; set; }
        public string MainStore { get; set; }
        public Guid MainStoreId { get; set; }
        public List<string> ExtraStore { get; set; }
        public string Status { get; set; }
        public Guid PermissionIds { get; set; }
    }
}
