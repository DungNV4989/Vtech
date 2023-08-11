using System;

namespace VTECHERP.DTOs.UserManagement
{
    public class UserStoreDTO
    {
        public Guid StoreId { get; set; }
        public string StoreName { get; set; }
        public bool IsDefault { get; set; }
    }
}
