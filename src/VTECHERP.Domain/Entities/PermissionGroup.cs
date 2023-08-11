using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.MultiTenancy;

namespace VTECHERP.Entities
{
    [Table("PermissionGroups")]
    public class PermissionGroup : BaseEntity<Guid>
    {
        public Guid ModuleId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameVN { get; set; }

        public PermissionGroup()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Sao chép permissionGroup
        /// </summary>
        /// <param name="permissionGroup">Đối tượng cần sao chép</param>
        /// <param name="moduleId">Liên kết bảng PermissionModule</param>
        /// <param name="tenantId">Tenant mà đối tượng sao chép đc lưu</param>
        /// <param name="isActive">Trạng thái của đối tượng (True:Không hoạt động/False:Có hoạt động)</param>
        public PermissionGroup(PermissionGroup permissionGroup,Guid moduleId, Guid tenantId, bool isActive = true)
        {
            Id= Guid.NewGuid();
            ModuleId = moduleId;
            Code = permissionGroup.Code;
            Name = permissionGroup.Name;
            NameVN = permissionGroup.NameVN;
            TenantId = tenantId;
            IsActive = isActive;
        }
    }
}
