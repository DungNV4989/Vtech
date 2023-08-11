
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Mvc;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VTECHERP.Constants;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.Permissions;
using VTECHERP.DTOs.Product;
using VTECHERP.Entities;
using VTECHERP.Helper;
using VTECHERP.Permissions;
using VTECHERP.ServiceInterfaces;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace VTECHERP.Services
{
    public class PermissionService: IPermissionService
    {
        private readonly IdentityUserManager _userManager;
        private readonly IPermissionManager _permissionManager;
        private readonly IIdentityRoleRepository _roleRepository;
        private readonly IdentityRoleManager _identityRoleManager;
        private readonly IUnitOfWorkManager _uowManager;
        private readonly ICurrentUser _currentUser;
        private readonly ICurrentTenant _currentTenant;
        private readonly IRepository<PermissionModule> _permissionModuleRepository;
        private readonly IRepository<PermissionGroup> _permissionGroupRepository;
        private readonly IRepository<Permission> _permissionRepository;
        private readonly IRepository<IdentityUser> _identityUserRepository;
        private readonly IRepository<PermissionGrant> _permissionGrantRepository;
        private readonly IObjectMapper _mapper;

        /// <summary>
        /// Vận chuyển nội bộ: InternalTransport
        /// Lịch sử giao vận: TransportHistory
        /// </summary>
        private readonly IRepository<TransportInformation> _transportInformationRepository;
        /// <summary>
        /// Tổng hợp thu chi: PaymentSummary
        /// </summary>
        private readonly IRepository<PaymentReceipt> _paymentReceiptRepository;
        /// <summary>
        /// Vận chuyển qua hãng: ExternalTransport
        /// </summary>
        private readonly IRepository<CarrierShippingInformation> _carrierShippingInformationRepository;
        /// <summary>
        /// Bảng giá: PriceTable
        /// </summary>
        private readonly IRepository<PriceTableStore> _priceTableStoreRepository;
        /// <summary>
        /// Nhà cung cấp: Supplier
        /// </summary>
        private readonly IRepository<Suppliers> _suppliersRepository;
        /// <summary>
        /// Bán hàng: CustomerSale
        /// </summary>
        private readonly IRepository<BillCustomer> _billCustomerRepository;
        /// <summary>
        /// Đơn hàng nhà cung cấp: SupplierOrder
        /// </summary>
        private readonly IRepository<SaleOrders> _saleOrdersRepository;
        /// <summary>
        /// Sản phẩm: Product
        /// </summary>
        private readonly IRepository<StoreProduct> _storeProductRepository;
        /// <summary>
        /// Kế toán: Accounting
        /// </summary>
        private readonly IRepository<Account> _accountRepository;
        /// <summary>
        /// khách hàng: Customer
        /// </summary>
        private readonly IRepository<Entities.Customer> _customerRepository;
        /// <summary>
        /// Chuyển kho: WarehouseTransfer
        /// </summary>
        private readonly IRepository<WarehouseTransferBill> _warehouseTransferBillRepository;
        /// <summary>
        /// Xuất nhập kho: Warehousing
        /// </summary>
        private readonly IRepository<WarehousingBill> _warehousingBillRepository;
        /// <summary>
        /// Công nợ: Debt
        /// </summary>
        private readonly IRepository<Entities.Debt> _debtRepository;

        public PermissionService(
            IdentityUserManager userManager,
            IPermissionManager permissionManager,
            IIdentityRoleRepository roleRepository,
            IdentityRoleManager identityRoleManager,
            IUnitOfWorkManager uowManager,
            ICurrentUser currentUser,
            ICurrentTenant currentTenant,
            IRepository<PermissionModule> permissionModuleRepository,
            IRepository<PermissionGroup> permissionGroupRepository,
            IRepository<Permission> permissionRepository,
            IRepository<IdentityUser> identityUserRepository,
            IRepository<PermissionGrant> permissionGrantRepository,
            IObjectMapper mapper,
            IRepository<TransportInformation> transportInformationRepository,
            IRepository<PaymentReceipt> paymentReceiptRepository,
            IRepository<CarrierShippingInformation> carrierShippingInformationRepository,
            IRepository<PriceTableStore> priceTableStoreRepository,
            IRepository<Suppliers> suppliersRepository,
            IRepository<BillCustomer> billCustomerRepository,
            IRepository<SaleOrders> saleOrdersRepository,
            IRepository<StoreProduct> storeProductRepository,
            IRepository<Account> accountRepository,
            IRepository<Entities.Customer> customerRepository,
            IRepository<WarehouseTransferBill> warehouseTransferBillRepository,
            IRepository<WarehousingBill> warehousingBillRepository,
            IRepository<Entities.Debt> debtRepository)
        {
            _userManager = userManager;
            _permissionManager = permissionManager;
            _roleRepository = roleRepository;
            _identityRoleManager = identityRoleManager;
            _uowManager = uowManager;
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _permissionModuleRepository = permissionModuleRepository;
            _permissionGroupRepository = permissionGroupRepository;
            _permissionRepository = permissionRepository;
            _identityUserRepository=identityUserRepository;
            _permissionGrantRepository = permissionGrantRepository;
            _mapper = mapper;
            _transportInformationRepository = transportInformationRepository;
            _paymentReceiptRepository = paymentReceiptRepository;
            _carrierShippingInformationRepository = carrierShippingInformationRepository;
            _priceTableStoreRepository = priceTableStoreRepository;
            _suppliersRepository = suppliersRepository;
            _billCustomerRepository = billCustomerRepository;
            _saleOrdersRepository = saleOrdersRepository;
            _storeProductRepository = storeProductRepository;
            _accountRepository = accountRepository;
            _customerRepository = customerRepository;
            _warehouseTransferBillRepository = warehouseTransferBillRepository;
            _warehousingBillRepository = warehousingBillRepository;
            _debtRepository = debtRepository;
        }

        public async Task InitializeTenantPermissions(Guid? tenantId)
        {
            using var uow = _uowManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                using (_currentTenant.Change(tenantId))
                {
                    var allModules = VTECHERPPermissions.GetAllModule();
                    var existModules = await _permissionModuleRepository.GetListAsync();
                    var modules = new List<PermissionModule>();
                    foreach (var module in allModules)
                    {
                        if (!existModules.Any(p => p.Code == module))
                        {
                            modules.Add(new PermissionModule
                            {
                                Code = module,
                                Name = module
                            });
                        }
                    }
                    if (modules.Any())
                    {
                        await _permissionModuleRepository.InsertManyAsync(modules);
                        await uow.SaveChangesAsync();
                        existModules = await _permissionModuleRepository.GetListAsync();
                    }
                    var identityModule = new PermissionModule();
                    if (!existModules.Any(p => p.Code == "AbpIdentity"))
                    {
                        identityModule = new PermissionModule
                        {
                            Code = "AbpIdentity",
                            Name = "AbpIdentity"
                        };
                        await _permissionModuleRepository.InsertAsync(identityModule);
                        await uow.SaveChangesAsync();
                    }
                    else
                    {
                        identityModule = existModules.FirstOrDefault(p => p.Code == "AbpIdentity");
                    }

                    var allGroups = VTECHERPPermissions.GetAllGroups();
                    var existGroups = await _permissionGroupRepository.GetListAsync();
                    var groups = new List<PermissionGroup>();
                    foreach (var group in allGroups)
                    {
                        if (!existGroups.Any(p => p.Code == group.GroupCode))
                        {
                            var groupModule = modules.FirstOrDefault(p => p.Name == group.ModuleName);
                            groups.Add(new PermissionGroup
                            {
                                ModuleId = groupModule.Id,
                                Code = group.GroupCode,
                                Name = group.GroupName
                            });
                        }
                    }

                    await _permissionGroupRepository.InsertManyAsync(groups);
                    await uow.SaveChangesAsync();
                    var userGroup = new PermissionGroup();
                    if (!existGroups.Any(p => p.Code == "AbpIdentity.Users"))
                    {
                        userGroup = new PermissionGroup
                        {
                            Code = "AbpIdentity.Users",
                            Name = "User Management",
                            ModuleId = identityModule.Id
                        };
                        await _permissionGroupRepository.InsertAsync(userGroup);
                        await uow.SaveChangesAsync();
                    }
                    else
                    {
                        userGroup = existGroups.FirstOrDefault(p => p.Code == "AbpIdentity.Users");
                    }
                    var roleGroup = new PermissionGroup();
                    if (!existGroups.Any(p => p.Code == "AbpIdentity.Roles"))
                    {
                        roleGroup = new PermissionGroup
                        {
                            Code = "AbpIdentity.Roles",
                            Name = "Role Management",
                            ModuleId = identityModule.Id
                        };
                        await _permissionGroupRepository.InsertAsync(roleGroup);
                        await uow.SaveChangesAsync();
                    }
                    else
                    {
                        roleGroup = existGroups.FirstOrDefault(p => p.Code == "AbpIdentity.Roles");
                    }

                    var allPermissions = VTECHERPPermissions.GetAllPermissions();
                    var existPermissions = await _permissionRepository.GetListAsync();
                    var permissions = new List<Permission>();
//                  
                    foreach (var permission in allPermissions)
                    {
                        if(!existPermissions.Any(p => p.Code == permission.PermissionCode))
                        {
                            var permissionModule = modules.FirstOrDefault(p => p.Name == permission.ModuleName);
                            var permissionGroup = groups.FirstOrDefault(p => p.Name == permission.GroupName && p.ModuleId == permissionModule.Id);
                            permissions.Add(new Permission
                            {
                                GroupId = permissionGroup.Id,
                                Code = permission.PermissionCode,
                                Name = permission.PermissionName
                            });
                        }
                    }
                    if (!existPermissions.Any(p => p.Code == IdentityPermissions.Roles.Create))
                    {
                        permissions.Add(new Permission
                        {
                            Code = IdentityPermissions.Roles.Create,
                            Name = IdentityPermissions.Roles.Create,
                            GroupId = roleGroup.Id
                        });
                    }
                    if (!existPermissions.Any(p => p.Code == IdentityPermissions.Roles.Update))
                    {
                        permissions.Add(new Permission
                        {
                            Code = IdentityPermissions.Roles.Update,
                            Name = IdentityPermissions.Roles.Update,
                            GroupId = roleGroup.Id
                        });
                    }
                    if (!existPermissions.Any(p => p.Code == IdentityPermissions.Roles.Delete))
                    {
                        permissions.Add(new Permission
                        {
                            Code = IdentityPermissions.Roles.Delete,
                            Name = IdentityPermissions.Roles.Delete,
                            GroupId = roleGroup.Id
                        });
                    }
                    if (!existPermissions.Any(p => p.Code == IdentityPermissions.Roles.ManagePermissions))
                    {
                        permissions.Add(new Permission
                        {
                            Code = IdentityPermissions.Roles.ManagePermissions,
                            Name = IdentityPermissions.Roles.ManagePermissions,
                            GroupId = roleGroup.Id
                        });
                    }
                    if (!existPermissions.Any(p => p.Code == IdentityPermissions.Users.Create))
                    {
                        permissions.Add(new Permission
                        {
                            Code = IdentityPermissions.Users.Create,
                            Name = IdentityPermissions.Users.Create,
                            GroupId = userGroup.Id
                        });
                    }
                    if (!existPermissions.Any(p => p.Code == IdentityPermissions.Users.Update))
                    {
                        permissions.Add(new Permission
                        {
                            Code = IdentityPermissions.Users.Update,
                            Name = IdentityPermissions.Users.Update,
                            GroupId = userGroup.Id
                        });
                    }
                    if (!existPermissions.Any(p => p.Code == IdentityPermissions.Users.Delete))
                    {
                        permissions.Add(new Permission
                        {
                            Code = IdentityPermissions.Users.Delete,
                            Name = IdentityPermissions.Users.Delete,
                            GroupId = userGroup.Id
                        });
                    }

                    if (!existPermissions.Any(p => p.Code == IdentityPermissions.Users.ManagePermissions))
                    {
                        permissions.Add(new Permission
                        {
                            Code = IdentityPermissions.Users.ManagePermissions,
                            Name = IdentityPermissions.Users.ManagePermissions,
                            GroupId = userGroup.Id
                        });
                    }
                    if (!existPermissions.Any(p => p.Code == IdentityPermissions.UserLookup.Default))
                    {
                        permissions.Add(new Permission
                        {
                            Code = IdentityPermissions.UserLookup.Default,
                            Name = IdentityPermissions.UserLookup.Default,
                            GroupId = userGroup.Id
                        });
                    }
                    if (permissions.Any())
                    {
                        await _permissionRepository.InsertManyAsync(permissions);
                        await uow.SaveChangesAsync();
                    }

                    await uow.CompleteAsync();
                }
            }
            catch (Exception e)
            {
                await uow.RollbackAsync();
                throw;
            }

        }
        public async Task<PagingResponse<RoleDTO>> GetAllRole(SearchRoleDTO request)
        {
            var list = (await _roleRepository.GetListAsync())
                .WhereIf(!(string.IsNullOrEmpty(request.RoleName)), x => x.Name.ToLower().Contains(request.RoleName.ToLower()));
            var users = await _identityUserRepository.GetListAsync();
            var listRole = (from role in list
                            
                           select new RoleDTO
                           {
                               RoleId = role.Id,
                               RoleName = role.Name,
                               CreatorName = users.Where(x=>x.Id.ToString().ToLower()== role.GetProperty("CreatorId").ToString().ToLower()).FirstOrDefault().UserName ,
                               Description=role.GetProperty("Description") != null ? role.GetProperty("Description").ToString() : "",
                           }
                           ).ToList();
            if (listRole != null && listRole.Count>0)
            {
                var result = listRole.Skip(request.Offset)
                        .Take(request.PageSize).ToList();
                return new PagingResponse<RoleDTO>()
                {
                    Data = result,
                    Total = listRole.Count(),
                };
            } 
            else
            {
                return new PagingResponse<RoleDTO>()
                {
                    Data = new List<RoleDTO>(),
                    Total = 0,
                };
            }    
            
        }

        public async Task<IActionResult> CreateRole(CreateRoleRequest request)
        {
            using var uow = _uowManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                // tạo role
                var findrole = await _roleRepository.GetListAsync();
                if(findrole != null && findrole.Count>0)
                {
                    if(findrole.Where(x=>x.Name.ToLower()==request.RoleName.ToLower()).Any())
                    {
                        return new GenericActionResult(400, false, "Tên nhóm quyền đã tồn tại trong hệ thống");
                    }    
                }    
                var role = new IdentityRole(Guid.NewGuid(), request.RoleName, _currentUser.TenantId);
                role.SetProperty("Description", request.Description);
                role.SetProperty("CreatorId", _currentUser.Id);
                await _roleRepository.InsertAsync(role);
                await uow.SaveChangesAsync();
                await uow.CompleteAsync();
                return new GenericActionResult(200, true);
            }
            catch
            {
                return new GenericActionResult(400, false, "Có lỗi xảy ra trong quá trình tạo nhóm quyền");
            }
        }

        public async Task<IActionResult> UpdateRole(Guid Id,CreateRoleRequest request)
        {
            using var uow = _uowManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var findallrole = (await _roleRepository.GetListAsync()).Where(x => x.Id != Id);
                if (findallrole != null && findallrole.Count() > 0)
                {
                    if (findallrole.Where(x => x.Name.ToLower() == request.RoleName.ToLower()).Any())
                    {
                        return new GenericActionResult(400, false, "Tên nhóm quyền đã tồn tại trong hệ thống");
                    }
                }
                var findrole = await _roleRepository.FindAsync(Id);

                if(findrole==null)
                {
                    return new GenericActionResult(400, false, "Không tìm thấy Id nhóm quyền phù hợp");
                }   
                else
                {
                    findrole.SetProperty("Description", request.Description);
                    findrole.ChangeName(request.RoleName);
                    //findrole.NormalizedName = request.RoleName;
                    await _roleRepository.UpdateAsync(findrole);
                    await uow.SaveChangesAsync();
                    await uow.CompleteAsync();
                    return new GenericActionResult(200, true,"Cập nhật thành công!");
                }    
            }
            catch
            {
                return new GenericActionResult(400, false, "Có lỗi xảy ra trong quá trình cập nhật nhóm quyền");
            }
        }

        public async Task<IActionResult> DeleteRole(Guid Id)
        {
            using var uow = _uowManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                
                var role = await _roleRepository.FindAsync(Id);

                if (role == null)
                {
                    return new GenericActionResult(400, false, "Không tìm thấy Id nhóm quyền phù hợp");
                }
                else
                {
                    await _roleRepository.DeleteAsync(role);
                    await uow.SaveChangesAsync();
                    await uow.CompleteAsync();
                    return new GenericActionResult(200, true, "Xóa thành công!");
                }
            }
            catch
            {
                return new GenericActionResult(400, false, "Có lỗi xảy ra trong quá trình cập nhật nhóm quyền");
            }
        }
        //public async Task<IActionResult> GrantPermissionForRole(string roleName, List<GrantPermissionRequest> request)
        //{
        //    using var uow = _uowManager.Begin(requiresNew: true, isTransactional: true);
        //    try
        //    {
        //        // lấy ra vai trò
        //        var role = await _identityRoleManager.FindByNameAsync(roleName);
        //        if (role == null)
        //        {
        //            return new GenericActionResult(404, false, "Không tìm thấy vai trò");
        //        }
        //        if(request!=null && request.Count > 0)
        //        {
        //            foreach (var item in request)
        //            {
        //                var newPermissionGrant = new PermissionGrant(Guid.NewGuid(), item.PermissionName, "R", roleName, _currentUser.TenantId);
        //                var existPermissionGrant = await _permissionGrantRepository.FindAsync(s => s.ProviderKey == roleName && s.Name == item.PermissionName);
        //                if (item.IsGrant)
        //                {
        //                    if (existPermissionGrant == null)
        //                    {
        //                        await _permissionGrantRepository.InsertAsync(newPermissionGrant);
        //                    }
        //                }
        //                else
        //                {
        //                    if (existPermissionGrant != null)
        //                    {
        //                        await _permissionGrantRepository.DeleteAsync(existPermissionGrant);
        //                    }
        //                }
        //            }
        //        }    
        //        await uow.CompleteAsync();
        //        return new GenericActionResult(200, true);
        //    }
        //    catch(Exception ex)
        //    {
        //        var x = ex.Message;
        //        await uow.RollbackAsync();
        //        return new GenericActionResult(400, false, ex.Message);
        //    }
        //}

        public async Task<IActionResult> GrantPermissionForRole(string id, List<string> request)
        {
            using var uow = _uowManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                // lấy ra vai trò
                var role = await _identityRoleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return new GenericActionResult(404, false, "Không tìm thấy vai trò");
                }

                var permissions = await _permissionManager.GetAllForRoleAsync(role.Name);
                var permissionList = await _permissionRepository.GetListAsync();
                if (request != null)
                {
                    foreach (var item in permissions)
                    {
                        if (!permissionList.Select(x => x.Code).Contains(item.Name) || item.Name == "AbpIdentity.UserLookup")
                            continue;
                        var p = request.FirstOrDefault(x => x == item.Name);
                        if (p != null)
                        {
                            await _permissionManager.SetForRoleAsync(role.Name, p, true);
                        }
                        else
                        {
                            await _permissionManager.SetForRoleAsync(role.Name, item.Name, false);
                        }
                    }
                }
                await uow.CompleteAsync();
                return new GenericActionResult(200, true);
            }
            catch (Exception ex)
            {
                var x = ex.Message;
                await uow.RollbackAsync();
                return new GenericActionResult(400, false, ex.Message);
            }
        }

        public async Task<List<PermissionDTO>> GetPermissions(string moduleCode, string groupCode)
        {
            var permissionQuery =
                from module in await _permissionModuleRepository.GetQueryableAsync()
                join grp in await _permissionGroupRepository.GetQueryableAsync() on module.Id equals grp.ModuleId
                join permission in await _permissionRepository.GetQueryableAsync() on grp.Id equals permission.GroupId
                where
                (moduleCode == null || moduleCode == "" || module.Code == moduleCode)
                && (groupCode == null || groupCode == "" || grp.Code == groupCode)
                orderby module.Code, grp.Code, permission.Code
                select new PermissionDTO
                {
                    Code = permission.Code,
                    Name = permission.Name,
                    ModuleCode = module.Code,
                    GroupCode = grp.Code
                };

            var permissions = permissionQuery.ToList();

            return permissions;
        }

        public async Task<List<string>> GetCurrentUserPermission()
        {
            try
            {
                var permissions = await _permissionManager.GetAllForUserAsync(_currentUser.Id.Value);
                return permissions.Where(x => x.IsGranted == true).Select(x => x.Name).ToList();
            }
            catch(Exception e)
            {
                throw;
            }
        }

        public async Task<List<PermissionModuleDTO>> GetUserPermission(Guid id) 
        {
            try
            {
                var permissions = await _permissionManager.GetAllForUserAsync(id);
                return await GetTreeViewPermission(permissions);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<PermissionModuleDTO>> GetAllPermissions()
        {
            try
            {
                var permissions = new List<PermissionWithGrantedProviders>();
                return await GetTreeViewPermission(permissions);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<PermissionModuleDTO>> GetAllPermissionActive(Guid tenantId)
        {
            try
            {
                var permissions = new List<PermissionWithGrantedProviders>();
                return await GetTreeViewPermission(permissions, tenantId);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<PermissionModuleDTO>> GetRolePermission(Guid id)
        {
            try
            {
                var role = await _roleRepository.GetAsync(id);
                List<PermissionWithGrantedProviders> permissions = new List<PermissionWithGrantedProviders>();
                if (role != null)
                {
                    permissions = await _permissionManager.GetAllForRoleAsync(role.Name);
                }
                return await GetTreeViewPermission(permissions);

            }
            catch (Exception e)
            {
                throw;
            }
        }

        private async Task<List<PermissionModuleDTO>> GetTreeViewPermission(List<PermissionWithGrantedProviders> permissions, Guid? tenantId = null)
        {
            var module = await _permissionModuleRepository.GetListAsync();
            var group = await _permissionGroupRepository.GetListAsync();
            var permission = await _permissionRepository.GetListAsync();

            var moduleDTO = _mapper.Map<List<PermissionModule>, List<PermissionModuleDTO>>(module);
            var groupDTO = _mapper.Map<List<PermissionGroup>, List<PermissionGroupDTO>>(group);
            var permissionDTO = _mapper.Map<List<Permission>, List<PermissionDTO>>(permission);

            if(tenantId == null)
            {
                permissionDTO.ForEach(x => {
                    var perm = permissions.FirstOrDefault(p => p.Name == x.Code);
                    if (perm != null)
                    {
                        x.IsGranted = perm.IsGranted;
                    }
                });
            }
            else
            {
                using (_currentTenant.Change(tenantId))
                {
                    var permissionByTenant = (await _permissionRepository.GetQueryableAsync()).ToList();
                    permissionDTO.ForEach(x => {
                        var perm = permissionByTenant.FirstOrDefault(p => p.Code == x.Code);
                        x.IsGranted = perm == null ? false : !perm.IsActive;
                    });
                }
            }

            groupDTO.ForEach(g =>
            {
                g.Permissions = permissionDTO.Where(x => x.GroupId == g.Id).ToList();
            });

            moduleDTO.ForEach(async m => { 
                m.Groups = groupDTO.Where(x => x.ModuleId == m.Id).ToList();
                if(tenantId  != null)
                    m.IsDeleted = await IsNullDataAsync(m.Code, tenantId.Value);
            });

            return moduleDTO;
        }

        public async Task<List<PermissionModuleDTO>> GetRolePermissionActive(Guid id)
        {
            try
            {
                var role = await _roleRepository.GetAsync(id);
                List<PermissionWithGrantedProviders> permissions = new List<PermissionWithGrantedProviders>();
                if (role != null)
                {
                    permissions = await _permissionManager.GetAllForRoleAsync(role.Name);
                }
                return await GetTreeViewPermissionActive(permissions);

            }
            catch (Exception e)
            {
                throw;
            }
        }

        private async Task<List<PermissionModuleDTO>> GetTreeViewPermissionActive(List<PermissionWithGrantedProviders> permissions)
        {

            var permission = (await _permissionRepository.GetQueryableAsync()).Where(x=>x.IsActive == false).ToList();

            var groups = (await _permissionGroupRepository.GetQueryableAsync()).ToList();
            var group = groups.Where(x => permission.Any(p => p.GroupId == x.Id)).ToList();
            var modules = (await _permissionModuleRepository.GetQueryableAsync()).ToList();
            var module = modules.Where(x => group.Any(g => g.ModuleId == x.Id)).ToList();

            var moduleDTO = _mapper.Map<List<PermissionModule>, List<PermissionModuleDTO>>(module);
            var groupDTO = _mapper.Map<List<PermissionGroup>, List<PermissionGroupDTO>>(group);
            var permissionDTO = _mapper.Map<List<Permission>, List<PermissionDTO>>(permission);

            permissionDTO.ForEach(x => 
            {
                var perm = permissions.FirstOrDefault(p => p.Name == x.Code);
                if (perm != null)
                {
                    x.IsGranted = perm.IsGranted;
                }
            });

            groupDTO.ForEach(g =>
            {
                g.Permissions = permissionDTO.Where(x => x.GroupId == g.Id && x.Code != "AbpIdentity.UserLookup").ToList();
            });

            moduleDTO.ForEach(m => {
                m.Groups = groupDTO.Where(x => x.ModuleId == m.Id).ToList();
            });

            return moduleDTO;
        }

        private async Task<bool> IsNullDataAsync(string code, Guid tenantId)
        {
            using (_currentTenant.Change(tenantId))
            {
                if (typeof(VTECHERPPermissions.AbpIdentity).Name == code)
                    return false;
                else if (typeof(VTECHERPPermissions.InternalTransport).Name == code)
                {
                    if ((await _transportInformationRepository.GetQueryableAsync()).Any())
                        return false;
                }
                else if (typeof(VTECHERPPermissions.PaymentSummary).Name == code)
                {
                    if ((await _paymentReceiptRepository.GetQueryableAsync()).Any())
                        return false;
                }
                else if (typeof(VTECHERPPermissions.ExternalTransport).Name == code)
                {
                    if ((await _carrierShippingInformationRepository.GetQueryableAsync()).Any())
                        return false;
                }
                else if (typeof(VTECHERPPermissions.PriceTable).Name == code)
                {
                    if ((await _priceTableStoreRepository.GetQueryableAsync()).Any())
                        return false;
                }
                else if (typeof(VTECHERPPermissions.CustomerSale).Name == code)
                {
                    if ((await _billCustomerRepository.GetQueryableAsync()).Any())
                        return false;
                }
                else if (typeof(VTECHERPPermissions.SupplierOrder).Name == code)
                {
                    if ((await _saleOrdersRepository.GetQueryableAsync()).Any())
                        return false;
                }
                else if (typeof(VTECHERPPermissions.Product).Name == code)
                {
                    if ((await _storeProductRepository.GetQueryableAsync()).Any())
                        return false;
                }
                else if (typeof(VTECHERPPermissions.Accounting).Name == code)
                {
                    if ((await _accountRepository.GetQueryableAsync()).Any())
                        return false;
                }
                else if (typeof(VTECHERPPermissions.TransportHistory).Name == code)
                {
                    if ((await _transportInformationRepository.GetQueryableAsync()).Any())
                        return false;
                }
                else if (typeof(VTECHERPPermissions.Customer).Name == code)
                {
                    if ((await _customerRepository.GetQueryableAsync()).Any())
                        return false;
                }
                else if (typeof(VTECHERPPermissions.WarehouseTransfer).Name == code)
                {
                    if ((await _warehouseTransferBillRepository.GetQueryableAsync()).Any())
                        return false;
                }
                else if (typeof(VTECHERPPermissions.Warehousing).Name == code)
                {
                    if ((await _warehousingBillRepository.GetQueryableAsync()).Any())
                        return false;
                }
                else if (typeof(VTECHERPPermissions.Debt).Name == code)
                {
                    if ((await _debtRepository.GetQueryableAsync()).Any())
                        return false;
                }
                return true;
            }
        }
    }
}
