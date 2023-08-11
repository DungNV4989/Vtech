using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.MultiTenancy;

namespace VTECHERP.Entities
{
    [Table("Permissions")]
    public class Permission: BaseEntity<Guid>
    {
        public Guid GroupId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameVN { get; set; }

        public Permission()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Sao chép permission
        /// </summary>
        /// <param name="permission">Đối tượng cần sao chép</param>
        /// <param name="groupId">Liên kết bảng permissionGroup</param>
        /// <param name="tenantId">Tenant mà đối tượng sao chép đc lưu</param>
        /// <param name="isActive">Trạng thái của đối tượng (True:Không hoạt động/False:Có hoạt động)</param>
        public Permission(Permission permission, Guid groupId, Guid tenantId, bool isActive = true)
        {
            Id = Guid.NewGuid();
            GroupId = groupId;
            Code = permission.Code;
            Name = permission.Name;
            NameVN = permission.NameVN;
            TenantId = tenantId;
            IsActive = isActive;

        }
    }
}
