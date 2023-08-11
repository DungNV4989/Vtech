using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;

namespace VTECHERP.DTOs.Users.Params
{
    public class UsersSearchParam : BasePagingRequest
    {
        /// <summary>
        /// Id cửa hàng chính
        /// </summary>
        public Guid? MainStoreId { get; set; }
        /// <summary>
        /// Danh sách id cửa hàng phụ
        /// </summary>
        public List<Guid> ExtraStoreId { get; set; } = new List<Guid>();
        /// <summary>
        /// Mã người dùng
        /// </summary>
        public string UserCode { get; set; }
        /// <summary>
        /// Tên người dùng
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Tên tài khoản
        /// </summary>
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// Trạng thái: true: hoạt động, false : ko hoạt động
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        /// Id nhóm quyền
        /// </summary>
        public Guid? GroupPermission { get; set; }
    }
}
