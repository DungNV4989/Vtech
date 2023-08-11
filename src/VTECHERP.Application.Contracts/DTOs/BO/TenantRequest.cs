using VTECHERP.DTOs.Stores;

namespace VTECHERP.DTOs.BO
{
    public class CreateTenantRequest
    {
        /// <summary>
        /// Tên Tenant
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string AdminUserName { get; set; }
        /// <summary>
        /// Email admin của tenant
        /// </summary>
        public string AdminEmail { get; set; }
        /// <summary>
        /// Password admin
        /// </summary>
        public string AdminPassword { get; set; }
        /// <summary>
        /// Họ tên
        /// </summary>
        public string AdminName { get; set; }
        /// <summary>
        /// Tenant được tạo của Vtech
        /// </summary>
        public bool IsVtech { get; set; }
        public StoreDto Store { get; set; }
    }
    public class CreatedUserRequest
    {
        /// <summary>
        /// Họ tên
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Email admin của tenant
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Password admin
        /// </summary>
        public string Password { get; set; }
    }
}
