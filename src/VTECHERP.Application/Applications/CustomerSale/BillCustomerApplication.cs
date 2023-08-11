using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VTECHERP.Constants;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.BillCustomers.Params;
using VTECHERP.DTOs.BillCustomers.Respons;
using VTECHERP.DTOs.ExchangeReturn;
using VTECHERP.Entities;
using VTECHERP.Enums;
using VTECHERP.Enums.Bills;
using VTECHERP.Helper;
using VTECHERP.ServiceInterfaces;
using VTECHERP.Services;
using BillCustomer = VTECHERP.Entities.BillCustomer;
using BillCustomerProductDto = VTECHERP.DTOs.BillCustomers.Respons.BillCustomerProductDto;

namespace VTECHERP
{
    [Authorize]
    public class BillCustomerApplication : ApplicationService
    {
        private readonly IBillCustomerService _billCustomerService;
        private readonly IRepository<BillCustomer> _billCustomerRepository;
        private readonly IRepository<Products> _productRepository;
        private readonly IRepository<StoreProduct> _storeProductRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<BillCustomerProduct> _billCustomerProductRepository;
        private readonly IRepository<PriceTable> _priceTableRepository;
        private readonly IRepository<PriceTableStore> _priceTableStoreRepository;
        private readonly IRepository<PriceTableProduct> _priceTableProductRepository;
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<Stores> _storeRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IdentityUserManager _userManager;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly IRepository<BillCustomerProductBonus> _billCustomerProductBonusRepository;
        private readonly IRepository<HistoryPrintBillCustomer> _historyPrintBillCustomerRepository;
        private readonly ICurrentUser _currentUser;
        private readonly ICustomerReturnService _customerReturnService;
        private readonly IDebtCustomerService _customerDebtService;
        private readonly IRepository<PriceTableCustomer> _priceTableCustomerRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IAttachmentService _attachmentService;
        public BillCustomerApplication(
            IRepository<BillCustomer> billCustomerRepository
            , IRepository<Products> productRepository
            , IRepository<StoreProduct> storeProductRepository
            , IRepository<Customer> customerRepository
            , IRepository<BillCustomerProduct> billCustomerProductRepository
            , IRepository<PriceTable> priceTableRepository
            , IRepository<PriceTableStore> priceTableStoreRepository
            , IRepository<PriceTableProduct> priceTableProductRepository
            , IRepository<Account> accountRepository
            , IBillCustomerService billCustomerService
            , IRepository<Stores> storeRepository
            , IdentityUserManager userManager
            , IRepository<Employee> employeeRepository
            , IRepository<UserStore> userStoreRepository
            , ICurrentUser currentUser
            , ICustomerReturnService customerReturnService
            , IRepository<BillCustomerProductBonus> billCustomerProductBonusRepository
            , IRepository<HistoryPrintBillCustomer> historyPrintBillCustomerRepository
            , IDebtCustomerService customerDebtService
            , IRepository<PriceTableCustomer> priceTableCustomerRepository
            , IUnitOfWorkManager unitOfWorkManager
            , IAttachmentService attachmentService
            )
        {
            _billCustomerRepository = billCustomerRepository;
            _productRepository = productRepository;
            _storeProductRepository = storeProductRepository;
            _customerRepository = customerRepository;
            _billCustomerProductRepository = billCustomerProductRepository;
            _priceTableRepository = priceTableRepository;
            _priceTableStoreRepository = priceTableStoreRepository;
            _priceTableProductRepository = priceTableProductRepository;
            _accountRepository = accountRepository;
            _billCustomerService = billCustomerService;
            _storeRepository = storeRepository;
            _userManager = userManager;
            _employeeRepository = employeeRepository;
            _userStoreRepository = userStoreRepository;
            _currentUser = currentUser;
            _customerReturnService = customerReturnService;
            _historyPrintBillCustomerRepository = historyPrintBillCustomerRepository;
            _billCustomerProductBonusRepository = billCustomerProductBonusRepository;
            _customerDebtService = customerDebtService;
            _priceTableCustomerRepository = priceTableCustomerRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _attachmentService = attachmentService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(BillCustomerCreateParam param)
        {
            using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
            {
                if (param == null)
                    return new GenericActionResult(400, false, "Dữ liệu không hợp lệ");

                var respon = await _billCustomerService.CreateCustomerBill(param);
                var httpStatus = respon.Item3 ? 200 : 400;
                if (respon.Item3)
                    await uow.CompleteAsync();

                return new GenericActionResult(httpStatus, respon.Item3, respon.Item2, respon.Item1);
            }    
        }

        [HttpPost]
        public async Task<IActionResult> GetListSeach(BillCustomerGetListParam param)
        {
            try
            {
                var result = new List<BillCustomerListItem>();

                var billCustomerQueryable = await _billCustomerRepository.GetQueryableAsync();
                var billCustomerProductQueryable = await _billCustomerProductRepository.GetQueryableAsync();
                var productQueryable = await _productRepository.GetQueryableAsync();
                var customerQueryable = await _customerRepository.GetQueryableAsync();
                var storeQueryable = await _storeRepository.GetQueryableAsync();
                var userStore = await _userStoreRepository.GetQueryableAsync();

                if (param.BillCustomerCode != null)
                    param.BillCustomerCode = param.BillCustomerCode.Trim();

                var storeOfUser = userStore.Where(x => x.UserId == _currentUser.Id).Select(x => x.StoreId).ToList();

                var query = from billCustomer in billCustomerQueryable

                            join store in storeQueryable
                            on billCustomer.StoreId equals store.Id

                            join customer in customerQueryable
                            on billCustomer.CustomerId equals customer.Id

                            let billProducts = (from billProduct in billCustomerProductQueryable

                                                join product in productQueryable
                                                on billProduct.ProductId equals product.Id

                                                where billCustomer.Id == billProduct.BillCustomerId
                                                select new BillCustomerProductDto
                                                {
                                                    ProductId = product.Id,
                                                    ProductName = product.Name,
                                                    Price = billProduct.Price.GetValueOrDefault(),
                                                    Quantity = billProduct.Quantity,
                                                    Unit = product.Unit,
                                                    BillCustomerProductId = billProduct.Id,
                                                    ProductCategory = product.CategoryId,
                                                    CostPrice = billProduct.CostPrice.GetValueOrDefault(),
                                                })
                                                .ToList()

                            where storeOfUser.Contains(billCustomer.StoreId ?? Guid.Empty)

                            select new BillCustomerListItem
                            {
                                BillCustomerProducts = billProducts,
                                BillCustomerCode = billCustomer.Code,
                                BillCustomerId = billCustomer.Id,
                                BillCustomerPayStatus = billCustomer.CustomerBillPayStatus,
                                CreatorId = billCustomer.CreatorId,
                                CustomerId = billCustomer.Id,
                                DiscountUnit = billCustomer.DiscountUnit,
                                DiscountValue = billCustomer.DiscountValue,
                                StoreId = billCustomer.StoreId,
                                VatUnit = billCustomer.VatUnit,
                                VatValue = billCustomer.VatValue,
                                PayNote = billCustomer.PayNote,
                                CustomerText = customer.Name,
                                CreateTime = billCustomer.CreationTime,
                                AmountAfterDiscount = billCustomer.AmountAfterDiscount,
                                AmountCustomerPay = billCustomer.AmountCustomerPay,
                                StoreText = store.Name,
                                AmountTotal = billProducts.Sum(x => x.Quantity * x.Price),
                                Cash = billCustomer.Cash.GetValueOrDefault(),
                                Banking = billCustomer.Banking.GetValueOrDefault(),
                                TransportForm = billCustomer.TransportForm,
                                EmployeeNote = billCustomer.EmployeeNote,
                                DiscountCash = billCustomer.DiscountCash
                            };

                if (param.StoreIds != null && param.StoreIds.Any())
                    query = query.Where(x => param.StoreIds.Contains(x.StoreId ?? Guid.Empty));

                if (!string.IsNullOrEmpty(param.BillCustomerCode))
                    query = query.Where(x => x.BillCustomerCode.Contains(param.BillCustomerCode));

                if (param.CreateTimeFrom.HasValue)
                    query = query.Where(x => x.CreateTime.Date >= param.CreateTimeFrom.Value.Date);

                if (param.CreateTimeTo.HasValue)
                    query = query.Where(x => x.CreateTime.Date <= param.CreateTimeTo.Value.Date);

                if (!string.IsNullOrEmpty(param.CustomerName))
                    query = query.Where(x => x.CustomerText.Contains(param.CustomerName));

                if (!string.IsNullOrEmpty(param.ProductName))
                    query = query.Where(x => x.BillCustomerProducts.Select(y => y.ProductName).Any(c => c.ToLower().Contains(param.ProductName.Trim().ToLower())));

                if (param.ProductCategory.HasValue)
                    query = query.Where(x => x.BillCustomerProducts.Select(y => y.ProductCategory).Contains(param.ProductCategory));

                if (param.CustomerBillPayStatus.HasValue)
                    query = query.Where(x => x.BillCustomerPayStatus == param.CustomerBillPayStatus);

                if (param.IsCheckData)
                {
                    var statusNotCheck = new List<CustomerBillPayStatus> { CustomerBillPayStatus.WaitCall, CustomerBillPayStatus.CustomerOrder };
                    query = query.Where(x => x.BillCustomerProducts.Any(y => y.Price < y.CostPrice));
                    query = query.Where(x => !statusNotCheck.Contains((x.BillCustomerPayStatus ?? CustomerBillPayStatus.CustomerOrder)));
                }

                result = query.OrderByDescending(x => x.BillCustomerCode).Skip((param.PageIndex - 1) * param.PageSize).Take(param.PageSize).ToList();
                
                foreach (var item in result)
                {
                    item.Attachments = await _attachmentService.GetAttachmentByObjectIdAsync(item.BillCustomerId);
                    item.BillCustomerPayStatusText = _billCustomerService.MapBillCustomerStatus(item.BillCustomerPayStatus);
                    var user = await _userManager.FindByIdAsync(item.CreatorId.ToString());
                    item.CreatorText = user == null ? "" : user.Name;
                }

                return new GenericActionResult(new
                {
                    Total = query.Count(),
                }, result);
            }
            catch (Exception ex)
            {
                return new GenericActionResult(500, false, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProductDropList(Guid StoreId, CustomerType? customerType)
        {
            var result = new List<ProductDropList>();
            var productQueryable = await _productRepository.GetQueryableAsync();
            var storeProductQueryable = (await _storeProductRepository.GetQueryableAsync()).Where(x => x.StoreId == StoreId);
            var attachments = (await _attachmentService.ListAttachmentByObjectIdAsync(productQueryable.Select(x => x.Id).ToList()));
            result = (from  product in productQueryable

                      join storePro in storeProductQueryable
                      on product.Id equals storePro.ProductId
                      into storeProGr
                      from storePro in storeProGr.DefaultIfEmpty()

                      //where storePro.StoreId == StoreId

                      select new ProductDropList
                      {
                          ProductId = product.Id,
                          ProductName = product.Name,
                          Inventory = storePro == null ? 0 : storePro.StockQuantity,

                          SalePrice = customerType.HasValue ? (customerType == CustomerType.RetailCustomer ? product.SalePrice ?? 0 
                          : (customerType == CustomerType.SPACustomer ? product.SPAPrice ?? 0 
                          : product.WholeSalePrice ?? 0)) : 0,

                          CostPrice = product.StockPrice,
                          SalePriceDto = product.SalePrice,
                          SPAPrice = product.SPAPrice,
                          WholeSalePrice = product.WholeSalePrice
                      })
                        .ToList();

            result.ForEach(x =>
            {
                x.Attachments = attachments.Where(a => a.ObjectId == x.ProductId).OrderBy(x=>x.CreationTime).ToList();
            });

            return new GenericActionResult(200, true, "", result);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsByParent(Guid ParentId, CustomerType? customerType, Guid StoreId)
        {
            var result = new List<ProductDropList>();
            var productQueryable = await _productRepository.GetQueryableAsync();
            var storeProductQueryable = (await _storeProductRepository.GetQueryableAsync()).Where(x => x.StoreId == StoreId);

            result = (from product in productQueryable

                      join storePro in storeProductQueryable
                      on product.Id equals storePro.ProductId 
                      into storePrGr
                      from storePro in storePrGr.DefaultIfEmpty()

                      where product.ParentId == ParentId
                      //&& storePro.StoreId == StoreId

                      select new ProductDropList
                      {
                          ProductId = product.Id,
                          ProductName = product.Name,
                          Inventory = storePro == null ? 0 : storePro.StockQuantity,
                          SalePrice = customerType.HasValue ? (customerType == CustomerType.RetailCustomer ? product.SalePrice ?? 0 : (customerType == CustomerType.SPACustomer ? product.SPAPrice ?? 0 : product.WholeSalePrice ?? 0)) : 0,
                          CostPrice = product.StockPrice,
                          SalePriceDto = product.SalePrice,
                          SPAPrice = product.SPAPrice,
                          WholeSalePrice = product.WholeSalePrice
                      })
                      .ToList();

            return new GenericActionResult(200, true, "", result);
        }

        [HttpGet]
        public async Task<IActionResult> SearchCustomer(string CustomerName, string CustomerPhone)
        {
            var result = new List<CustomerSearch>();

            var customerQueryable = await _customerRepository.GetQueryableAsync();
            if (!string.IsNullOrEmpty(CustomerName))
                customerQueryable = customerQueryable.Where(x => x.Name.Trim().ToLower().Contains(CustomerName.Trim().ToLower()));

            if (!string.IsNullOrEmpty(CustomerPhone))
                customerQueryable = customerQueryable.Where(x => x.PhoneNumber.Trim().ToLower().Contains(CustomerPhone.Trim().ToLower()));

            result = customerQueryable.Select(x => new CustomerSearch
            {
                CustomerId = x.Id
                ,
                CustomerName = x.Name
                ,
                Address = x.Address
                ,
                CustomerPhone = x.PhoneNumber
                ,
                CustomerType = x.CustomerType
                ,
                ProvinceId = x.ProvinceId
                ,
                EmployeeCare = x.SupportEmployeeId
                ,
                EmployeeRespon = x.HandlerEmployeeId
                ,
                HandlerEmpName = x.HandlerEmpName
                ,
                DebtLimit = x.DebtLimit.GetValueOrDefault()
                ,
                DebtGroup = x.DebtGroup
                , Note = x.Note
            })
            .ToList();

            return new GenericActionResult(200, true, "", result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CustomerNewParam param)
        {
            var result = await _billCustomerService.AddCustomerForBillCustomer(param);
            var httpStatus = result.Item3 ? 200 : 400;
            return new GenericActionResult(httpStatus, result.Item3, result.Item2, result.Item1);
        }

        [HttpGet]
        public async Task<IActionResult> GetPriceTables(Guid StoreId, Guid? CustomerId)
        {
            var storePriceTableQueryable = await _priceTableStoreRepository.GetQueryableAsync();
            var priceTableQueryable = await _priceTableRepository.GetQueryableAsync();
            var priceTableCustomerQueryable = await _priceTableCustomerRepository.GetQueryableAsync();

            var timeNow = DateTime.Now;

            var priceTableStore = (from tablePrice in priceTableQueryable
                                   join store in storePriceTableQueryable
                                   on tablePrice.Id equals store.PriceTableId
                                   where store.StoreId == StoreId
                                   && tablePrice.Status == PriceTableStatus.Active
                                   && tablePrice.AppliedFrom.Date <= timeNow.Date
                                   && (tablePrice.AppliedTo == null ? true : tablePrice.AppliedTo >= timeNow.Date)
                                   select new
                                   {
                                       Name = tablePrice.Name,
                                       Id = tablePrice.Id,
                                       IsCustomer = false
                                   })
                                  .ToList();

            if (CustomerId.HasValue && CustomerId != Guid.Empty)
            {
                var priceTables = (from priceTableCus in priceTableCustomerQueryable
                                  join priTable in priceTableQueryable
                                  on priceTableCus.PriceTableId equals priTable.Id

                                  where 
                                  //!priceTableStore.Select(x => x.Id).Contains(priceTableCus.PriceTableId) &&
                                  priceTableCus.CustomerId == CustomerId &&
                                  priTable.Status == PriceTableStatus.Active &&
                                  priTable.AppliedFrom.Date <= timeNow.Date &&
                                  (priTable.AppliedTo == null ? true : priTable.AppliedTo >= timeNow.Date)

                                   select new
                                  {
                                      Name = priTable.Name,
                                      Id = priTable.Id,
                                      IsCustomer = true
                                  })
                                  .ToList();

                if(priceTables.Any())
                {
                    var priceTableCustomerId = priceTables.Select(x => x.Id).ToList();
                    priceTableStore = priceTableStore.Where(x => !priceTableCustomerId.Contains(x.Id)).ToList();
                    priceTableStore.AddRange(priceTables);
                }    
            }

            return new GenericActionResult(200, true, "", priceTableStore);
        }

        [HttpPost]
        public async Task<IActionResult> GetPriceProductByPriceTable(ProductPriceByPriceTableParam param)
        {
            var result = new List<ProductBasicInfo>();

            foreach (var item in param.ProductId)
            {
                var resultItem = new ProductBasicInfo();
                resultItem.ProductId = item;

                var priceTableProduct = await _priceTableProductRepository.FirstOrDefaultAsync(x => x.PriceTableId == param.PriceTableId && x.ProductId == item);
                if (priceTableProduct == null)
                    resultItem.SalePrice = 0;
                else
                    resultItem.SalePrice = priceTableProduct.Price;

                result.Add(resultItem);
            }

            return new GenericActionResult(200, true, "", result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAccountByStore(Guid StoreId, AccountType accountType)
        {
            var account = await _accountRepository.GetListAsync(x => x.StoreId == StoreId 
            && x.AccountType == accountType
            && x.IsActive
            );

            var result = account.Select(x => new
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            })
            .ToList();

            return new GenericActionResult(200, true, "", result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBillCustomer(Guid BillCustomerId, BillCustomerCreateParam param)
        {
            using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
            {
                try
                {
                    var billCustomer = await _billCustomerRepository.FindAsync(x => x.Id == BillCustomerId);
                    if (billCustomer == null)
                    {
                        return new BadRequestObjectResult(new
                        {
                            message = $"Không tìm thấy hóa đơn id là {BillCustomerId}"
                        });
                    }

                    var userStore = await _userStoreRepository.GetQueryableAsync();
                    var storeOfUser = userStore.Where(x => x.UserId == _currentUser.Id).Select(x => x.StoreId).ToList();

                    if (!storeOfUser.Contains(billCustomer.StoreId ?? Guid.Empty))
                        return new BadRequestObjectResult("User không có quyền với hóa đơn");

                    if (!_billCustomerService.CheckRuleUpdateStatus(billCustomer.CustomerBillPayStatus, param.CustomerBillPayStatus, null))
                    {
                        return new BadRequestObjectResult(new
                        {
                            message = $"Trạng thái không hợp lệ",
                            success = false
                        });
                    }

                    var result = await _billCustomerService.UpdateBillCustomer(BillCustomerId, param);

                    var httpStatus = result.Item3 ? 200 : 400;
                    if (result.Item3)
                        await uow.CompleteAsync();

                    return new GenericActionResult(httpStatus, result.Item3, result.Item2);
                }
                catch (Exception ex)
                {
                    return new GenericActionResult(500, false, ex.Message);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDetail(Guid BillCustomerId)
        {
            try
            {
                var result = await _billCustomerService.GetDetail(BillCustomerId);
                var httpStatus = result.Item3 ? 200 : 400;
                result.Item1.Attachments = await _attachmentService.GetAttachmentByObjectIdAsync(BillCustomerId);
                return new GenericActionResult(httpStatus, result.Item3, result.Item2, result.Item1);
            }
            catch (Exception ex)
            {
                return new GenericActionResult(500, false, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployee()
        {
            var employees = await _employeeRepository.GetListAsync(x => x.IsActive);
            var result = employees.Select(x => new
            {
                Id = x.Id,
                Name = x.Name,
            })
            .ToList();

            return new GenericActionResult(200, true, "", result);
        }

        [HttpGet]
        public async Task<IActionResult> GetDetailById(Guid BillCustomerId)
        {
            try
            {
                var result = await _billCustomerService.GetDetailById(BillCustomerId);
                var httpStatus = result.Item3 ? 200 : 400;
                return new GenericActionResult(httpStatus, result.Item3, result.Item2, result.Item1);
            }
            catch (Exception ex)
            {
                return new GenericActionResult(500, false, ex.Message);
            }
        }

        [HttpPost]
        public async Task<PagingLogBillCustomerResponse> GetLogBillByIdAsync(LogBillCustomerRequest request)
        {
            return await _billCustomerService.GetLogBillByIdAsync(request);
        }

        [HttpGet]
        public async Task<IActionResult> GetBillProductByBillCustomerId(BillCustomerByIdParam param)
        {
            try
            {
                var result = await _billCustomerService.GetBillProductByBillCustomerId(param);
                var httpStatus = result.Item3 ? 200 : 400;
                return new GenericActionResult(new
                {
                    Total = result.Item4
                }, result.Item1);
            }
            catch (Exception ex)
            {
                return new GenericActionResult(500, false, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEntriesByBillCustomerId(BillCustomerByIdParam param)
        {
            try
            {
                var result = await _billCustomerService.GetEntriesByBillCustomerId(param);
                var httpStatus = result.Item3 ? 200 : 400;
                return new GenericActionResult(new
                {
                    Total = result.Item4
                }, result.Item1);
            }
            catch (Exception ex)
            {
                return new GenericActionResult(500, false, ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBillCustomer(Guid BillCustomerId)
        {
            using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
            {
                try
                {
                    var billCustomer = await _billCustomerRepository.FindAsync(x => x.Id == BillCustomerId);
                    if (billCustomer == null)
                    {
                        return new BadRequestObjectResult(new
                        {
                            message = $"Không tìm thấy hóa đơn id là {BillCustomerId}"
                        });
                    }

                    var userStore = await _userStoreRepository.GetQueryableAsync();
                    var storeOfUser = userStore.Where(x => x.UserId == _currentUser.Id).Select(x => x.StoreId).ToList();

                    if (!storeOfUser.Contains(billCustomer.StoreId ?? Guid.Empty))
                        return new BadRequestObjectResult("User không có quyền với hóa đơn");

                    var result = await _billCustomerService.DeleteBillCustomer(BillCustomerId);
                    if (result.Item2)
                        await uow.CompleteAsync();

                    var httpStatus = result.Item2 ? 200 : 400;
                    return new GenericActionResult(httpStatus, result.Item2, result.Item1);
                }
                catch (Exception ex)
                {
                    return new GenericActionResult(500, false, ex.Message);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetListStoreByUser()
        {
            var storeQueryable = await _storeRepository.GetQueryableAsync();
            var userStoreQueryable = await _userStoreRepository.GetQueryableAsync();

            var stores = (from store in storeQueryable
                          join userStore in userStoreQueryable
                          on store.Id equals userStore.StoreId

                          where userStore.UserId == _currentUser.Id
                          select new
                          {
                              Id = store.Id,
                              Name = store.Name,
                          }).ToList();
            stores = stores.Distinct().ToList();   
            return new OkObjectResult(stores);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatusCancel(UpdateStatusCancelBillCustomerParam param)
        {
            try
            {
                var billCustomer = await _billCustomerRepository.FindAsync(x => x.Id == param.BillCustomerId);
                if (billCustomer == null)
                {
                    return new BadRequestObjectResult(new
                    {
                        message = $"Không tìm thấy hóa đơn id là {param.BillCustomerId}"
                    });
                }

                var userStore = await _userStoreRepository.GetQueryableAsync();
                var storeOfUser = userStore.Where(x => x.UserId == _currentUser.Id).Select(x => x.StoreId).ToList();

                if (!storeOfUser.Contains(billCustomer.StoreId ?? Guid.Empty))
                    return new BadRequestObjectResult("User không có quyền với hóa đơn");

                if (!_billCustomerService.CheckRuleUpdateStatus(billCustomer.CustomerBillPayStatus, param.Status, null))
                {
                    return new BadRequestObjectResult(new
                    {
                        message = $"Trạng thái không hợp lệ",
                        success = false
                    });
                }

                var billCustomerProduct = await _billCustomerProductRepository.GetListAsync(x => x.BillCustomerId == billCustomer.Id);
                var billCustomerProductIds = billCustomerProduct.Select(y => y.Id).ToList();
                var billCustomerProductBonus = await _billCustomerProductBonusRepository.GetListAsync(x => billCustomerProductIds.Contains(x.BillCustomerProductId ?? Guid.Empty));

                // Kiểm tra số lượng yêu cầu với tồn của sản phẩm
                var statusCheckQuantity = new List<CustomerBillPayStatus> { CustomerBillPayStatus.Success
                    ,CustomerBillPayStatus.Taked
                    ,CustomerBillPayStatus.Checked
                    ,CustomerBillPayStatus.Confirm
                };
                if (statusCheckQuantity.Contains(param.Status))
                {
                    var productChecks = billCustomerProduct.Select(x => new BillCustomerProductCheckValid
                    {
                        ProductId = x.ProductId.GetValueOrDefault(),
                        Quantity = x.Quantity
                    })
                .ToList();

                    var productBonus = billCustomerProductBonus.Select(x => new BillCustomerProductCheckValid
                    {
                        ProductId = x.ProductId.GetValueOrDefault(),
                        Quantity = x.Quantity
                    })
                    .ToList();

                    productChecks.AddRange(productBonus);
                    var responCheckQuantity = await _billCustomerService.CheckInventoryProduct(productChecks, billCustomer.StoreId);
                    if (!responCheckQuantity.isValid)
                    {
                        return new BadRequestObjectResult(new
                        {
                            message = responCheckQuantity.message,
                            success = false
                        });
                    }
                }

                if (param.Status == CustomerBillPayStatus.Cancel)
                {
                    billCustomer.ReasonCancel = param.ReasonCancel;
                    
                    var paramCreateCustomerReturn = new CreateCustomerReturnRequest
                    {
                        ReturnAmount = billCustomer.AmountCustomerPay,
                        StoreId = billCustomer.StoreId,
                        CustomerId = billCustomer.CustomerId,
                        BillCustomerId = billCustomer.Id,
                        DiscountUnit = billCustomer.DiscountUnit,
                        DiscountValue = billCustomer.DiscountValue,
                        AccountCodeBanking = billCustomer.AccountCodeBanking,
                        Banking = billCustomer.Banking,
                        Cash = billCustomer.Cash,
                        AccountCode = billCustomer.AccountCode,
                    };

                    var productReturn = billCustomerProduct.Select(x => new CustomerReturnProductDTO
                    {
                        DiscountUnit = x.DiscountUnit,
                        DiscountValue = x.DiscountValue,
                        Price = x.Price,
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                    })
                    .ToList();

                    paramCreateCustomerReturn.Products = productReturn;

                    var customerReturn = await _customerReturnService.Create(paramCreateCustomerReturn);
                    var idCustomerReturn = (customerReturn.Item1).Id;
                    var responConfirm = await _customerReturnService.ConfirmAsync(idCustomerReturn);

                    if (!responConfirm.Item2)
                    {
                        return new BadRequestObjectResult(new
                        {
                            message = responConfirm.Item1
                        });
                    }
                }

                if (param.Status == CustomerBillPayStatus.Success || param.Status == CustomerBillPayStatus.Checked)
                    await _billCustomerService.HandleDocumentBillCustomer(billCustomer.Id, param.Status);

                if (param.Status == CustomerBillPayStatus.Checked)
                    await _billCustomerService.HandleTransportInformationForBill(billCustomer);

                billCustomer.CustomerBillPayStatus = param.Status;

                return new OkObjectResult(new
                {
                    success = true,
                    message = "Cập nhật trạng thái thành công"
                }) ;
            }
            catch (Exception ex)
            {
                return new ObjectResult(new {message = ex.Message, success = false})
                {
                    StatusCode = 403
                };
            }
        }

        [HttpGet]
        public async Task<IActionResult> Print(Guid BillCustomerId)
        {
            try
            {
                var billCustomer = await _billCustomerRepository.FirstOrDefaultAsync(x => x.Id == BillCustomerId);
                if (billCustomer == null)
                {
                    return new BadRequestObjectResult(new
                    {
                        message = $"Không tìm thấy hóa đơn có id là {BillCustomerId}"
                    });
                }
                CultureInfo culture = new CultureInfo("en-US");

                // Lấy template
                string template = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "Resources\\Templates\\TemplateInHoaDon.html");
                template = template.Replace("{{BillCustomerCode}}", billCustomer.Code);
                template = template.Replace("{{BillCustomerNote}}", billCustomer.EmployeeNote);
                template = template.Replace("{{BillCustomer_CreateTime}}", billCustomer.CreationTime.ToString("dd/MM/yyyy"));
               
                var store = await _storeRepository.FirstOrDefaultAsync(x => x.Id == billCustomer.StoreId);
                var storeName = store == null ? "" : store.Name;
                template = template.Replace("{{Store}}", storeName);

                // Thông tin khách hàng
                var customer = await _customerRepository.FirstOrDefaultAsync(x => x.Id == billCustomer.CustomerId);
                if (customer != null)
                {
                    template = template.Replace("{{CustomerName}}", customer.Name);
                    template = template.Replace("{{CustomerPhone}}", customer.PhoneNumber);
                    template = template.Replace("{{CustomerAddress}}", customer.Address);
                }

                // Thông tin sản phẩm
                var billCustomerProducts = await _billCustomerProductRepository.GetListAsync(x => x.BillCustomerId == billCustomer.Id);
                var billCustomerProductDto = billCustomerProducts.Select(x => new BillCustomerProductItem
                {
                    Id = x.Id,
                    CostPrice = x.CostPrice,
                    Price = x.Price.GetValueOrDefault(),
                    ProductId = x.ProductId.GetValueOrDefault(),
                    Quantity = x.Quantity,
                    DiscountUnit = x.DiscountUnit,
                    DiscountValue = x.DiscountValue,
                })
                .ToList();

                var billCustomerProductIds = billCustomerProducts.Select(x => x.Id).ToList();
                var productBonus = await _billCustomerProductBonusRepository.GetListAsync(x => billCustomerProductIds.Contains(x.BillCustomerProductId ?? Guid.Empty));

                foreach (var item in productBonus)
                {
                    var product = new BillCustomerProductItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = 0,
                        DiscountValue = 0,
                        DiscountUnit = MoneyModificationType.VND
                    };

                    billCustomerProductDto.Add(product);
                }

                var productIds = billCustomerProducts.Select(x => x.ProductId).ToList();
                var productOrigin = await _productRepository.GetListAsync(x => productIds.Contains(x.Id));

                var totalAmount = billCustomerProductDto.Sum(x => x.Price * x.Quantity);
                var customerPaid = billCustomer.Cash + billCustomer.Banking;
                decimal customerDebtOld = (await _customerDebtService.TotalDebtCustomer(new DTOs.DebtCustomer.SearchDebtCustomerRequest
                {
                    CustomerId = billCustomer.CustomerId,
                    ToDate= billCustomer.CreationTime.AddDays(-1),
                })).Debt;

                var customerDebtBill = billCustomer.AmountCustomerPay;
                var vat = billCustomer.VatValue ?? 0;
                decimal discountAmount = 0;

                var bodyTable = "";
                for (int i = 0; i < billCustomerProductDto.Count; i++)
                {
                    var item = billCustomerProductDto[i];

                    var product = productOrigin.FirstOrDefault(x => x.Id == item.ProductId);
                    var productName = product == null ? "" : product.Name;
                    var productUnit = product == null ? "" : product.Unit;

                    bodyTable += $"<tr height='19' style='mso-height-source:userset;height:14.5pt'>";
                    bodyTable += $"<td height='17' class='x39' style='height:13pt;'>{i + 1}</td>";
                    bodyTable += $"<td colspan='2' class='x40' style='border-right:1px solid windowtext;border-bottom:1px solid windowtext;'>{productName}</td>";
                    bodyTable += $"<td class='x41'>{productUnit}</td>";
                    bodyTable += $"<td class='x37'>{item.Quantity.ToString("N0", culture)}</td>";
                    bodyTable += $"<td class='x41' align='right'>{item.Price.GetValueOrDefault().ToString("N0", culture)}</td>";
                    bodyTable += $"<td class='x42' align='right'>{(item.Quantity * item.Price.GetValueOrDefault()).ToString("N0", culture)}</td>";
                    bodyTable += $"<tr></tr>";

                    if (item.DiscountValue.HasValue && item.DiscountValue > 0)
                    {
                        if (item.DiscountUnit == MoneyModificationType.VND)
                            discountAmount += item.DiscountValue.Value;

                        if (billCustomer.DiscountUnit == MoneyModificationType.Percent)
                            discountAmount += (item.DiscountValue.Value / 100) * (item.Quantity * item.Price.GetValueOrDefault());
                    }
                }

                var footer = "";
                footer += $@" <tr height='18' style='mso-height-source:userset;height:13.5pt'>
<td colspan='6' height='16' class='x43' style='border-right:1px solid windowtext;border-bottom:1px solid windowtext;height:12pt;'>
     <div style='float:right'>Tổng tiền đơn</div>
</td>
<td class='x42' align='right'>{totalAmount.GetValueOrDefault().ToString("N0", culture)}</td>
 </tr>";

                if (discountAmount > 0)
                {
                    footer += $@" <tr height='18' style='mso-height-source:userset;height:13.5pt'>
<td colspan='6' height='16' class='x43' style='border-right:1px solid windowtext;border-bottom:1px solid windowtext;height:12pt;'>
     <div style='float:right'>Chiết khấu</div>
</td>
<td class='x42' align='right'>{discountAmount.ToString("N0", culture)}</td>
 </tr>";
                }

                if (vat > 0)
                {
                    footer += $@" <tr height='18' style='mso-height-source:userset;height:13.5pt'>
<td colspan='6' height='16' class='x43' style='border-right:1px solid windowtext;border-bottom:1px solid windowtext;height:12pt;'>
     <div style='float:right'>VAT</div>
</td>
<td class='x42' align='right'>{vat.ToString("N0", culture)}</td>
 </tr>";
                }

                template = template.Replace("{{Products}}", bodyTable);
                template = template.Replace("{{Footer}}", footer);

                template = template.Replace("{{customerDebtBill}}", customerDebtBill.GetValueOrDefault().ToString("N0", culture));
                template = template.Replace("{{customerDebtOld}}", customerDebtOld.ToString("N0", culture));
                template = template.Replace("{{customerPaid}}", customerPaid.GetValueOrDefault().ToString("N0", culture));
                template = template.Replace("{{totalDebt}}", (customerDebtBill + customerDebtOld - customerPaid).GetValueOrDefault().ToString("N0", culture));

                var amountToText = ConvertNumberToText.NumberToText((double)customerDebtBill);
                template = template.Replace("{{AmountWithText}}", amountToText);

                HtmlToPdf convertor = new HtmlToPdf();
                SelectPdf.PdfDocument document = convertor.ConvertHtmlString(template);
                byte[] data = document.Save();
                document.Close();

                var fileName = $"Hóa đơn bán hàng";

                var history = new HistoryPrintBillCustomer
                {
                    BillCustomerId = billCustomer.Id
                };

                await _historyPrintBillCustomerRepository.InsertAsync(history);

                return new FileContentResult(data, "application/pdf")
                {
                    FileDownloadName = fileName,
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public async Task<IActionResult> UpdateBillCustomerNote(BillCustomerUpdateNote param)
        {
            var billCustomer = await _billCustomerRepository.FindAsync(x => x.Id == param.BillCustomerId);
            if (billCustomer == null)
            {
                return new BadRequestObjectResult(new
                {
                    success = false,
                    message = $"Không tìm thấy hóa đơn id là {param.BillCustomerId}"
                }) ;
            }

            billCustomer.PayNote = param.Note;

            await _billCustomerRepository.UpdateAsync(billCustomer);

            return new OkObjectResult(new
            {
                success = true,
                message = $"Cập nhật hóa đơn thành công"
            });
        }

        [HttpPost]
        public async Task<PagingResponse<HistoryBillResponse>> GetHistoryBillByCustomerId(HistoryBillRequest request)
        {
            return await _billCustomerService.GetHistoryBillByCustomerId(request);
        }

        [HttpPost]
        public async Task<FileResult> ExportBillCustomerAsync(BillCustomerGetListParam request)
        {
            try
            {
                var file = await _billCustomerService.ExportBillCustomer(request);
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"DanhSach_Phiếu nháp_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
                };
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                throw;
            }
        }
    }
}
