using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.Users.Params
{
    public class CreateUserParam
    {
        /// <summary>
        /// Tên người dùng
        /// </summary>
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Cửa hàng chính
        /// </summary>
        public Guid MainStoreId { get; set; }
        /// <summary>
        /// Ds cửa hàng phụ
        /// </summary>
        public List<Guid> ExtraStoreId { get; set; } = new List<Guid>();
        /// <summary>
        /// Id nhóm quyền
        /// </summary>
        public Guid? RoleId { get; set; }
        /// <summary>
        /// Trạng thái: true => hoạt động, false : ko hoạt động
        /// </summary>
        public bool IsActive { get; set; }
    }
}
