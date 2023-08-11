using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VTECHERP.Constants;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.Stores.Params;
using VTECHERP.Entities;
using VTECHERP.ServiceInterfaces;
using VTECHERP.Services;

namespace VTECHERP
{
    [Route("api/app/Store/[action]")]
    [Authorize]
    public class StoreApplication : ApplicationService
    {
        private readonly IStoreService _storeService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<BankInfo> _bankRepository;
        private readonly IDataFilter _dataFilter;
        private readonly IRepository<PriceTableStore> _priceTableStoreRepository;
        private readonly IRepository<StoreProduct> _storeProductRepository;
        private readonly IRepository<Stores> _storeRepository;
        private readonly IRepository<StoreShippingInformation> _storeShippingInformationRepository;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly IRepository<SaleOrders> _saleOrderRepository;
        private readonly IRepository<PaymentReceipt> _paymentReceiptRepository;
        private readonly IRepository<BillCustomer> _billCustomerRepository;
        private readonly IRepository<CustomerReturn> _customerReturnRepository;
        private readonly IRepository<WarehouseTransferBill> _warehouseTransferBillRepository;
        private readonly IRepository<WarehousingBill> _warehousingBillRepository;
        private readonly ICurrentTenant _currentTenant;
        public StoreApplication(IStoreService storeService
            ,IUnitOfWorkManager unitOfWorkManager
            , IRepository<BankInfo> bankRepository
            , IDataFilter dataFilter
            , IRepository<PriceTableStore> priceTableStoreRepository
            , IRepository<StoreProduct> storeProductRepository
            , IRepository<Stores> storeRepository
            , IRepository<StoreShippingInformation> storeShippingInformationRepository
            , IRepository<UserStore> userStoreRepository
            , IRepository<SaleOrders> saleOrderRepository
            , IRepository<PaymentReceipt> paymentReceiptRepository
            , IRepository<BillCustomer> billCustomerRepository
            , IRepository<CustomerReturn> customerReturnRepository
            , IRepository<WarehouseTransferBill> warehouseTransferBillRepository
            , IRepository<WarehousingBill> warehousingBillRepository
            , ICurrentTenant currentTenant
            )
        {
            _storeService = storeService;
            _unitOfWorkManager = unitOfWorkManager;
            _bankRepository = bankRepository;
            _dataFilter = dataFilter;
            _priceTableStoreRepository = priceTableStoreRepository;
            _storeProductRepository = storeProductRepository;
            _storeRepository = storeRepository;
            _storeShippingInformationRepository = storeShippingInformationRepository;
            _userStoreRepository = userStoreRepository;
            _saleOrderRepository = saleOrderRepository;
            _paymentReceiptRepository = paymentReceiptRepository;
            _billCustomerRepository = billCustomerRepository;
            _customerReturnRepository = customerReturnRepository;
            _warehouseTransferBillRepository = warehouseTransferBillRepository;
            _warehousingBillRepository = warehousingBillRepository;
            _currentTenant = currentTenant;
        }

        [HttpGet]
        public async Task<List<MasterDataDTO>> GetUserManagedStores()
        {
            return await _storeService.GetUserManagedStoresAsync();
        }

        [HttpGet]
        public async Task<List<MasterDataDTO>> GetCurrentTenantStores()
        {
            return await _storeService.GetTenantStoresAsync();
        }

        [HttpPost]
        public async Task<bool> SetFlagStore(Guid storeId)
        {
            return await _storeService.SetFlagStore(storeId);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(CreateStoreParam param)
        {
            using(var uow = _unitOfWorkManager.Begin(requiresNew : true
                , isTransactional: true )
                )
            {
                try
                {
                    var result = await _storeService.Create(param);
                    int statusCode = 400;
                    if (result.success)
                    {
                        await uow.CompleteAsync();
                        statusCode = 200;
                    }

                    return new ObjectResult(new
                    {
                        Message = result.message,
                        Data = result.data,
                        HttpStatusCode = statusCode
                    })
                    {
                        StatusCode = statusCode
                    };
                }
                catch (Exception ex)
                {
                    return new ObjectResult(new
                    {
                        Message = ex.Message,
                        HttpStatusCode = 500
                    })
                    {
                        StatusCode = 500
                    };
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Search(SearchListStoreParam param)
        {
            try
            {
                var result = await _storeService.Search(param);
              
                return new ObjectResult(new
                {
                    Data = result.data,
                    HttpStatusCode = 200,
                    Count = result.count
                })
                {
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ObjectResult(new
                {
                    Message = ex.Message,
                    HttpStatusCode = 500
                })
                {
                    StatusCode = 500
                };
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDeatil(Guid StoreId)
        {
            try
            {
                var result = await _storeService.GetDetail(StoreId);
                int httpStatus = result.success ? 200 : 400;

                return new ObjectResult(new
                {
                    Data = result.data,
                    HttpStatusCode = httpStatus,
                    Message = result.message
                })
                {
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ObjectResult(new
                {
                    Message = ex.Message,
                    HttpStatusCode = 500
                })
                {
                    StatusCode = 500
                };
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetListBank()
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var result = await _bankRepository.GetListAsync();
                return new OkObjectResult(new
                {
                    Data = result.Select(x => new
                    {
                        x.Id,
                        x.TransactionName
                    })
                });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid StoreId)
        {
            var store = await _storeRepository.FirstOrDefaultAsync(x => x.Id == StoreId);
            if (store == null)
            {
                return new BadRequestObjectResult(new
                {
                    Success = false,
                    Message = $"Không tìm thấy cửa hàng có id là {StoreId}"
                });
            }

            //Validation
            var existSaleOrder = await _saleOrderRepository.AnyAsync(x => x.StoreId == StoreId);
            var existWareBill = await _warehousingBillRepository.AnyAsync(x => x.StoreId == StoreId);
            var existWareTransferBill = await _warehouseTransferBillRepository.AnyAsync(x => x.SourceStoreId == StoreId || x.DestinationStoreId == StoreId);
            var existBillCustomer = await _billCustomerRepository.AnyAsync(x => x.StoreId == StoreId);
            var existCustomerReturn = await _customerReturnRepository.AnyAsync(x => x.StoreId == StoreId);
            var existPaymentReceipt = await _paymentReceiptRepository.AnyAsync(x => x.StoreId == StoreId);

            if (existSaleOrder || existWareBill || existWareTransferBill
                || existBillCustomer || existCustomerReturn || existPaymentReceipt)
            {
                return new BadRequestObjectResult(new
                {
                    Success = false,
                    Message = $"Cửa hàng không được phép xóa"
                });
            }

            using (var uow = _unitOfWorkManager.Begin(requiresNew: true
                , isTransactional: true)
                )
            {
                var priceTableStores = await _priceTableStoreRepository.GetListAsync(x => x.StoreId == StoreId);
                await _priceTableStoreRepository.DeleteManyAsync(priceTableStores);

                var storeProduct = await _storeProductRepository.GetListAsync(x => x.StoreId == StoreId);
                await _storeProductRepository.DeleteManyAsync(storeProduct);

                var storeShippingInformation = await _storeShippingInformationRepository.GetListAsync(x => x.StoreId == StoreId);
                await _storeShippingInformationRepository.DeleteManyAsync(storeShippingInformation);

                var userStores = await _userStoreRepository.GetListAsync(x => x.StoreId == StoreId);
                await _userStoreRepository.DeleteManyAsync(userStores);

                await _storeRepository.DeleteAsync(store);
                await uow.CompleteAsync();

                return new OkObjectResult(new
                {
                    Success = true,
                    Message = "Xóa cửa hàng thành công"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetByTenant(List<Guid> TenantIds)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var stores = await _storeRepository.GetListAsync(x => x.IsActive);

                if ((TenantIds != null || TenantIds.Any()) && _currentTenant.Id == null)
                    stores = stores.Where(x => TenantIds.Contains(x.TenantId ?? Guid.Empty)).ToList();
                else if (_currentTenant.Id != null)
                {
                    var userStore = await _userStoreRepository.GetListAsync(x => x.UserId == CurrentUser.Id);
                    var storeIds = userStore.Select(x => x.StoreId).ToList();
                    stores = stores.Where(x => storeIds.Contains(x.Id)).ToList();
                }

                return new OkObjectResult(new
                {
                    data = stores.Select(x => new
                    {
                        x.TenantId,
                        x.Id,
                        x.Name
                    }).ToList(),
                });  
            }
        }

        [HttpPost]
        public async Task<IActionResult> Export(ExportStoreParam param)
        {
            var file = await _storeService.ExportExcel(param);
            return new FileContentResult(file, ContentTypes.SPREADSHEET)
            {
                FileDownloadName = $"Export_cuahang_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
            };
        }
    }
}
