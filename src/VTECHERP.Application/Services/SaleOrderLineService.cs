using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;
using VTECHERP.Domain.Shared.Helper.Excel.Model;
using VTECHERP.DTOs.SaleOrderLines;
using VTECHERP.DTOs.SaleOrderLines.Params;
using VTECHERP.Entities;

namespace VTECHERP.Services
{
    public class SaleOrderLineService : ISaleOrderLineService
    {
        private readonly IRepository<SaleOrderLines> _saleOrderLineRepository;
        private readonly IRepository<SaleOrders> _saleOrderRepository;
        private readonly IRepository<Suppliers> _suplierRepository;
        private readonly IRepository<Products> _productRepository;
        private readonly IObjectMapper _objectMapper;
        private readonly ICurrentUser _currentUser;
        private readonly IProductService _productService;
        private readonly IRepository<ProductCategories> _productCategoryRepository;
        private readonly IRepository<UserStore> _userStoreRepository;

        public SaleOrderLineService(
            IRepository<SaleOrderLines> saleOrderLineRepository
            , IObjectMapper objectMapper
            , ICurrentUser currentUser
            , IProductService productService
            , IRepository<SaleOrders> saleOrderRepository
            , IRepository<Suppliers> suplierRepository
            , IRepository<Products> productRepository
            , IRepository<ProductCategories> productCategoryRepository
            , IRepository<UserStore> userStoreRepository
            )
        {
            _saleOrderLineRepository = saleOrderLineRepository;
            _objectMapper = objectMapper;
            _currentUser = currentUser;
            _productService = productService;
            _saleOrderRepository = saleOrderRepository;
            _suplierRepository = suplierRepository;
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
            _userStoreRepository = userStoreRepository;
        }

        public async Task<object> AddRangeAsync(Guid orderId, List<SaleOrderLineCreateRequest> requests)
        {
            List<SaleOrderLines> saleOrderLines = _objectMapper.Map<List<SaleOrderLineCreateRequest>, List<SaleOrderLines>>(requests);
            saleOrderLines.ForEach(x =>
            {
                x.OrderId = orderId;
                x.TotalYuan = x.RequestQuantity * x.RequestPrice;
                x.CreatorId = _currentUser.Id;
            });
            await _saleOrderLineRepository.InsertManyAsync(saleOrderLines);
            return requests;
        }

        public async Task<object> UpdateRangeAsync(Guid orderId, List<SaleOrderLineUpdateRequest> requests)
        {
            var adds = _objectMapper.Map<List<SaleOrderLineUpdateRequest>, List<SaleOrderLineCreateRequest>>(requests.Where(x => !x.Id.HasValue).ToList());
            await AddRangeAsync(orderId, adds);

            var updates = new List<SaleOrderLines>();
            foreach (var item in requests.Where(x => x.Id.HasValue))
            {
                var saleOrderLine = await _saleOrderLineRepository.FindAsync(x => x.Id == item.Id);

                if (saleOrderLine.ImportQuantity < saleOrderLine.RequestQuantity)
                {
                    if (!item.IsDelete)
                    {
                    
                        saleOrderLine.RequestQuantity = item.RequestQuantity ?? saleOrderLine.RequestQuantity;
                        saleOrderLine.SuggestedPrice = item.SuggestedPrice ?? saleOrderLine.SuggestedPrice;
                        saleOrderLine.TotalYuan = item.RequestQuantity * saleOrderLine.RequestPrice;
                        saleOrderLine.LastModifierId = _currentUser.Id;
                    }
                    else if(saleOrderLine.ImportQuantity <= 0)
                    {
                        saleOrderLine.IsDeleted = true;
                        saleOrderLine.DeleterId = _currentUser.Id;
                    }

                    updates.Add(saleOrderLine);
                }
            }

            if (updates.Any())
                await _saleOrderLineRepository.UpdateManyAsync(updates);

            return requests;
        }

        public async Task<SaleOrderLineDto> GetByIdAsync(Guid id)
        {
            var saleOrderLine = await _saleOrderLineRepository.FindAsync(x => x.Id == id);
            if (saleOrderLine == null)
                return null;
            return _objectMapper.Map<SaleOrderLines, SaleOrderLineDto>(saleOrderLine);
        }

        public async Task<List<SaleOrderLineDetailDto>> GetByOrderIdAsync(Guid orderId)
        {
            var saleOrderLines = await _saleOrderLineRepository.GetListAsync(x => x.OrderId == orderId);
            if (saleOrderLines == null)
                return null;
            var result = _objectMapper.Map<List<SaleOrderLines>, List<SaleOrderLineDetailDto>>(saleOrderLines);
            var productIds = result.Select(x => x.ProductId).Distinct().ToList();
            var products = await _productService.GetByIdsAsync(productIds);

            foreach (var item in result)
            {
                var product = products.FirstOrDefault(x => x.Id == item.ProductId);
                item.Code = product.SequenceId;
                item.ProductName = product.Name;
            }

            return result;
        }

        public async Task<List<SaleOrderLineDto>> ListByOrderIdAsync(Guid orderId)
        {
            var saleOrderLine = await _saleOrderLineRepository.GetListAsync(x => x.OrderId == orderId);
            if (saleOrderLine == null)
                return null;
            return _objectMapper.Map<List<SaleOrderLines>, List<SaleOrderLineDto>>(saleOrderLine);
        }


        public async Task<IQueryable<SaleOrderListItem>> GetListFull(SaleOrderLineGetListParam param)
        {
            var saleOrders = await GetList();

            if (param.FromDate.HasValue)
                saleOrders = saleOrders.Where(x => x.CreateTime.Date >= param.FromDate.Value.Date);

            if (param.ToDate.HasValue)
                saleOrders = saleOrders.Where(x => x.CreateTime.Date <= param.ToDate.Value.Date);

            if (!string.IsNullOrEmpty(param.ProductName))
                saleOrders = saleOrders.Where(x => x.ProductName.Trim().ToLower().Contains(param.ProductName.Trim().ToLower()));

            if (!string.IsNullOrEmpty(param.SuplierName))
                saleOrders = saleOrders.Where(delegate (SaleOrderListItem x) {
                    if (!string.IsNullOrEmpty(x.SuplierText))
                        return x.SuplierText.Trim().ToLower().Contains(param.SuplierName.Trim().ToLower());

                    return false;
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(param.InvoiceNumber))
                saleOrders = saleOrders.Where(delegate (SaleOrderListItem x) {
                    if (!string.IsNullOrEmpty(x.InvoiceNumber))
                        return x.InvoiceNumber.Trim().ToLower().Contains(param.InvoiceNumber.Trim().ToLower());

                    return false;
                })
                .AsQueryable();

            if (param.StoreId.Any())
                saleOrders = saleOrders.Where(x => param.StoreId.Contains(x.StoreId));

            if (param.SaleOrderStatus.HasValue)
                saleOrders = saleOrders.Where(x => x.SaleOrderStatus == param.SaleOrderStatus.Value);

            if (param.ProductCategory.Any())
                saleOrders = saleOrders.Where(x => param.ProductCategory.Contains(x.ProductType));

            if (param.CategoryProductSearch.HasValue && param.CategoryProductSearch == CategoryProductSearch.NoteExecute)
                saleOrders = saleOrders.Where(x => x.ImportQuantity <= 0);

            if (param.CategoryProductSearch.HasValue && param.CategoryProductSearch == CategoryProductSearch.Executed)
                saleOrders = saleOrders.Where(x => x.ImportQuantity > 0);

            if (!string.IsNullOrEmpty(param.SaleOrderLineCode))
                saleOrders = saleOrders.Where(x => x.SaleOrderCode.Contains(param.SaleOrderLineCode.Trim()));

            return saleOrders;
        }

        public async Task<IQueryable<SaleOrderListItem>> GetList()
        {
            var result = new List<SaleOrderListItem>();
            var userStore = (await _userStoreRepository.GetQueryableAsync()).Where(x => x.UserId == _currentUser.GetId());
            if (userStore == null)
                return result.AsQueryable();
            var saleOrderQueryable = (await _saleOrderRepository.GetQueryableAsync()).Where(x=> userStore.Any(uS=>uS.StoreId == x.StoreId));
            var saleProductQueryable = (await _saleOrderLineRepository.GetQueryableAsync()).Where(x=> saleOrderQueryable.Any(sO => sO.Id == x.OrderId));
            var productQueryable = (await _productRepository.GetQueryableAsync()).Where(x => saleProductQueryable.Any(sP => sP.ProductId == x.Id));
            var suplierQueryable = await _suplierRepository.GetQueryableAsync();
            var productCategoryQueryable = await _productCategoryRepository.GetQueryableAsync();

            var query = from product in productQueryable

                        join category in productCategoryQueryable
                        on product.CategoryId equals category.Id
                        into categoryGr
                        from category in categoryGr.DefaultIfEmpty()

                        join saleProduct in saleProductQueryable
                        on product.Id equals saleProduct.ProductId

                        join saleOrder in saleOrderQueryable
                        on saleProduct.OrderId equals saleOrder.Id

                        join suplier in suplierQueryable
                        on saleOrder.SupplierId equals suplier.Id
                        into supliderGr
                        from suplier in supliderGr.DefaultIfEmpty()

                        orderby saleProduct.Code descending

                        select new SaleOrderListItem
                        {
                            ProductName = product.Name,
                            InvoiceNumber = saleOrder.InvoiceNumber,
                            Note = saleProduct.Note,
                            Rate = saleOrder.Rate,
                            RequestPrice = saleProduct.RequestPrice,
                            SuggestedPrice = saleProduct.SuggestedPrice,
                            SuplierText = suplier == null ? "": suplier.Name,
                            RequestQuantity= saleProduct.RequestQuantity,
                            TotalYuan= saleProduct.RequestPrice * saleProduct.RequestQuantity,
                            TotalPrice = saleProduct.RequestPrice * saleProduct.RequestQuantity * Convert.ToDecimal(saleOrder.Rate),
                            ImportQuantity= saleProduct.ImportQuantity,
                            StoreId = saleOrder.StoreId,
                            ProductType = product.CategoryId,
                            SaleOrderStatus = saleOrder.Status,
                            CreateTime = saleOrder.CreationTime,
                            SaleOrderLineCode = saleProduct.Code,
                            SaleOrderCode = saleOrder.Code,
                            Id = saleProduct.Id
                        };

            return query;
        }

        public async Task<byte[]> ExportAsync(SaleOrderLineGetListParam param)
        {
            var workbook = new CustomWorkBook();
            var orderTransports = await GetListFull(param);
            var sheet = RenderTemplate(orderTransports.ToList());
            workbook.Sheets.Add(sheet);

            return ExcelHelper.ExportExcel(workbook);
        }

        private CustomSheet RenderTemplate(List<SaleOrderListItem> data)
        {
            var sheet = new CustomSheet("Sản phẩm phiếu đặt hàng");

            var startRow = 2;

            var header = new CustomDataTable()
            {
                StartRowIndex = startRow,
                StartColumnIndex = 1,
                RowDirection = Directions.Horizontal,
                Rows = new List<DataRow>
                    {
                        new DataRow(
                            new HeaderCell("ID"),
                            new HeaderCell("Tên sản phẩm"),
                            new HeaderCell("Nhà cung cấp"),
                            new HeaderCell("Số hóa đơn"),
                            new HeaderCell("Giá tiền tệ"),
                            new HeaderCell("Giá yêu cầu"),
                            new HeaderCell("Giá đề xuất"),
                            new HeaderCell("TT tệ"),
                            new HeaderCell("Tổng tiền"),
                            new HeaderCell("Số lượng"),
                            new HeaderCell("Mô tả")
                            )
                    }
            };

            sheet.Tables.Add(header);

            var indexFirstColumn = startRow + 1;
            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];
                var firstColumn = new CustomDataTable()
                {
                    StartRowIndex = indexFirstColumn++,
                    StartColumnIndex = 1,
                    RowDirection = Directions.Horizontal,
                    Rows = new List<DataRow>
                    {
                        new DataRow(
                            new Cell(item.SaleOrderLineCode)
                            {
                                Format = "@"
                            },
                            new Cell(item.ProductName),
                            new Cell(item.SuplierText),
                            new Cell(item.InvoiceNumber),
                            new Cell(item.Rate),
                            new Cell(item.RequestPrice),
                            new Cell(item.SuggestedPrice),
                            new Cell(item.TotalYuan),
                            new Cell(item.TotalPrice),
                            new Cell($"{item.ImportQuantity}/{item.RequestQuantity}"),
                            new Cell(item.Note)
                            )
                    }
                };

                sheet.Tables.Add(firstColumn);
            }

            return sheet;
        }

    }
}