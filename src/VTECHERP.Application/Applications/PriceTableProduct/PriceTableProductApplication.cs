using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using VTECHERP.Constants;
using VTECHERP.DTOs.OrderTransports.Params;
using VTECHERP.DTOs.PriceTableProduct.Param;
using VTECHERP.DTOs.PriceTableProduct.Respon;
using VTECHERP.Entities;
using VTECHERP.Helper;
using VTECHERP.ServiceInterfaces;
using PriceTableProduct = VTECHERP.Entities.PriceTableProduct;

namespace VTECHERP.Applications.PriceTableProduct
{
    [Authorize]
    public class PriceTableProductApplication : ApplicationService
    {
        private readonly IPriceTableProductService _priceTableProductService;
        private readonly IRepository<PriceTable> _priceTableRepository;
        private readonly IRepository<Entities.PriceTableProduct> _priceTableProductRepository;
        private readonly IRepository<PriceTableStore> _priceTableStoreRepository;
        private readonly IRepository<Stores> _storeRepository;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly ICurrentUser _currentUser;
        public PriceTableProductApplication(
            IPriceTableProductService priceTableProductService
            , IRepository<PriceTable> priceTableRepository
            , IRepository<Entities.PriceTableProduct> priceTableProductRepository
            , IRepository<PriceTableStore> priceTableStoreRepository
            , IRepository<Stores> storeRepository
            , IRepository<UserStore> userStoreRepository
            , ICurrentUser currentUser
            )
        {
            _priceTableProductService = priceTableProductService;
            _priceTableRepository = priceTableRepository;
            _priceTableProductRepository = priceTableProductRepository;
            _priceTableStoreRepository = priceTableStoreRepository;
            _currentUser = currentUser;
            _storeRepository = storeRepository;
            _userStoreRepository = userStoreRepository;
        }

        [HttpGet]
        public async Task<IActionResult> DownloadTemplateImport()
        {
            var file = await _priceTableProductService.DownloadTemplateImport();
            return new FileContentResult(file, ContentTypes.SPREADSHEET)
            {
                FileDownloadName = $"Mau_import_data_bang_gia_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
            };
        }

        [HttpPost]
        public async Task<IActionResult> ImportPriceTableProductData([FromForm] PriceTableProductImportParam param)
        {
            if (param.PriceTableId == Guid.Empty || param.File == null || param.File.Length <= 0)
                return new BadRequestObjectResult($"Dữ liệu không hợp lệ");

            // Kiểm tra phần mở rộng tệp
            var fileExtension = Path.GetExtension(param.File.FileName);
            if (fileExtension.ToLower() != ".xlsx" && fileExtension.ToLower() != ".xls")
                return new BadRequestObjectResult("Sai định dạng file");

            var priceTable = await _priceTableRepository.AnyAsync(x => x.Id == param.PriceTableId);
            if (!priceTable)
                return new BadRequestObjectResult($"Bảng giá không tồn tại");

            var file = await _priceTableProductService.ImportPriceTableProductData(param);
            return new FileContentResult(file, ContentTypes.SPREADSHEET)
            {
                FileDownloadName = $"Ket_qua_import_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
            };
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePrice(PriceProduductUpdateParam param)
        {
            if (param == null)
                return new GenericActionResult(400, false, "Dữ liệu không hợp lệ");

            try
            {
                var priceTableProduct = await _priceTableProductRepository.FindAsync(x => x.Id == param.Id);
                if (priceTableProduct == null)
                    return new GenericActionResult(400, false, $"Không tìm thấy sản phẩm của bảng giá có id là {param.Id}");

                priceTableProduct.Price = param.Price;
                await CurrentUnitOfWork.SaveChangesAsync();

                return new GenericActionResult(200, true, "Cập nhập thành công");
            }
            catch (Exception ex)
            {
                return new GenericActionResult(500, false, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDetailByTablePrice(Guid TablePriceId)
        {
            var priceTable = await _priceTableRepository.FindAsync(x => x.Id == TablePriceId);
            if (priceTable == null)
            {
                return new BadRequestObjectResult(new
                {
                    message = $"Không tìm thấy thông tin bảng giá có id là {TablePriceId}"
                });
            }

            var storeQueryable = await _storeRepository.GetQueryableAsync();
            var priceTableStoreQueryable = await _priceTableStoreRepository.GetQueryableAsync();

            var result = new PriceTableDetail()
            {
                Id = priceTable.Id,
                PriceTableName = priceTable.Name,
                ParentId = priceTable.ParentPriceTableId,
                Status = priceTable.Status,
                AppliedFrom = priceTable.AppliedFrom,
                AppliedTo = priceTable.AppliedTo
            };

            var priceTableParent = await _priceTableRepository.FindAsync(x => x.Id == result.ParentId);
            result.PriceTableParentName = priceTableParent == null ? "" : priceTableParent.Name;

            var userStores = await _userStoreRepository.GetListAsync(x => x.UserId == _currentUser.Id);
            var storeIdUser = userStores.Select(x => x.StoreId).ToList();

            var stores = priceTableStoreQueryable.Where(x => x.PriceTableId == priceTable.Id && storeIdUser.Contains(x.StoreId)).ToList();

            return new OkObjectResult(new
            {
                data = result,
                message = ""
            });
        }

        //[HttpGet]
        //public async Task<IActionResult> GetListByTablePriceId(Guid TablePriceId)
        //{

        //}
    }
}
