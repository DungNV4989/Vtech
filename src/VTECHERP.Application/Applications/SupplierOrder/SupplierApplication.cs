using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using VTECHERP.Constants;
using VTECHERP.Domain.Shared.Helper.Excel.Model;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.PriceTableProduct.Param;
using VTECHERP.DTOs.Suppliers.Params;
using VTECHERP.DTOs.Suppliers.Respons;
using VTECHERP.Entities;
using VTECHERP.ServiceInterfaces;
using VTECHERP.Services;

namespace VTECHERP
{
    [Route("api/app/Supplier/[action]")]
    [Authorize]
    public class SupplierApplication : ApplicationService
    {
        private readonly ISupplierService _supplierService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Suppliers> _supplierRepository;
        private readonly IRepository<WarehousingBill> _warehousingBillRepository;
        private readonly IRepository<SaleOrders> _saleOrderRepository;

        public SupplierApplication(ISupplierService supplierService
            , IUnitOfWorkManager unitOfWorkManager
            , IRepository<Suppliers> supplierRepository
            , IRepository<WarehousingBill> warehousingBillRepository
            , IRepository<SaleOrders> saleOrderRepository
            )
        {
            _supplierService = supplierService;
            _unitOfWorkManager = unitOfWorkManager;
            _supplierRepository = supplierRepository;
            _warehousingBillRepository = warehousingBillRepository;
            _saleOrderRepository = saleOrderRepository;
        }

        [HttpGet]
        public async Task<List<MasterDataDTO>> GetIdCodeNameAsync()
        {
            return await _supplierService.GetIdCodeNameAsync();
        }

        [HttpGet]
        public async Task<List<MasterDataDTO>> GetChineseSupplierAsync()
        {
            return await _supplierService.GetChineseSupplierAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSupplierParam param)
        {
            if (param == null)
            {
                return new BadRequestObjectResult(new
                {
                    Message = "Dữ liệu không hợp lệ",
                    HttpStatusCode = 400
                });
            }

            using (var uow = _unitOfWorkManager.Begin(requiresNew: true
               , isTransactional: true)
               )
            {
                try
                {
                    var result = await _supplierService.Create(param);
                    int statusCode = 400;
                    if (result.success)
                    {
                        await uow.CompleteAsync();
                        statusCode = 200;
                    }

                    return new OkObjectResult(new
                    {
                        Message = result.message,
                        Data = result.data,
                        HttpStatusCode = statusCode
                    });
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
        public async Task<IActionResult> Search(SearchParam param)
        {
            var result = new List<SupplierItemList>();

            var query = (await _supplierRepository.GetQueryableAsync())
                .Select(x => new SupplierItemList
                {
                    Id= x.Id,
                    Code= x.Code,
                    Name= x.Name,
                    PhoneNumber= x.PhoneNumber,
                    Squence= x.Squence,
                    Type = x.Origin,
                    CreationTime= x.CreationTime,
                });

            if (!string.IsNullOrEmpty(param.Squence))
                query = query.Where(x => x.Squence == null ? false : x.Squence.Contains(param.Squence));

            if (!string.IsNullOrEmpty(param.Name))
                query = query.Where(x => x.Name.Trim().ToLower() == null ? false : x.Name.Contains(param.Name));

            if (!string.IsNullOrEmpty(param.Code))
                query = query.Where(x => x.Code == null ? false : x.Code.Contains(param.Code));

            if (param.Type.HasValue)
                query = query.Where(x => x.Type == param.Type.Value);

            result = query.OrderByDescending(x => x.CreationTime)
                          .Skip((param.PageIndex - 1) * param.PageSize)
                          .Take(param.PageSize)
                          .ToList();

            foreach (var item in result)
            {
                item.TypeText = _supplierService.GetSupplierTypeText(item.Type);
            }

            var count = query.Count();

            return new OkObjectResult(new
            {
                Count = count,
                Data = result,
                Success = true
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetDetail(Guid SupplierId)
        {
            var supplier = await _supplierRepository.FirstOrDefaultAsync(x => x.Id == SupplierId);
            if (supplier == null)
            {
                return new BadRequestObjectResult(new
                {
                    Success = false,
                    Message = $"Không tìm thấy nhà cung cấp có id là {SupplierId}"
                });
            }

            var result = new SupplierDetail
            {
                Id= supplier.Id,
                Code = supplier.Code,
                Name = supplier.Name,
                PhoneNumber= supplier.PhoneNumber,
                Squence= supplier.Squence,
                Type = supplier.Origin,
                TypeText = _supplierService.GetSupplierTypeText(supplier.Origin)
            };

            return new OkObjectResult(new
            {
                SuccessCell= true,
                Data = result
            });
        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid SupplierId, CreateSupplierParam param)
        {
            if (param == null)
            {
                return new BadRequestObjectResult(new
                {
                    Message = "Dữ liệu không hợp lệ",
                    HttpStatusCode = 400
                });
            }

            using (var uow = _unitOfWorkManager.Begin(requiresNew: true
               , isTransactional: true)
               )
            {
                try
                {
                    var result = await _supplierService.Update(SupplierId, param);
                    int statusCode = 400;
                    if (result.success)
                    {
                        await uow.CompleteAsync();
                        statusCode = 200;
                    }

                    return new OkObjectResult(new
                    {
                        Message = result.message,
                        HttpStatusCode = statusCode
                    });
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

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid SupplierId)
        {
            var supplier = await _supplierRepository.FirstOrDefaultAsync(x => x.Id == SupplierId);
            if (supplier == null)
            {
                return new BadRequestObjectResult(new
                {
                    Success = false,
                    Message = $"Không tìm thấy nhà cung cấp có id là {SupplierId}"
                });
            }

            var isExistSaleOrder = await _saleOrderRepository.AnyAsync(x => x.SupplierId == SupplierId);
            var isExistWarehouse = await _warehousingBillRepository.AnyAsync(x => x.AudienceId == SupplierId 
            && (x.AudienceType == Enums.AudienceTypes.SupplierCN || x.AudienceType == Enums.AudienceTypes.SupplierVN));

            if (isExistSaleOrder || isExistWarehouse)
            {
                return new BadRequestObjectResult(new
                {
                    Success = false,
                    Message = $"Nhà cung cấp không được phép xóa"
                });
            }

            await _supplierRepository.DeleteAsync(supplier);

            return new OkObjectResult(new
            {
                Success = true,
                Message = "Xóa nhà cung cấp thành công"
            });
        }

        [HttpGet]
        public async Task<IActionResult> DownloadTemplateImport()
        {
            var file = await _supplierService.DownloadTemplateImport();
            return new OkObjectResult(new
            {
                Success = true,
                Data = file
            });
        }

        [HttpPost]
        public async Task<IActionResult> ImportPriceTableProductData([FromForm] IFormFile param)
        {
            if (param == null || param.Length <= 0)
                return new BadRequestObjectResult($"Dữ liệu không hợp lệ");

            // Kiểm tra phần mở rộng tệp
            var fileExtension = Path.GetExtension(param.FileName);
            if (fileExtension.ToLower() != ".xlsx" && fileExtension.ToLower() != ".xls")
                return new BadRequestObjectResult("Sai định dạng file");

            var file = await _supplierService.ImportPriceTableProductData(param);
            return new OkObjectResult(new
            {
                Success = true,
                Data = file
            });
        }
    }
}