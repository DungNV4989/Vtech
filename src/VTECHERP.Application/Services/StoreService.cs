using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using Volo.Abp.Validation;
using VTECHERP.Constants;
using VTECHERP.Domain.Shared.Helper.Excel.Model;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.BillCustomers.Respons;
using VTECHERP.DTOs.Stores;
using VTECHERP.DTOs.Stores.Params;
using VTECHERP.Entities;
using VTECHERP.Helper;
using static VTECHERP.Constants.ErrorMessages;
using Cell = VTECHERP.Domain.Shared.Helper.Excel.Model.Cell;
using DataRow = VTECHERP.Domain.Shared.Helper.Excel.Model.DataRow;
using UserStore = VTECHERP.Entities.UserStore;

namespace VTECHERP.Services
{
    public class StoreService : IStoreService
    {
        private readonly IRepository<Stores> _storeRepository;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly IRepository<StoreShippingInformation> _storeShippingInformationRepository;
        private readonly IRepository<IdentityUser> _userRepository;
        private readonly IObjectMapper _objectMapper;
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly ICurrentTenant _currentTenant;
        private readonly IRepository<Enterprise> _enterpriseRepository;
        private readonly IRepository<Agency> _agencyRepository;
        private readonly IDataFilter _dataFilter;
        public StoreService(IRepository<Stores> storeRepository
            , IObjectMapper objectMapper
            , IRepository<UserStore> userStoreRepository
            , ICurrentUser userManager
            , IRepository<StoreShippingInformation> storeShippingInformationRepository
            , IUnitOfWorkManager unitOfWorkManager
            , IRepository<IdentityUser> userRepository
            , IConfiguration configuration
            , IRepository<Tenant> tenantRepository
            , ICurrentTenant currentTenant
            , IRepository<Enterprise> enterpriseRepository
            , IRepository<Agency> agencyRepository
            , IDataFilter dataFilter
            )
        {
            _storeRepository = storeRepository;
            _objectMapper = objectMapper;
            _userStoreRepository = userStoreRepository;
            _currentUser = userManager;
            _storeShippingInformationRepository = storeShippingInformationRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _userRepository = userRepository;
            _configuration = configuration;
            _tenantRepository = tenantRepository;
            _currentTenant = currentTenant;
            _enterpriseRepository = enterpriseRepository;
            _agencyRepository = agencyRepository;
            _dataFilter = dataFilter;
        }

        public async Task<List<MasterDataDTO>> GetUserManagedStoresAsync()
        {
            var stores = await _storeRepository.GetListAsync();
            var user = _currentUser.Id;
            var userStores = (await _userStoreRepository.GetListAsync(x => x.UserId == user));
            var query = from store in stores
                        join userStore in userStores on store.Id equals userStore.StoreId
                        select store;
            query = query.Distinct();
            var result = _objectMapper.Map<List<Stores>, List<MasterDataDTO>>(query.ToList());
            return result;
        }

        public async Task<List<MasterDataDTO>> GetTenantStoresAsync()
        {
            var stores = await _storeRepository.GetListAsync();
            var result = _objectMapper.Map<List<Stores>, List<MasterDataDTO>>(stores);
            return result;
        }

        public async Task<StoreDetailDto> GetByIdAsync(Guid id)
        {
           using(_dataFilter.Disable<ISoftDelete>())
           {
                var store = await _storeRepository.FirstOrDefaultAsync(x => x.Id == id);
                if (store == null)
                    return new StoreDetailDto();
                var result = _objectMapper.Map<Stores, StoreDetailDto>(store);
                return result;
           }    
        }

        public async Task<List<StoreDto>> GetByIdsAsync(List<Guid> ids)
        {
            using (_dataFilter.Disable<ISoftDelete>())
            {
                var result = new List<StoreDto>();
                var stores = (await _storeRepository.GetQueryableAsync()).Where(x => ids.Any(id => id == x.Id));
                result = _objectMapper.Map<List<Stores>, List<StoreDto>>(stores.ToList());
                return result;
            }
        }

        public async Task<bool> Exist(Guid id)
        {
            var store = await _storeRepository.FindAsync(x => x.Id == id);
            if (store == null)
                return false;
            return true;
        }

        public async Task<bool> SetFlagStore(Guid storeId)
        {
            try
            {
                var listUserStores = await _userStoreRepository.GetListAsync(x => x.StoreId == storeId);
                await _userStoreRepository.DeleteManyAsync(listUserStores);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<(Stores data, string message, bool success)> Create(CreateStoreParam param)
        {
            var uow = _unitOfWorkManager.Current;
            Guid uerIdAdmin = Guid.Empty;
            Stores store = new Stores();
            var storeQueryable = await _storeRepository.GetQueryableAsync();
            string storeCode;

            if (param.StoreId.HasValue && param.IsUpdate)
            {
                //Trường hợp cập nhật
                store = await _storeRepository.FirstOrDefaultAsync(x => x.Id == param.StoreId);
                if (store == null)
                    return (data: null, message: $"Không tìm thấy cửa hàng có id là {param.StoreId}", success: false);

                var lstStoreShippingDb = await _storeShippingInformationRepository.GetListAsync(x => x.StoreId == store.Id);
                await _storeShippingInformationRepository.DeleteManyAsync(lstStoreShippingDb);

                if(store.IsApprove.HasValue && store.IsApprove.Value)
                    store.IsActive = param.IsActive.GetValueOrDefault();
            }
            else
            {
                var currentTenant = _currentTenant.Id;
                var tenant = await _tenantRepository.FirstOrDefaultAsync(x => x.Id == currentTenant);
                var isVtechTenant = tenant.GetProperty<bool>("IsVTech");
                var connectionStringBO = _configuration["ConnectionStrings:BO"];
                string query = "";
                string queryExpiration = "";
                DateTime? expirationTimeTenant = null;

                if (isVtechTenant)
                {
                    query = $"select UserId from Enterprises where TenantId = '{currentTenant}'";
                    queryExpiration = $"select Expiration from Enterprises where TenantId = '{currentTenant}'";
                }    
                else
                {
                    query = $"select UserId from Agencies where TenantId = '{currentTenant}'";
                    queryExpiration = $"select Expiration from Agencies where TenantId = '{currentTenant}'";
                }    
                    
                using (IDbConnection db = new SqlConnection(connectionStringBO))
                {
                    db.Open();
                    uerIdAdmin = db.Query<Guid>(query).SingleOrDefault();
                    expirationTimeTenant = db.Query<DateTime?>(queryExpiration).SingleOrDefault();
                }

                var resultValidator = ValidatorCreateStore(param.StoreName, param.PhoneNumber, param.ExpriDate, expirationTimeTenant);
                if (!resultValidator.isValid)
                    return (null, resultValidator.message, false);

                var user = await _userRepository.FindAsync(x => x.Id == _currentUser.Id);
                var isVtech = user.GetProperty<bool>("IsVTech");
                var checkUser = user == null || isVtech == false;

                store.IsActive = checkUser ? false : true;
                store.IsApprove = checkUser ? false : true;

                var lastStore = storeQueryable.OrderByDescending(x => x.CreationTime).FirstOrDefault();
                if (lastStore == null || string.IsNullOrEmpty(lastStore.Code))
                {
                    storeCode = "1";
                    store.Code = "CH-" + storeCode.PadLeft(10, '0');
                }
                else
                {
                    var storeCodeSplit = lastStore.Code.Split('-');
                    var codeLastSection = storeCodeSplit.LastOrDefault();
                    var codeNumer = int.TryParse(codeLastSection, out int codeTryParse) ? codeTryParse + 1 : 1;
                    store.Code = "CH-" + codeNumer.ToString().PadLeft(10, '0');
                }

                store.ExpriDate = param.ExpriDate;
                store.Name = param.StoreName;
            }

            store.Address = param.Address;
            store.DistricId = param.DistricId.GetValueOrDefault();
            store.Email = param.Email;
            store.Note = param.Note;
            store.Order = param.Order.GetValueOrDefault();
            store.PhoneNumber = param.PhoneNumber;
            store.ProvinceId = param.ProvinceId.GetValueOrDefault();
            store.WardId = param.WardId.GetValueOrDefault();

            //MinhNH: Generate API Token
            CustomAuthFilter customAuthFilter = new CustomAuthFilter(_configuration);
            store.VTECHApiToken = customAuthFilter.GenerateVTechApiToken(store.Code);

            if(!param.StoreId.HasValue)
            {
                await _storeRepository.InsertAsync(store);
                var storeUser = new UserStore
                {
                    UserId = uerIdAdmin,
                    StoreId = store.Id,
                    IsDefault = false,
                };
                await _userStoreRepository.InsertAsync(storeUser);
            }
               
            var lstStoreShipping = new List<StoreShippingInformation>();
            foreach (var item in param.StoreShippingInformations)
            {
                if (item.BankId.HasValue && (string.IsNullOrEmpty(item.AccountBankNumber) || string.IsNullOrEmpty(item.OwnerAccountBank)))
                    return (null, "Thông tin hãng vận chuyển thiếu thông tin của ngân hàng", false);

                var storeShipping = new StoreShippingInformation()
                {
                    AccountBankNumber = item.AccountBankNumber,
                    BankId = item.BankId,
                    Carrier = item.Carrier,
                    OwnerAccountBank = item.OwnerAccountBank,
                    PasswordCarries = item.PasswordCarries,
                    StoreId = store.Id,
                    UserNameCarriers = item.UserNameCarriers,
                    Token = item.Token,
                    CustomerCode = item.CustomerCode,
                };

                lstStoreShipping.Add(storeShipping);
            }

            if (lstStoreShipping.Any())
                await _storeShippingInformationRepository.InsertManyAsync(lstStoreShipping);

            return (data: store, message: $"Thành công", success: true);
        }

        public async Task<(List<StoreListItem> data, int count)> Search(SearchListStoreParam param)
        {
            var result = new List<StoreListItem>();

            var storeQueryable = await _storeRepository.GetQueryableAsync();

            var query = await GetAllQueryable(param);

            result = query.OrderBy(x => x.Order).ThenByDescending(x => x.CreationTime)
                .Skip((param.PageIndex - 1) * param.PageSize)
                .Take(param.PageSize)
                .ToList();

            var creatorIds = result.Select(x => x.CreatorId).ToList();
            var users = await _userRepository.GetListAsync(x => creatorIds.Contains(x.Id));

            foreach (var item in result)
                item.CreatorName = users.FirstOrDefault(x => x.Id == item.CreatorId)?.Name;

            int count = query.Count();

            return (result, count);
        }

        public async Task<(StoreDetailToUpdate data, string message, bool success)> GetDetail(Guid StoreId)
        {
            var result = new StoreDetailToUpdate();

            var store = await _storeRepository.FirstOrDefaultAsync(x => x.Id == StoreId);
            if (store == null)
                return (data: null, message: $"Không tìm thấy cửa hàng có id là {StoreId}", success: false);

            result.Address = store.Address;
            result.DistricId = store.DistricId;
            result.Email = store.Email;
            result.ExpriDate = store.ExpriDate;
            result.IsActive = store.IsActive;
            result.StoreName = store.Name;
            result.Note = store.Note;
            result.Order = store.Order;
            result.PhoneNumber = store.PhoneNumber;
            result.ProvinceId = store.ProvinceId;
            result.WardId = store.WardId;

            var lstStoreShippingDb = await _storeShippingInformationRepository.GetListAsync(x => x.StoreId == store.Id);
            result.StoreShippingInformations = lstStoreShippingDb.Select(x => new StoreShippingInformationDto
            {
                AccountBankNumber = x.AccountBankNumber,
                BankId = x.BankId,
                Carrier = x.Carrier,
                OwnerAccountBank = x.OwnerAccountBank,
                PasswordCarries = x.PasswordCarries,
                StoreId = x.StoreId,
                UserNameCarriers = x.UserNameCarriers,
                Token = x.Token,    
            })
            .ToList();

            return (result, "", true);
        }

        private (string message, bool isValid) ValidatorCreateStore(string Name, string PhoneNumber, DateTime? ExpireDate, DateTime? ExpireTenant)
        {
            var tenantId = _currentTenant.Id;

            if (ExpireDate.HasValue && ExpireDate.Value.Date < DateTime.Now.Date)
                return (message: "Ngày hết hạn cửa hàng bị nhỏ hơn ngày hiện tại", false);

            if (ExpireDate.HasValue && ExpireTenant.HasValue && ExpireDate.Value.Date > ExpireTenant.Value.Date)
                return (message: "Ngày hết hạn cửa hàng lớn hơn ngày hết hạn của doanh nghiệp", false);

            var stores = _storeRepository.GetListAsync().Result;
            var isExistName = _storeRepository.AnyAsync(x => x.Name == Name).Result;
            if (isExistName)
                return (message: "Trùng tên cửa hàng", false);

            var isExistPhoneNumber = _storeRepository.AnyAsync(x => x.PhoneNumber == PhoneNumber).Result;
            if (isExistPhoneNumber)
                return (message: "Trùng số điện thoại cửa hàng", false);

            return ("", true);
        }
   
        private async Task<IQueryable<StoreListItem>> GetAllQueryable(SearchListStoreParam param)
        {
            var storeQueryable = await _storeRepository.GetQueryableAsync();

            var query = from store in storeQueryable
                        select new StoreListItem
                        {
                            StoreId = store.Id,
                            CreatorId = store.CreatorId,
                            CreationTime = store.CreationTime,
                            Address = store.Address,
                            Email = store.Email,
                            ExpireDate = store.ExpriDate,
                            Order = store.Order,
                            PhoneNumber = store.PhoneNumber,
                            StatusText = store.IsActive ? "Hoạt động" : "Không hoạt động",
                            StoreCode = store.Code,
                            StoreName = store.Name,
                            Status = store.IsActive,
                        };

            if (param.CreationTimeFrom.HasValue)
                query = query.Where(x => x.CreationTime.Date >= param.CreationTimeFrom.Value.Date);

            if (param.CreationTimeTo.HasValue)
                query = query.Where(x => x.CreationTime.Date <= param.CreationTimeTo.Value.Date);

            if (!string.IsNullOrEmpty(param.StoreCode))
                query = query.Where(x => x.StoreCode.Contains(param.StoreCode.Trim()));

            if (!string.IsNullOrEmpty(param.StoreName))
                query = query.Where(x => x.StoreName.ToLower().Contains(param.StoreName.Trim().ToLower()));

            if (param.Status.HasValue)
                query = query.Where(x => x.Status == param.Status.Value);

            return query;
        }

        public async Task<byte[]> ExportExcel(ExportStoreParam param)
        {
            var workbook = new CustomWorkBook();
            var data = await GetAllQueryable(param.Search);
            var result = new List<StoreListItem>();
            if (param.ListStoreIds != null && param.ListStoreIds.Any())
            {
                result = data.Where(x => param.ListStoreIds.Contains(x.StoreId)).ToList();
            }
            else result = data.ToList();

            var creatorIds = result.Select(x => x.CreatorId).ToList();
            var users = await _userRepository.GetListAsync(x => creatorIds.Contains(x.Id));

            foreach (var item in result)
                item.CreatorName = users.FirstOrDefault(x => x.Id == item.CreatorId)?.Name;
            var sheet =  RenderTemplateAfterImport(result);

            workbook.Sheets.Add(sheet);

            return ExcelHelper.ExportExcel(workbook);
        }

        private CustomSheet RenderTemplateAfterImport(List<StoreListItem> list)
        {
            var sheet = new CustomSheet("Sheet 1");

            var startRow = 1;

            var header = new CustomDataTable()
            {
                StartRowIndex = startRow,
                StartColumnIndex = 1,
                RowDirection = Directions.Horizontal,
                Rows = new List<DataRow>
                    {
                        new DataRow(
                            new HeaderCell("STT"),
                            new HeaderCell("ID"),
                            new HeaderCell("Tên cửa hàng"),
                            new HeaderCell("Số điện thoại"),
                            new HeaderCell("Email"),
                            new HeaderCell("Địa chỉ"),
                            new HeaderCell("Ngày hết hạn"),
                            new HeaderCell("Trạng thái"),
                            new HeaderCell("Ngày tạo"),
                            new HeaderCell("Người tạo")
                            )
                    }
            };
            sheet.Tables.Add(header);

            var indexSaleOrderColumn = startRow + 1;
            var index = 0;
            foreach (var item in list)
            {
                var row = new CustomDataTable()
                {
                    StartRowIndex = indexSaleOrderColumn++,
                    StartColumnIndex = 1,
                    RowDirection = Directions.Horizontal,
                    Rows = new List<DataRow>
                        {
                        new DataRow(
                            new Cell(index++),
                            new Cell(item.StoreCode),
                            new Cell(item.StoreName),
                            new Cell(item.PhoneNumber),
                            new Cell(item.Email),
                            new Cell(item.Address),
                            new Cell(item.ExpireDate),
                            new Cell(item.Status ? "Hoạt động" : "Không hoạt động"),
                            new Cell(item.CreationTime),
                            new Cell(item.CreatorName)
                            )
                    }
                };

                sheet.Tables.Add(row);
            }

            return sheet;
        }
    }
}