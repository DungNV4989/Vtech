using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Timing;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.PriceTable;
using VTECHERP.DTOs.PriceTableProduct.Param;
using VTECHERP.DTOs.PriceTableProduct.Respon;
using VTECHERP.Entities;
using VTECHERP.ServiceInterfaces;

namespace VTECHERP.Services
{
    public class PriceTableService : IPriceTableService
    {
        private readonly IObjectMapper _mapper;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<PriceTable> _priceTableRepository;
        private readonly IRepository<PriceTableCustomer> _priceTableCustomerRepository;
        private readonly IRepository<PriceTableStore> _priceTableStoreRepository;
        private readonly IRepository<Products> _productRepository;
        private readonly IRepository<PriceTableProduct> _priceTableProductRepository;
        private readonly IRepository<Stores> _storeRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<IdentityUser> _userRepository;
        private readonly IRepository<ProductCategories> _productCategoryRepository;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly IRepository<StoreProduct> _storeProductRepository;
        private readonly IClock _clock;
        private readonly ICurrentUser _currentUser;

        public PriceTableService(
            IObjectMapper mapper,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<PriceTable> priceTableRepository,
            IRepository<PriceTableCustomer> priceTableCustomerRepository,
            IRepository<PriceTableStore> priceTableStoreRepository,
            IRepository<Products> productRepository,
            IRepository<PriceTableProduct> priceTableProductRepository,
            IRepository<Stores> storeRepository,
            IRepository<Customer> customerRepository,
            IRepository<IdentityUser> userRepository,
            IRepository<ProductCategories> productCategoryRepository,
            IRepository<UserStore> userStoreRepository,
            IRepository<StoreProduct> storeProductRepository,
            IClock clock,
            ICurrentUser currentUser)
        {
            _mapper = mapper;
            _unitOfWorkManager = unitOfWorkManager;
            _priceTableRepository = priceTableRepository;
            _priceTableCustomerRepository = priceTableCustomerRepository;
            _priceTableStoreRepository = priceTableStoreRepository;
            _productRepository = productRepository;
            _priceTableProductRepository = priceTableProductRepository;
            _storeRepository = storeRepository;
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _productCategoryRepository = productCategoryRepository;
            _userStoreRepository = userStoreRepository;
            _storeProductRepository = storeProductRepository;
            _clock = clock;
            _currentUser = currentUser;
        }

        public async Task<List<MasterDataDTO>> ListAllPriceTable(ListAllPriceTableRequest request)
        {
            var priceTables = (await _priceTableRepository.GetQueryableAsync())
                .WhereIf(request.IgnoredPriceTables.Any(), p => !request.IgnoredPriceTables.Contains(p.Id))
                .WhereIf(!request.IdOrName.IsNullOrEmpty(), 
                    p => p.Code.Contains(request.IdOrName)
                    || EF.Functions.Collate(p.Name, "SQL_Latin1_General_CP1_CI_AI").Contains(EF.Functions.Collate(request.IdOrName, "SQL_Latin1_General_CP1_CI_AI")));
            if (request.StoreIds.Any())
            {
                var priceTableStores = await _priceTableStoreRepository.GetListAsync(p => request.StoreIds.Contains(p.StoreId));
                var storePriceTableIds = priceTableStores.Select(p => p.PriceTableId).Distinct().ToList();
                priceTables = priceTables.Where(p => storePriceTableIds.Contains(p.Id));
            }

            if (request.CustomerIds.Any())
            {
                var priceTableCustomers = await _priceTableCustomerRepository.GetListAsync(p => request.CustomerIds.Contains(p.CustomerId));
                var customerPriceTableIds = priceTableCustomers.Select(p => p.PriceTableId).Distinct().ToList();
                priceTables = priceTables.Where(p => customerPriceTableIds.Contains(p.Id));
            }

            return priceTables
                .Select(p => new MasterDataDTO
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name
                }).ToList();
        }

        public async Task CreatePriceTable(CreatePriceTableRequest request)
        {
            await ValidateCreatePriceTable(request);
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var priceTable = _mapper.Map<CreatePriceTableRequest, PriceTable>(request);
                var currentDate = _clock.Now.Date;
                if(request.AppliedTo.HasValue && currentDate > request.AppliedTo)
                {
                    priceTable.Status = Enums.PriceTableStatus.InActive;
                }
                await _priceTableRepository.InsertAsync(priceTable);
                await uow.SaveChangesAsync();

                if (request.CustomerIds.Any())
                {
                    var priceCustomers = request.CustomerIds
                        .Select(customerId => new PriceTableCustomer
                        {
                            CustomerId = customerId,
                            PriceTableId = priceTable.Id,
                        }).ToList();

                    await _priceTableCustomerRepository.InsertManyAsync(priceCustomers);
                    await uow.SaveChangesAsync();
                }

                if (request.StoreIds.Any())
                {
                    var priceStores = request.StoreIds
                        .Select(storeId => new PriceTableStore
                        {
                            StoreId = storeId,
                            PriceTableId = priceTable.Id,
                        }).ToList();

                    await _priceTableStoreRepository.InsertManyAsync(priceStores);
                    await uow.SaveChangesAsync();
                }
                // clone current product list to new price table
                //var products = await _productRepository.GetListAsync();

                //if (products.Any())
                //{
                //    var priceTableProducts = products.Select(p => new PriceTableProduct
                //    {
                //        PriceTableId = priceTable.Id,
                //        ProductId = p.Id,
                //        Price = 0,
                //    });
                //    await _priceTableProductRepository.InsertManyAsync(priceTableProducts);
                //    await uow.SaveChangesAsync();
                //}

                await uow.CompleteAsync();
            }
            catch
            {
                await uow.RollbackAsync();
                throw;
            }

        }

        public async Task UpdatePriceTable(UpdatePriceTableRequest request)
        {
            await ValidateUpdatePriceTable(request);
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var currentDate = _clock.Now.Date;
                var priceTable = await _priceTableRepository.GetAsync(p => p.Id == request.Id);
                var priceTableCustomers = await _priceTableCustomerRepository.GetListAsync(p => p.PriceTableId == priceTable.Id);
                var priceTableStores = await _priceTableStoreRepository.GetListAsync(p => p.PriceTableId == priceTable.Id);
                priceTable.Status = request.Status;
                if (request.AppliedTo.HasValue && currentDate > request.AppliedTo)
                {
                    priceTable.Status = Enums.PriceTableStatus.InActive;
                }
                priceTable.Name = request.Name;
                priceTable.AppliedFrom = request.AppliedFrom;
                priceTable.AppliedTo = request.AppliedTo;
                priceTable.Note = request.Note;
                priceTable.ParentPriceTableId = request.ParentPriceTableId;
                priceTable.STT = request.STT;

                var ptCustomersToInsert = request
                    .CustomerIds
                    .Where(customerId => !priceTableCustomers.Any(customer => customer.CustomerId == customerId))
                    .Select(customerId => new PriceTableCustomer
                    {
                        PriceTableId = priceTable.Id,
                        CustomerId = customerId,
                    }).ToList();
                var ptCustomersToUpdate = priceTableCustomers
                    .Where(customer => request.CustomerIds.Any(customerId => customer.CustomerId == customerId))
                    .ToList();
                var ptCustomersToDelete = priceTableCustomers
                    .Where(customer => !request.CustomerIds.Any(customerId => customer.CustomerId == customerId))
                    .ToList();

                var ptStoresToInsert = request
                    .StoreIds
                    .Where(storeId => !priceTableStores.Any(store => store.StoreId == storeId))
                    .Select(storeId => new PriceTableStore
                    {
                        PriceTableId = priceTable.Id,
                        StoreId = storeId
                    });
                var ptStoresToUpdate = priceTableStores
                    .Where(store => request.StoreIds.Any(storeId => store.StoreId == storeId))
                    .ToList();
                var ptStoresToDelete = priceTableStores
                    .Where(store => !request.StoreIds.Any(storeIds => store.StoreId == storeIds))
                    .ToList();

                if (ptCustomersToInsert.Any())
                {
                    await _priceTableCustomerRepository.InsertManyAsync(ptCustomersToInsert);
                }

                if (ptCustomersToUpdate.Any())
                {
                    await _priceTableCustomerRepository.UpdateManyAsync(ptCustomersToUpdate);
                }

                if (ptCustomersToDelete.Any())
                {
                    await _priceTableCustomerRepository.DeleteManyAsync(ptCustomersToDelete);
                }

                if (ptStoresToInsert.Any())
                {
                    await _priceTableStoreRepository.InsertManyAsync(ptStoresToInsert);
                }

                if (ptStoresToUpdate.Any())
                {
                    await _priceTableStoreRepository.UpdateManyAsync(ptStoresToUpdate);
                }

                if (ptStoresToDelete.Any())
                {
                    await _priceTableStoreRepository.DeleteManyAsync(ptStoresToDelete);
                }
                await uow.SaveChangesAsync();
                await uow.CompleteAsync();
            }
            catch
            {
                await uow.RollbackAsync();
                throw;
            }
        }

        public async Task<PriceTableDetail> GetPriceTable(Guid id)
        {
            var priceTable = await _priceTableRepository.GetAsync(p => p.Id == id);
            var detail = new PriceTableDetail
            {
                Id = priceTable.Id,
                STT = priceTable.STT,
                Code = priceTable.Code,
                PriceTableName = priceTable.Name,
                AppliedFrom = priceTable.AppliedFrom,
                AppliedTo = priceTable.AppliedTo,
                ParentId = priceTable.ParentPriceTableId,
                Status = priceTable.Status,
                StatusText = priceTable.Status == Enums.PriceTableStatus.Active ? "Hoạt động" : "Không hoạt động",
                CreationTime = priceTable.CreationTime,
                CreatorId = priceTable.CreatorId,
                LastModificationTime = priceTable.LastModificationTime,
                LastModifierId = priceTable.LastModifierId,
                Note = priceTable.Note
            };

            var priceTableCustomer = await _priceTableCustomerRepository.GetListAsync(p => p.PriceTableId == priceTable.Id);
            if (priceTableCustomer.Any())
            {
                var customerIds = priceTableCustomer.Select(p => p.CustomerId).Distinct().ToList();
                var customers = await _customerRepository.GetListAsync(p => customerIds.Contains(p.Id));
                var customerDetails = priceTableCustomer.Select(p =>
                                    new PriceTableCustomerDetail
                                    {
                                        CustomerId = p.CustomerId,
                                        PriceTableCustomerId = p.Id,
                                        PriceTableId = p.PriceTableId,
                                        CustomerName = customers.FirstOrDefault(c => c.Id == p.CustomerId)?.Name
                                    }).ToList();
                detail.Customers = customerDetails;
            }

            var priceTableStores = await _priceTableStoreRepository.GetListAsync(p => p.PriceTableId == priceTable.Id);
            if (priceTableStores.Any())
            {
                var storeIds = priceTableStores.Select(p => p.StoreId).Distinct().ToList();
                var stores = await _storeRepository.GetListAsync(p => storeIds.Contains(p.Id));
                var storeDetails = priceTableStores.Select(p =>
                                    new PriceTableStoreDetail
                                    {
                                        StoreId = p.StoreId,
                                        PriceTableStoreId = p.Id,
                                        PriceTableId = p.PriceTableId,
                                        StoreName = stores.FirstOrDefault(c => c.Id == p.StoreId)?.Name
                                    }).ToList();
                detail.Stores = storeDetails;
            }

            if(priceTable.ParentPriceTableId != null)
            {
                var parentPriceTable = await _priceTableRepository.GetAsync(p => p.Id == priceTable.ParentPriceTableId);
                detail.PriceTableParentName = parentPriceTable?.Name;
            }

            return detail;
        }

        public async Task<PagingResponse<PriceTableDetail>> SearchPriceTable(SearchPriceTableRequest request)
        {
            var priceTableQueryable = _priceTableRepository.GetQueryableAsync().Result;
            var priceTableCustomerQueryable = _priceTableCustomerRepository.GetQueryableAsync().Result;
            var priceTableStoreQueryable = _priceTableStoreRepository.GetQueryableAsync().Result;
            var storeQueryable = _storeRepository.GetQueryableAsync().Result;
            var customerQueryable = _customerRepository.GetQueryableAsync().Result;
            var allUsers = await _userRepository.GetListAsync();
            var userStores = await _userStoreRepository.GetListAsync();
            var storeIds = userStores.Where(x => x.UserId == _currentUser.Id).Select(x => x.StoreId).ToList();
            var query = from priceTable in priceTableQueryable
                        join priceTableCustomer in priceTableCustomerQueryable on priceTable.Id equals priceTableCustomer.PriceTableId into priceTableCustomerLeftJoin
                        from priceTableCustomer in priceTableCustomerLeftJoin.DefaultIfEmpty()
                        join customer in customerQueryable on priceTableCustomer.CustomerId equals customer.Id into customerLeftJoin
                        from customer in customerLeftJoin.DefaultIfEmpty()
                        join priceTableStore in priceTableStoreQueryable on priceTable.Id equals priceTableStore.PriceTableId into priceTableStoreLeftJoin
                        from priceTableStore in priceTableStoreLeftJoin.DefaultIfEmpty()
                        join store in storeQueryable on priceTableStore.StoreId equals store.Id into storeLeftJoin
                        from store in storeLeftJoin.DefaultIfEmpty()
                        select new
                        {
                            priceTable.Id,
                            priceTable.STT,
                            priceTable.Name,
                            priceTable.Code,
                            priceTable.AppliedFrom,
                            priceTable.AppliedTo,
                            priceTable.CreatorId,
                            priceTable.CreationTime,
                            priceTable.LastModifierId,
                            priceTable.LastModificationTime,
                            priceTable.Status,
                            priceTable.ParentPriceTableId,
                            CustomerId = priceTableCustomer != null ? priceTableCustomer.CustomerId : (Guid?)null,
                            CustomerName = customer != null ? customer.Name : "",
                            StoreId = priceTableStore != null ? priceTableStore.StoreId : (Guid?)null,
                            StoreName = store != null ? store.Name : "",
                            Note = priceTable.Note
                        };
            var datas = query.WhereIf(storeIds != null && storeIds.Count > 0, p => storeIds.Contains(p.StoreId ?? Guid.Empty))
                .WhereIf(request.StoreIds.Any(), p => p.StoreId != null && request.StoreIds.Contains(p.StoreId ?? Guid.Empty))
                .WhereIf(request.CustomerIds.Any(), p => p.CustomerId != null && request.CustomerIds.Contains(p.CustomerId ?? Guid.Empty))
                .WhereIf(!request.Id.IsNullOrWhiteSpace(), p => p.Code.Contains(request.Id))
                .WhereIf(!request.Name.IsNullOrWhiteSpace(), p => EF.Functions.Collate(p.Name, "SQL_Latin1_General_CP1_CI_AI").Contains(EF.Functions.Collate(request.Name, "SQL_Latin1_General_CP1_CI_AI")))
                .WhereIf(request.AppliedFrom != null, p => p.AppliedFrom >= request.AppliedFrom)
                .WhereIf(request.AppliedTo != null, p => p.AppliedTo == null || (p.AppliedTo != null && p.AppliedTo <= request.AppliedFrom))
                .WhereIf(request.Status != null, p => p.Status == request.Status.Value)
                .WhereIf(request.ParentPriceTableId != null, p => p.ParentPriceTableId == request.ParentPriceTableId.Value)
                .ToList();

            var groupData = datas.GroupBy(p => p.Id)
                .Select(grp =>
                {
                    var id = grp.Key;
                    var data = grp.FirstOrDefault();

                    var customers = grp.Where(p => p.CustomerId != null).GroupBy(p => p.CustomerId ?? Guid.Empty).Select(cGrp => new PriceTableCustomerDetail
                    {
                        PriceTableId = id,
                        CustomerId = cGrp.Key,
                        CustomerName = cGrp.FirstOrDefault()?.CustomerName
                    }).ToList();

                    var stores = grp.Where(p => p.StoreId != null).GroupBy(p => p.StoreId ?? Guid.Empty).Select(cGrp => new PriceTableStoreDetail
                    {
                        PriceTableId = id,
                        StoreId = cGrp.Key,
                        StoreName = cGrp.FirstOrDefault()?.StoreName
                    }).ToList();

                    return new PriceTableDetail
                    {
                        Id = id,
                        ParentId = data.ParentPriceTableId,
                        PriceTableName = data.Name,
                        AppliedFrom = data.AppliedFrom,
                        AppliedTo = data.AppliedTo,
                        Code = data.Code,
                        STT = data.STT,
                        Customers = customers,
                        Stores = stores,
                        Status = data.Status,
                        StatusText = data.Status == Enums.PriceTableStatus.Active ? "Hoạt động" : "Không hoạt động",
                        PriceTableParentName = "",
                        CreatorId = data.CreatorId,
                        CreationTime = data.CreationTime,
                        LastModifierId = data.LastModifierId,
                        LastModificationTime = data.LastModificationTime,
                        Note = data.Note
                    };
                });

            var paged = groupData.OrderByDescending(p => p.Code)
                .Skip(request.Offset)
                .Take(request.PageSize)
                .ToList();
            var parentIds = paged.Where(p => p.ParentId != null).Select(p => p.ParentId).ToList();
            var parentPriceTables = await _priceTableRepository.GetListAsync(p => parentIds.Contains(p.Id));
            paged.ForEach(p =>
            {
                if (p.ParentId != null)
                {
                    var parentPriceTable = parentPriceTables.FirstOrDefault(ppt => ppt.Id == p.ParentId);
                    p.PriceTableParentName = parentPriceTable?.Name;
                }
                var creator = allUsers.FirstOrDefault(user => user.Id == p.CreatorId);
                p.CreatorName = creator?.Name;
                var modifier = allUsers.FirstOrDefault(user => user.Id == p.LastModifierId);
                p.LastModifierName = modifier?.Name;
            });

            return new PagingResponse<PriceTableDetail>(groupData.Count(), paged);
        }

        public async Task<SearchProductPriceResponse> SearchProductPrice(SearchPriceProductRequest request)
        {
            var allProducts = await _productRepository.GetQueryableAsync();
            var userStoreIds = (await _userStoreRepository.GetListAsync(p => p.UserId == _currentUser.Id))
                .Select(p => p.StoreId).ToList();
            var productCategories = await _productCategoryRepository.GetQueryableAsync();
            var priceTables = _priceTableRepository
                .GetQueryableAsync()
                .Result
                .WhereIf(request.PriceTableIds.Any(), p => request.PriceTableIds.Contains(p.Id));

            if (request.StoreIds.Any())
            {
                var priceTableStores = await _priceTableStoreRepository.GetListAsync(p => request.StoreIds.Contains(p.StoreId));
                var storePriceTableIds = priceTableStores.Select(p => p.PriceTableId).Distinct().ToList();
                priceTables = priceTables.Where(p => storePriceTableIds.Contains(p.Id));
            }

            if (request.CustomerIds.Any())
            {
                var priceTableCustomers = await _priceTableCustomerRepository.GetListAsync(p => request.CustomerIds.Contains(p.CustomerId));
                var customerPriceTableIds = priceTableCustomers.Select(p => p.PriceTableId).Distinct().ToList();
                priceTables = priceTables.Where(p => customerPriceTableIds.Contains(p.Id));
            }

            if(!request.PriceTableIds.Any())
            {
                priceTables = priceTables.Where(p => p.AppliedFrom <= DateTime.Now.Date &&(p.AppliedTo == null || DateTime.Now.Date <= p.AppliedTo));
            }

            var allPriceTables = priceTables.Take(5).ToList();
            var priceTableIds = allPriceTables.Select(p => p.Id).ToList();
            var productPrices = _priceTableProductRepository.GetQueryableAsync()
                .Result
                .Where(p => priceTableIds.Contains(p.PriceTableId));

            var productQuery =
                from product in allProducts
                join category in productCategories on product.CategoryId equals category.Id
                into productCategory
                from category in productCategory.DefaultIfEmpty()
                select new ProductPriceDetail
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductCode = product.Code,
                    ProductCategoryId = category != null ? null : category.Id,
                    ProductCategoryName = category != null ? null : category.Name,
                    EntryPrice = product.EntryPrice,
                    SalePrice = product.SalePrice,
                    WholeSalePrice = product.WholeSalePrice,
                    SPAPrice = product.SPAPrice,
                    StockPrice = product.StockPrice
                };
            var productList =
                productQuery.WhereIf(!request.ProductCodeName.IsNullOrEmpty(), p =>
                    p.ProductCode.Contains(request.ProductCodeName)
                    || EF.Functions.Collate(p.ProductName, "SQL_Latin1_General_CP1_CI_AI").Contains(EF.Functions.Collate(request.ProductCodeName, "SQL_Latin1_General_CP1_CI_AI")))
                .WhereIf(request.ProductCategoryIds.Any(), p => request.ProductCategoryIds.Contains(p.ProductCategoryId.Value))
                .OrderBy(p => p.ProductName);

            var pagedProduct = productList.Skip(request.Offset).Take(request.PageSize).ToList();
            var pagedProductIds = pagedProduct.Select(p => p.ProductId).ToList();
            var pagedProductPrices = productPrices.Where(p => pagedProductIds.Contains(p.ProductId)).ToList();
            var pagedStoreProduct = (await _storeProductRepository
                    .GetListAsync(p => 
                        pagedProductIds.Contains(p.ProductId) 
                        && userStoreIds.Contains(p.StoreId)
                )).ToList();
            if (request.StoreIds.Any())
            {
                pagedStoreProduct = pagedStoreProduct.Where(p => request.StoreIds.Contains(p.StoreId)).ToList();
            }

            var productPriceDetails = new List<ProductPriceDetail>();
            foreach (var product in pagedProduct)
            {
                var productStock = pagedStoreProduct
                    .Where(p => p.ProductId == product.ProductId)
                    .Sum(p => p.StockQuantity);
                product.StockQuantity = productStock;

                foreach (var priceTable in allPriceTables)
                {
                    var priceTableProduct = pagedProductPrices.FirstOrDefault(p => p.ProductId == product.ProductId && p.PriceTableId == priceTable.Id);
                    
                    product.ProductPrices.Add(new PriceTableProductPriceDetail
                    {
                        PriceTableProductId = priceTableProduct?.Id,
                        ProductId = product.ProductId,
                        PriceTableId = priceTable.Id,
                        Price = priceTableProduct != null ? priceTableProduct.Price : 0,
                    });
                }
            }

            return new SearchProductPriceResponse()
            {
                PriceTables = allPriceTables.Select(p => new MasterDataDTO()
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name
                }).ToList(),
                Total = productList.Count(),
                Data = pagedProduct
            };
        }

        public async Task<SearchProductPriceResponse> SearchProductPriceByTableId(SearchPriceProductByTableIdRequest request)
        {
            var allProducts = await _productRepository.GetQueryableAsync();
            var userStoreIds = (await _userStoreRepository.GetListAsync(p => p.UserId == _currentUser.Id))
                .Select(p => p.StoreId).ToList();
            var productCategories = await _productCategoryRepository.GetQueryableAsync();
            var priceTable = _priceTableRepository
                .GetQueryableAsync()
                .Result
                .Where(p => p.Id == request.PriceTableId)
                .FirstOrDefault();

            var tableProductPrices = _priceTableProductRepository.GetQueryableAsync()
                .Result
                .Where(p => p.PriceTableId == priceTable.Id);

            var productQuery =
                from product in allProducts
                join category in productCategories on product.CategoryId equals category.Id into productCategory
                from category in productCategory.DefaultIfEmpty()
                join tablePriceProduct in tableProductPrices on product.Id equals tablePriceProduct.ProductId
                select new ProductPriceDetail
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductCode = product.Code,
                    ProductCategoryId = category.Id,
                    ProductCategoryName = category.Name,
                    EntryPrice = product.EntryPrice,
                    SalePrice = product.SalePrice,
                    WholeSalePrice = product.WholeSalePrice,
                    SPAPrice = product.SPAPrice,
                    StockPrice = product.StockPrice
                };
            var productList =
                productQuery.WhereIf(!request.ProductCodeName.IsNullOrEmpty(), p =>
                    p.ProductCode.Contains(request.ProductCodeName)
                    || EF.Functions.Collate(p.ProductName, "SQL_Latin1_General_CP1_CI_AI").Contains(EF.Functions.Collate(request.ProductCodeName, "SQL_Latin1_General_CP1_CI_AI")))
                .WhereIf(request.ProductCategoryIds.Any(), p => request.ProductCategoryIds.Contains(p.ProductCategoryId.Value))
                .OrderBy(p => p.ProductName);

            var pagedProduct = productList.Skip(request.Offset).Take(request.PageSize).ToList();
            var pagedProductIds = pagedProduct.Select(p => p.ProductId).ToList();
            var pagedProductPrices = tableProductPrices.Where(p => pagedProductIds.Contains(p.ProductId)).ToList();
            var pagedStoreProduct = (await _storeProductRepository
                    .GetListAsync(p => pagedProductIds.Contains(p.ProductId)
                    && userStoreIds.Contains(p.StoreId)
                )).ToList();

            var productPriceDetails = new List<ProductPriceDetail>();
            foreach (var product in pagedProduct)
            {
                var productStock = pagedStoreProduct
                    .Where(p => p.ProductId == product.ProductId)
                    .Sum(p => p.StockQuantity);
                product.StockQuantity = productStock;

                var priceTableProduct = pagedProductPrices.FirstOrDefault(p => p.ProductId == product.ProductId && p.PriceTableId == priceTable.Id);

                product.ProductPrices.Add(new PriceTableProductPriceDetail
                {
                    PriceTableProductId = priceTableProduct?.Id,
                    ProductId = product.ProductId,
                    PriceTableId = priceTable.Id,
                    Price = priceTableProduct != null ? priceTableProduct.Price : 0,
                });
            }

            return new SearchProductPriceResponse()
            {
                PriceTables = new List<MasterDataDTO>()
                {
                    new MasterDataDTO()
                    {
                        Id = priceTable.Id,
                        Code = priceTable.Code,
                        Name = priceTable.Name
                    }
                },
                Total = productList.Count(),
                Data = pagedProduct
            };
        }

        public async Task<List<MasterDataDTO>> SearchProductNotInPriceTable(SearchPriceProductNotInPriceTableRequest request)
        {
            var productIdInPriceTable = (await _priceTableProductRepository
                .GetQueryableAsync())
                .Where(p => p.PriceTableId == request.PriceTableId)
                .Select(p => p.ProductId)
                .ToList();

            var productNotInPriceTable = (await _productRepository
                .GetQueryableAsync())
                .Where(p => !productIdInPriceTable.Contains(p.Id))
                .WhereIf(!request.ProductCodeName.IsNullOrEmpty(), 
                    p => p.Code.Contains(request.ProductCodeName) 
                    || EF.Functions.Collate(p.Name, "SQL_Latin1_General_CP1_CI_AI").Contains(EF.Functions.Collate(request.ProductCodeName, "SQL_Latin1_General_CP1_CI_AI"))
                )
                .Select(p => new MasterDataDTO
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name
                }).ToList();

            return productNotInPriceTable;
        }

        public async Task AddProductPrice(AddPriceProductRequest request)
        {
            await ValidateAddProductPrice(request);
            var priceTableProducts = request.Products.Select(p => new PriceTableProduct
            {
                PriceTableId = request.PriceTableId,
                ProductId = p.ProductId,
                Price = p.Price
            });
            await _priceTableProductRepository.InsertManyAsync(priceTableProducts);
        }

        private async Task ValidateAddProductPrice(AddPriceProductRequest request)
        {
            var productCount = request.Products.GroupBy(p => p.ProductId).Any(grp => grp.Count() > 1);
            if (productCount)
            {
                throw new BusinessException("Không thể thêm - Sản phẩm được chọn nhiều lần");
            }
            var productIds = request.Products.Select(p => p.ProductId).ToList();
            var existProductPrice = await _priceTableProductRepository.AnyAsync(p => 
                p.PriceTableId == request.PriceTableId 
                && productIds.Contains(p.ProductId));

            if (existProductPrice)
            {
                throw new BusinessException("Không thể thêm - Đã tồn tại giá sản phẩm trong bảng giá");
            }
        }

        public async Task UpdateProductPrice(UpdatePriceProductRequest request)
        {
            await ValidateUpdateProductPrice(request);
            var priceTableProduct = await _priceTableProductRepository.GetAsync(p => p.Id == request.PriceTableProductId);
            if(priceTableProduct != null)
            {
                priceTableProduct.Price = request.Price;
                await _priceTableProductRepository.UpdateAsync(priceTableProduct);
            }
        }

        private async Task ValidateUpdateProductPrice(UpdatePriceProductRequest request)
        {
            var priceTableProduct = await _priceTableProductRepository.GetAsync(p => p.Id == request.PriceTableProductId);
            if(priceTableProduct == null)
            {
                throw new BusinessException("Không thể cập nhật - Giá sản phẩm không tồn tại");
            }
        }

        public async Task DeleteProductPrice(DeletePriceProductRequest request)
        {
            var priceTableProducts = await _priceTableProductRepository.GetListAsync(p => request.PriceTableProductIds.Contains(p.Id));
            if (priceTableProducts.Any())
            {
                await _priceTableProductRepository.DeleteManyAsync(priceTableProducts);
            }
        }

        public async Task DeleteMultipleProductPrice(DeleteMultiplePriceProductRequest request)
        {
            var priceTableProducts = await 
                _priceTableProductRepository.GetListAsync(p => request.ProductIds.Contains(p.ProductId));
            if (priceTableProducts.Any())
            {
                await _priceTableProductRepository.DeleteManyAsync(priceTableProducts);
            }
        }

        private async Task ValidateCreatePriceTable(CreatePriceTableRequest request)
        {
            if(request.AppliedFrom > request.AppliedTo)
            {
                throw new BusinessException("Thời gian áp dụng từ không thể lớn hơn Thời gian áp dụng đến");
            }
            var priceTableSameName = await _priceTableRepository
                .FirstOrDefaultAsync(p => EF.Functions.Collate(p.Name, "SQL_Latin1_General_CP1_CI_AI") == EF.Functions.Collate(request.Name, "SQL_Latin1_General_CP1_CI_AI"));
            if (priceTableSameName != null)
            {
                throw new BusinessException($"Đã tồn tại bảng giá giống tên {request.Name} ({priceTableSameName.Code} - {priceTableSameName.Name})");
            }

            //var priceTableInDateRange = await _priceTableRepository
            //    .FirstOrDefaultAsync(priceTable =>
            //        (
            //            (request.AppliedTo == null && request.AppliedFrom <= priceTable.AppliedFrom)
            //            || (priceTable.AppliedTo == null && request.AppliedTo != null && request.AppliedTo >= priceTable.AppliedFrom)
            //            || 
            //            (
            //                request.AppliedTo != null && priceTable.AppliedTo != null
            //                && 
            //                (
            //                    (request.AppliedFrom <= priceTable.AppliedFrom && request.AppliedTo >= priceTable.AppliedFrom)
            //                    || (request.AppliedFrom <= priceTable.AppliedTo && request.AppliedTo >= priceTable.AppliedTo)
            //                    || (priceTable.AppliedFrom <= request.AppliedFrom && priceTable.AppliedTo >= request.AppliedTo)
            //                )
            //            )
            //        )
            //        && priceTable.Status == Enums.PriceTableStatus.Active
            //        && !priceTable.IsDeleted
            //    );
        }

        private async Task ValidateUpdatePriceTable(UpdatePriceTableRequest request)
        {
            if (request.AppliedFrom > request.AppliedTo)
            {
                throw new BusinessException("Thời gian áp dụng từ không thể lớn hơn Thời gian áp dụng đến");
            }
            var priceTableSameName = await _priceTableRepository
                .FirstOrDefaultAsync(p => p.Id != request.Id
                && EF.Functions.Collate(p.Name, "SQL_Latin1_General_CP1_CI_AI") == EF.Functions.Collate(request.Name, "SQL_Latin1_General_CP1_CI_AI"));
            if (priceTableSameName != null)
            {
                throw new BusinessException($"Đã tồn tại bảng giá giống tên {request.Name} ({priceTableSameName.Code} - {priceTableSameName.Name})");
            }
        }

        public async Task<byte[]> ExportProductPrice(SearchPriceProductRequest request)
        {
            var allProducts = await _productRepository.GetQueryableAsync();
            var userStoreIds = (await _userStoreRepository.GetListAsync(p => p.UserId == _currentUser.Id))
                .Select(p => p.StoreId).ToList();
            var productCategories = await _productCategoryRepository.GetQueryableAsync();
            var priceTables = _priceTableRepository
                .GetQueryableAsync()
                .Result
                .WhereIf(request.PriceTableIds.Any(), p => request.PriceTableIds.Contains(p.Id));

            if (request.StoreIds.Any())
            {
                var priceTableStores = await _priceTableStoreRepository.GetListAsync(p => request.StoreIds.Contains(p.StoreId));
                var storePriceTableIds = priceTableStores.Select(p => p.PriceTableId).Distinct().ToList();
                priceTables = priceTables.Where(p => storePriceTableIds.Contains(p.Id));
            }

            if (request.CustomerIds.Any())
            {
                var priceTableCustomers = await _priceTableCustomerRepository.GetListAsync(p => request.CustomerIds.Contains(p.CustomerId));
                var customerPriceTableIds = priceTableCustomers.Select(p => p.PriceTableId).Distinct().ToList();
                priceTables = priceTables.Where(p => customerPriceTableIds.Contains(p.Id));
            }

            if (!request.PriceTableIds.Any())
            {
                priceTables = priceTables.Where(p => p.AppliedFrom <= DateTime.Now.Date && (p.AppliedTo == null || DateTime.Now.Date <= p.AppliedTo));
            }

            var allPriceTables = priceTables.Take(5).ToList();
            var priceTableIds = allPriceTables.Select(p => p.Id).ToList();
            var productPrices = _priceTableProductRepository.GetQueryableAsync()
                .Result
                .Where(p => priceTableIds.Contains(p.PriceTableId));

            var productQuery =
                from product in allProducts
                join category in productCategories on product.CategoryId equals category.Id
                into productCategory
                from category in productCategory.DefaultIfEmpty()
                select new ProductPriceDetail
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductCode = product.Code,
                    ProductCategoryId = category != null ? null : category.Id,
                    ProductCategoryName = category != null ? null : category.Name,
                    EntryPrice = product.EntryPrice,
                    SalePrice = product.SalePrice,
                    WholeSalePrice = product.WholeSalePrice,
                    SPAPrice = product.SPAPrice,
                    StockPrice = product.StockPrice
                };
            var productList =
                productQuery.WhereIf(!request.ProductCodeName.IsNullOrEmpty(), p =>
                    p.ProductCode.Contains(request.ProductCodeName)
                    || EF.Functions.Collate(p.ProductName, "SQL_Latin1_General_CP1_CI_AI").Contains(EF.Functions.Collate(request.ProductCodeName, "SQL_Latin1_General_CP1_CI_AI")))
                .WhereIf(request.ProductCategoryIds.Any(), p => request.ProductCategoryIds.Contains(p.ProductCategoryId.Value))
                .OrderBy(p => p.ProductName);

            var pagedProductIds = productList.Select(p => p.ProductId).ToList();
            var pagedProductPrices = productPrices.Where(p => pagedProductIds.Contains(p.ProductId)).ToList();
            var pagedStoreProduct = (await _storeProductRepository
                    .GetListAsync(p =>
                        pagedProductIds.Contains(p.ProductId)
                        && userStoreIds.Contains(p.StoreId)
                )).ToList();
            if (request.StoreIds.Any())
            {
                pagedStoreProduct = pagedStoreProduct.Where(p => request.StoreIds.Contains(p.StoreId)).ToList();
            }
            var exportData = new List<ExportPriceProductResponse>();
            foreach (var product in productList)
            {
                exportData.Add(new ExportPriceProductResponse()
                {
                    ProductName = product.ProductName,
                    ProductCode = product.ProductCode,
                    ProductCategoryName = product.ProductCategoryName,
                    StockPrice = product.StockPrice,
                    SPAPrice = product.SPAPrice.HasValue ? product.SPAPrice.Value : 0,
                    SalePrice = product.SalePrice.HasValue ? product.SalePrice.Value : 0,
                    StockQuantity = product.StockQuantity,
                    EntryPrice = product.EntryPrice.HasValue ? product.EntryPrice.Value : 0,
                    //PriceTable1 = 
                });
            }
            return ExcelHelper.ExportExcel(exportData);
        }
    }
}
