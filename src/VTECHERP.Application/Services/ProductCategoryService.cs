using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Validation;
using VTECHERP.Constants;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.DraftTickets;
using VTECHERP.DTOs.ProductCategories.Requests;
using VTECHERP.DTOs.ProductCategories.Responses;
using VTECHERP.Entities;
using static VTECHERP.Constants.ErrorMessages;

namespace VTECHERP.Services
{
    
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IRepository<ProductCategories> _productCategoryRepository;
        private readonly IRepository<Products> _productRepository;
        private readonly IIdentityUserRepository _userRepository;

        private readonly IObjectMapper _objectMapper;
        public ProductCategoryService(IRepository<ProductCategories> productCategoryRepository
            , IRepository<Products> productRepository
            , IIdentityUserRepository userRepository
            , IObjectMapper objectMapper)
        {
            _productCategoryRepository = productCategoryRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _objectMapper = objectMapper;
        }

        public async Task<List<MasterDataDTO>> GetAllDropdown()
        {
            var query = (await _productCategoryRepository.GetQueryableAsync()).Where(x=>x.Status == Enums.ProductCategory.Status.Active);
            var res = query.Select(x => new MasterDataDTO()
            {
                Code = x.Code,
                Name = x.Name,
                Id = x.Id,
            }).ToList();
            return res;
        }

        public async Task<object> CreateAsync(CreateProductCategoryRequest request)
        {
            await ValidateCreateAsync(request);

            var productCategory = _objectMapper.Map<CreateProductCategoryRequest, ProductCategories>(request);
            await _productCategoryRepository.InsertAsync(productCategory);
            return productCategory;
        }

        public async Task<DetailProductCategoryResponse> GetByIdAsync(Guid id)
        {
            var productCategory = (await _productCategoryRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == id);
            if(productCategory == null)
                throw new AbpValidationException(ErrorMessages.ProductCategory.BaseNotExist);
            var result = _objectMapper.Map<ProductCategories,DetailProductCategoryResponse>(productCategory);
            var parent = (await _productCategoryRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == productCategory.ParentId);
            result.ParentName = parent == null ? null : parent.Name;
            return result;
        }

        public async Task<object> UpdateAsync(UpdateProductCategoryRequest request)
        {
            await ValidateUpdateAsync(request);
            var updateProductCategories = new List<ProductCategories>();
            var productCategories = (await _productCategoryRepository.GetQueryableAsync()).Where(x=> x.Id == request.Id || x.ParentId == request.Id);
            var productCategory = productCategories.FirstOrDefault(x=>x.Id == request.Id);
            productCategory.ParentId = request.ParentId;
            productCategory.CategoryCode = request.CategoryCode;
            productCategory.Name = request.Name;
            productCategory.Ratio = request.Ratio;
            productCategory.ManagerId = request.ManagerId.Value;
            productCategory.Status = request.Status.Value;
            updateProductCategories.Add(productCategory);
            if(request.Status.Value == Enums.ProductCategory.Status.InActive)
            {
                var updateStatus = productCategories.Where(x => x.ParentId == request.Id).ToList();
                updateStatus.ForEach(x =>
                {
                    x.Status = Enums.ProductCategory.Status.InActive;
                });
                updateProductCategories.AddRange(updateStatus);
            }    
            await _productCategoryRepository.UpdateManyAsync(updateProductCategories);

            return updateProductCategories;
        }

        public async Task<object> DeleteAsync(Guid? id)
        {
            await ValidateDeleteAsync(id);
            
            var productCategories = (await _productCategoryRepository.GetListAsync());

            var deleteProductCategories = new List<ProductCategories>();
            deleteProductCategories = productCategories.Where(x => x.ParentId == id.Value).ToList();

            var productCategory = productCategories.FirstOrDefault(x => x.Id == id);
            deleteProductCategories.Add(productCategory);

 
            await _productCategoryRepository.DeleteManyAsync(deleteProductCategories);
            return deleteProductCategories;
        }

        public async Task<PagingResponse<SearchProductCategoryResponse>> SearchAsync(SearchProductCategoryRequest request)
        {
            var result = new List<SearchProductCategoryResponse>();
            var productCategorys = await _productCategoryRepository.GetListAsync();
            var productCategoryQuery = productCategorys
                .WhereIf(request.To.HasValue, x => x.CreationTime.Date >= request.From.Value.Date)
                .WhereIf(request.From.HasValue, x => x.CreationTime.Date <= request.To.Value.Date)
                .WhereIf(!request.Code.IsNullOrWhiteSpace(), x => x.Code == request.Code)
                .WhereIf(!request.Name.IsNullOrWhiteSpace(), x => x.Name == request.Name);

            if (!productCategoryQuery.Any())
                return new PagingResponse<SearchProductCategoryResponse>(0, result);

            var parentProductCategories = productCategoryQuery.Where(x => x.ParentId == null).OrderByDescending(x=>x.Code).ToList();
            var resultOrdered = new List<ProductCategories>();
            if(parentProductCategories.Any())
            {
                foreach (var parent in parentProductCategories)
                {
                    resultOrdered.Add(parent);
                    var childs = productCategoryQuery.Where(x => x.ParentId == parent.Id).OrderByDescending(x => x.Code).ToList();
                    resultOrdered.AddRange(childs);
                }
            }
            else
            {
                resultOrdered = productCategoryQuery.OrderBy(x => x.Code).ToList();
            }
            var resultCount = resultOrdered
                .WhereIf(request.Status.HasValue, x => x.Status == request.Status.Value)
                .WhereIf(request.ManagerIds.Any(), x => request.ManagerIds.Any(id => id == x.ManagerId))
                .ToList();
            var page = resultCount
                .Skip(request.Offset)
                .Take(request.PageSize)
                .ToList();

            result = _objectMapper.Map<List<ProductCategories>, List<SearchProductCategoryResponse>>(page);

            var products = await _productRepository.GetListAsync();

            var managers = (await _userRepository.GetListAsync()).Where(x => x.IsActive == true && result.Any(r=>r.ManagerId == x.Id)).ToList() ?? new List<IdentityUser>();
            result.ForEach(x =>
            {
                var childProductCategorys = productCategorys.Where(pC=>pC.Id == x.Id || pC.ParentId == x.Id).ToList() ?? new List<ProductCategories>();
                var productOfCategory = products.Where(p => childProductCategorys.Any(cPC => cPC.Id == p.CategoryId)).ToList() ?? new List<Products>();
                var manager = managers.FirstOrDefault(m => m.Id == x.ManagerId);
                x.ManagerName = manager == null ? null : manager.Name;
                x.Amount = productOfCategory.Count();
            });

            return new PagingResponse<SearchProductCategoryResponse>(resultCount.Count(), result);
        }

        /// <summary>
        /// Danh sách danh mục cha có trạng thái hoạt động
        /// </summary>
        /// <returns></returns>
        public async Task<List<ParentResponse>> ParentAsync()
        {
            var result = new List<ParentResponse>();
            var parents = (await _productCategoryRepository.GetQueryableAsync()).Where(x=>x.Status == Enums.ProductCategory.Status.Active && x.ParentId  == null).OrderByDescending(x=>x.CreationTime).ToList() ?? new List<ProductCategories>();
            if(parents.Any())
                foreach (var parent in parents)
                    result.Add(new ParentResponse()
                    {
                        Id = parent.Id,
                        Name = parent.Name,
                    });
            return result;
        }

        /// <summary>
        /// Danh sách người phụ trách
        /// </summary>
        /// <returns></returns>
        public async Task<List<ManagerResponse>> ManagerAsync()
        {
            var result = new List<ManagerResponse>();
            var managers = (await _userRepository.GetListAsync()).Where(x=>x.IsActive == true).OrderByDescending(x=>x.CreationTime).ToList() ?? new List<IdentityUser>();
            if (managers.Any())
                foreach (var manager in managers)
                    result.Add(new ManagerResponse()
                    {
                        Id = manager.Id,
                        Data = $"{manager.Name}-{manager.GetProperty("UserCode", "0000000000") ?? "0000000000"}-{manager.PhoneNumber}",
                    });
            return result;
        }

        private async Task ValidateCreateAsync(CreateProductCategoryRequest request)
        {
            var validationErrors = new List<ValidationResult>();

            if (request.ParentId.HasValue)
            {
                var parentProductCategory = (await _productCategoryRepository.GetQueryableAsync()).FirstOrDefault(x=>x.Id == request.ParentId.Value);
                if(parentProductCategory == null)
                    validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.ParentCategoryNotExist));
                else
                {
                    if (parentProductCategory.Status == Enums.ProductCategory.Status.InActive)
                        validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.ParentCategoryInActive));

                    if (parentProductCategory.ParentId != null)
                        validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.ParentCategoryInCorrect));
                }
                
            }

            if (request.CategoryCode.IsNullOrWhiteSpace())
                validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.ParentCategoryInCorrect));
            else
            {
                var productCategory = (await _productCategoryRepository.GetQueryableAsync()).FirstOrDefault(x => x.CategoryCode == request.CategoryCode);
                if (productCategory != null)
                    validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.CategoryCodeIsExist));
            }

            if (request.Name.IsNullOrWhiteSpace())
                validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.NameNotNull));
            else
            {
                var productCategory = (await _productCategoryRepository.GetQueryableAsync()).FirstOrDefault(x => x.Name == request.Name);
                if (productCategory != null)
                    validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.NameIsExist));
            }

            if (request.Ratio.HasValue)
            {
                if (request.Ratio.Value <= 0 || request.Ratio.Value > 100)
                    validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.RatioInCorrect));
            }    

            if (!request.ManagerId.HasValue)
                validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.ManagerNotNull));
            else
            {
                var manager = (await _userRepository.GetListAsync()).FirstOrDefault(x => x.Id == request.ManagerId);
                if (manager == null)
                    validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.ManagerNotExist));
            }

            if(request.Status.HasValue)
            {
                if(request.Status.Value != Enums.ProductCategory.Status.Active && request.Status.Value != Enums.ProductCategory.Status.InActive)
                    validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.StatusInCorrect));
            }    

            if (validationErrors.Any())
                throw new AbpValidationException(validationErrors);
        }

        private async Task ValidateUpdateAsync(UpdateProductCategoryRequest request)
        {
            var validationErrors = new List<ValidationResult>();

            if (!request.Id.HasValue)
                validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.BaseNotNull));
            else
            {
                var productCategory = (await _productCategoryRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == request.Id.Value);
                if(productCategory == null)
                    validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.BaseNotExist));
            }

            if (request.ParentId.HasValue)
            {
                var parentProductCategory = (await _productCategoryRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == request.ParentId.Value);
                if (parentProductCategory == null)
                    validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.ParentCategoryNotExist));
                else
                {
                    if (parentProductCategory.Status == Enums.ProductCategory.Status.InActive)
                        validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.ParentCategoryInActive));

                    if (parentProductCategory.ParentId != null)
                        validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.ParentCategoryInCorrect));
                }

            }

            if (request.CategoryCode.IsNullOrWhiteSpace())
                validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.ParentCategoryInCorrect));
            else
            {
                var productCategory = (await _productCategoryRepository.GetQueryableAsync()).FirstOrDefault(x => x.CategoryCode == request.CategoryCode && x.Id != request.Id.Value);
                if (productCategory != null)
                    validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.CategoryCodeIsExist));
            }

            if (request.Name.IsNullOrWhiteSpace())
                validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.NameNotNull));
            else
            {
                var productCategory = (await _productCategoryRepository.GetQueryableAsync()).FirstOrDefault(x => x.Name == request.Name && x.Id != request.Id.Value);
                if (productCategory != null)
                    validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.NameIsExist));
            }

            if (request.Ratio.HasValue)
            {
                if (request.Ratio.Value <= 0 || request.Ratio.Value > 100)
                    validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.RatioInCorrect));
            }    

            if (!request.ManagerId.HasValue)
                validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.ManagerNotNull));
            else
            {
                var manager = (await _userRepository.GetListAsync()).FirstOrDefault(x => x.Id == request.ManagerId);
                if (manager == null)
                    validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.ManagerNotExist));
            }

            if (request.Status.HasValue)
            {
                if (request.Status.Value != Enums.ProductCategory.Status.Active && request.Status.Value != Enums.ProductCategory.Status.InActive)
                    validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.StatusInCorrect));
            }

            if (validationErrors.Any())
                throw new AbpValidationException(validationErrors);
        }

        private async Task ValidateDeleteAsync(Guid? id)
        {
            var validationErrors = new List<ValidationResult>();

            if (!id.HasValue)
                validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.BaseNotNull));
            else
            {
                var productCategories = (await _productCategoryRepository.GetListAsync());
                var productCategory = productCategories.FirstOrDefault(x => x.Id == id.Value);
                if (productCategory == null)
                    validationErrors.Add(new ValidationResult(ErrorMessages.ProductCategory.BaseNotExist));
                else
                {
                    var products = await _productRepository.GetListAsync();

                    var productOfCategories = products.Where(x => x.CategoryId == id.Value);
                    if (productOfCategories.Any())
                    {
                        var productNames = productOfCategories.Select(x => x.Name).JoinAsString(";");
                        validationErrors.Add(new ValidationResult($"{ErrorMessages.ProductCategory.BaseExistProduct}({productNames})"));
                    }    

                    var categoryChilds = productCategories.Where(x => x.ParentId == id).ToList();
                    if (categoryChilds.Any())
                    {
                        var productOfCategoryChilds = products.Where(x => categoryChilds.Any(cC => cC.Id == x.CategoryId));
                        if(productOfCategoryChilds.Any())
                        {
                            var categoryChildIdExistProducts = productOfCategoryChilds.Select(x => x.CategoryId);
                            var categoryChildExistProducts = productCategories.Where(x => categoryChildIdExistProducts.Any(id => id == x.Id));
                            var categoryChildNameExistProducts = categoryChildExistProducts.Select(x => x.Name).JoinAsString(";");
                            validationErrors.Add(new ValidationResult($"{ErrorMessages.ProductCategory.BaseChildExistProduct}({categoryChildNameExistProducts})"));
                        }
                    }    

                }
            }

            if (validationErrors.Any())
                throw new AbpValidationException(validationErrors);
        }


    }
}