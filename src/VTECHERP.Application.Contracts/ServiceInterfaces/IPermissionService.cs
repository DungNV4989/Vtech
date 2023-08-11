using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.Permissions;
using static Volo.Abp.Identity.IdentityPermissions;

namespace VTECHERP.ServiceInterfaces
{
    public interface IPermissionService: IScopedDependency
    {
        Task InitializeTenantPermissions(Guid? tenantId);
        Task<PagingResponse<RoleDTO>> GetAllRole(SearchRoleDTO request);
        Task<IActionResult> CreateRole(CreateRoleRequest request);
        Task<IActionResult> UpdateRole(Guid Id,CreateRoleRequest request);
        Task<IActionResult> DeleteRole(Guid Id);
        Task<IActionResult> GrantPermissionForRole(string id, List<string> request);
        Task<List<PermissionDTO>> GetPermissions(string moduleCode, string groupCode);
        Task<List<string>> GetCurrentUserPermission();
        Task<List<PermissionModuleDTO>> GetUserPermission(Guid id);
        Task<List<PermissionModuleDTO>> GetAllPermissions();
        Task<List<PermissionModuleDTO>> GetAllPermissionActive(Guid tenantId);
        Task<List<PermissionModuleDTO>> GetRolePermission(Guid id);
        Task<List<PermissionModuleDTO>> GetRolePermissionActive(Guid id);
    }
}
