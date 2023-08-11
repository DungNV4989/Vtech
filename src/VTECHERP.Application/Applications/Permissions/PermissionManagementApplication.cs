using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.Permissions;
using VTECHERP.ServiceInterfaces;

namespace VTECHERP.Permissions
{
    [Route("api/app/permission-management/[action]")]
    //[Authorize]
    public class PermissionManagementApplication: ApplicationService
    {
        private readonly IPermissionService _permissionService;
        private readonly ITenantManager _tenantManager;

        public PermissionManagementApplication(IPermissionService permissionService, ITenantManager tenantManager)
        {
            _permissionService = permissionService;
            _tenantManager = tenantManager;
        }

        [HttpPost]
        public async Task<bool> InitPermission(Guid? tenantId)
        {
            try
            {
                await _permissionService.InitializeTenantPermissions(tenantId);
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }
        [HttpPost]
        public async Task<PagingResponse<RoleDTO>> GetAllRole(SearchRoleDTO request)
        {
            return await _permissionService.GetAllRole(request);
        }

        [HttpGet]
        public async Task<List<string>> GetCurrentUserPermission()
        {
            return await _permissionService.GetCurrentUserPermission();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleRequest request)
        {
            return await _permissionService.CreateRole(request);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRole(Guid Id)
        { 

            return await _permissionService.DeleteRole(Id);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(Guid Id,CreateRoleRequest request)
        {
            return await _permissionService.UpdateRole(Id,request);
        }
        [HttpPost]
        public async Task<IActionResult> GrantPermissionForRole(string id, List<string> request)
        {
            return await _permissionService.GrantPermissionForRole(id, request);
        }

        [HttpGet]
        public async Task<List<PermissionDTO>> GetPermissionCodes(string moduleCode, string groupCode)
        {
            var permissions = await _permissionService.GetPermissions(moduleCode, groupCode);
            return permissions;
        }

        [HttpGet]
        public async Task<List<PermissionModuleDTO>> GetUserPermission(Guid id)
        {
            var permissions = await _permissionService.GetUserPermission(id);
            return permissions;
        }

        [HttpGet]
        public async Task<List<PermissionModuleDTO>> GetAllPermissions()
        {
            var permissions = await _permissionService.GetAllPermissions();
            return permissions;
        }

        [HttpGet]
        public async Task<List<PermissionModuleDTO>> GetAllPermissionActive(Guid tenantId)
        {
            var permissions = await _permissionService.GetAllPermissionActive(tenantId);
            return permissions;
        }

        [HttpGet]
        public async Task<List<PermissionModuleDTO>> GetRolePermission(Guid id)
        {
            var permissions = await _permissionService.GetRolePermission(id);
            return permissions;
        }

        [HttpGet]
        public async Task<List<PermissionModuleDTO>> GetRolePermissionActive(Guid id)
        {
            var permissions = await _permissionService.GetRolePermissionActive(id);
            return permissions;
        }
    }
}
