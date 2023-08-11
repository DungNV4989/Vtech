using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VTECHERP.Constants;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.PriceTableProduct.Param;
using VTECHERP.DTOs.PriceTableProduct.Respon;
using VTECHERP.ServiceInterfaces;

namespace VTECHERP.Applications.PriceTableProduct
{
    [Route("api/app/price-table/[action]")]
    [Authorize]
    public class PriceTableApplication : ApplicationService
    {
        private readonly IPriceTableService _priceTableService;
        public PriceTableApplication(IPriceTableService priceTableService)
        {
            _priceTableService = priceTableService;
        }

        [HttpGet]
        public async Task<PriceTableDetail> Get(Guid id)
        {
            return await _priceTableService.GetPriceTable(id);
        }

        [HttpPost]
        public async Task<List<MasterDataDTO>> ListAllPriceTable(ListAllPriceTableRequest request)
        {
            return await _priceTableService.ListAllPriceTable(request);
        }

        [HttpPost]
        public async Task<bool> Create(CreatePriceTableRequest request)
        {
            await _priceTableService.CreatePriceTable(request);
            return true;
        }

        //Update
        [HttpPost]
        public async Task<bool> Update(UpdatePriceTableRequest request)
        {
            await _priceTableService.UpdatePriceTable(request);
            return true;
        }

        [HttpPost]
        public async Task<PagingResponse<PriceTableDetail>> Search(SearchPriceTableRequest request)
        {
            return await _priceTableService.SearchPriceTable(request);
        }

        [HttpPost]
        public async Task<PagingResponse<ProductPriceDetail>> SearchProductPrice(SearchPriceProductRequest request)
        {
            return await _priceTableService.SearchProductPrice(request);
        }

        [HttpPost]
        public async Task<PagingResponse<ProductPriceDetail>> SearchProductPriceByTableId(SearchPriceProductByTableIdRequest request)
        {
            return await _priceTableService.SearchProductPriceByTableId(request);
        }

        [HttpPost]
        public async Task<List<MasterDataDTO>> SearchProductToAddToPriceTable(SearchPriceProductNotInPriceTableRequest request)
        {
            return await _priceTableService.SearchProductNotInPriceTable(request);
        }

        [HttpPost]
        public async Task<bool> AddProductPrice(AddPriceProductRequest request)
        {
            await _priceTableService.AddProductPrice(request);
            return true;
        }

        [HttpPost]
        public async Task<bool> UpdateProductPrice(UpdatePriceProductRequest request)
        {
            await _priceTableService.UpdateProductPrice(request);
            return true;
        }

        [HttpPost]
        public async Task<bool> DeleteProductPrice(DeletePriceProductRequest request)
        {
            await _priceTableService.DeleteProductPrice(request);
            return true;
        }

        [HttpPost]
        public async Task<bool> DeleteMultipleProductPrice(DeleteMultiplePriceProductRequest request)
        {
            await _priceTableService.DeleteMultipleProductPrice(request);
            return true;
        }

        [HttpPost]
        public async Task<FileResult> ExportProductPriceAsync(SearchPriceProductRequest request)
        {
            try
            {
                var file = await _priceTableService.ExportProductPrice(request);
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"DanhSach_Sản phẩm bảng giá_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
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
