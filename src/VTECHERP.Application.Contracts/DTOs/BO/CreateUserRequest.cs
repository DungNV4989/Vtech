using System;

namespace VTECHERP.DTOs.BO
{
    public class CreateUserRequest
    {
        public Guid? TenantId { get; set; }
        public string UserName { get; set; }
        /// <summary>
        /// Email admin của tenant
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Password admin
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Họ tên
        /// </summary>
        public string Name { get; set; }
    }
}
