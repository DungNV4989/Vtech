using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTECHERP.Constants;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.Product;
using VTECHERP.Helper;
using VTECHERP.Permissions;
using VTECHERP.Services;

namespace VTECHERP
{
    [Route("api/app/Product/[action]")]
    [Authorize]
    public class ProductApplication : VTECHERPAppService
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductApplication> _logger;

        public ProductApplication(IProductService productService, ILogger<ProductApplication> logge)
        {
            _productService = productService;
            _logger = logge;
        }

        [HttpGet]
        public async Task<List<MasterDataDTO>> GetIdCodeNameAsync()
        {
            return await _productService.GetIdCodeNameAsync();
        }

        [HttpGet]
        public async Task<List<ProductMasterDataDto>> MapStockQuantityAsync(Guid storeId)
        {
            return await _productService.GetProductByStordId(storeId);
        }

        [HttpPost]
        //[Authorize]
        [Authorize(VTECHERPPermissions.Product.Management.List)]
        public async Task<PagingResponse<SearchProductResponse>> Search(SearchProductRequest request)
        {
            return await _productService.SearchClone(request);
        }
        [HttpPost]
        public async Task<PagingResponse<SearchProductResponse>> SearchClone(SearchProductRequest request)
        {
            return await _productService.SearchClone(request);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateProductDto input)
        {
            var validate = await _productService.ValidateCreateDto(input);
            if(input!=null)
            {
                if (!string.IsNullOrEmpty(validate))
                {
                    return new GenericActionResult(400, false, string.Format(ErrorMessages.Common.Required, validate), null);
                }
            }    
            var result = await _productService.Create(input);
            return result;
        }

        [HttpPost]
        public async Task<FileResult> ExportProductAsync(SearchProductRequest request)
        {
            try
            {
                _logger.LogWarning("[VTECH] Start Export");
                var file = await _productService.ExportProductAsync(request);
                _logger.LogWarning($"[VTECH] Get Export File: File Length - {file.Length}");
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"DanhSach_Sản phẩm_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
                };
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                _logger.LogError($"[VTECH]{e.Message}");
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var detail = await _productService.GetByIdAsync(id);
            return new GenericActionResult(200, true, "", detail);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductDetailInventories(Guid ProductId)
        {
            var result = await _productService.GetProductDetailInventories(ProductId);
            return new OkObjectResult(new
            {
                success = true,
                data = result
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetProductDetailXnk(SearchProductDetailXnk request)
        {
            var result = await _productService.GetProductDetailXnk(request);
            return new OkObjectResult(new
            {
                success = true,
                count = result.count,
                data = result.data
            });
        }
    }
}