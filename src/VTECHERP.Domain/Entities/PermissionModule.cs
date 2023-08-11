using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.MultiTenancy;

namespace VTECHERP.Entities
{
    [Table("PermissionModules")]
    public class PermissionModule : BaseEntity<Guid>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameVN { get; set; }

        public PermissionModule()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Sao chép permissionModule
        /// </summary>
        /// <param name="permissionModule">đối tượng cần sao chép</param>
        /// <param name="tenantId">Tenant mà đối tượng sao chép đc lưu</param>
        /// <param name="isActive">Trạng thái của đối tượng (True:Không hoạt động/False:Có hoạt động)</param>
        public PermissionModule(PermissionModule permissionModule, Guid tenantId, bool isActive = true)
        {
            Id = Guid.NewGuid();
            Code = permissionModule.Code;
            Name = permissionModule.Name;
            NameVN = permissionModule.NameVN;
            TenantId = tenantId;
            IsActive = isActive;
        }
    }
}
