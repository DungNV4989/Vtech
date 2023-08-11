using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.ProductCategories.Requests;
using VTECHERP.Services;

namespace VTECHERP.Applications.ProductCategories
{
    [Route("api/app/ProductCategory/[action]")]
    [Authorize]
    public class ProductCategoryApplication : ApplicationService
    {
        private readonly IProductCategoryService _productCategoryService;

        public ProductCategoryApplication(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        [HttpGet]
        public async Task<List<MasterDataDTO>> GetAllDropdown()
        {
            return await _productCategoryService.GetAllDropdown();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateProductCategoryRequest request)
        {
            var productCategory = await _productCategoryService.CreateAsync(request);
            return new OkObjectResult(new
            {
                StatusCode = 200,
                Data = productCategory,
                Message = "Tạo danh mục sản phẩm thành công."
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var result = await _productCategoryService.GetByIdAsync(id);
            return new OkObjectResult(new
            {
                StatusCode = 200,
                Data = result,
                Message = "Thông tin danh mục sản phẩm."
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdateProductCategoryRequest request)
        {
            var productCategory = await _productCategoryService.UpdateAsync(request);
            return new OkObjectResult(new
            {
                StatusCode = 200,
                Data = productCategory,
                Message = "Cập nhật danh mục sản phẩm thành công."
            });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(Guid? id)
        {
            var productCategory = await _productCategoryService.DeleteAsync(id);
            return new OkObjectResult(new
            {
                StatusCode = 200,
                Data = productCategory,
                Message = "Xóa danh mục sản phẩm thành công."
            });
        }

        [HttpPost]
        public async Task<IActionResult> SearchAsync(SearchProductCategoryRequest request)
        {
            var result = (await _productCategoryService.SearchAsync(request));
            return new OkObjectResult(new
            {
                StatusCode = 200,
                Data = result,
                Message = "Danh sách danh mục sản phẩm"
            });
        }

        [HttpGet]
        public async Task<IActionResult> ParentAsync()
        {
            var parents = await _productCategoryService.ParentAsync();
            return new OkObjectResult(new
            {
                StatusCode = 200,
                Data = parents,
                Message = "Danh sách danh mục cha"
            });
        }

        [HttpGet]
        public async Task<IActionResult> ManagerAsync()
        {
            var managers = await _productCategoryService.ManagerAsync();
            return new OkObjectResult(new
            {
                StatusCode = 200,
                Data = managers,
                Message = "Danh sách người phụ trách"
            });
        }
    }
}