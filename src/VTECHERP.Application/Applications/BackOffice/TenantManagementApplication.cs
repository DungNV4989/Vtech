using Dapper;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using Volo.Abp.Validation;
using VTECHERP.Constants;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.BO;
using VTECHERP.DTOs.BO.Tenants.Requests;
using VTECHERP.DTOs.BO.Tenants.Responses;
using VTECHERP.DTOs.Entries;
using VTECHERP.DTOs.Stores;
using VTECHERP.Entities;
using VTECHERP.Permissions;
using VTECHERP.ServiceInterfaces;
using VTECHERP.Services;
using static VTECHERP.Permissions.VTECHERPPermissions;
using static VTECHERP.Permissions.VTECHERPPermissions.Product;

namespace VTECHERP.BO
{
    [Route("api/bo/tenant-management/[action]")]
    [Authorize]
    public class TenantManagementApplication : ApplicationService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ITenantManager _tenantManager;
        private readonly IdentityRoleManager _identityRoleManager;
        private readonly IdentityUserManager _userManager;
        private readonly IConfiguration _configuration;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IPermissionManager _permissionManager;
        private readonly IRepository<Products> _productRepository;
        private readonly IRepository<Stores> _storesRepository;
        private readonly IRepository<VTECHERP.Entities.UserStore> _userstoresRepository;
        private readonly IRepository<StoreProduct> _storeproductRepository;
        private readonly ICurrentTenant _currentTenant;
        private readonly ICurrentUser _currentUser;
        private readonly IPermissionService _permissionService;

        private readonly IRepository<Enterprise> _enterpriseRepository;
        private readonly IRepository<Agency> _agencyRepository;
        private readonly ICommonService _commonService;

        private readonly IRepository<Provinces> _provinceRepository;
        private readonly IRepository<Districts> _districtRepository;
        private readonly IRepository<PermissionModule> _permissionModuleRepository;
        private readonly IRepository<PermissionGroup> _permissionGroupRepository;
        private readonly IRepository<Permission> _permissionRepository;
        private readonly IRepository<IdentityUser> _iuserRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        private readonly IRepository<BillCustomer> _billCustomerReposotory;
        private readonly IRepository<PermissionGrant> _permissionGrantReposotory;

        private readonly IDataFilter _dataFilter;
        private readonly IObjectMapper _objectMapper;

        private readonly ILogger<TenantManagementApplication> _logger;

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

        public TenantManagementApplication(
            IdentityUserManager userManager,
            IdentityRoleManager identityRoleManager,
            ITenantRepository tenantRepository,
            ITenantManager tenantManager,
            IConfiguration configuration,
            IIdentityUserRepository userRepository,
            IPermissionManager permissionManager,
            IRepository<Products> productRepository,
            IRepository<Stores> storesRepository,
            IRepository<VTECHERP.Entities.UserStore> userstoresRepository,
            IRepository<StoreProduct> storeproductRepository,
            ICurrentTenant currentTenant,
            ICurrentUser currentUser,
            IPermissionService permissionService,
            IRepository<Enterprise> enterpriseRepository,
            IRepository<Agency> agencyRepository,
            ICommonService commonService,
            IRepository<Provinces> provinceRepository,
            IRepository<Districts> districtRepository,
            IRepository<PermissionModule> permissionModuleRepository,
            IRepository<PermissionGroup> permissionGroupRepository,
            IRepository<Permission> permissionRepository,
            IRepository<IdentityUser> iuserRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<BillCustomer> billCustomerReposotory,
            IRepository<PermissionGrant> permissionGrantReposotory,
            IDataFilter dataFilter,
            IObjectMapper objectMapper,
            ILogger<TenantManagementApplication> logger,
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
            IRepository<Entities.Debt> debtRepository
            )
        {
            _tenantRepository = tenantRepository;
            _tenantManager = tenantManager;
            _userManager = userManager;
            _identityRoleManager = identityRoleManager;
            _configuration = configuration;
            _userRepository = userRepository;
            _permissionManager = permissionManager;
            _productRepository = productRepository;
            _storesRepository = storesRepository;
            _userstoresRepository = userstoresRepository;
            _storeproductRepository = storeproductRepository;
            _currentTenant = currentTenant;
            _currentUser = currentUser;
            _permissionService = permissionService;

            _enterpriseRepository = enterpriseRepository;
            _agencyRepository = agencyRepository;
            _commonService = commonService;

            _provinceRepository = provinceRepository;
            _districtRepository = districtRepository;
            _permissionModuleRepository = permissionModuleRepository;
            _permissionGroupRepository = permissionGroupRepository;
            _permissionRepository = permissionRepository;
            _iuserRepository = iuserRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _billCustomerReposotory = billCustomerReposotory;
            _permissionGrantReposotory = permissionGrantReposotory;
            _dataFilter = dataFilter;
            _objectMapper = objectMapper;

            _transportInformationRepository = transportInformationRepository;
            _paymentReceiptRepository = paymentReceiptRepository;
            _carrierShippingInformationRepository = carrierShippingInformationRepository;
            _priceTableStoreRepository = priceTableStoreRepository;
            _suppliersRepository = suppliersRepository;
            _billCustomerRepository = billCustomerRepository;
            _saleOrdersRepository = saleOrdersRepository;
            _storeproductRepository = storeproductRepository;
            _accountRepository = accountRepository;
            _customerRepository = customerRepository;
            _warehouseTransferBillRepository = warehouseTransferBillRepository;
            _warehousingBillRepository = warehousingBillRepository;
            _debtRepository = debtRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<TenantDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Tenant, TenantDto>(
                await _tenantRepository.GetAsync(id)
            );
        }

        [HttpPost]
        public async Task<PagingResponse<TenantDto>> GetListAsync()
        {
            var count = await _tenantRepository.GetCountAsync();
            var list = await _tenantRepository.GetListAsync();

            return new PagingResponse<TenantDto>(
                (int)count,
                ObjectMapper.Map<List<Tenant>, List<TenantDto>>(list)
            );
        }

        [HttpGet]
        public async Task<bool> ChangePassword(Guid tenantId, Guid id, string password)
        {
            using (CurrentTenant.Change(tenantId))
            {
                var user = await _userManager.GetByIdAsync(id);
                if (user == null)
                    return false;
                else
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _userManager.ResetPasswordAsync(user, token, "1q2w3E*");
                    return true;
                }
            }
            return false;
        }

        [HttpPost]
        [AllowAnonymous]
        [Authorize(TenantManagementPermissions.Tenants.Create)]
        public async Task<bool> CreateAsync(VTECHERP.DTOs.BO.CreateTenantRequest request)
        {
            try
            {
                // tạo tenant mới
                var tenant = await _tenantManager.CreateAsync(request.Name);

                // set connection string vtech hoặc đại lý
                if (request.IsVtech)
                {
                    var connectionString = _configuration["ConnectionStrings:VTech"];
                    tenant.SetDefaultConnectionString(connectionString);
                }
                else
                {
                    var connectionString = _configuration["ConnectionStrings:Agency"];
                    tenant.SetDefaultConnectionString(connectionString);
                }

                await _tenantRepository.InsertAsync(tenant);
                await CurrentUnitOfWork.SaveChangesAsync();

                using (CurrentTenant.Change(tenant.Id))
                {
                    // tạo roles mặc định của tenant
                    await _identityRoleManager.CreateAsync(new IdentityRole(GuidGenerator.Create(), "admin", tenant.Id));
                    await CurrentUnitOfWork.SaveChangesAsync();
                    // tạo permission mặc định của tenant
                    //await _permissionManager
                    //.SetForRoleAsync("admin", TenantManagementPermissions.Tenants.Default, true);
                    //   await _permissionManager
                    //.SetForRoleAsync("admin", TenantManagementPermissions.Tenants.Create, true);
                    //   await _permissionManager
                    //.SetForRoleAsync("admin", TenantManagementPermissions.Tenants.Update, true);
                    //   await _permissionManager
                    //.SetForRoleAsync("admin", TenantManagementPermissions.Tenants.Delete, true);
                    //   await _permissionManager
                    //.SetForRoleAsync("admin", TenantManagementPermissions.Tenants.ManageFeatures, true);
                    //   await _permissionManager
                    //.SetForRoleAsync("admin", TenantManagementPermissions.Tenants.ManageConnectionStrings, true);
                    await _permissionManager
                        .SetForRoleAsync("admin", IdentityPermissions.Roles.Default, true);
                    await _permissionManager
                        .SetForRoleAsync("admin", IdentityPermissions.Roles.Create, true);
                    await _permissionManager
                        .SetForRoleAsync("admin", IdentityPermissions.Roles.Update, true);
                    await _permissionManager
                        .SetForRoleAsync("admin", IdentityPermissions.Roles.Delete, true);
                    await _permissionManager
                        .SetForRoleAsync("admin", IdentityPermissions.Roles.ManagePermissions, true);
                    await _permissionManager
                        .SetForRoleAsync("admin", IdentityPermissions.Users.Default, true);
                    await _permissionManager
                        .SetForRoleAsync("admin", IdentityPermissions.Users.Create, true);
                    await _permissionManager
                        .SetForRoleAsync("admin", IdentityPermissions.Users.Update, true);
                    await _permissionManager
                        .SetForRoleAsync("admin", IdentityPermissions.Users.Delete, true);
                    await _permissionManager
                        .SetForRoleAsync("admin", IdentityPermissions.Users.ManagePermissions, true);
                    await CurrentUnitOfWork.SaveChangesAsync();

                    // tạo tài khoản admin của tenant
                    var user = new IdentityUser(GuidGenerator.Create(), request.AdminUserName, request.AdminEmail, tenant.Id);
                    user.Name = request.AdminName;
                    (await _userManager.CreateAsync(user, request.AdminPassword)).CheckErrors();
                    await _userManager.SetEmailAsync(user, request.AdminEmail);
                    await _userManager.AddDefaultRolesAsync(user);
                    await _userManager.AddToRoleAsync(user, "admin");
                    await CurrentUnitOfWork.SaveChangesAsync();
                    var listProduct = new List<Products>();
                    var listImportProduct = new List<Products>();
                    using (CurrentTenant.Change(null))
                    {
                        //tạo danh sách sản phẩm
                        //1: lấy danh sách sản phẩm
                        listProduct = await _productRepository.GetListAsync(x => x.TenantId == null);
                    }
                    using (CurrentTenant.Change(tenant.Id))
                    {
                        foreach (var product in listProduct)
                        {
                            var p = new Products()
                            {
                                Id = Guid.NewGuid(),
                                TenantId = tenant.Id,
                                Name = product.Name,
                                ParentId = product.ParentId,
                                ParentCode = product.Code,
                                ParentName = product.Name,
                                Attachments = product.Attachments,
                                BarCode = product.BarCode,
                                CategoryId = product.CategoryId,
                                Code = product.Code,
                                ConcurrencyStamp = product.ConcurrencyStamp,
                                StockPrice = product.StockPrice,
                                StockPriceBeforeLatest = product.StockPriceBeforeLatest,
                                CreationTime = product.CreationTime,
                                CreatorId = product.CreatorId,
                                DeleterId = product.DeleterId,
                                DeletionTime = product.DeletionTime,
                                Type = product.Type,
                                Enterprise = product.Enterprise,
                                OtherName = product.OtherName,
                                Unit = product.Unit,
                                Weight = product.Weight,
                                VAT = product.VAT,
                                OldPrice = product.OldPrice,
                                Profit = product.Profit,
                                WholeSalePrice = product.WholeSalePrice,
                                Status = product.Status,
                                SupplierId = product.SupplierId,
                                WebsiteLink = product.WebsiteLink,
                                Height = product.Height,
                                Width = product.Width,
                                Length = product.Length,
                                SPAPrice = product.SPAPrice,
                                OrganizationId = product.OrganizationId,
                            };
                            listImportProduct.Add(p);
                        }

                        await _productRepository.InsertManyAsync(listImportProduct);
                    }
                    //2: lấy danh sách tài khoản kế toán
                    //đổi tennatId sang BO
                    var listAccount = new List<Account>();
                    using (CurrentTenant.Change(null))
                    {
                        //lấy danh sách tài khoản kế toán trên BO
                        listAccount = await _accountRepository.GetListAsync();
                    }
                    using (CurrentTenant.Change(tenant.Id))
                    {
                        var listAccountImport = new List<Account>();
                        {
                            foreach (var item in listAccount)
                            {
                                var acc = new Account()
                                {
                                    Id = Guid.NewGuid(),
                                    Code = item.Code,
                                    Name = item.Name,
                                    CreationTime = DateTime.Now,
                                    TenantId = tenant.Id,
                                    ParentAccountCode = item.ParentAccountCode,
                                    AccountType = item.AccountType,
                                };
                                listAccountImport.Add(acc);
                            }
                            // tạo cửa hàng
                            var storeInsert = ObjectMapper.Map<StoreDto, Stores>(request.Store);
                            storeInsert.TenantId = tenant.Id;
                            storeInsert.Id = Guid.NewGuid();
                            storeInsert.IsMainStore = true;
                            await _storesRepository.InsertAsync(storeInsert);
                            //phân quyền cho user truy cập cửa hàng
                            var userstore = new VTECHERP.Entities.UserStore();
                            {
                                userstore.Id = Guid.NewGuid();
                                userstore.UserId = user.Id;
                                userstore.StoreId = storeInsert.Id;
                                userstore.TenantId = tenant.Id;
                            }
                            await _userstoresRepository.InsertAsync(userstore);
                            // tạo storeProduct
                            var storeproductImport = new List<StoreProduct>();
                            foreach (var item in listImportProduct)
                            {
                                var sp = new StoreProduct
                                {
                                    Id = Guid.NewGuid(),
                                    StoreId = storeInsert.Id,
                                    ProductId = item.Id,
                                    StockQuantity = 0,
                                    QuantityBeforeLatest = 0,
                                    TenantId = tenant.Id,
                                };
                                storeproductImport.Add(sp);
                            }
                            await _storeproductRepository.InsertManyAsync(storeproductImport);
                        }
                        await _accountRepository.InsertManyAsync(listAccountImport);
                    }
                    await _permissionService.InitializeTenantPermissions(tenant.Id);
                }
                await CurrentUnitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception e)
            {
                await CurrentUnitOfWork.RollbackAsync();
                throw;
            }
        }

        [HttpPost]
        [Authorize()]
        public async Task<bool> CreateUserAsync(CreatedUserRequest request)
        {
            try
            {
                // lấy tennantId
                var tenantId = _currentTenant.Id;
                using (CurrentTenant.Change(tenantId))
                {
                    // tạo roles mặc định của tenant
                    await _identityRoleManager.CreateAsync(new IdentityRole(GuidGenerator.Create(), "admin", tenantId));
                    await CurrentUnitOfWork.SaveChangesAsync();
                    // tạo permission mặc định của tenant
                    await _permissionManager
                  .SetForRoleAsync("admin", IdentityPermissions.Roles.Default, true);
                    await _permissionManager
                 .SetForRoleAsync("admin", IdentityPermissions.Roles.Create, true);
                    await _permissionManager
                 .SetForRoleAsync("admin", IdentityPermissions.Roles.Update, true);
                    await _permissionManager
                 .SetForRoleAsync("admin", IdentityPermissions.Roles.Delete, true);
                    await _permissionManager
                 .SetForRoleAsync("admin", IdentityPermissions.Roles.ManagePermissions, true);
                    await _permissionManager
                 .SetForRoleAsync("admin", IdentityPermissions.Users.Default, true);
                    await _permissionManager
                 .SetForRoleAsync("admin", IdentityPermissions.Users.Create, true);
                    await _permissionManager
                 .SetForRoleAsync("admin", IdentityPermissions.Users.Update, true);
                    await _permissionManager
                 .SetForRoleAsync("admin", IdentityPermissions.Users.Delete, true);
                    await _permissionManager
                 .SetForRoleAsync("admin", IdentityPermissions.Users.ManagePermissions, true);
                    await CurrentUnitOfWork.SaveChangesAsync();

                    // tạo tài khoản mới
                    var user = new IdentityUser(GuidGenerator.Create(), request.UserName, request.Email, tenantId);
                    user.Name = request.Name;
                    (await _userManager.CreateAsync(user, request.Password)).CheckErrors();
                    await _userManager.SetEmailAsync(user, request.Email);
                    await _userManager.AddDefaultRolesAsync(user);
                    await _userManager.AddToRoleAsync(user, "admin");
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
                await CurrentUnitOfWork.CompleteAsync();
                return true;
            }
            catch
            {
                await CurrentUnitOfWork.RollbackAsync();
                throw;
            }
        }

        [HttpPost]
        [Authorize()]
        public async Task<bool> CreateNewStoreAsync(StoreDto request)
        {
            var tenantId = _currentTenant.Id;
            var userId = _currentUser.Id;
            var storeInsert = ObjectMapper.Map<StoreDto, Stores>(request);
            storeInsert.TenantId = tenantId;
            storeInsert.Id = Guid.NewGuid();
            await _storesRepository.InsertAsync(storeInsert);
            //phân quyền cho user truy cập cửa hàng
            var userstore = new VTECHERP.Entities.UserStore();
            {
                userstore.Id = Guid.NewGuid();
                userstore.UserId = userId ?? Guid.NewGuid();
                userstore.StoreId = storeInsert.Id;
                userstore.TenantId = tenantId;
            }
            await _userstoresRepository.InsertAsync(userstore);
            await CurrentUnitOfWork.CompleteAsync();
            return true;
        }

        [HttpDelete]
        [Authorize(TenantManagementPermissions.Tenants.Delete)]
        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var tenant = await _tenantRepository.FindAsync(id);
                if (tenant == null)
                {
                    return;
                }

                using (CurrentTenant.Change(tenant.Id))
                {
                    var tenantUsers = await _userRepository.GetListAsync();
                    await _userRepository.DeleteManyAsync(tenantUsers);
                }

                await _tenantRepository.DeleteAsync(tenant);
                await CurrentUnitOfWork.SaveChangesAsync();
                await CurrentUnitOfWork.CompleteAsync();
            }
            catch
            {
                await CurrentUnitOfWork.RollbackAsync();
                throw;
            }
        }

        [HttpGet]
        [Authorize(TenantManagementPermissions.Tenants.ManageConnectionStrings)]
        public async Task<string> GetDefaultConnectionStringAsync(Guid id)
        {
            var tenant = await _tenantRepository.GetAsync(id);
            return tenant?.FindDefaultConnectionString();
        }

        #region VBN

        [HttpPost]
        public async Task<PagingResponse<ListTenantResponse>> ListTenantAsync(ListTenantRequest request)
        {
            var enterprises = new List<Enterprise>();
            var agencies = new List<Agency>();
            using (_dataFilter.Disable<IMultiTenant>())
            {
                enterprises = (await _enterpriseRepository.GetQueryableAsync())
                .WhereIf(request.From.HasValue, x => x.CreationTime.Date >= request.From.Value.Date)
                .WhereIf(request.To.HasValue, x => x.CreationTime.Date <= request.To.Value.Date)
                .WhereIf(!request.Code.IsNullOrWhiteSpace(), x => x.Code == request.Code)
                .WhereIf(!request.Name.IsNullOrWhiteSpace(), x => x.Name.Trim().ToLower().Contains(request.Name.Trim().ToLower()))
                .WhereIf(request.Status.HasValue, x => x.Status == request.Status.Value).ToList() ?? new List<Enterprise>();

                agencies = (await _agencyRepository.GetQueryableAsync())
                    .WhereIf(request.From.HasValue, x => x.CreationTime.Date >= request.From.Value.Date)
                    .WhereIf(request.To.HasValue, x => x.CreationTime.Date <= request.To.Value.Date)
                    .WhereIf(!request.Code.IsNullOrWhiteSpace(), x => x.Code == request.Code)
                    .WhereIf(!request.Name.IsNullOrWhiteSpace(), x => x.Name.Trim().ToLower().Contains(request.Name.Trim().ToLower()))
                    .WhereIf(request.Status.HasValue, x => x.Status == request.Status.Value).ToList() ?? new List<Agency>();
            }

            var users = await _userRepository.GetListAsync();

            var vTechStores = new List<Stores>();
            using (var sqlConnectionVTech = new SqlConnection(_configuration["ConnectionStrings:VTech"]))
            {
                sqlConnectionVTech.Open();
                string queryStores = "SELECT Id, TenantId FROM Stores WHERE isDeleted = 0";
                vTechStores = sqlConnectionVTech.Query<Stores>(queryStores).ToList() ?? new List<Stores>();
            }

            var agencyStores = new List<Stores>();
            using (var sqlConnectionAgency = new SqlConnection(_configuration["ConnectionStrings:Agency"]))
            {
                sqlConnectionAgency.Open();
                string queryStores = "SELECT Id, TenantId FROM Stores WHERE isDeleted = 0";
                agencyStores = sqlConnectionAgency.Query<Stores>(queryStores).ToList() ?? new List<Stores>();
            }

            var tenantResponses = new List<ListTenantResponse>();
            if (!request.IsEnterprise.HasValue || request.IsEnterprise == true)
            {
                if (enterprises.Any())
                {
                    foreach (var enterprise in enterprises)
                    {
                        var vTechStorebyTenantId = vTechStores.Where(s => s.TenantId == enterprise.TenantId) ?? new List<Stores>();
                        var user = users.FirstOrDefault(x => x.Id == enterprise.CreatorId);
                        tenantResponses.Add(new ListTenantResponse()
                        {
                            Id = enterprise.Id,
                            Code = enterprise.Code,
                            Name = enterprise.Name,
                            IsEnterprise = true,
                            PhoneNumber = enterprise.PhoneNumber,
                            ProvinceId = enterprise.ProvinceId,
                            DistrictId = enterprise.DistrictId,
                            Address = enterprise.Address,
                            StoreCount = vTechStorebyTenantId.Count(),
                            Expiration = enterprise.Expiration,
                            Status = enterprise.Status,
                            CreateTime = enterprise.CreationTime,
                            CreatorName = user == null ? null : user.Name,
                            TenantId = enterprise.TenantId.Value,
                        });
                    }
                }
            }
            if (!request.IsEnterprise.HasValue || request.IsEnterprise == false)
            {
                if (agencies.Any())
                {
                    foreach (var agency in agencies)
                    {
                        var agencyStorebyTenantId = agencyStores.Where(s => s.TenantId == agency.TenantId) ?? new List<Stores>();
                        var user = users.FirstOrDefault(x => x.Id == agency.CreatorId);
                        tenantResponses.Add(new ListTenantResponse()
                        {
                            Id = agency.Id,
                            Code = agency.Code,
                            Name = agency.Name,
                            IsEnterprise = false,
                            PhoneNumber = agency.PhoneNumber,
                            ProvinceId = agency.ProvinceId,
                            DistrictId = agency.DistrictId,
                            Address = agency.Address,
                            StoreCount = agencyStorebyTenantId.Count(),
                            Expiration = agency.Expiration,
                            Status = agency.Status,
                            CreateTime = agency.CreationTime,
                            CreatorName = user == null ? null : user.Name,
                            TenantId = agency.TenantId.Value,
                        });
                    }
                }
            }

            var result = tenantResponses
                .OrderByDescending(x => x.CreateTime)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PagingResponse<ListTenantResponse>(tenantResponses.Count(), result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTenantAsync(VTECHERP.DTOs.BO.Tenants.Requests.CreateTenantRequest request)
        {
            await ValidateCreateTenantAsync(request);
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                // tạo tenant mới
                var tenant = await _tenantManager.CreateAsync(request.Institute);

                tenant.SetProperty("IsVTech", request.IsEnterprise);

                if (request.IsEnterprise)
                {
                    var connectionString = _configuration["ConnectionStrings:VTech"];
                    tenant.SetDefaultConnectionString(connectionString);
                }
                else
                {
                    var connectionString = _configuration["ConnectionStrings:Agency"];
                    tenant.SetDefaultConnectionString(connectionString);
                }

                await _tenantRepository.InsertAsync(tenant);
                await uow.SaveChangesAsync();

                var permissionModules = (await _permissionModuleRepository.GetQueryableAsync()).Where(x => x.TenantId == null).ToList() ?? new List<PermissionModule>();
                var permissionGroups = (await _permissionGroupRepository.GetQueryableAsync()).Where(x => x.TenantId == null).ToList() ?? new List<PermissionGroup>();
                var permissions = (await _permissionRepository.GetQueryableAsync()).Where(x => x.TenantId == null).ToList() ?? new List<Permission>();
                var user = new IdentityUser(GuidGenerator.Create(), request.UserName, request.Email, tenant.Id);
                var isActive = true;
                if (request.Status == Enums.TenantExtension.Status.InActive || request.Status == null)
                {
                    user.SetIsActive(false);
                    request.Status = Enums.TenantExtension.Status.InActive;
                    isActive = false;
                }    
                    
                using (CurrentTenant.Change(tenant.Id))
                {
                    var role = new IdentityRole(GuidGenerator.Create(), "admin", tenant.Id);
                    // tạo roles mặc định của tenant
                    role.SetProperty("CreatorId", user.Id);
                    await _identityRoleManager.CreateAsync(role);

                    await uow.SaveChangesAsync();

                    #region Tạo cửa hàng chính (Store)

                    var store = new Stores()
                    {
                        Id = Guid.NewGuid(),
                        Name = request.Name,
                        ProvinceId = request.ProvinceId ?? Guid.Empty,
                        DistricId = request.DistrictId ?? Guid.Empty,
                        Address = request.Address,
                        PhoneNumber = request.PhoneNumber,
                        ExpriDate = request.Expiration,
                        IsMainStore = true,
                        Email = request.Email,
                        CreationTime = DateTime.Now,
                        CreatorId = _currentUser.Id,
                        IsActive = isActive,
                    };
                    await _storesRepository.InsertAsync(store);
                    await uow.SaveChangesAsync();

                    #endregion Tạo cửa hàng chính (Store)

                    #region tạo tài khoản admin của tenant

                    user.Name = request.Name;
                    var password = "1q2w3E*";
                    if (!request.Password.IsNullOrWhiteSpace())
                        password = request.Password;
                    (await _userManager.CreateAsync(user, request.Password ?? password)).CheckErrors();
                    await _userManager.SetPhoneNumberAsync(user, request.PhoneNumber);
                    await _userManager.AddDefaultRolesAsync(user);
                    await _userManager.AddToRoleAsync(user, "admin");
                    user.SetProperty("MainStoreId", store.Id);
                    if (!request.IsEnterprise)
                        user.SetProperty("IsVTech", false);
                    await uow.SaveChangesAsync();

                    #endregion tạo tài khoản admin của tenant

                    #region Tạo mối quan hệ giữa người dùng và cửa hàng (UserStore)

                    var userStore = new Entities.UserStore()
                    {
                        Id = Guid.NewGuid(),
                        StoreId = store.Id,
                        UserId = user.Id,
                        IsDefault = true,
                        CreationTime = DateTime.Now,
                        CreatorId = _currentUser.Id,
                        IsActive = isActive,
                    };
                    await _userstoresRepository.InsertAsync(userStore);
                    await uow.SaveChangesAsync();

                    #endregion Tạo mối quan hệ giữa người dùng và cửa hàng (UserStore)

                    await AddPermissionByTenantIdAsync(permissionModules, permissionGroups, permissions, request.Ids, tenant.Id, request.IsEnterprise);
                }

                #region set connection string vtech hoặc đại lý

                if (request.IsEnterprise)
                {
                    var enterprise = _objectMapper.Map<VTECHERP.DTOs.BO.Tenants.Requests.CreateTenantRequest, Enterprise>(request);
                    enterprise.TenantId = tenant.Id;
                    enterprise.CreatorId = _currentUser.Id;
                    enterprise.UserId = user.Id;
                    await _enterpriseRepository.InsertAsync(enterprise);
                }
                else
                {
                    var agency = _objectMapper.Map<VTECHERP.DTOs.BO.Tenants.Requests.CreateTenantRequest, Agency>(request);
                    agency.TenantId = tenant.Id;
                    agency.CreatorId = _currentUser.Id;
                    agency.UserId = user.Id;
                    await _agencyRepository.InsertAsync(agency);
                }

                #endregion set connection string vtech hoặc đại lý

                await uow.CompleteAsync();
                return new OkObjectResult(new
                {
                    StatusCode = 200,
                    Message = "Tạo mới thành công"
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

        [HttpGet]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var result = new DetailTenant();
                var enterprise = (await _enterpriseRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == id);
                if (enterprise == null)
                {
                    var agency = (await _agencyRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == id);
                    if (agency == null)
                        return new ObjectResult($"Id: '{id}' {ErrorMessages.TenantError.NotExist}")
                        {
                            StatusCode = 404,
                        };

                    result = _objectMapper.Map<Agency, DetailTenant>(agency);
                    result.IsEnterprise = false;
                    var tenantByAgency = await _tenantRepository.FindAsync(agency.TenantId.Value);
                    result.Institute = tenantByAgency == null ? null : tenantByAgency.Name;
                    using (CurrentTenant.Change(tenantByAgency.Id))
                    {
                        var user = (await _iuserRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == agency.UserId);
                        result.UserName = user == null ? null : user.UserName;
                    }
                    return new OkObjectResult(new
                    {
                        StatusCode = 200,
                        Data = result,
                        Message = "Thông tin chi tiết đại lý."
                    });
                }
                result = _objectMapper.Map<Enterprise, DetailTenant>(enterprise);
                result.IsEnterprise = true;
                var tenantByEnterprise = await _tenantRepository.FindAsync(enterprise.TenantId.Value);
                result.Institute = tenantByEnterprise == null ? null : tenantByEnterprise.Name;
                using (CurrentTenant.Change(tenantByEnterprise.Id))
                {
                    var user = (await _iuserRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == enterprise.UserId);
                    result.UserName = user == null ? null : user.UserName;
                }
                return new OkObjectResult(new
                {
                    StatusCode = 200,
                    Data = result,
                    Message = "Thông tin chi tiết doanh nghiệp."
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTenantAsync(VTECHERP.DTOs.BO.Tenants.Requests.UpdateTenantRequest request)
        {
            await ValidateUpdateTenantAsync(request);
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var enterprise = new Enterprise();
                var agency = new Agency();
                using (_dataFilter.Disable<IMultiTenant>())
                {
                    enterprise = (await _enterpriseRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == request.Id);
                    agency = (await _agencyRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == request.Id);
                }

                if (enterprise == null)
                {
                    if (request.ProvinceId.HasValue && request.ProvinceId.Value != agency.ProvinceId.Value)
                        agency.ProvinceId = request.ProvinceId;
                    if (request.DistrictId.HasValue && request.DistrictId.Value != agency.DistrictId.Value)
                        agency.DistrictId = request.DistrictId;
                    if (!request.Address.IsNullOrWhiteSpace() && request.Address != agency.Address)
                        agency.Address = request.Address;
                    if (request.Expiration.HasValue && request.Expiration != agency.Expiration)
                        agency.Expiration = request.Expiration;
                    agency.Note = request.Note;
                    if (request.Status.HasValue && request.Status != agency.Status)
                    {
                        agency.Status = request.Status.Value;
                        var status = false;
                        if (request.Status == Enums.TenantExtension.Status.Active)
                            status = true;
                        using (CurrentTenant.Change(agency.TenantId))
                        {
                            var users = (await _iuserRepository.GetQueryableAsync()).ToList();
                            users.ForEach(user =>
                            {
                                user.SetIsActive(status);
                            });

                            var stores = (await _storesRepository.GetQueryableAsync()).ToList();
                            stores.ForEach(store =>
                            {
                                store.IsActive = status;
                            });

                            await uow.SaveChangesAsync();
                        }
                    }
                    await _agencyRepository.UpdateAsync(agency);
                    await uow.SaveChangesAsync();
                    await uow.CompleteAsync();
                    return new OkObjectResult(new
                    {
                        StatusCode = 200,
                        Message = "Cập nhật thành công."
                    });
                }

                if (request.ProvinceId.HasValue && request.ProvinceId.Value != enterprise.ProvinceId.Value)
                    enterprise.ProvinceId = request.ProvinceId;
                if (request.DistrictId.HasValue && request.DistrictId.Value != enterprise.DistrictId.Value)
                    enterprise.DistrictId = request.DistrictId;
                if (!request.Address.IsNullOrWhiteSpace() && request.Address != enterprise.Address)
                    enterprise.Address = request.Address;
                if (request.Expiration.HasValue && request.Expiration != enterprise.Expiration)
                    enterprise.Expiration = request.Expiration;
                enterprise.Note = request.Note;
                if (request.Status.HasValue && request.Status != enterprise.Status)
                {
                    enterprise.Status = request.Status.Value;
                    var status = false;
                    if (request.Status == Enums.TenantExtension.Status.Active)
                        status = true;
                    using (CurrentTenant.Change(enterprise.TenantId))
                    {
                        var users = (await _iuserRepository.GetQueryableAsync()).ToList();
                        users.ForEach(user =>
                        {
                            user.SetIsActive(status);
                        });

                        var stores = (await _storesRepository.GetQueryableAsync()).ToList();
                        stores.ForEach(store =>
                        {
                            store.IsActive = status;
                        });

                        await uow.SaveChangesAsync();
                    }
                }
                await _enterpriseRepository.UpdateAsync(enterprise);
                await uow.SaveChangesAsync();
                await uow.CompleteAsync();
                return new OkObjectResult(new
                {
                    StatusCode = 200,
                    Message = "Cập nhật thành công."
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

        [HttpDelete]
        public async Task<IActionResult> DeleteTenantAsync(Guid id)
        {
            await ValidateDeleteTenantAsync(id);
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var enterprise = new Enterprise();
                var agency = new Agency();
                var connection = _configuration["ConnectionStrings:VTech"];
                using (_dataFilter.Disable<IMultiTenant>())
                {
                    enterprise = (await _enterpriseRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == id);
                    if (enterprise == null)
                    {
                        connection = _configuration["ConnectionStrings:Agency"];
                        agency = (await _agencyRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == id);
                        agency.IsDeleted = true;
                        await _agencyRepository.UpdateAsync(agency);
                        await uow.SaveChangesAsync();
                    }
                    else
                    {
                        enterprise.IsDeleted = true;
                        await _enterpriseRepository.UpdateAsync(enterprise);
                        await uow.SaveChangesAsync();
                    }
                }
                var tenantId = enterprise != null ? enterprise.TenantId : agency.TenantId;
                await _commonService.ClearDataInTables(connection, tenantId.Value, "IsDeleted");
                await _tenantRepository.DeleteAsync(tenantId.Value);
                await uow.SaveChangesAsync();
                await uow.CompleteAsync();
                return new OkObjectResult(new
                {
                    StatusCode = 200,
                    Message = "Đã xóa các thông tin liên quan."
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

        [HttpGet]
        public async Task<List<VTECHERP.DTOs.BO.Tenants.Responses.LinkEnterpriseResponse>> GetLinkEnterprise()
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var result = new List<VTECHERP.DTOs.BO.Tenants.Responses.LinkEnterpriseResponse>();
                var enterprises = (await _enterpriseRepository.GetQueryableAsync()).Where(x=>x.Status == Enums.TenantExtension.Status.Active).ToList();
                if (!enterprises.Any())
                    return result;
                result = _objectMapper.Map<List<Enterprise>, List<VTECHERP.DTOs.BO.Tenants.Responses.LinkEnterpriseResponse>>(enterprises);
                return result;
            }
        }

        [HttpPost]
        public async Task<bool> ValidateCreateTenantAsync(VTECHERP.DTOs.BO.Tenants.Requests.CreateTenantRequest request)
        {
            var validationErrors = new List<ValidationResult>();

            if (request == null)
            {
                validationErrors.Add(new ValidationResult($"{ErrorMessages.Common.RequestNotNull}"));
                throw new AbpValidationException(validationErrors);
            }

            var enterprises = new List<Enterprise>();
            var agencies = new List<Agency>();

            using (_dataFilter.Disable<IMultiTenant>())
            {
                enterprises = await _enterpriseRepository.GetListAsync();
                agencies = await _agencyRepository.GetListAsync();
            }

            if (request.Name.IsNullOrWhiteSpace())
            {
                if (request.IsEnterprise)
                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.Name} {ErrorMessages.TenantError.Enterprise} {ErrorMessages.TenantError.NotNull}"));
                else
                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.Name} {ErrorMessages.TenantError.Agency} {ErrorMessages.TenantError.NotNull}"));
            }
            else
            {
                var checkNameEnterprise = enterprises.FirstOrDefault(x => x.Name.ToLower() == request.Name.ToLower());
                var checkNameAgency = agencies.FirstOrDefault(x => x.Name.ToLower() == request.Name.ToLower());
                if (checkNameEnterprise != null || checkNameAgency != null)
                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.Name} {ErrorMessages.TenantError.IsExist}"));
            }

            if (request.PhoneNumber.IsNullOrWhiteSpace())
                validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.PhoneNumber} {ErrorMessages.TenantError.NotNull}"));
            else
            {
                var checkPhoneNumberEnterprise = enterprises.FirstOrDefault(x => x.PhoneNumber == request.PhoneNumber);
                var checkPhoneNumberAgency = agencies.FirstOrDefault(x => x.PhoneNumber == request.PhoneNumber);
                if (checkPhoneNumberEnterprise != null || checkPhoneNumberAgency != null)
                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.PhoneNumber} {ErrorMessages.TenantError.IsExist}"));

                if (!_commonService.ValidatePhoneNumber(request.PhoneNumber))
                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.PhoneNumber} {ErrorMessages.TenantError.InCorrect}"));
            }

            if (request.Email.IsNullOrWhiteSpace())
                validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.Email} {ErrorMessages.TenantError.NotNull}"));
            else
            {
                var checkEmailEnterprise = enterprises.FirstOrDefault(x => x.Email.ToLower() == request.Email.ToLower());
                var checkEmailAgency = agencies.FirstOrDefault(x => x.Email.ToLower() == request.Email.ToLower());
                if (checkEmailEnterprise != null || checkEmailAgency != null)
                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.Email} {ErrorMessages.TenantError.IsExist}"));

                if (!_commonService.ValidateEmail(request.Email))
                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.Email} {ErrorMessages.TenantError.InCorrect}"));
            }

            if (request.Address.IsNullOrWhiteSpace())
                validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.Address} {ErrorMessages.TenantError.NotNull}"));

            if (request.Expiration.HasValue)
                if (DateTime.Now.Date > request.Expiration.Value.Date)
                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.Expiration} {ErrorMessages.TenantError.InCorrect}"));

            if (!request.IsEnterprise)
            {
                if (!request.Expiration.HasValue)
                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.Expiration} {ErrorMessages.TenantError.NotNull}"));

                if (!request.EnterpriseId.HasValue)
                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.EnterpriseLink} {ErrorMessages.TenantError.NotNull}"));
                else
                {
                    var checkEnterpriseLink = enterprises.FirstOrDefault(x => x.Id == request.EnterpriseId.Value);
                    if (checkEnterpriseLink == null)
                        validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.EnterpriseLink} {ErrorMessages.TenantError.NotExist}"));

                    if (checkEnterpriseLink.Status == Enums.TenantExtension.Status.InActive)
                        validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.EnterpriseLink} {ErrorMessages.TenantError.InActive}"));
                }
            }

            if (request.Institute.IsNullOrWhiteSpace())
                validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.Institute} {ErrorMessages.TenantError.NotNull}"));
            else
            {
                var tenant = (await _tenantRepository.GetListAsync()).FirstOrDefault(x => x.Name.ToLower() == request.Institute.ToLower());
                if (tenant != null)
                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.Institute} {ErrorMessages.TenantError.IsExist}"));
            }

            var connection = _configuration["ConnectionStrings:VTech"];
            if (!request.IsEnterprise)
            {
                connection = _configuration["ConnectionStrings:Agency"];
            }
            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();
                var queryProvince = "SELECT Id, Code FROM Provinces WHERE IsDeleted = 0";
                var provinces = sqlConnection.Query<Provinces>(queryProvince).ToList() ?? new List<Provinces>();
                var province = provinces.FirstOrDefault(x => x.Id == request.ProvinceId.Value);
                if (!request.ProvinceId.HasValue)
                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.Province} {ErrorMessages.TenantError.NotNull}"));
                else
                {
                    if (province == null)
                        validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.Province} {ErrorMessages.TenantError.NotExist}"));
                }

                if (!request.DistrictId.HasValue)
                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.District} {ErrorMessages.TenantError.NotNull}"));
                else
                {
                    var queryDistrict = $"SELECT Id, ProvinceCode FROM Districts WHERE IsDeleted = 0 AND ProvinceCode = '{province.Code}'";
                    var districts = sqlConnection.Query<Districts>(queryDistrict).ToList() ?? new List<Districts>();
                    if (districts.FirstOrDefault(x => x.Id == request.DistrictId.Value) == null)
                        validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.District} {ErrorMessages.TenantError.NotExist}"));
                }

                if (request.UserName.IsNullOrWhiteSpace())
                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.UserName} {ErrorMessages.TenantError.NotNull}"));
                else
                {
                    string queryUser = "SELECT UserName, Email, PhoneNumber FROM AbpUsers WHERE IsDeleted = 0";
                    var users = sqlConnection.Query<IdentityUser>(queryUser).ToList() ?? new List<IdentityUser>();
                    if (users.Any())
                    {
                        var userByName = users.FirstOrDefault(x => x.UserName.ToLower() == request.UserName.ToLower());
                        if (userByName != null)
                            validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.UserName} {ErrorMessages.TenantError.IsExist}"));

                        var userByEmail = users.FirstOrDefault(x => x.Email.ToLower() == request.Email.ToLower());
                        if (userByEmail != null)
                            validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.Email} {ErrorMessages.TenantError.IsExist}"));

                        var userByPhone = users.FirstOrDefault(x => x.PhoneNumber == request.PhoneNumber);
                        if (userByPhone != null)
                            validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.PhoneNumber} {ErrorMessages.TenantError.IsExist}"));
                    }
                }

                if (!request.Password.IsNullOrWhiteSpace())
                    if (!_commonService.ValidatePassword(request.Password))
                        validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.Password} {ErrorMessages.TenantError.InCorrect}"));

                if (request.Ids.Any())
                {
                    using (var sqlConnectionVTech = new SqlConnection(_configuration["ConnectionStrings:VTech"]))
                    {
                        sqlConnectionVTech.Open();
                        string queryPermissions = "SELECT Id FROM Permissions WHERE isDeleted = 0 And TenantId IS NULL";
                        var permissions = sqlConnectionVTech.Query<Permission>(queryPermissions).ToList() ?? new List<Permission>();
                        if (!permissions.Any())
                            validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.Module} {ErrorMessages.TenantError.NotExist}"));
                        else
                        {
                            if (!request.Ids.All(id => permissions.Select(p => p.Id).Contains(id)))
                                validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.Module} {ErrorMessages.TenantError.NotExist}"));
                        }
                    }
                }
            }

            if (validationErrors.Any())
                throw new AbpValidationException(validationErrors);

            return true;
        }

        [HttpPost]
        public async Task<PagingResponse<StoreByTenantResponse>> ListStoreByTenantIdAsync(StoreByTenantRequest request)
        {
            
            using(CurrentTenant.Change(request.TenantId))
            {
                var result = new List<StoreByTenantResponse>();
                var stores = (await _storesRepository.GetQueryableAsync())
                    .WhereIf(!request.StoreName.IsNullOrWhiteSpace(), x => x.Name.Trim().ToLower().Contains(request.StoreName.Trim().ToLower())).ToList() ?? new List<Stores>();
                if (!stores.Any())
                    return new PagingResponse<StoreByTenantResponse>(0, result);

                result = (_objectMapper.Map<List<Stores>,List<StoreByTenantResponse>>(stores))
                    .OrderByDescending(x => x.Code)
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                return new PagingResponse<StoreByTenantResponse>(stores.Count, result);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateActionStoreAsync(UpdateActionStoreRequest request)
        {
            using (CurrentTenant.Change(request.TenantId))
            {
                var store = (await _storesRepository.GetQueryableAsync()).FirstOrDefault(x=>x.Id == request.StoreId);
                if(store == null)
                    return new ObjectResult("Không tìm thấy cửa hàng tương ứng.")
                    {
                        StatusCode = 404,
                    };
                store.IsActive = !store.IsActive;
                await _storesRepository.UpdateAsync(store);
                return new OkObjectResult(new
                {
                    StatusCode = 200,
                    Message = "Cập nhật trạng thái thành công.",
                    Data = store,
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ModuleManagerAsync(ModuleManagerRequest request)
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                using (CurrentTenant.Change(request.TenantId))
                {
                    var permissionTenantNull = new List<Permission>();
                    var permissionGroupTenantNull = new List<PermissionGroup>();
                    var permissionModuleTenantNull = new List<PermissionModule>();
                    using (_dataFilter.Disable<IMultiTenant>())
                    {
                        permissionTenantNull = (await _permissionRepository.GetQueryableAsync()).Where(x => request.Ids.Any(id => id == x.Id)).ToList();
                        permissionGroupTenantNull = (await _permissionGroupRepository.GetQueryableAsync()).Where(x => permissionTenantNull.Select(p=>p.GroupId).Any(groupId => groupId == x.Id)).ToList();
                        permissionModuleTenantNull = (await _permissionModuleRepository.GetQueryableAsync()).Where(x => permissionGroupTenantNull.Select(g=>g.ModuleId).Any(moduleId => moduleId == x.Id)).ToList();
                    }

                    var permissions = new List<Permission>();
                    var permissionGroups = new List<PermissionGroup>();
                    var permissionModules = new List<PermissionModule>();
                    using (_dataFilter.Disable<ISoftDelete>())
                    {

                        permissions = (await _permissionRepository.GetQueryableAsync()).ToList();
                        permissionGroups = (await _permissionGroupRepository.GetQueryableAsync()).ToList();
                        permissionModules = (await _permissionModuleRepository.GetQueryableAsync()).ToList();
                    }

                    var permissionGrants = (await _permissionGrantReposotory.GetQueryableAsync()).ToList();
                    permissionGrants.RemoveAll(permissionGrants.Where(x => permissions.Any(p => p.Code == x.Name) && x.ProviderKey != "admin").ToList());
                    await _permissionGrantReposotory.DeleteManyAsync(permissionGrants);
                    await uow.SaveChangesAsync();

                    var newPermissionGrantAdmin = new List<PermissionGrant>();
                    var defaultPermissions = permissions.Where(x => x.Code.StartsWith("AbpIdentity")).ToList() ?? new List<Permission>();
                    if (defaultPermissions.Any())
                        defaultPermissions.ForEach(x =>
                        {
                            newPermissionGrantAdmin.Add(new PermissionGrant(Guid.NewGuid(), x.Code, "R", "admin", request.TenantId));
                        });
                    
                    permissions.ForEach(permission =>
                    {
                        var take = permissionTenantNull.Any(x => x.Code == permission.Code);
                        var isDeleted = take ? false : true;
                        permission.IsDeleted = isDeleted;
                        permission.IsActive = isDeleted;
                        if (take)
                            newPermissionGrantAdmin.Add(new PermissionGrant(Guid.NewGuid(), permission.Code, "R", "admin", request.TenantId));
                    });

                    permissionGroups.ForEach(permissionGroup =>
                    {
                        var isDeleted = permissionGroupTenantNull.Any(x => x.Code == permissionGroup.Code) ? false : true;
                        permissionGroup.IsDeleted = isDeleted;
                        permissionGroup.IsActive = isDeleted;
                    });

                    permissionModules.ForEach(permissionModule =>
                    {
                        var isDeleted = permissionModuleTenantNull.Any(x => x.Code == permissionModule.Code) ? false : true;
                        permissionModule.IsDeleted = isDeleted;
                        permissionModule.IsActive = isDeleted;
                    });

                    await _permissionGrantReposotory.InsertManyAsync(newPermissionGrantAdmin);
                    await _permissionRepository.UpdateManyAsync(permissions);
                    await _permissionGroupRepository.UpdateManyAsync(permissionGroups);
                    await _permissionModuleRepository.UpdateManyAsync(permissionModules);
                    await uow.SaveChangesAsync();

                    await uow.CompleteAsync();
                    return new OkObjectResult(new
                    {
                        StatusCode = 200,
                        Message = "Cập nhật thành công"
                    });
                }
            }
            catch(Exception ex)
            {
                await uow.RollbackAsync();
                return new ObjectResult(ex.Message)
                {
                    StatusCode = 500,
                };
            }
        }

        [HttpPost]
        public async Task<bool> CheckDropPermissionAsync(ModuleManagerRequest request)
        {
            if (request.Ids == null || !request.Ids.Any())
                return true;
            var permissionId = request.Ids.FirstOrDefault();
            var permission = (await _permissionRepository.FirstOrDefaultAsync(x => x.Id == permissionId));
            if (permission == null)
                return false;
            var permissionGroup = (await _permissionGroupRepository.FirstOrDefaultAsync(x => x.Id == permission.GroupId));
            if (permissionGroup == null)
                return false;
            var permissionModule = (await _permissionModuleRepository.FirstOrDefaultAsync(x => x.Id == permissionGroup.ModuleId));
            if (permissionModule == null)
                return false;

            return (await IsNullDataAsync(permissionModule.Code, request.TenantId));
        }

        [HttpPost]
        public async Task<FileResult> ExportTenantAsync(ListTenantRequest request)
        {
            try
            {
                _logger.LogWarning("[VTECH] Start Export");
                var file = await _ExportTenantAsync(request);
                _logger.LogWarning($"[VTECH] Get Export File: File Length - {file.Length}");
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"DanhSach_DoanhNghiep_DaiLy_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
                };
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                _logger.LogError($"[VTECH]{e.Message}");
                throw;
            }
        }

        private async Task ValidateUpdateTenantAsync(VTECHERP.DTOs.BO.Tenants.Requests.UpdateTenantRequest request)
        {
            var validationErrors = new List<ValidationResult>();

            if (request == null)
            {
                validationErrors.Add(new ValidationResult($"{ErrorMessages.Common.RequestNotNull}"));
                throw new AbpValidationException(validationErrors);
            }

            using (_dataFilter.Disable<IMultiTenant>())
            {
                var enterprise = (await _enterpriseRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == request.Id);
                if (enterprise == null)
                {
                    var agency = (await _agencyRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == request.Id);
                    if (agency == null)
                        validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.InputInCorrect}: {request.Id}"));

                    using (CurrentTenant.Change(agency.TenantId))
                    {
                        if (request.ProvinceId.HasValue)
                        {
                            var province = (await _provinceRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == request.ProvinceId.Value);
                            if (province == null)
                                validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.Province} {ErrorMessages.TenantError.NotExist}"));

                            if (request.DistrictId.HasValue)
                            {
                                var district = (await _districtRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == request.DistrictId.Value);
                                if (district == null)
                                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.District} {ErrorMessages.TenantError.NotExist}"));
                                else if (district.ProvinceCode != province.Code)
                                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.District} {ErrorMessages.TenantError.NotIn} {ErrorMessages.TenantError.Province}"));
                            }
                        }
                    }
                }
                else
                {
                    using (CurrentTenant.Change(enterprise.TenantId))
                    {
                        if (request.ProvinceId.HasValue)
                        {
                            var province = (await _provinceRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == request.ProvinceId.Value);
                            if (province == null)
                                validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.Province} {ErrorMessages.TenantError.NotExist}"));

                            if (request.DistrictId.HasValue)
                            {
                                var district = (await _districtRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == request.DistrictId.Value);
                                if (district == null)
                                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.District} {ErrorMessages.TenantError.NotExist}"));
                                else if (province == null || district.ProvinceCode != province.Code)
                                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.District} {ErrorMessages.TenantError.NotIn} {ErrorMessages.TenantError.Province}"));
                            }
                        }
                    }
                }

                if (request.Expiration.HasValue && request.Expiration.Value.Date < DateTime.Now.Date)
                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.Expiration} {ErrorMessages.TenantError.ExpirationInCorrect}"));

                if (request.Status.HasValue && !Enum.IsDefined(typeof(Enums.TenantExtension.Status), request.Status.Value))
                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.Status} {ErrorMessages.TenantError.NotExist}"));

                if (validationErrors.Any())
                    throw new AbpValidationException(validationErrors);
            }
        }

        private async Task ValidateDeleteTenantAsync(Guid id)
        {
            var validationErrors = new List<ValidationResult>();
            var enterprise = new Enterprise();
            var agency = new Agency();
            using (_dataFilter.Disable<IMultiTenant>())
            {
                enterprise = (await _enterpriseRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == id);
                agency = (await _agencyRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == id);
            }

            if (enterprise == null && agency == null)
            {
                validationErrors.Add(new ValidationResult($"{ErrorMessages.Common.NotFound}"));
                throw new AbpValidationException(validationErrors);
            }
            var tenantId = enterprise != null ? enterprise.TenantId : agency.TenantId;

            using (CurrentTenant.Change(tenantId))
            {
                var saleOrders = (await _saleOrdersRepository.GetQueryableAsync()).ToList() ?? new List<SaleOrders>();
                var warehousingBills = (await _warehousingBillRepository.GetQueryableAsync()).ToList() ?? new List<WarehousingBill>();
                var billCustomers = (await _billCustomerReposotory.GetQueryableAsync()).ToList() ?? new List<BillCustomer>();
                if (saleOrders.Any())
                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.SaleOrder} {ErrorMessages.TenantError.IsExist}"));
                if (warehousingBills.Any())
                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.WarehousingBill} {ErrorMessages.TenantError.IsExist}"));
                if (billCustomers.Any())
                    validationErrors.Add(new ValidationResult($"{ErrorMessages.TenantError.BillCustomer} {ErrorMessages.TenantError.IsExist}"));
            }

            if (validationErrors.Any())
                throw new AbpValidationException(validationErrors);
        }

        /// <summary>
        /// Thêm Module, Group, Permission cho TenantId được chỉ định
        /// </summary>
        /// <param name="permissionModules">Danh sách Module</param>
        /// <param name="permissionGroups">Danh sách Group</param>
        /// <param name="permissions">Danh sách Permission</param>
        /// <param name="Ids">Danh sách Id có trạng thái hoạt động (isActive:False)</param>
        /// <param name="tenantId">Id tenant cần thêm dữ liệu</param>
        /// <param name="IsEnterprise">Có phải doanh nghiệp ko (True:phải/False:không)</param>
        /// <returns></returns>
        private async Task AddPermissionByTenantIdAsync(List<PermissionModule> permissionModules, List<PermissionGroup> permissionGroups, List<Permission> permissions,
            List<Guid> Ids, Guid tenantId, bool IsEnterprise = true)
        {
            var permissionGrants = new List<PermissionGrant>();
            var defaultPermissions = permissions.Where(x => x.Code.StartsWith("AbpIdentity")).ToList() ??  new List<Permission>();
            if (defaultPermissions.Any())
                defaultPermissions.ForEach(x =>
                {
                    permissionGrants.Add(new PermissionGrant(Guid.NewGuid(), x.Code, "R", "admin", tenantId));
                });
            var addPermissionModules = new List<PermissionModule>();
            var addPermissionGroups = new List<PermissionGroup>();
            var addPermissions = new List<Permission>();
            if (permissionModules.Any())
            {
                foreach (var permissionModule in permissionModules)
                {
                    var groupByModuleIds = permissionGroups.Where(x => x.ModuleId == permissionModule.Id).ToList() ?? new List<PermissionGroup>();
                    var permissionByGroupIds = permissions.Where(x => groupByModuleIds.Any(g => g.Id == x.GroupId)).ToList() ?? new List<Permission>();
                    var isActive = true;
                    if (IsEnterprise || Ids.Any(id => id == permissionModule.Id) || Ids.Any(id => groupByModuleIds.Any(g => g.Id == id)) || Ids.Any(id => permissionByGroupIds.Any(p => p.Id == id)))
                        isActive = false;

                    var addPermissionModule = new PermissionModule(permissionModule, tenantId, isActive);
                    addPermissionModules.Add(addPermissionModule);

                    if (groupByModuleIds.Any())
                    {
                        foreach (var permissionGroup in groupByModuleIds)
                        {
                            permissionByGroupIds = permissions.Where(x => x.GroupId == permissionGroup.Id).ToList() ?? new List<Permission>();
                            isActive = true;
                            if (IsEnterprise || Ids.Any(id => id == permissionGroup.Id) || Ids.Any(id => permissionByGroupIds.Any(p => p.Id == id)))
                                isActive = false;

                            var addPermissionGroup = new PermissionGroup(permissionGroup, addPermissionModule.Id, tenantId, isActive);
                            addPermissionGroups.Add(addPermissionGroup);

                            var perByGroupIds = permissions.Where(x => x.GroupId == permissionGroup.Id) ?? new List<Permission>();
                            if (perByGroupIds.Any())
                            {
                                foreach (var permission in perByGroupIds)
                                {
                                    isActive = true;
                                    if (IsEnterprise || Ids.Any(id => id == permission.Id))
                                        isActive = false;

                                    addPermissions.Add(new Permission(permission, addPermissionGroup.Id, tenantId, isActive));
                                    
                                    if ((!defaultPermissions.Any() || defaultPermissions.FirstOrDefault(x=>x.Code == permission.Code) == null) && !isActive)
                                        permissionGrants.Add(new PermissionGrant(Guid.NewGuid(), permission.Code, "R", "admin", tenantId));
                                }
                            }
                        }
                    }
                }
                await _permissionGrantReposotory.InsertManyAsync(permissionGrants);
                await _permissionModuleRepository.InsertManyAsync(addPermissionModules);
                await _permissionGroupRepository.InsertManyAsync(addPermissionGroups);
                await _permissionRepository.InsertManyAsync(addPermissions);
            }
        }

        private async Task<bool> IsNullDataAsync(string code, Guid tenantId)
        {
            using (CurrentTenant.Change(tenantId))
            {
                if (typeof(VTECHERPPermissions.AbpIdentity).Name == code)
                    return false;
                else if (typeof(VTECHERPPermissions.InternalTransport).Name == code)
                {
                    if((await _transportInformationRepository.GetQueryableAsync()) != null)
                        return false;
                }
                else if (typeof(VTECHERPPermissions.PaymentSummary).Name == code)
                {
                    if ((await _paymentReceiptRepository.GetQueryableAsync()) != null)
                        return false;
                }
                else if (typeof(VTECHERPPermissions.ExternalTransport).Name == code)
                {
                    if ((await _carrierShippingInformationRepository.GetQueryableAsync()) != null)
                        return false;
                }
                else if (typeof(VTECHERPPermissions.PriceTable).Name == code)
                {
                    if ((await _priceTableStoreRepository.GetQueryableAsync()) != null)
                        return false;
                }
                else if (typeof(VTECHERPPermissions.CustomerSale).Name == code)
                {
                    if ((await _billCustomerRepository.GetQueryableAsync()) != null)
                        return false;
                }
                else if (typeof(VTECHERPPermissions.SupplierOrder).Name == code)
                {
                    if ((await _saleOrdersRepository.GetQueryableAsync()) != null)
                        return false;
                }
                else if (typeof(VTECHERPPermissions.Product).Name == code)
                {
                    if ((await _storeproductRepository.GetQueryableAsync()) != null)
                        return false;
                }
                else if (typeof(VTECHERPPermissions.Accounting).Name == code)
                {
                    if ((await _accountRepository.GetQueryableAsync()) != null)
                        return false;
                }
                else if (typeof(VTECHERPPermissions.TransportHistory).Name == code)
                {
                    if ((await _transportInformationRepository.GetQueryableAsync()) != null)
                        return false;
                }
                else if (typeof(VTECHERPPermissions.Customer).Name == code)
                {
                    if ((await _customerRepository.GetQueryableAsync()) != null)
                        return false;
                }
                else if (typeof(VTECHERPPermissions.WarehouseTransfer).Name == code)
                {
                    if ((await _warehouseTransferBillRepository.GetQueryableAsync()) != null)
                        return false;
                }
                else if (typeof(VTECHERPPermissions.Warehousing).Name == code)
                {
                    if ((await _warehousingBillRepository.GetQueryableAsync()) != null)
                        return false;
                }
                else if (typeof(VTECHERPPermissions.Debt).Name == code)
                {
                    if ((await _debtRepository.GetQueryableAsync()) != null)
                        return false;
                }
                return true;
            }
        }

        private async Task<byte[]> _ExportTenantAsync(ListTenantRequest request)
        {
            request.PageIndex = 1;
            request.PageSize = int.MaxValue;
            var data  = (await ListTenantAsync(request)).Data.ToList();

            if (request.Ids != null)
                data = data.Where(x => request.Ids.Any(id => id == x.Id)).ToList();

            var exportData = new List<ExportTenantResponse>();
            var provinces = (await _provinceRepository.GetQueryableAsync()).ToList();
            var districts = (await _districtRepository.GetQueryableAsync()).ToList();
            foreach(var item in  data)
            {
                var province = provinces.FirstOrDefault(x => x.Id == item.ProvinceId.Value);
                var district = districts.FirstOrDefault(x => x.Id == item.DistrictId.Value);
                exportData.Add(new ExportTenantResponse()
                {
                    Id = item.Code,
                    Name = item.Name,
                    Type = item.IsEnterprise ? "Doanh nghiệp" : "Đại lý",
                    PhoneNumber = item.PhoneNumber,
                    Address = item.Address,
                    Province =  province == null ? string.Empty : province.Name,
                    District =  district == null ? string.Empty : district.Name,
                    StoreCount = item.StoreCount.ToString(),
                    Expiration = item.Expiration.Value.ToString("dd/MM/yyyy"),
                    Status = item.Status == Enums.TenantExtension.Status.InActive ? "Không hoạt động" : "Hoạt động",
                    CreationTime = item.CreateTime.ToString("dd/MM/yyyy"),
                    CreatorName = item.CreatorName,
                });
            }
            return ExcelHelper.ExportExcel(exportData);
        }

        #endregion VBN
    }
}