using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using VTECHERP.DTOs.ProductByCustomerReport;
using VTECHERP.DTOs.WarehousingBills;
using VTECHERP.Entities;
using VTECHERP.Helper;
using VTECHERP.Reports;
using VTECHERP.ServiceInterfaces;

namespace VTECHERP.Services.ReportService
{
    public class ProductByCustomerReportService : IProductByCustomerReportService
    {
        private readonly IRepository<Products> _productRepository;
        private readonly IRepository<ProductCategories> _productCategoriesRepository;
        private readonly IRepository<BillCustomerProduct> _billCustomerProductRepository;
        private readonly IRepository<BillCustomer> _billCustomerRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerReturn> _customerReturnRepository;
        private readonly IRepository<CustomerReturnProduct> _customerReturnProductRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICommonService _commonService;
        public ProductByCustomerReportService(IRepository<Products> productRepository, IUnitOfWorkManager unitOfWorkManager
            , ICommonService commonService
            , IRepository<BillCustomerProduct> billCustomerProductRepository
            , IRepository<BillCustomer> billCustomerRepository
            , IRepository<Customer> customerRepository
            , IRepository<CustomerReturn> customerReturnRepository
            , IRepository<CustomerReturnProduct> customerReturnProductRepository
            , IRepository<ProductCategories> productCategoriesRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _productRepository = productRepository;
            _commonService = commonService;
            _billCustomerProductRepository = billCustomerProductRepository;
            _billCustomerRepository = billCustomerRepository;
            _customerReturnRepository = customerReturnRepository;
            _customerRepository = customerRepository;
            _customerReturnProductRepository = customerReturnProductRepository;
            _productCategoriesRepository = productCategoriesRepository;
        }
        public async Task<IActionResult> SearchReportAsync(ProductByCustomerReportRequest input, CancellationToken cancellationToken = default)
        {
            try
            {
                var billCustomerProducts = await _billCustomerProductRepository.GetQueryableAsync();
                var billCustomers = await _billCustomerRepository.GetQueryableAsync();
                var customers = await _customerRepository.GetQueryableAsync();
                var customerReturns = await _customerReturnRepository.GetQueryableAsync();
                var customerReturnProducts = await _customerReturnProductRepository.GetQueryableAsync();

                var error = Validate(input);
                if (!string.IsNullOrEmpty(error))
                {
                    return new GenericActionResult(200, true, "No Data", null);
                }
                var data = SearchFilter(input);
                return new GenericActionResult(data.Result.Count, data.Result);
            }
            catch (Exception ex)
            {
                var errM = ex.Message;
                return new GenericActionResult(400, false, "Lỗi xảy ra", null);
            }
        }


        public async Task<List<ProductByCustomerReportDto>> SearchFilter(ProductByCustomerReportRequest input)
        {
            var products = await _productRepository.GetQueryableAsync();
            var productCategories = await _productCategoriesRepository.GetQueryableAsync();
            var billCustomerProducts = await _billCustomerProductRepository.GetQueryableAsync();
            var billCustomers = await _billCustomerRepository.GetQueryableAsync();
            var customers = await _customerRepository.GetQueryableAsync();
            var customerReturns = await _customerReturnRepository.GetQueryableAsync();
            var customerReturnProducts = await _customerReturnProductRepository.GetQueryableAsync();
            var error = Validate(input);
            if (!string.IsNullOrEmpty(error))
            {
                return null;
            }
            var query = from bc in billCustomers 
                        join bcp in billCustomerProducts on bc.Id equals bcp.BillCustomerId
                        join cu in customers
                        on bc.CustomerId equals cu.Id
                        into cusProGr
                        from cusPro in cusProGr.DefaultIfEmpty()
                        join pro in products
                        on bcp.ProductId equals pro.Id
                        into proGr
                        from pro in proGr.DefaultIfEmpty()
                        join proCa in productCategories
                        on pro.CategoryId equals proCa.Id
                        into proCaGr
                        from proCa in proCaGr.DefaultIfEmpty()
                        select new 
                        {
                            Code = bcp.Code,
                            CustomerName = cusPro.Name,
                            ProductCode = pro.Code,
                            CustomerId = bc.CustomerId,
                            ProductId = bcp.ProductId,
                            ProductName = pro.Name,
                            CategoryId = pro.CategoryId,
                            CategoryName = proCa.Name,
                            HandlerEmployeeId = bc.EmployeeSell,
                            HandlerEmployeeName = "",
                            EmployeeCare = bc.EmployeeCare,
                            EmployeeCareName = "",
                            StoreId = bc.StoreId,
                            TenantId = bc.TenantId,
                            CityId = cusPro.ProvinceId,
                            SalePrice = pro.SalePrice,
                            DiscountValue = bc.DiscountValue,
                            AmountAfterDiscount = bc.AmountAfterDiscount,
                            StockPrice = pro.StockPrice,
                            Quantity = bcp.Quantity,
                            CreationTime = bc.CreationTime
                        };
            query = query
                .WhereIf(input.DateFrom != null, x => x.CreationTime.Date >= input.DateFrom.Value.Date)
                .WhereIf(input.DateTo != null, x => x.CreationTime.Date <= input.DateTo.Value.Date)
                .WhereIf(input.LstEnterpriseId != null && input.LstEnterpriseId.Count > 0,x => input.LstEnterpriseId.Contains(x.TenantId.Value))
                .WhereIf(input.LstStoreId != null && input.LstStoreId.Count > 0,x => input.LstStoreId.Contains(x.StoreId.Value))
                .WhereIf(!string.IsNullOrEmpty(input.CustomerName), x => x.CustomerName.ToLower().Contains(input.CustomerName))
                .WhereIf(!string.IsNullOrEmpty(input.ProducName), x => x.ProductName.ToLower().Contains(input.ProducName))
                .WhereIf(input.LstCategoryId != null && input.LstCategoryId.Count > 0, x => input.LstCategoryId.Contains(x.CategoryId))
                .WhereIf(input.City != null, x => x.CityId == input.City);

            
            var queryReturn = from cr in customerReturns
                              join crp in customerReturnProducts
                              on cr.Id equals crp.CustomerReturnId
                              select new {
                                  CustomerId = cr.CustomerId,
                                  ProductId = crp.ProductId,
                                  Quatity = crp.Quantity,
                                  IsConfirm = cr.isConfirmed,
                                  TenantId = cr.TenantId,
                                  StoreId = cr.StoreId,
                                  CreationTime = cr.CreationTime,
                              };
            var queryReturnResult = queryReturn.Where(x => x.IsConfirm == 1);

            

            var data = query.ToList().GroupBy(x => new { x.CustomerId, x.ProductId });
            var result = new List<ProductByCustomerReportDto>();
            foreach (var item in data)
            {
                if (queryReturnResult != null && queryReturnResult.Count() > 0 )
                {
                    var returnQuatity = queryReturnResult.Where(i => i.ProductId == item.Key.ProductId && i.CustomerId == item.Key.CustomerId && item.FirstOrDefault().TenantId == i.TenantId && item.FirstOrDefault().StoreId == i.StoreId).Sum(x => x.Quatity);
                    var response = new ProductByCustomerReportDto
                    {
                        CustomerName = item.FirstOrDefault().CustomerName,
                        //TenantName = item.FirstOrDefault().Te
                        ProductCode = item.FirstOrDefault().ProductCode,
                        ProductId = item.Key.ProductId,
                        ProductName = item.FirstOrDefault().ProductName,
                        HandlerEmployeeId = item.FirstOrDefault().HandlerEmployeeId,
                        HandlerEmployeeName = item.FirstOrDefault().HandlerEmployeeName,
                        EmployeeCare = item.FirstOrDefault().EmployeeCare,
                        EmployeeCareName = item.FirstOrDefault().EmployeeCareName,
                        StoreId = item.FirstOrDefault().StoreId,
                        TenantId = item.FirstOrDefault().TenantId,
                        CategoryId = item.FirstOrDefault().CategoryId,
                        CategoryName = item.FirstOrDefault().CategoryName,
                        SaleQuantity = item.Sum(x => x.Quantity),
                        ReturnQuantity= returnQuatity.Value,
                        SalePrice = Math.Round(item.FirstOrDefault().SalePrice.Value,0),
                        DiscountValue = Math.Round(item.Sum(x =>x.DiscountValue).Value,0),
                        AmountAfterDiscount = Math.Round(item.Sum(x => x.AmountAfterDiscount).Value,0),
                        StockPrice  = Math.Round(item.FirstOrDefault().StockPrice,0),
                        Profit = Math.Round(item.Sum(x => x.AmountAfterDiscount).Value, 0) - Math.Round(item.FirstOrDefault().StockPrice, 0),
                        LastBuyDate = item.FirstOrDefault().CreationTime
                    };
                    result.Add(response);
                }
            }
            return result;
        }

        private string Validate(ProductByCustomerReportRequest input)
        {
            var error = "";
            if (input.LstEnterpriseId == null || input.LstEnterpriseId.Count == 0)
            {
                error = error + "doanh nghiệp là bắt buộc,";
            }
            if (input.DateFrom == null || input.DateTo == null)
            {
                error = error + "ngày là bắt buộc";
            }
            if (input.LstStoreId == null || input.LstStoreId.Count == 0)
            {
                error = error + "cửa hàng là bắt buộc,";
            }
            return error;
        }

        public async Task<byte[]> ExportProductByCustomerAsync(ProductByCustomerReportRequest request)
        {
            var data = SearchFilter(request);
            var exportData = new List<ExportProductByCustomerResponse>();

            if (data.Result != null && data.Result.Count > 0)
            {
                foreach (var item in data.Result)
                {
                    exportData.Add(new ExportProductByCustomerResponse()
                    {
                        CustomerName = item.CustomerName,
                        TenantName = item.TenantName,
                        HandlerEmployeeName = item.HandlerEmployeeName,
                        EmployeeCareName = item.EmployeeCareName,
                        ProductCode = item.ProductCode,
                        ProductName = item.ProductName,
                        CategoryName = item.CategoryName,
                        SalePrice = item.SalePrice,
                        StockPrice = item.StockPrice,
                        SaleQuantity = item.SaleQuantity,
                        ReturnQuantity = item.ReturnQuantity,
                        DiscountValue = item.DiscountValue,
                        AmountAfterDiscount = item.AmountAfterDiscount,
                        Profit = item.Profit,
                        LastBuyDate = item.LastBuyDate,
                    });
                }
            }
            return ExcelHelper.ExportExcel(exportData);
        }
    }
}
