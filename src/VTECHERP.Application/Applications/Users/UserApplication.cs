using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using Volo.Abp.Validation;
using VTECHERP.Constants;
using VTECHERP.DTOs.Users.Dto;
using VTECHERP.DTOs.Users.Params;
using VTECHERP.Entities;
using VTECHERP.Services;
using UserStore = VTECHERP.Entities.UserStore;

namespace VTECHERP.Applications.Users
{
    [Authorize]
    [Route("api/app/User/[action]")]
    public class UserApplication : ApplicationService
    {
        private readonly IdentityUserManager _userManager;
        private readonly IRepository<IdentityRole> _roleRepository;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly IRepository<Stores> _storeRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<IdentityUser> _userRepository;
        private readonly ICommonService _commonService;
        private readonly IDataFilter _dataFilter;
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<Tenant> _tenantManager;
        private readonly IRepository<Enterprise> _enterpriseRepository;
        private readonly IRepository<Agency> _agencyRepository;
        private readonly IConfiguration _configuration;

        public UserApplication(
            IdentityUserManager userManager,
            IRepository<IdentityRole> roleRepository,
            IRepository<UserStore> userStoreRepository,
            IRepository<Stores> storeRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<IdentityUser> userRepository,
            ICommonService commonService,
            IDataFilter dataFilter,
            ICurrentUser currentUser,
            IRepository<Tenant> tenantManager,
            IRepository<Enterprise> enterpriseRepository,
            IRepository<Agency> agencyRepository,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _roleRepository = roleRepository;
            _userStoreRepository = userStoreRepository;
            _storeRepository = storeRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _userRepository = userRepository;
            _commonService = commonService;
            _dataFilter = dataFilter;
            _currentUser = currentUser;
            _tenantManager = tenantManager;
            _enterpriseRepository = enterpriseRepository;
            _agencyRepository = agencyRepository;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserParam request)
        {
            if (request.RoleId.HasValue && !(await _roleRepository.AnyAsync(x => x.Id == request.RoleId.Value)))
            {
                return new BadRequestObjectResult(new
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Message = "Không tìm thấy nhóm quyền trong hệ thống",
                });
            }

            using (var uow = _unitOfWorkManager.Begin(isTransactional: true))
            {
                try
                {
                    var currentTenant = CurrentTenant.Id;
                    var tenant = await _tenantManager.FirstOrDefaultAsync(x => x.Id == currentTenant);
                    var isVtechTenant = tenant.GetProperty<bool>("IsVTech");
                    var user = new IdentityUser(GuidGenerator.Create(), request.UserName, request.Email, currentTenant);
                    user.SetProperty("IsVTech", isVtechTenant);
                    user.Name = request.Name;

                    if (!string.IsNullOrEmpty(request.PhoneNumber))
                    {
                        if (!request.PhoneNumber.StartsWith("0"))
                            return new BadRequestObjectResult(new
                            {
                                HttpStatusCode = HttpStatusCode.BadRequest,
                                Message = "Số điện thoại sai định dạng",
                            });

                        if (request.PhoneNumber.Length > 11 || request.PhoneNumber.Length < 10)
                            return new BadRequestObjectResult(new
                            {
                                HttpStatusCode = HttpStatusCode.BadRequest,
                                Message = "Số điện thoại độ dài không hợp lệ",
                            });

                        user.SetPhoneNumber(request.PhoneNumber, false);
                    }

                    user.SetIsActive(request.IsActive);
                    user.SetProperty("MainStoreId", request.MainStoreId);
                    var result = await _userManager.CreateAsync(user, request.Password);
                    if (result.Succeeded)
                    {
                        if (request.RoleId.HasValue)
                            user.AddRole(request.RoleId.Value);

                        var userStores = new List<UserStore>()
                        {
                            new UserStore()
                            {
                                 UserId = user.Id,
                                 StoreId = user.GetProperty<Guid>("MainStoreId"),
                                 IsDefault = false,
                            }
                        };
                        foreach (var item in request.ExtraStoreId)
                        {
                            var userStore = new UserStore()
                            {
                                UserId = user.Id,
                                StoreId = item,
                                IsDefault = false,
                            };
                            userStores.Add(userStore);
                        }

                        if (userStores.Any())
                            await _userStoreRepository.InsertManyAsync(userStores);

                        await uow.CompleteAsync();

                        return new OkObjectResult(new
                        {
                            StatusCode = 200,
                            Data = new
                            {
                                user.UserName,
                                user.Id
                            },
                            Message = "Tạo người dùng thành công"
                        });
                    }
                    else
                    {
                        return new BadRequestObjectResult(new
                        {
                            Errors = result.Errors.ToList(),
                            Message = "Dữ liệu không hợp lệ",
                            StatusCode = 400
                        });
                    }
                }
                catch (Exception ex)
                {
                    return new ObjectResult(ex.Message)
                    {
                        StatusCode = 500,
                    };
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Search(UsersSearchParam param)
        {
            try
            {
                var result = new List<UserItemList>();

                var userQueryable = (await _userRepository.GetQueryableAsync()).Include(x => x.Roles)
                    .WhereIf(param.MainStoreId.HasValue, x => EF.Property<Guid>(x, "MainStoreId") == param.MainStoreId.Value)
                    .WhereIf(!string.IsNullOrEmpty(param.UserCode), x => EF.Property<string>(x, "UserCode").Contains(param.UserCode.Trim()))
                    .WhereIf(!string.IsNullOrEmpty(param.Name), x => x.Name.ToLower().Contains(param.Name.ToLower().Trim()))
                    .WhereIf(!string.IsNullOrEmpty(param.UserName), x => x.UserName.ToLower().Contains(param.UserName.ToLower().Trim()))
                    .WhereIf(!string.IsNullOrEmpty(param.PhoneNumber), x => x.PhoneNumber.ToLower().Contains(param.PhoneNumber.ToLower().Trim()))
                    .WhereIf(!string.IsNullOrEmpty(param.Email), x => x.Email.ToLower().Contains(param.Email.ToLower().Trim()))
                    .WhereIf(param.IsActive.HasValue, x => x.IsActive == param.IsActive)
                    .WhereIf(param.GroupPermission.HasValue, x => x.Roles.Any(r => r.RoleId == param.GroupPermission.Value))
                    .Select(x => new UserItemList
                    {
                        Code = x.GetProperty("UserCode", ""),
                        UserName = x.UserName,
                        Email = x.Email,
                        Name = x.Name,
                        UserId = x.Id,
                        PhoneNumber = x.PhoneNumber,
                        Status = x.IsActive ? "Hoạt động" : "Không hoạt động",
                        MainStoreId = x.GetProperty("MainStoreId", Guid.Empty),
                        PermissionIds = x.Roles.Select(x => x.RoleId).FirstOrDefault()
                    });

                if (param.ExtraStoreId.Any())
                {
                    var userStores = await _userStoreRepository.GetListAsync(x => param.ExtraStoreId.Contains(x.StoreId));
                    var userIds = userStores.Select(x => x.UserId).ToList();
                    userQueryable = userQueryable.Where(x => userIds.Contains(x.UserId));
                }

                result = userQueryable.Skip((param.PageIndex - 1) * param.PageSize)
                                      .Take(param.PageSize)
                                      .ToList();

                var storeQueryable = await _storeRepository.GetQueryableAsync();
                var userStoreQueryable = await _userStoreRepository.GetQueryableAsync();

                var lstUserIds = result.Select(x => x.UserId).ToList();
                var mainStoreUserIds = result.Select(x => x.MainStoreId).ToList();

                var lstStoreUser = (from store in storeQueryable
                                    join userStore in userStoreQueryable
                                    on store.Id equals userStore.StoreId
                                    where lstUserIds.Contains(userStore.UserId)
                                    select new
                                    {
                                        UserId = userStore.UserId,
                                        StoreId = store.Id,
                                        StoreName = store.Name
                                    }).ToList();

                var userRoleIds = result.Select(x => x.PermissionIds);
                var roles = await _roleRepository.GetListAsync(x => userRoleIds.Contains(x.Id));

                foreach (var item in result)
                {
                    item.MainStore = lstStoreUser.FirstOrDefault(x => x.StoreId == item.MainStoreId)?.StoreName;
                    item.ExtraStore = lstStoreUser.Where(x => x.UserId == item.UserId && x.StoreId != item.MainStoreId).Select(x => x.StoreName).ToList();
                    item.GroupPermission = roles.FirstOrDefault(x => x.Id == item.PermissionIds)?.Name;
                }

                return new OkObjectResult(new
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    Message = "",
                    Data = result,
                    Count = userQueryable.Count()
                });
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message)
                {
                    StatusCode = 500,
                };
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetByIdAsync(Guid userId)
        {
            var result = new UserDetailDto();

            var user = (await _userManager.GetByIdAsync(userId));
            if (user == null)
                return new BadRequestObjectResult(new
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Message = "Không tìm thấy người dùng trong hệ thống",
                });
            result.Id = user.Id;
            result.UserName = user.UserName;
            result.Name = user.Name;
            result.Email = user.Email;
            result.IsActive = user.IsActive;
            result.PhoneNumber = user.PhoneNumber;
            var roleName = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            var role = (await _roleRepository.GetQueryableAsync()).FirstOrDefault(x => x.Name == roleName);
            if (role != null)
            {
                result.RoleId = role.Id;
                result.RoleName = role.Name;
            }

            var stores = await _storeRepository.GetListAsync();

            var mainStore = stores.FirstOrDefault(x => x.Id == user.GetProperty("MainStoreId", Guid.Empty)) ?? new Stores();
            result.MainStoreId = mainStore.Id;
            result.MainStoreName = mainStore.Name;

            var userStores = (await _userStoreRepository.GetQueryableAsync()).Where(x => x.UserId == userId && x.StoreId != mainStore.Id);
            var extraStores = stores.Where(x => userStores.Any(uS => uS.StoreId == x.Id)).ToList();
            if (extraStores.Any())
            {
                extraStores.ForEach(x =>
                {
                    result.ExtraStores.Add(new ExtraStore()
                    {
                        Id = x.Id,
                        Name = x.Name,
                    });
                });
            }

            return new OkObjectResult(new
            {
                StatusCode = 200,
                Data = result,
                Message = $"Lấy thông tin thành công."
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdateUserParam request)
        {
            await ValidateUpdate(request);

            using (var uow = _unitOfWorkManager.Begin(isTransactional: true))
            {
                try
                {
                    var user = await _userManager.GetByIdAsync(request.Id);
                    if (request.Name != null)
                        user.Name = request.Name;

                    if (user.UserName != request.UserName)
                        await _userManager.SetUserNameAsync(user, request.UserName);

                    if (user.Email != request.Email)
                        await _userManager.SetEmailAsync(user, request.Email);

                    if (user.PhoneNumber != request.PhoneNumber)
                        await _userManager.SetPhoneNumberAsync(user, request.PhoneNumber);

                    if (user.GetProperty("MainStoreId", Guid.Empty) != request.MainStoreId)
                        user.SetProperty("MainStoreId", request.MainStoreId);

                    user.SetIsActive(request.IsActive);

                    await uow.SaveChangesAsync();

                    #region vì 1 người dùng chỉ có 1 nhóm quyền nên: 1. Xóa tất cả nhóm quyền của người dùng; 2.Thêm mới nhóm quyền cho người dùng đó.

                    var roleNames = (await _userManager.GetRolesAsync(user));
                    var roles = (await _roleRepository.GetQueryableAsync()).Where(x => roleNames.Any(rN => rN == x.Name)).ToList();
                    if (roles.Any())
                        roles.ForEach(x =>
                        {
                            user.RemoveRole(x.Id);
                        });
                    if (request.RoleId.HasValue)
                        user.AddRole(request.RoleId ?? Guid.Empty);
                    await uow.SaveChangesAsync();

                    #endregion vì 1 người dùng chỉ có 1 nhóm quyền nên: 1. Xóa tất cả nhóm quyền của người dùng; 2.Thêm mới nhóm quyền cho người dùng đó.

                    #region Xóa các cửa hàng phụ của người dùng.

                    var userStores = (await _userStoreRepository.GetQueryableAsync()).Where(x => x.UserId == user.Id);
                    if (userStores.Any())
                        await _userStoreRepository.DeleteManyAsync(userStores);

                    #endregion Xóa các cửa hàng phụ của người dùng.

                    #region Thêm mới các cửa hàng cho người dùng.

                    var newUserStores = new List<UserStore>();
                    if (request.MainStoreId.HasValue)
                        newUserStores.Add(new UserStore()
                        {
                            UserId = user.Id,
                            StoreId = request.MainStoreId.Value,
                        });

                    if (request.ExtraStoreIds.Any())
                    {
                        request.ExtraStoreIds.ForEach(storeId =>
                        {
                            newUserStores.Add(new UserStore
                            {
                                UserId = user.Id,
                                StoreId = storeId,
                            });
                        });
                    }

                    if (newUserStores.Any())
                        await _userStoreRepository.InsertManyAsync(newUserStores);

                    #endregion Thêm mới các cửa hàng cho người dùng.

                    await uow.SaveChangesAsync();
                    await uow.CompleteAsync();

                    return new OkObjectResult(new
                    {
                        StatusCode = 200,
                        Data = user,
                        Message = "Cập nhật người dùng thành công"
                    });
                }
                catch (Exception ex)
                {
                    await uow.RollbackAsync();
                    return new ObjectResult(ex.Message)
                    {
                        StatusCode = 500,
                    };
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetListRoleAsync()
        {
            var roles = await _roleRepository.GetQueryableAsync();

            return new OkObjectResult(new
            {
                StatusCode = 200,
                Data = roles,
                Message = $"Danh sách nhóm quyền"
            });
        }

        private async Task ValidateUpdate(UpdateUserParam request)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var validationErrors = new List<ValidationResult>();
                var user = await _userManager.GetByIdAsync(request.Id);
                if (user == null)
                    throw new AbpValidationException($"Không tìn thấy người dùng({request.Id}) trong hệ thống.");

                if (request.Name.IsNullOrWhiteSpace())
                    validationErrors.Add(new ValidationResult(ErrorMessages.User.NameRequired));

                if (request.UserName.IsNullOrWhiteSpace())
                    validationErrors.Add(new ValidationResult(ErrorMessages.User.UseNameRequired));

                if (user.UserName != request.UserName)
                {
                    var checkUserName = (await _userRepository.GetQueryableAsync()).FirstOrDefault(x => x.UserName == request.UserName);
                    if (checkUserName != null)
                        validationErrors.Add(new ValidationResult(ErrorMessages.User.UserNameIsExist));
                }

                if (user.Email != request.Email)
                {
                    var checkEmail = (await _userRepository.GetQueryableAsync()).FirstOrDefault(x => x.Email == request.Email);
                    if (checkEmail != null)
                        validationErrors.Add(new ValidationResult(ErrorMessages.User.PhoneIsExist));

                    if (!_commonService.ValidateEmail(request.Email) && request.Email != null)
                        validationErrors.Add(new ValidationResult(ErrorMessages.User.EmailInCorrect));
                }

                if (request.PhoneNumber.IsNullOrWhiteSpace())
                    validationErrors.Add(new ValidationResult(ErrorMessages.User.PhoneNumberRequired));
                else
                {
                    var checkPhoneNumber = (await _userRepository.GetQueryableAsync()).FirstOrDefault(x => x.PhoneNumber == request.PhoneNumber);
                    if (checkPhoneNumber != null)
                        validationErrors.Add(new ValidationResult(ErrorMessages.User.PhoneIsExist));

                    if (!_commonService.ValidatePhoneNumber(request.PhoneNumber))
                        validationErrors.Add(new ValidationResult(ErrorMessages.User.PhoneNumberInCorrect));
                }

                if (request.RoleId.HasValue)
                {
                    var role = (await _roleRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == (request.RoleId ?? Guid.Empty));
                    if (role == null)
                        validationErrors.Add(new ValidationResult(ErrorMessages.User.RoleNotExist));
                }

                if (!request.MainStoreId.HasValue)
                    validationErrors.Add(new ValidationResult(ErrorMessages.User.MainStoreRequired));
                else
                {
                    var store = (await _storeRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == request.MainStoreId);
                    if (store == null)
                        validationErrors.Add(new ValidationResult(ErrorMessages.User.MainStoreNotExist));
                }

                if (request.ExtraStoreIds.Any())
                {
                    var stores = (await _storeRepository.GetQueryableAsync()).Where(x => request.ExtraStoreIds.Any(id => id == x.Id));
                    if (stores.Count() != request.ExtraStoreIds.Count())
                        validationErrors.Add(new ValidationResult(ErrorMessages.User.ExtraStoreNotExist));
                }

                if (validationErrors.Any())
                    throw new AbpValidationException(validationErrors);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetStores(List<Guid> StoreExcept)
        {
            var query = await _storeRepository.GetQueryableAsync();
            if (StoreExcept != null && StoreExcept.Any())
                query = query.Where(x => !StoreExcept.Contains(x.Id));

            var result = query.Select(x => new
            {
                Id = x.Id,
                Name = x.Name,
            })
            .ToList();

            return new ObjectResult(new
            {
                Data = result
            })
            {
                StatusCode = 200
            };
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserStatus(UpdateUserStatusParam param)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.Id == param.UserId);
            if (user == null)
            {
                return new BadRequestObjectResult(new
                {
                    Success = false,
                    Message = $"Không tìm thấy user có id là {param.UserId}"
                });
            }

            user.SetIsActive(param.IsActive);

            await _userRepository.UpdateAsync(user);
            return new OkObjectResult(new
            {
                Success = true,
                Message = $"Cập nhật thành công"
            }); ;
        }

        [HttpGet]
        public async Task<IActionResult> CheckUserIsVtech()
        {
            var user = await _userRepository.FindAsync(x => x.Id == _currentUser.Id);
            var isVtech = user.GetProperty<bool>("IsVTech");

            return new OkObjectResult(new
            {
                Success = true,
                IsVtech = isVtech
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<string> ChangePasswordAsync(Guid tenantId, Guid id, string newPassword)
        {
            using (CurrentTenant.Change(tenantId))
            {
                using (_dataFilter.Disable<ISoftDelete>())
                {
                    var user = await _userRepository.FirstOrDefaultAsync(x => x.Id == id);
                    await _userManager.RemovePasswordAsync(user);
                    await _userManager.AddPasswordAsync(user, newPassword);
                    return user.UserName;
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUserAsync(List<Guid> ids)
        {
            await ValidateDelete(ids);
            var users = (await _userRepository.GetQueryableAsync()).Where(x => ids.Any(id => id == x.Id)).ToList() ?? new List<IdentityUser>();
            await _userRepository.DeleteManyAsync(users);
            return new OkObjectResult(new
            {
                StatusCode = 200,
                Data = users,
                Message = "Xóa người dùng thành công!"
            });
        }

        private async Task ValidateDelete(List<Guid> ids)
        {
            var validationErrors = new List<ValidationResult>();
            var users = (await _userRepository.GetQueryableAsync()).Where(x => ids.Any(id => id == x.Id)).ToList() ?? new List<IdentityUser>();
            if (!users.Any() || users.Count() != ids.Count())
                throw new AbpValidationException($"Thông tin người dùng không hợp lệ.");

            using (var sqlConnection = new SqlConnection(_configuration["ConnectionStrings:BO"]))
            {
                sqlConnection.Open();

                var queryEnterprise = $"SELECT UserId FROM Enterprises WHERE IsDeleted = 0 AND TenantId = '{users.FirstOrDefault().TenantId}'";
                var Enterprises = sqlConnection.Query<Enterprise>(queryEnterprise).ToList() ?? new List<Enterprise>();

                var queryAgency = $"SELECT UserId FROM Agencies WHERE IsDeleted = 0 AND TenantId = '{users.FirstOrDefault().TenantId}'";
                var Agencies = sqlConnection.Query<Agency>(queryAgency).ToList() ?? new List<Agency>();

                var userIsVTechs = Enterprises.Where(x => ids.Any(id => id == x.UserId)).ToList() ?? new List<Enterprise>();
                var userNonVTechs = Agencies.Where(x => ids.Any(id => id == x.UserId)).ToList() ?? new List<Agency>();
                if (userIsVTechs.Any() || userNonVTechs.Any())
                {
                    var userName = "";
                    if (userIsVTechs.Any())
                        userName = users.FirstOrDefault(x => x.Id == userIsVTechs.FirstOrDefault().UserId).UserName;
                    if (userNonVTechs.Any())
                        userName = users.FirstOrDefault(x => x.Id == userNonVTechs.FirstOrDefault().UserId).UserName;
                    validationErrors.Add(new ValidationResult($"Không thể xóa tài khoản Admin({userName})"));
                }
            }
                if (validationErrors.Any())
                throw new AbpValidationException(validationErrors);
        }
    }
}