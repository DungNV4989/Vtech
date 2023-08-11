using System;
using System.Collections.Generic;

namespace VTECHERP.DTOs.Users.Params
{
    public class UpdateUserParam
    {
        /// <summary>
        /// Id người dùng
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Họ và tên
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Nhóm quyền mẫu
        /// </summary>
        public Guid? RoleId { get; set; }

        /// <summary>
        /// Cửa hàng chính
        /// </summary>
        public Guid? MainStoreId { get; set; }

        /// <summary>
        /// Trạng thái hoạt động (true: hoạt đông/ false: không hoạt động)
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Danh sách cửa hàng phụ
        /// </summary>
        public List<Guid> ExtraStoreIds { get; set; } = new List<Guid>();
    }
}
