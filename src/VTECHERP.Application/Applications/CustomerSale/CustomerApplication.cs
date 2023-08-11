using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Timing;
using Volo.Abp.Users;
using VTECHERP.Datas;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.BillCustomers.Params;
using VTECHERP.DTOs.BillCustomers.Respons;
using VTECHERP.DTOs.Customer;
using VTECHERP.DTOs.TransportInformation;
using VTECHERP.Entities;
using VTECHERP.Enums;
using VTECHERP.Helper;
using Customer = VTECHERP.Entities.Customer;

namespace VTECHERP
{
    [Route("api/app/customer/[action]")]
    [Authorize]
    public class CustomerApplication : ApplicationService
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<BillCustomer> _billCustomerRepository;
        private readonly IRepository<BillCustomerProduct> _billCustomerProductRepository;
        private readonly IRepository<Provinces> _provinceRepository;
        private readonly IRepository<Stores> _storeRepository;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IClock _clock;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<CustomerReturn> _customerReturnRepository;
        private readonly IRepository<CustomerReturnProduct> _customerReturnProductRepository;
        private readonly IRepository<Products> _productRepository;
        public CustomerApplication(
            IRepository<Customer> customerRepository,
            IRepository<BillCustomer> billCustomerRepository,
            IRepository<BillCustomerProduct> billCustomerProductRepository,
            IRepository<Provinces> provinceRepository,
            IRepository<Stores> storeRepository,
            IIdentityUserRepository userRepository,
            IClock clock,
            IRepository<UserStore> userStoreRepository,
            ICurrentUser currentUser,
            IRepository<Employee> employeeRepository,
            IRepository<CustomerReturn> customerReturnRepository,
            IRepository<CustomerReturnProduct> customerReturnProductRepository,
            IRepository<Products> productRepository
            )
        {
            _customerRepository = customerRepository;
            _billCustomerRepository = billCustomerRepository;
            _billCustomerProductRepository = billCustomerProductRepository;
            _provinceRepository = provinceRepository;
            _storeRepository = storeRepository;
            _userRepository = userRepository;
            _clock = clock;
            _userStoreRepository = userStoreRepository;
            _currentUser = currentUser;
            _employeeRepository = employeeRepository;
            _customerReturnRepository = customerReturnRepository;
            _customerReturnProductRepository = customerReturnProductRepository;
            _productRepository = productRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.PhoneNumber))
            {
                return new GenericActionResult(400, false, "Dữ liệu không hợp lệ");
            }
            try
            {
                var customer = ObjectMapper.Map<CreateCustomerRequest, Customer>(request);
                //if(request.HandlerStoreId.Any())
                //    customer.HandlerStoreIds = JsonConvert.SerializeObject(request.HandlerStoreId);

                var isDuplicatePhoneNumber = await _customerRepository.AnyAsync(x => x.PhoneNumber.Trim() == request.PhoneNumber.Trim());
                if(isDuplicatePhoneNumber)
                    return new GenericActionResult(400, false, "Trùng số điện thoại");

                var emp = await _employeeRepository.FirstOrDefaultAsync(x => x.Id == request.HandlerEmployeeId);
                if (emp != null)
                    customer.HandlerEmpName = emp.Name;
                await _customerRepository.InsertAsync(customer);
                return new GenericActionResult(200, true, "Tạo mới khách hàng thành công", customer);
            }
            catch (Exception ex)
            {
                return new GenericActionResult(500, false, ex.Message);
            }       
        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid CustomerId , UpdateCustomerRequest request)
        {
            var customer = await _customerRepository.FirstOrDefaultAsync(p => p.Id == CustomerId);
            if(customer == null)
            {
                return new GenericActionResult(400, false, $"Không tìm thấy khách hàng có id là {CustomerId}");
            }

            customer.Name = request.Name;
            customer.CustomerType = request.CustomerType;
            customer.ProvinceId = request.ProvinceId;
            customer.Address = request.Address;
            customer.PhoneNumber = request.PhoneNumber;
            customer.DateOfBirth = request.DateOfBirth;
            customer.Gender = request.Gender;
            customer.DebtGroup = request.DebtGroup;
            customer.DebtLimit = request.DebtLimit;
            customer.HandlerEmployeeId = request.HandlerEmployeeId;
            customer.SupportEmployeeId = request.SupportEmployeeId;
            customer.HandlerStoreId = request.HandlerStoreId;
            customer.Facebook = request.Facebook;
            customer.Zalo = request.Zalo;
            customer.Note = request.Note;
            //customer.HandlerStoreIds = JsonConvert.SerializeObject(request.HandlerStoreId);
            var emp = await _employeeRepository.GetAsync(x => x.Id == request.HandlerEmployeeId);
            if (emp != null)
                customer.HandlerEmpName = emp.Name;
            await CurrentUnitOfWork.SaveChangesAsync();

            return new GenericActionResult(200, true, "Cập nhật khách hàng thành công", customer);
        }

        [HttpDelete]
        public async Task<bool> Delete(Guid id)
        {
            await _customerRepository.DeleteAsync(p => p.Id == id);
            return true;
        }

        [HttpGet]
        public async Task<List<MasterDataDTO>> SearchByNameOrPhone(string? nameOrPhone)
        {
            var customers = await _customerRepository.GetQueryableAsync();

            var customerQuery = customers
                .WhereIf(!nameOrPhone.IsNullOrEmpty(), p => (p.Name != null && p.Name.ToLower().Contains(nameOrPhone.ToLower().Trim())) ||
                  (p.PhoneNumber != null && p.PhoneNumber.ToLower().Contains(nameOrPhone.ToLower().Trim())));

            return customerQuery.OrderByDescending(x => x.CreationTime).Select(p => new MasterDataDTO
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Phone = p.PhoneNumber 
            }).ToList();
        }

        [HttpGet]
        public async Task<List<MasterDataDTO>> GetAllUser(string? name)
        {
            var users = await _userRepository.GetListAsync();
            var userQuery = users
                .WhereIf(!name.IsNullOrEmpty(), p => p.Name != null && p.Name.ToLower().Contains(name.ToLower().Trim()));

            return userQuery.OrderByDescending(x => x.CreationTime).Select(p => new MasterDataDTO
            {
                Id = p.Id,
                Name = p.Name,
                Phone = p.PhoneNumber
            }).ToList();
        }

        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(PagingResponse<CustomerDTO>))]
        public async Task<PagingResponse<CustomerDTO>> Search(SearchCustomerRequest request)
        {
            var result = new List<CustomerDTO>();

            try
            {
                var customers = await _customerRepository.GetQueryableAsync();
                var billCustomers = (await _billCustomerRepository.GetQueryableAsync()).Where(x => x.CustomerBillPayStatus != Enums.Bills.CustomerBillPayStatus.Cancel);
                var billCustomerProducts = await _billCustomerProductRepository.GetQueryableAsync();
                var provinces = await _provinceRepository.GetQueryableAsync();
                var stores = await _storeRepository.GetQueryableAsync();
                var userStore = await _userStoreRepository.GetQueryableAsync();

                var storeOfUser = userStore.Where(x => x.UserId == _currentUser.Id).Select(x => x.StoreId).ToList();

                //var curDate = _clock.Now.Date;
                //var query = from cus in customers

                //            join st in stores
                //            on cus.HandlerStoreId equals st.Id
                //            into stGr
                //            from st in stGr.DefaultIfEmpty()

                //            let totalPurchaseAmount = billCustomers.Where(x => x.CustomerId == cus.Id).Sum(x => x.AmountCustomerPay)
                //            let purchaseQuantity = (from bill in billCustomers
                //                                       join billPro in billCustomerProducts
                //                                       on bill.Id equals billPro.BillCustomerId
                //                                       where cus.Id == bill.CustomerId
                //                                       select billPro
                //                                       ).Count()
                //            let numberOfPurchaseTime = billCustomers.Count(x => x.CustomerId == cus.Id)
                //            let lastPurchaseDate = billCustomers.Max(x => x.CreationTime)
                //            let firstPurchaseDate = billCustomers.Min(x => x.CreationTime)

                //            select new CustomerDTO
                //            {
                //                Id = cus.Id,
                //                Code= cus.Code,
                //                Name = cus.Name,
                //                Address = cus.Address,
                //                HandlerStoreId= cus.HandlerStoreId,
                //                HandlerStoreName = st.Name,
                //                CustomerType = cus.CustomerType,
                //                PhoneNumber = cus.PhoneNumber,
                //                DateOfBirth = cus.DateOfBirth,
                //                TotalPurchaseAmount = totalPurchaseAmount,
                //                PurchaseQuantity= purchaseQuantity,
                //                NumberOfPurchaseTime = numberOfPurchaseTime,
                //                LastPurchaseDate = lastPurchaseDate,
                //                FirstPurchaseDate = firstPurchaseDate,
                //                NonPurchaseDays = (curDate - lastPurchaseDate).Days
                //            };
                if (!string.IsNullOrEmpty(request.NameOrPhone))
                {
                    var nameOrPhoneFilter = await SearchByNameOrPhone(request.NameOrPhone);

                    if (nameOrPhoneFilter != null)
                    {
                        var lstCus = nameOrPhoneFilter.Select(a => a.Id);
                        customers = customers.Where(x => lstCus.Contains(x.Id));
                    }
                }
                if (!string.IsNullOrEmpty(request.SupportEmployeeName))
                {
                    var supportEmployeeFilter = await GetAllUser(request.SupportEmployeeName);

                    if (supportEmployeeFilter != null && supportEmployeeFilter.Count > 0)
                    {
                        var lstSupportEmployeeFilter = supportEmployeeFilter.Select(a => a.Id);
                        customers = customers.Where(x => x.SupportEmployeeId != null &&  lstSupportEmployeeFilter.Contains(x.SupportEmployeeId.Value));
                    }
                    else
                    {
                        return new PagingResponse<CustomerDTO>(0, result);
                    }
                }
                if (!string.IsNullOrEmpty(request.HandlerEmployeeName))
                {
                    var handlerEmployeeFilter = await GetAllUser(request.HandlerEmployeeName);

                    if (handlerEmployeeFilter != null && handlerEmployeeFilter.Count > 0)
                    {
                        var lstHandlerEmployeeFilter = handlerEmployeeFilter.Select(a => a.Id);
                        customers = customers.Where(x => x.HandlerEmployeeId != null && lstHandlerEmployeeFilter.Contains(x.HandlerEmployeeId.Value));
                    }
                    else
                    {
                        return new PagingResponse<CustomerDTO>(0, result);
                    }
                }
                var customerQuery = customers
                    .WhereIf(!request.ID.IsNullOrEmpty(), p => p.Code.Contains(request.ID))                    
                    //.WhereIf(request.SupportEmployeeId != null, p => request.SupportEmployeeId.Contains(p.SupportEmployeeId))
                    //.WhereIf(request.HandlerEmployeeId != null, p => request.HandlerEmployeeId.Contains(p.HandlerEmployeeId))
                    //.WhereIf(request.CustomerIds.Any(), p => request.CustomerIds.Contains(p.Id))
                    .WhereIf(request.CustomerType.HasValue, p => p.CustomerType == request.CustomerType)
                    .WhereIf(request.Gender != null, p => p.Gender == request.Gender)
                    .WhereIf(request.DebtGroup != null, p => p.DebtGroup == request.DebtGroup)
                    .WhereIf(request.ProvinceId != null, p => p.ProvinceId == request.ProvinceId)
                    .WhereIf(request.HandlerStoreId != null && request.HandlerStoreId.Any(), p => request.HandlerStoreId.Contains(p.HandlerStoreId.HasValue ? p.HandlerStoreId.Value : Guid.Empty))
                    .ToList();
               
                var preFilterCustomerIds = customerQuery.Select(p => p.Id).ToList();
                var curDate = _clock.Now.Date;

                var billCustomerProductGroups = billCustomerProducts
                    .Join(billCustomers, prod => prod.BillCustomerId, bill => bill.Id, (prod, bill) => new
                    {
                        BillId = bill.Id,
                        CustomerId = bill.CustomerId ?? Guid.Empty,
                        ProductId = prod.ProductId,
                        Quantity = prod.Quantity,
                        Price = prod.Price,
                        BillCreationTime = bill.CreationTime,
                        CustomerDebtBill = bill.AmountCustomerPay,
                        StatusBill = bill.CustomerBillPayStatus
                    })
                    .Where(p => preFilterCustomerIds.Contains(p.CustomerId) && p.StatusBill != Enums.Bills.CustomerBillPayStatus.Cancel)
                    .GroupBy(p => p.CustomerId)
                    .Select(grp => new
                    {
                        CustomerId = grp.Key,
                        TotalPurchaseAmount = grp.Select(p => p.CustomerDebtBill ?? 0).Distinct().Sum(),
                        PurchaseQuantity = grp.Sum(p => p.Quantity),
                        FirstPurchaseDate = grp.Min(b => b.BillCreationTime.Date),
                        LastPurchaseDate = grp.Max(b => b.BillCreationTime.Date),
                        NumberOfPurchaseTime = grp.Select(p => p.BillId).Distinct().Count(),
                    })
                    .Select(p => new
                                        {
                        p.CustomerId,
                        p.FirstPurchaseDate,
                        p.LastPurchaseDate,
                        NonPurchaseDays = (curDate - p.LastPurchaseDate).Days,
                        PurchaseCycle = p.NumberOfPurchaseTime > 0 ? (p.LastPurchaseDate - p.FirstPurchaseDate).Days / p.NumberOfPurchaseTime : 0,
                        p.NumberOfPurchaseTime,
                        p.TotalPurchaseAmount,
                        p.PurchaseQuantity
                    })
                    .WhereIf(request.FirstPurchaseDateFrom != null, p => p.FirstPurchaseDate.Date >= request.FirstPurchaseDateFrom.Value.Date)
                    .WhereIf(request.FirstPurchaseDateTo != null, p => p.FirstPurchaseDate.Date <= request.FirstPurchaseDateTo.Value.Date)
                    .WhereIf(request.LastPurchaseDateFrom != null, p => p.LastPurchaseDate.Date >= request.LastPurchaseDateFrom.Value.Date)
                    .WhereIf(request.LastPurchaseDateTo != null, p => p.LastPurchaseDate.Date <= request.LastPurchaseDateTo.Value.Date)
                    .ToList();

                // TANPQ: Đang gặp vấn đề khi join với queryable billCustomerGroups + billCustomerProductGroups
                // TANPQ: TODO: Fix join để giảm số lần gọi DB
                var queryData =
                    (from customer in customerQuery
                     join customerPurchaseInfo in billCustomerProductGroups on customer.Id equals customerPurchaseInfo.CustomerId
                     into purchaseInfos
                     from customerPurchaseInfo in purchaseInfos.DefaultIfEmpty()
                     join province in provinces on customer.ProvinceId equals province.Id into customerProvinces
                     from province in customerProvinces.DefaultIfEmpty()
                     join store in stores on customer.HandlerStoreId equals store.Id into customerStores
                     from store in customerStores.DefaultIfEmpty()
                     select new CustomerDTO
                     {
                         Id = customer.Id,
                         Code = customer.Code,
                         Name = customer.Name,
                         DateOfBirth = customer.DateOfBirth,
                         PhoneNumber = customer.PhoneNumber,
                         ProvinceId = customer.ProvinceId,
                         ProvinceName = province != null ? province.Name : null,
                         Address = customer.Address,

                         Gender = customer.Gender,
                         CustomerType = customer.CustomerType,

                         DebtGroup = customer.DebtGroup,
                         DebtLimit = customer.DebtLimit,

                         HandlerEmployeeId = customer.HandlerEmployeeId,
                         SupportEmployeeId = customer.SupportEmployeeId,
                         HandlerStoreId = customer.HandlerStoreId,
                         HandlerStoreName = store == null ? null : store.Name ,

                         FirstPurchaseDate = customerPurchaseInfo == null ? null : customerPurchaseInfo.FirstPurchaseDate,
                         LastPurchaseDate = customerPurchaseInfo == null ? null: customerPurchaseInfo.LastPurchaseDate,

                         TotalPurchaseAmount = customerPurchaseInfo == null ? 0 : customerPurchaseInfo.TotalPurchaseAmount,
                         PurchaseQuantity = customerPurchaseInfo == null ? 0 : customerPurchaseInfo.PurchaseQuantity,
                         PurchaseCycle = customerPurchaseInfo == null ? 0 : customerPurchaseInfo.PurchaseCycle,
                         NonPurchaseDays = customerPurchaseInfo == null ? 0 : customerPurchaseInfo.NonPurchaseDays,
                         NumberOfPurchaseTime = customerPurchaseInfo == null ? 0 : customerPurchaseInfo.NumberOfPurchaseTime,

                         Note = customer.Note,
                         Zalo = customer.Zalo,
                         Facebook = customer.Facebook,

                         IsEditable = true,
                         IsDeletable = true,
                         CreationTime = customer.CreationTime,
                         CreatorId = customer.CreatorId,
                         LastModificationTime = customer.LastModificationTime,
                         LastModifierId = customer.LastModifierId,
                         HandlerStoreIds = customer.HandlerStoreIds
                     })
                    .WhereIf(request.FirstPurchaseDateFrom != null, p => p.FirstPurchaseDate != null && p.FirstPurchaseDate.Value.Date >= request.FirstPurchaseDateFrom.Value.Date)
                    .WhereIf(request.FirstPurchaseDateTo != null, p => p.FirstPurchaseDate != null && p.FirstPurchaseDate.Value.Date <= request.FirstPurchaseDateTo.Value.Date)
                    .WhereIf(request.LastPurchaseDateFrom != null, p => p.LastPurchaseDate != null && p.LastPurchaseDate.Value.Date >= request.LastPurchaseDateFrom.Value.Date)
                    .WhereIf(request.LastPurchaseDateTo != null, p => p.LastPurchaseDate != null && p.LastPurchaseDate.Value.Date <= request.LastPurchaseDateTo.Value.Date)
                    .WhereIf(request.NonPurchaseDaysFrom != null, p => p.NonPurchaseDays >= request.NonPurchaseDaysFrom.Value)
                    .WhereIf(request.NonPurchaseDaysTo != null, p => p.NonPurchaseDays <= request.NonPurchaseDaysTo.Value)
                    .WhereIf(request.PurchaseCycleFrom != null, p => p.PurchaseCycle >= request.PurchaseCycleFrom.Value)
                    .WhereIf(request.PurchaseCycleTo != null, p => p.PurchaseCycle <= request.PurchaseCycleTo.Value);

                var total = queryData.Count();
                var data = queryData.OrderByDescending(x => x.CreationTime).Skip(request.Offset).Take(request.PageSize).ToList();

                var storeLists = await _storeRepository.GetListAsync();

                data.ForEach(p =>
                {
                    p.GenderName = MasterDatas.Genders.FirstOrDefault(g => g.Id == p.Gender)?.Name;
                    p.CustomerTypeName = MasterDatas.CustomerTypes.FirstOrDefault(g => g.Id == p.CustomerType)?.Name;
                    p.DebtGroupName = MasterDatas.DebtGroups.FirstOrDefault(g => g.Id == p.DebtGroup)?.Name;

                    var storeHandles = storeLists.FirstOrDefault(x => x.Id == p.HandlerStoreId);
                    p.HandlerStoreName = storeHandles != null ? storeHandles.Name : "";
                });

                return new PagingResponse<CustomerDTO>(total, data);
            }
            catch(Exception e)
            {
                string x = e.Message;
                throw;
            }
            
        }

        [HttpGet]
        public async Task<CustomerDTO> Get(Guid id)
        {
            return new CustomerDTO();
        }

        [HttpGet]
        public async Task<List<CustomerResponse>> GetCustomers()
        {
            var results = new List<CustomerResponse>();

            var customers = await _customerRepository.GetQueryableAsync();
            if (!customers.Any())
                return results;

            results = ObjectMapper.Map<List<Customer>, List<CustomerResponse>>(customers.ToList());

            var storeIds = results.Select(x => x.HandlerStoreIds).Distinct();
            var creatorIds = results.Select(x => x.CreatorId).Distinct();
            var employeeIds = results.Select(x => x.HandlerEmployeeId).Distinct();

            var stores = (await _storeRepository.GetQueryableAsync()).Where(x => storeIds.Any(id => id == x.Id));
            var users = (await _userRepository.GetListAsync()).Where(x => employeeIds.Any(id => id == x.Id) || creatorIds.Any(id => id == x.Id));

            results.ForEach(x =>
            {
                if (stores != null)
                {
                    var store = stores.FirstOrDefault(s => s.Id == x.HandlerStoreIds);
                    x.HandlerStoreNames = store != null ? store.Name : string.Empty;
                }

                if (users != null)
                {
                    var creator = users.FirstOrDefault(u => u.Id == x.CreatorId);
                    var employee = users.FirstOrDefault(u => u.Id == x.HandlerEmployeeId);

                    x.CreateName = creator != null ? creator.Name : string.Empty;
                    x.HandlerEmployeeName = employee != null ? employee.Name : string.Empty;
                }
            });

            return results;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomerByCustomerType(CustomerType customerType)
        {
            var result = await _customerRepository.GetListAsync(x => x.CustomerType == customerType);
            return new GenericActionResult(200, true, "", result.Select(x => new
            {
                Name = x.Name,
                Id = x.Id
            }));
        }

        [HttpGet]
        public async Task<PagingPurchaseHistoryResponse> GetPurchaseHistoryAsync(PurchaseHistoryRequest request)
        {
            var PurchaseHistoryResponses = new List<PurchaseHistoryResponse>();
            var billCustomers = new List<BillCustomer>();
            if (request.BillLogType == null || request.BillLogType == BillLogType.Sale)
                billCustomers = (await _billCustomerRepository.GetQueryableAsync())
                    .Where(x => x.CustomerId == request.CustomerId)
                    .WhereIf(request.From.HasValue, x => x.CreationTime.Date >= request.From.Value.Date)
                    .WhereIf(request.To.HasValue, x => x.CreationTime.Date <= request.To.Value.Date)
                    .ToList();

            var customerReturns = new List<CustomerReturn>();
            if (request.BillLogType == null || request.BillLogType == BillLogType.Return)
                customerReturns = (await _customerReturnRepository.GetQueryableAsync())
                    .Where(x => x.CustomerId == request.CustomerId)
                    .WhereIf(request.From.HasValue, x => x.CreationTime.Date >= request.From.Value.Date)
                    .WhereIf(request.To.HasValue, x => x.CreationTime.Date <= request.To.Value.Date)
                    .ToList();

            var customer = (await _customerRepository.GetQueryableAsync())
                .FirstOrDefault(x => x.Id == request.CustomerId) ?? new Customer();

            var billCustomerProducts = (await _billCustomerProductRepository.GetQueryableAsync())
                .Where(x => billCustomers.Select(x => x.Id).Any(id => id == (x.BillCustomerId ?? Guid.Empty))).ToList() ?? new List<BillCustomerProduct>();

            var customerReturnProducts = (await _customerReturnProductRepository.GetQueryableAsync())
                .Where(x => customerReturns.Select(x => x.Id).Any(id => id == (x.CustomerReturnId ?? Guid.Empty))).ToList() ?? new List<CustomerReturnProduct>();

            var products = (await _productRepository.GetQueryableAsync())
                .Where(x => billCustomerProducts.Select(x => x.ProductId).Any(productId => productId == x.Id)
                || customerReturnProducts.Select(x => x.ProductId).Any(productId => productId == x.Id))
                .WhereIf(!request.ProductName.IsNullOrWhiteSpace(), x => x.Name.ToLower().Contains(request.ProductName.ToLower()))
                .ToList() ?? new List<Products>();

            var bills = billCustomers.Select(x => new BillLogDto()
            {
                Id = x.Id,
                Code = x.Code,
                CustomerId = x.CustomerId,
                StoreId = x.StoreId,
                CreationTime = x.CreationTime,
                BillLogType = BillLogType.Sale,
                DiscountValue = x.DiscountValue,
                DiscountUnit = x.DiscountUnit,
                AmountCustomerPay = x.AmountCustomerPay,
            }).Union(customerReturns.Select(x => new BillLogDto()
            {
                Id = x.Id,
                Code = x.Code,
                CustomerId = x.CustomerId,
                StoreId = x.StoreId,
                CreationTime = x.CreationTime,
                BillLogType = BillLogType.Return,
                DiscountValue = x.DiscountValue,
                DiscountUnit = x.DiscountUnit,
                AmountCustomerPay = customerReturnProducts.Where(cRP=>cRP.CustomerReturnId == x.Id).Sum(cRP=>cRP.TotalPriceAfterDiscount),
            })).ToList();

            var stores = (await _storeRepository.GetQueryableAsync())
                .Where(x => bills.Select(x => x.StoreId).Any(storeId => storeId == x.Id)).ToList() ?? new List<Stores>();

            foreach (var bill in bills)
            {
                var store = stores.FirstOrDefault(x => x.Id == bill.StoreId) ?? new Stores();
                var bCProducts = billCustomerProducts.Where(x => x.BillCustomerId == bill.Id && products.Select(x => x.Id).Any(id => id == (x.ProductId ?? Guid.Empty)));
                var cRProducts = customerReturnProducts.Where(x => x.CustomerReturnId == bill.Id);
                var productPurchaseHistorys = new List<ProductPurchaseHistory>();

                MoneyModificationType? discountUnit = null;
                decimal? discountValue = null;
                if (bill.DiscountUnit.HasValue && bill.DiscountValue.HasValue && bill.DiscountValue > 0)
                {
                    discountUnit = bill.DiscountUnit;
                    discountValue = bill.DiscountValue;
                    if (bill.DiscountUnit.HasValue && bill.DiscountUnit == MoneyModificationType.VND)
                    {
                        discountUnit = MoneyModificationType.Percent;
                        var total = (bill.DiscountValue ?? 0) + (bill.AmountCustomerPay ?? 0);
                        discountValue = (bill.DiscountValue ?? 0) / (total == 0 ? 1 : total) * 100;
                    }    
                    
                }
                //Bán hàng
                if (bCProducts.Any())
                {
                    foreach (var bCProduct in bCProducts)
                    {
                        var product = products.FirstOrDefault(x => x.Id == bCProduct.ProductId);
                        var preDiscountTotal = (bCProduct.Price ?? 0) * bCProduct.Quantity;
                        var afterDiscountTotal = preDiscountTotal - (bCProduct.DiscountValue ?? 0);
                        if (bCProduct.DiscountUnit == MoneyModificationType.Percent)
                            afterDiscountTotal = preDiscountTotal - preDiscountTotal * (bCProduct.DiscountValue ?? 0) / 100;

                        //Có chiết khấu của tổng hóa đơn
                        if (discountUnit.HasValue && discountUnit == MoneyModificationType.Percent && discountValue > 0)
                            afterDiscountTotal = preDiscountTotal - preDiscountTotal * (discountValue ?? 0) / 100;

                        productPurchaseHistorys.Add(new ProductPurchaseHistory()
                        {
                            ProductId = bCProduct.Id,
                            ProductCode = product?.Code,
                            ProductName = product?.Name,
                            Price = bCProduct.Price,
                            Quantity = bCProduct.Quantity,
                            DiscountValue = (discountValue.HasValue && discountValue > 0) ? discountValue : bCProduct.DiscountValue,
                            DiscountUnit =  discountUnit ?? bCProduct.DiscountUnit,
                            PreDiscountTotal = preDiscountTotal,
                            AfterDiscountTotal = afterDiscountTotal,
                        });
                    }
                }

                //trả hàng
                if (cRProducts.Any())
                {
                    foreach (var cRProduct in cRProducts)
                    {
                        var product = products.FirstOrDefault(x => x.Id == cRProduct.ProductId);
                        var preDiscountTotal = (cRProduct.Price ?? 0) * (cRProduct.Quantity ?? 0);
                        var afterDiscountTotal = preDiscountTotal - (cRProduct.DiscountValue ?? 0);
                        if (cRProduct.DiscountUnit == MoneyModificationType.Percent)
                            afterDiscountTotal = preDiscountTotal - preDiscountTotal * (cRProduct.DiscountValue ?? 0) / 100;

                        //Có chiết khẩu của cả hóa đơn
                        afterDiscountTotal = preDiscountTotal - (discountValue ?? 0);
                        if (discountUnit.HasValue && discountUnit == MoneyModificationType.Percent && discountValue > 0)
                            afterDiscountTotal = preDiscountTotal - preDiscountTotal * (discountValue ?? 0) / 100;

                        productPurchaseHistorys.Add(new ProductPurchaseHistory()
                        {
                            ProductId = cRProduct.Id,
                            ProductCode = product?.Code,
                            ProductName = product?.Name,
                            Price = cRProduct.Price,
                            Quantity = cRProduct.Quantity,
                            DiscountValue = (discountValue.HasValue && discountValue > 0) ? discountValue : cRProduct.DiscountValue,
                            DiscountUnit =  discountUnit ?? cRProduct.DiscountUnit,
                            PreDiscountTotal = preDiscountTotal,
                            AfterDiscountTotal = afterDiscountTotal,
                        });
                    }
                }

                PurchaseHistoryResponses.Add(new PurchaseHistoryResponse()
                {
                    BillId = bill.Id,
                    BillCode = bill.Code,
                    BillLogType = bill.BillLogType,
                    StoreId = store.Id,
                    StoreCode = store.Code,
                    StoreName = store.Name,
                    CustomerId = customer.Id,
                    CustomerCode = customer.Code,
                    CustomerName = customer.Name,
                    CustomerPhone = customer.PhoneNumber,
                    CreationTime = bill.CreationTime,
                    ProductPurchaseHistorys = productPurchaseHistorys,
                });

            }
            
            if (!PurchaseHistoryResponses.Any())
                return new PagingPurchaseHistoryResponse(0, new List<PurchaseHistoryResponse>(), 0);

            var result = PurchaseHistoryResponses
                .OrderByDescending(p => p.CreationTime)
                .Skip(request.Offset)
                .Take(request.PageSize)
                .ToList();

            var totalMoney = result.Sum(x => x.ProductPurchaseHistorys.Sum(x => x.AfterDiscountTotal));

            return new PagingPurchaseHistoryResponse(PurchaseHistoryResponses.Count, result, totalMoney);
        }
    }
}
