using System.Collections.Generic;
using System.Linq;
using VTECHERP.DTOs.Permissions;

namespace VTECHERP.DTOs.UserManagement
{
    public class UserProfile
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public List<RoleDTO> Roles { get; set; }
        public List<UserStoreDTO> UserStores { get; set; }
        public UserStoreDTO DefaultStore { 
            get
            {
                return UserStores.FirstOrDefault(p => p.IsDefault);
            }
        }
    }
}
