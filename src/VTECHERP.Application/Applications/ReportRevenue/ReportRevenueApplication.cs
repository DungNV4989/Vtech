using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using VTECHERP.DTOs.BO.Tenants;
using VTECHERP.Entities;
using VTECHERP.Enums;
using VTECHERP.Reports;
using VTECHERP.Reports.Param;

namespace VTECHERP.Applications.ReportRevenue
{
    [Authorize]
    public class ReportRevenueApplication : ApplicationService
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<BillCustomer> _billCustomerRepository;
        private readonly IRepository<Entities.CustomerReturn> _customerReturnRepository;
        private readonly IRepository<BillCustomerProduct> _billCustomerProductRepository;
        private readonly IRepository<CustomerReturnProduct> _customerReturnProductRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<Attachment> _attachmentRepository;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Entities.ProductCategories> _productCategroyRepository;
        private readonly IRepository<Products> _productRepository;
        private readonly ICurrentTenant _currentTenant;
        public ReportRevenueApplication(
            IRepository<Customer> customerRepository
            , IRepository<BillCustomer> billCustomerRepository
            , IRepository<Entities.CustomerReturn> customerReturnRepository
            , IRepository<BillCustomerProduct> billCustomerProductRepository
            , IRepository<CustomerReturnProduct> customerReturnProductRepository
            , IConfiguration configuration
            , IRepository<Tenant> tenantRepository
            , IRepository<Attachment> attachmentRepository
            , IRepository<Entities.ProductCategories> productCategroyRepository
            , IRepository<Products> productRepository
            , ICurrentTenant currentTenant
            )
        {
            _customerReturnRepository = customerReturnRepository;
            _billCustomerRepository = billCustomerRepository;
            _customerRepository = customerRepository;
            _billCustomerProductRepository = billCustomerProductRepository;
            _customerReturnProductRepository = customerReturnProductRepository;
            _configuration = configuration;
            _tenantRepository = tenantRepository;
            _attachmentRepository = attachmentRepository;
            _productCategroyRepository = productCategroyRepository;
            _productRepository = productRepository;
            _currentTenant = currentTenant;
        }

        [HttpPost]
        public async Task<IActionResult> GetReportByCustomer(ReportRevenueCustomerParam param)
        {
            var result = new List<SpReportRevenueCustomer>();
            var connectionString = (await _customerRepository.GetDbContextAsync()).Database.GetConnectionString();

            DataTable storeIds = new DataTable();
            storeIds.Columns.Add("Id", typeof(Guid));

            // Đổ dữ liệu từ List<Guid> vào DataTable
            foreach (var guid in param.StoreId)
            {
                storeIds.Rows.Add(guid);
            }

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@storeIds", storeIds.AsTableValuedParameter("dbo.ListGuid"));
                parameters.Add("@createBillFrom", param.BillCustomerDateFrom, DbType.DateTime);
                parameters.Add("@createBillTo", param.BillCustomerDateTo, DbType.DateTime);
                parameters.Add("@customerType", param.CustomerType, DbType.Int32);
                parameters.Add("@customerId", param.CustomerId, DbType.Guid);
                parameters.Add("@province", param.ProvinceId, DbType.Guid);
                parameters.Add("@employeeCareId", param.HandleEmployee, DbType.Guid);

                result = db.Query<SpReportRevenueCustomer>("sp_report_revenue_customer", parameters, commandType: CommandType.StoredProcedure).ToList();
            }

            var currentTenant = _currentTenant;

            if (param.TenantId.Any() && currentTenant.Id == null)
                result = result.Where(x => param.TenantId.Contains(x.TenantId ?? Guid.Empty)).ToList();

            if (currentTenant.Id != null && currentTenant.Id != Guid.Empty)
                result = result.Where(x => x.TenantId == currentTenant.Id).ToList();

            var connectionStringBO = _configuration["ConnectionStrings:BO"];
            using (IDbConnection db = new SqlConnection(connectionStringBO))
            {
                db.Open();
                var query = "select Id, Name from AbpTenants";
                var tenants = db.Query<TenantBaseDto>(query).ToList();

                foreach (var item in result)
                {
                    var tenantInfo = tenants.FirstOrDefault(x => x.Id == item.TenantId);
                    if (tenantInfo != null)
                        item.TenantName = tenantInfo.Name;
                }
            }

            var rowTotal = new
            {
                TotalBillCustomerBuy = result.Sum(x => x.TotalBillCustomerBuy),
                TotalBuyAmount = result.Sum((x) => x.TotalBuyAmount),
                TotalBillCustomerReturn = result.Sum(x => x.TotalBillCustomerReturn),
                TotalReturnAmount = result.Sum(x => x.TotalReturnAmount),
                TotalProductBuy = result.Sum(x => x.TotalProductBuy),
                TotalProductReturn = result.Sum(x => x.TotalProductReturn),
                DiscountAmount = result.Sum(x => x.DiscountAmount),
                Revenue = result.Sum(x => x.Revenue),
                CostPrice = result.Sum(x => x.CostPrice),
                Profit = result.Sum(x => x.Profit),
            };

            result = result.Where(x => x.TotalBillCustomerBuy > 0 || x.TotalBillCustomerReturn > 0).ToList();

            return new OkObjectResult(new
            {
                data = result,
                success = true,
                rowTotal = rowTotal,
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetReportByProduct(ReportRevenueProductParam param)
        {
            var result = new List<StoreReportRevenueProduct>();
            var connectionString = (await _customerRepository.GetDbContextAsync()).Database.GetConnectionString();

            var revenueTenants = new List<SpGetRevenueAllTenant>();

            DataTable storeIds = new DataTable();
            storeIds.Columns.Add("Id", typeof(Guid));

            // Đổ dữ liệu từ List<Guid> vào DataTable
            foreach (var guid in param.StoreIds)
                storeIds.Rows.Add(guid);

            DataTable category = new DataTable();
            category.Columns.Add("Id", typeof(Guid));

            // Đổ dữ liệu từ List<Guid> vào DataTable
            foreach (var guid in param.CategoryIds)
                category.Rows.Add(guid);

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@storeIds", storeIds.AsTableValuedParameter("dbo.ListGuid"));
                parameters.Add("@category", category.AsTableValuedParameter("dbo.ListGuid"));
                parameters.Add("@createBillFrom", param.DateFrom, DbType.DateTime);
                parameters.Add("@createBillTo", param.DateTo, DbType.DateTime);
                parameters.Add("@productId", param.ProductId, DbType.Guid);
                parameters.Add("@productStatus", param.ProductStatus, DbType.Int32);

                result = db.Query<StoreReportRevenueProduct>("sp_report_revenue_product", parameters, commandType: CommandType.StoredProcedure).ToList();

                // Get dữ liệu doanh thu doanh nghiệp
                var parametersRevenueTenants = new DynamicParameters();
                parametersRevenueTenants.Add("@createBillFrom", param.DateFrom, DbType.DateTime);
                parametersRevenueTenants.Add("@createBillTo", param.DateTo, DbType.DateTime);
                revenueTenants = db.Query<SpGetRevenueAllTenant>("GetRevenueAllTenant", parametersRevenueTenants, commandType: CommandType.StoredProcedure).ToList();
            }

            var currentTenant = _currentTenant;

            if (param.Tenants.Any() && currentTenant.Id == null)
                result = result.Where(x => param.Tenants.Contains(x.TenantId ?? Guid.Empty)).ToList();

            if (currentTenant.Id != null && currentTenant.Id != Guid.Empty)
                result = result.Where(x => x.TenantId == currentTenant.Id).ToList();

            var respon = result.Where(x => x.QuantitySell > 0 || x.QuantityReturn > 0).Skip((param.PageIndex - 1) * param.PageSize).Take(param.PageSize).ToList();

            var connectionStringBO = _configuration["ConnectionStrings:BO"];
            using (IDbConnection db = new SqlConnection(connectionStringBO))
            {
                db.Open();
                var query = "select Id, Name from AbpTenants";
                var tenants = db.Query<TenantBaseDto>(query).ToList();

                var productIds = respon.Select(x => x.ProductId).ToList();
                var attachments = await _attachmentRepository.GetListAsync(x => x.ObjectType == Enums.AttachmentObjectType.Product
                && productIds.Contains(x.ObjectId ?? Guid.Empty));

                foreach (var item in respon)
                {
                    var tenantInfo = tenants.FirstOrDefault(x => x.Id == item.TenantId);
                    if (tenantInfo != null)
                        item.TenantName = tenantInfo.Name;

                    var tenant = revenueTenants.FirstOrDefault(x => x.TenantId == item.TenantId);
                    if (tenant != null)
                        item.PrecentProfit = (item.Revenue / tenant.TotalRevenue) * 100;

                    var attachment = attachments.FirstOrDefault(x => x.ObjectId == item.ProductId);
                    if (attachment != null)
                        item.UrlImg = attachment.Url;
                }
            }

            var count = result.Count(x => x.QuantitySell > 0 || x.QuantityReturn > 0);
            var rowTotal = new
            {
                QuantitySell = respon.Sum(x => x.QuantitySell),
                TotalAmountSell = respon.Sum((x) => x.TotalAmountSell),
                QuantityReturn = respon.Sum(x => x.QuantityReturn),
                TotalAmountReturn = respon.Sum(x => x.TotalAmountReturn),
                QuantityReveue = respon.Sum(x => x.QuantityReveue),
                TotalAmountReveue = respon.Sum(x => x.TotalAmountReveue),
                Discount = respon.Sum(x => x.Discount),
                Revenue = respon.Sum(x => x.Revenue),
                CostPrice = respon.Sum(x => x.CostPrice),
                TotalProfit = respon.Sum(x => x.TotalProfit),
            };

            var TotalGroup = new
            {
                QuantitySell = result.Sum(x => x.QuantitySell),
                TotalAmountSell = result.Sum((x) => x.TotalAmountSell),
                QuantityReturn = result.Sum(x => x.QuantityReturn),
                TotalAmountReturn = result.Sum(x => x.TotalAmountReturn),
                QuantityReveue = result.Sum(x => x.QuantityReveue),
                TotalAmountReveue = result.Sum(x => x.TotalAmountReveue),
                Discount = result.Sum(x => x.Discount),
                Revenue = result.Sum(x => x.Revenue),
                CostPrice = result.Sum(x => x.CostPrice),
                TotalProfit = result.Sum(x => x.TotalProfit),
            };

            return new OkObjectResult(new
            {
                data = respon,
                success = true,
                rowTotal = rowTotal,
                totalGroup = TotalGroup,
                count = count
            });
        }
        // báo cáo doanh số theo cửa hàng

        [HttpPost]
        public async Task<IActionResult> GetReportByStore(ReportRevenueStoreParam param)
        {
            var result = new List<SpReportRevenueStore>();
            var connectionString = (await _customerRepository.GetDbContextAsync()).Database.GetConnectionString();

            DataTable storeIds = new DataTable();

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Open();
                result = db.Query<SpReportRevenueStore>("StoreRevenueReprot", null, commandType: CommandType.StoredProcedure).ToList();
            }

            var rowTotal = new
            {
                RetailSales = result.Sum(x => x.RetailSales),
                RetailRevenue = result.Sum((x) => x.RetailRevenue),
                RetailCostPrice = result.Sum(x => x.RetailCostPrice),

                AgencySales = result.Sum(x => x.AgencySales),
                AgencyRevenue = result.Sum(x => x.AgencyRevenue),
                AgencyCostPrice = result.Sum(x => x.AgencyCostPrice),

                SpaSales = result.Sum(x => x.SpaSales),
                SpaRevenue = result.Sum(x => x.SpaRevenue),
                SpaCostPrice = result.Sum(x => x.SpaCostPrice),

                TotalProfit = result.Sum(x => x.TotalProfit),

                TotalSales = result.Sum(x => x.TotalSales),
                TotalRevenue = result.Sum(x => x.TotalRevenue),
                TotalCostPrice = result.Sum(x => x.TotalCostPrice),

            };

            return new OkObjectResult(new
            {
                data = result,
                success = true,
                rowTotal = rowTotal,
            });
        }

        public async Task<IActionResult> GetReportByCategory(ReportRevenueCategoryProductParam param)
        {
            var result = new List<ReportRevenueCategoryDto>();
            var billCustomerQueryable = await _billCustomerRepository.GetQueryableAsync();

            if (param.DateFrom.HasValue)
                billCustomerQueryable = billCustomerQueryable.Where(x => x.CreationTime.Date >= param.DateFrom.Value.Date);

            if (param.DateTo.HasValue)
                billCustomerQueryable = billCustomerQueryable.Where(x => x.CreationTime.Date <= param.DateTo.Value.Date);

            if (param.StoreIds != null && param.StoreIds.Any())
                billCustomerQueryable = billCustomerQueryable.Where(x => param.StoreIds.Contains(x.StoreId ?? Guid.Empty));

            return null;
        }
    }
}
