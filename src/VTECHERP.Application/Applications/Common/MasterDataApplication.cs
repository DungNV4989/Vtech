using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using VTECHERP.Datas;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.MasterDatas;
using VTECHERP.Entities;
using VTECHERP.Enums;

namespace VTECHERP
{
    [Route("api/app/master-data/[action]")]
    [Authorize]
    public class MasterDataApplication : ApplicationService
    {
        private readonly IRepository<Suppliers> _supplierRepository;
        private readonly IRepository<Products> _productRepository;
        private readonly IRepository<StoreProduct> _storeProductRepository;
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<IdentityUser> _userRepository;
        private readonly IRepository<Provinces> _provinceRepository;
        private readonly IRepository<Districts> _districtRepository;
        private readonly IRepository<Wards> _wardRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly IRepository<Customer> _customerRepository;
        public MasterDataApplication(
            IRepository<Suppliers> supplierRepository,
            IRepository<Products> productRepository,
            IRepository<StoreProduct> storeProductRepository,
            IRepository<IdentityUser> userRepository,
            IRepository<UserStore> userStoreRepository,
            IRepository<Account> accountRepository,
            IRepository<Provinces> provinceRepository,
            IRepository<Districts> districtRepository,
            IRepository<Wards> wardRepository,
            IRepository<Employee> employeeRepository,
            IRepository<Customer> customerRepository
            )
        {
            _supplierRepository = supplierRepository;
            _productRepository = productRepository;
            _storeProductRepository = storeProductRepository;
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _provinceRepository = provinceRepository;
            _districtRepository = districtRepository;
            _wardRepository = wardRepository;
            _employeeRepository = employeeRepository;
            _userStoreRepository = userStoreRepository;
            _customerRepository = customerRepository;
        }

        [HttpPost]
        public async Task<List<MasterDataDTO>> SearchAudience(SearchAudienceRequest request)
        {
            var list = new List<MasterDataDTO>();
            var supplierList = (await _supplierRepository
                        .GetQueryableAsync())
                        .Where(p =>
                            (p.Name.Contains(request.SearchText) || p.Code.Contains(request.SearchText) || p.PhoneNumber.Contains(request.SearchText))
                            && !p.IsDeleted
                        );
            switch (request.AudienceType)
            {
                case AudienceTypes.SupplierCN:
                    list = supplierList
                        .Where(p => p.Origin == SupplierOrigin.CN)
                        .Select(p => new MasterDataDTO { Code = p.Code, Name = p.Name, Phone = p.PhoneNumber, Id = p.Id })
                        .OrderBy(p => p.Name)
                        //.Take(request.PageSize)
                        .ToList();
                    break;
                case AudienceTypes.SupplierVN:
                    list = supplierList
                        .Where(p =>p.Origin == SupplierOrigin.VN)
                        .Select(p => new MasterDataDTO { Code = p.Code, Name = p.Name, Phone = p.PhoneNumber, Id = p.Id })
                        .OrderBy(p => p.Name)
                        //.Take(request.PageSize)
                        .ToList();
                    break;
                case AudienceTypes.Customer:
                    list = _customerRepository.GetListAsync(x => (string.IsNullOrEmpty(request.SearchText) || x.Name.Contains(request.SearchText))).Result
                                            .Select(a => new MasterDataDTO { Code = a.Code, Name = a.Name, Phone = a.PhoneNumber, Id = a.Id }).ToList();
                    break;
                case AudienceTypes.Employee:
                    break;
                default:
                    var suppliers = supplierList
                        .Select(p => new MasterDataDTO { Code = p.Code, Name = p.Name, Phone = p.PhoneNumber, Id = p.Id }).ToList();
                    var customers = new List<MasterDataDTO>();
                    var employees = new List<MasterDataDTO>();
                    // TODO: union các đối tượng khác
                    list = suppliers
                        .Union(customers)
                        .Union(employees)
                        .OrderBy(p => p.Name)
                        //.Take(request.PageSize)
                        .ToList();
                    break;
            }

            return list;
        }

        [HttpPost]
        public async Task<List<MasterDataDTO>> SearchAccount(SearchMasterAccountRequest request)
        {
            var list = new List<MasterDataDTO>();

            list = (await _accountRepository
                .GetQueryableAsync())
                .Where(p =>
                    (request.SearchText == null || p.Name.Contains(request.SearchText) || p.Code.Contains(request.SearchText))
                    && (request.AccountCode.Count == 0 || request.AccountCode.Contains(p.Code))
                    && (request.ParentAccountCode.Count == 0 || request.ParentAccountCode.Contains(p.ParentAccountCode))
                )
                .Select(p => new MasterDataDTO { Code = p.Code, Name = p.Name, Id = p.Id })
                .OrderBy(p => p.Name)
                .ToList();

            return list;
        }

        [HttpPost]
        public async Task<List<MasterDataDTO>> SearchEmployee(SearchTextRequest request)
        {
            var users = await _userRepository.GetQueryableAsync();
            //var userStores = await _userStoreRepository.GetQueryableAsync();
            //var currentUserStoreIds = userStores.Where(p => p.UserId == CurrentUser.Id).Select(p => p.StoreId).ToList();
            var employees = await _employeeRepository.GetQueryableAsync();

            var res = 
                (
                    from employee in employees
                    join user in users on employee.UserId equals user.Id into userLeftJoin
                    from user in userLeftJoin.DefaultIfEmpty()
                    select new
                    {
                        UserName = user.UserName,
                        employee.Name,
                        employee.PhoneNumber,
                        employee.Id
                    }
                )
                .WhereIf(!string.IsNullOrWhiteSpace(request.SearchText), 
                    x => (x.UserName != null && x.UserName.Contains(request.SearchText))
                    || x.Name.Contains(request.SearchText)
                )
                .OrderBy(p => p.Name)
                .Select(p => new MasterDataDTO { Name = p.Name, Id = p.Id, Phone = p.PhoneNumber }).ToList();
            return res;
        }

        [HttpPost]
        public async Task<List<ProductMasterDataDto>> SearchProduct(SearchProductMasterDataRequest request)
        {
            var list = new List<ProductMasterDataDto>();

            var products = (await _productRepository.GetQueryableAsync()).Where(p => !p.IsDeleted);
            var stocks = (await _storeProductRepository.GetQueryableAsync()).Where(p => !p.IsDeleted && p.StoreId == request.StoreId);
            //var units = MasterDatas.ProductUnits;
            if (request.IsSearchByIMEI)
            {

            }
            else
            {
                list = (from p in products
                        join s in stocks on p.Id equals s.ProductId into p_join
                        from sp in p_join.DefaultIfEmpty()
                        where
                             (p.Name.Contains(request.SearchText) || p.Code.Contains(request.SearchText))
                             && (request.ProductIds == null || request.ProductIds.Contains(sp.ProductId))
                             && !p.IsDeleted
                        select new ProductMasterDataDto
                        {
                            Code = p.Code,
                            Name = p.Name,
                            Id = p.Id,
                            StockQuantity = sp == null ? 0 : sp.StockQuantity,
                            Unit = p.Unit,
                            BarCode = p.BarCode,
                            UnitName = p.Unit,
                            StockPrice = p.StockPrice
                        })
                       .OrderBy(p => p.Name)
                       //.Take(request.PageSize)
                       .ToList();
            }
            //list.ForEach(p => p.UnitName = units.First(u => u.Id == p.Unit).Name);
            return list;
        }

        [HttpPost]
        public List<EnumMasterData<DocumentDetailType>> SearchDocumentDetailType(SearchDocumentDetailTypeRequest request)
        {
            var masterDatas = DocumentDetailTypeData.Datas;
            // modify request để lấy data
            if (request.AudienceType == AudienceTypes.SupplierCN || request.AudienceType == AudienceTypes.SupplierVN)
            {
                request.AudienceType = AudienceTypes.SupplierCN;
            }
            var datas = masterDatas.Where(p =>
                (request.AudienceType == null || p.AudienceType == request.AudienceType)
                && (request.WarehousingBillType == null || p.WarehousingBillType == request.WarehousingBillType)
                && (request.DocumentType == null || p.DocumentType == request.DocumentType)
                && (request.TicketType == null || p.TicketType == request.TicketType)
                && (!request.IsWarehousingBillForm || p.IsWarehousingBillForm == request.IsWarehousingBillForm)
            ).Select(p => new EnumMasterData<DocumentDetailType>
            {
                Id = p.DocumentDetailType,
                Name = p.Name
            }).ToList();
            return datas;
        }

        [HttpGet]
        public List<EnumMasterData<Gender>> GetGenders()
        {
            return MasterDatas.Genders;
        }

        [HttpGet]
        public List<EnumMasterData<CustomerType>> GetCustomerTypes()
        {
            return MasterDatas.CustomerTypes;
        }

        [HttpGet]
        public List<EnumMasterData<DebtGroup>> GetDebtGroups()
        {
            return MasterDatas.DebtGroups;
        }

        [HttpGet]
        public async Task<List<MasterDataDTO>> GetProvinces()
        {
            var provinces = await _provinceRepository.GetListAsync();
            return provinces.Select(p => new MasterDataDTO
            {
                Code = p.Code,
                Id = p.Id,
                Name = p.Name
            }).OrderBy(p => p.Code).ToList();
        }

        [HttpGet]
        public async Task<List<MasterDataDTO>> GetDistricts(string? provinceCode)
        {
            var districts = await _districtRepository.GetQueryableAsync();
            return districts
                .WhereIf(!provinceCode.IsNullOrEmpty(), p => p.ProvinceCode == provinceCode)
                .Select(p => new MasterDataDTO
                {
                    Code = p.Code,
                    Id = p.Id,
                    Name = p.Name
                })
                .OrderBy(p => p.Code).ToList();
        }

        [HttpGet]
        public async Task<List<MasterDataDTO>> GetWards(string? provinceCode, string? districtCode)
        {
            var wards = await _wardRepository.GetQueryableAsync();
            var districts = await _districtRepository.GetQueryableAsync();
            return wards
                .Join(districts, ward => ward.DistrictCode, district => district.Code, (ward, district) => new
                {
                    ProvinceCode = district.ProvinceCode,
                    DistrictCode = district.Code,
                    Code = ward.Code,
                    Name = ward.Name,
                    Id = ward.Id
                })
                .WhereIf(!provinceCode.IsNullOrEmpty(), p => p.ProvinceCode == provinceCode)
                .WhereIf(!districtCode.IsNullOrEmpty(), p => p.DistrictCode == districtCode)
                .Select(p => new MasterDataDTO
                {
                    Code = p.Code,
                    Id = p.Id,
                    Name = p.Name
                }).OrderBy(p => p.Code).ToList();
        }
    }
}
