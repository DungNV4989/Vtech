using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.Product;
using VTECHERP.Models;

namespace VTECHERP.Services
{
    public interface IProductService : IScopedDependency
    {
        Task<List<MasterDataDTO>> GetIdCodeNameAsync();

        Task<ProductDetailDto> GetByIdAsync(Guid id);

        Task<List<ProductDetailDto>> GetByIdsAsync(List<Guid> ids);

        Task<bool> Exist(Guid id);

        Task<PagingResponse<SearchProductResponse>> Search(SearchProductRequest request);
        Task<PagingResponse<SearchProductResponse>> SearchClone(SearchProductRequest request);

        Task<List<ProductMasterDataDto>> MapStockQuantityAsync(Guid storeId, List<Guid> productIds = null);
        Task<List<ProductMasterDataDto>> GetProductByStordId(Guid storeId);
        Task<IActionResult> Create([FromForm] CreateProductDto request);
        /// <summary>
        /// Danh sách bút toán - Xuất Excel
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<byte[]> ExportProductAsync(SearchProductRequest request);
        Task<string> ValidateCreateDto(CreateProductDto input);

        Task<List<ProductDetailInventory>> GetProductDetailInventories(Guid ProductId);
        Task<(int count, List<ProductDetailXnk> data)> GetProductDetailXnk(SearchProductDetailXnk request);
    }
}