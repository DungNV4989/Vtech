using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using VTECHERP.DTOs.BO;
using VTECHERP.DTOs.Permissions;
using VTECHERP.DTOs.UserManagement;
using VTECHERP.Entities;

namespace VTECHERP
{
    [Route("api/bo/user-management/[action]")]
    [AllowAnonymous]
    public class UserManagementApplication : ApplicationService
    {
        private readonly IdentityUserManager _userManager;
        private readonly IRepository<IdentityRole> _roleRepository;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly IRepository<Stores> _storeRepository;
        public UserManagementApplication(
            IdentityUserManager userManager,
            IRepository<IdentityRole> roleRepository,
            IRepository<UserStore> userStoreRepository,
            IRepository<Stores> storeRepository
            )
        {
            _userManager = userManager;
            _roleRepository = roleRepository;
            _userStoreRepository = userStoreRepository;
            _storeRepository = storeRepository;
        }

        [Authorize]
        [HttpGet]
        public async Task<UserProfile> UserProfile()
        {
            var userRoles = (await _roleRepository
                .GetListAsync(p => CurrentUser.Roles.Contains(p.Name)))
                .Select(p => new RoleDTO
                {
                    RoleId = p.Id,
                    RoleName = p.Name,
                }).ToList();
            var userStores = (from userStore in await _userStoreRepository.GetQueryableAsync()
                              join store in await _storeRepository.GetQueryableAsync() on userStore.StoreId equals store.Id
                              where userStore.UserId == CurrentUser.Id
                              select new UserStoreDTO
                              {
                                  StoreId = store.Id,
                                  StoreName = store.Name,
                                  IsDefault = userStore.IsDefault
                              }).ToList();
            return new UserProfile
            {
                Name = CurrentUser.Name,
                Roles = userRoles,
                UserStores = userStores
            };
        }

        [HttpPost]
        public async Task Create(CreateUserRequest request)
        {
            try
            {
                // tạo tài khoản admin của tenant
                using (CurrentTenant.Change(request.TenantId))
                {
                    var user = new IdentityUser(GuidGenerator.Create(), request.UserName, request.Email, request.TenantId);
                    user.Name = request.Name;
                    (await _userManager.CreateAsync(user, request.Password)).CheckErrors();
                    await _userManager.SetEmailAsync(user, request.Email);
                    await _userManager.AddDefaultRolesAsync(user);
                    await CurrentUnitOfWork.SaveChangesAsync();
                    await CurrentUnitOfWork.CompleteAsync();
                }
            }
            catch (Exception e)
            {
                await CurrentUnitOfWork.RollbackAsync();
                throw;
            }
        }

        [HttpPost]
        public async Task AddUserToRoles(AddUserToRoleRequest request)
        {
            var user = await _userManager.GetByIdAsync(request.UserId);
            if(user != null)
            {
                var validRoles = await _roleRepository.GetListAsync(p => request.Roles.Contains(p.Name));
                if(validRoles.Any())
                {
                    await _userManager.AddToRolesAsync(user, request.Roles);
                }
            }
        }
    }
}