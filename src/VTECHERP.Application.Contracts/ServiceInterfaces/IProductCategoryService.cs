using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.ProductCategories.Requests;
using VTECHERP.DTOs.ProductCategories.Responses;

namespace VTECHERP.Services
{
    public interface IProductCategoryService : IScopedDependency
    {
        Task<List<MasterDataDTO>> GetAllDropdown();

        Task<object> CreateAsync(CreateProductCategoryRequest request);

        Task<DetailProductCategoryResponse> GetByIdAsync(Guid id);

        Task<object> UpdateAsync(UpdateProductCategoryRequest request);

        Task<object> DeleteAsync(Guid? id);

        Task<List<ParentResponse>> ParentAsync();

        Task<List<ManagerResponse>> ManagerAsync();

        Task<PagingResponse<SearchProductCategoryResponse>> SearchAsync(SearchProductCategoryRequest request);
    }
}