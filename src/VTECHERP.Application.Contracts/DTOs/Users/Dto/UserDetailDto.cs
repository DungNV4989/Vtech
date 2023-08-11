using System;
using System.Collections.Generic;

namespace VTECHERP.DTOs.Users.Dto
{
    /// <summary>
    /// Thông tin chi tiết người dùng
    /// </summary>
    public class UserDetailDto
    {
        /// <summary>
        /// Id người dùng
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Tên người dùng
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Họ người dùng
        /// </summary>
        public string SurName { get; set; }

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string UserName  { get; set; }

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
        /// Id nhóm quyền mẫu
        /// </summary>
        public Guid? RoleId { get; set; }

        /// <summary>
        /// Tên nhóm quyền mẫu
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Id cửa hàng chính
        /// </summary>
        public Guid MainStoreId { get; set; }

        /// <summary>
        /// Tên cửa hàng chính
        /// </summary>
        public string MainStoreName { get; set; }

        /// <summary>
        /// Trạng thái hoạt động (true: hoạt động/ false: không hoạt động)
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Danh sách cửa hàng phụ
        /// </summary>
        public List<ExtraStore> ExtraStores { get; set; } = new List<ExtraStore>();

    }

    /// <summary>
    /// Thông tin cửa hàng phụ
    /// </summary>
    public class ExtraStore
    {

        /// <summary>
        /// Id cửa hàng phụ
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Tên cửa hàng phụ
        /// </summary>
        public string Name { get; set; }
    }
}
