using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using VTECHERP.DTOs.BestSaleByProductReport;
using VTECHERP.DTOs.InventoryByProductReport;
using VTECHERP.DTOs.WarehousingBills;
using VTECHERP.Entities;
using VTECHERP.Helper;
using VTECHERP.Reports;
using VTECHERP.ServiceInterfaces;

namespace VTECHERP.Services.ReportService
{
    public class BestSaleByProductReportService : IBestSaleByProductReportService
    {
        private readonly IRepository<Products> _productRepository;
        private readonly IRepository<WarehousingBillProduct> _warehousingBillProductRepository;
        private readonly IRepository<WarehousingBill> _warehousingBillRepository;
        private readonly IRepository<BillCustomerProduct> _billCustomerProductRepository;
        private readonly IRepository<BillCustomer> _billCustomerRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerReturn> _customerReturnRepository;
        private readonly IRepository<CustomerReturnProduct> _customerReturnProductRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICommonService _commonService;
        public BestSaleByProductReportService(IRepository<Products> productRepository, IUnitOfWorkManager unitOfWorkManager
            , ICommonService commonService
            , IRepository<WarehousingBillProduct> warehousingBillProductRepository
            , IRepository<WarehousingBill> warehousingBillRepository
            , IRepository<BillCustomerProduct> billCustomerProductRepository
            , IRepository<BillCustomer> billCustomerRepository
            , IRepository<Customer> customerRepository
            , IRepository<CustomerReturn> customerReturnRepository
            , IRepository<CustomerReturnProduct> customerReturnProductRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _productRepository = productRepository;
            _warehousingBillProductRepository = warehousingBillProductRepository;
            _commonService = commonService;
            _warehousingBillRepository = warehousingBillRepository;
            _billCustomerProductRepository = billCustomerProductRepository;
            _billCustomerRepository = billCustomerRepository;
            _customerReturnRepository = customerReturnRepository;
            _customerRepository = customerRepository;
            _customerReturnProductRepository = customerReturnProductRepository;
        }
        public async Task<IActionResult> SearchReportAsync(BestSaleByProductReportRequest input, CancellationToken cancellationToken = default)
        {
            try
            {
                var warehousingBillProducts = await _warehousingBillProductRepository.GetQueryableAsync();
                var warehousingBillProductGroupByProduct = warehousingBillProducts.OrderByDescending(x => x.CreationTime);
                var warehousingBills = await _warehousingBillRepository.GetQueryableAsync();
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
                var retVal = new List<BestSaleByProductReportDto>();
                var dataTableStoreId = new DataTable();
                dataTableStoreId.Columns.Add("StoreId", typeof(Guid));
                if (input.LstStoreId != null && input.LstStoreId.Count > 0)
                {
                    foreach (var guidValue in input.LstStoreId)
                    {
                        dataTableStoreId.Rows.Add(guidValue);
                    }
                }

                var dataTableEnterpriseId = new DataTable();
                dataTableEnterpriseId.Columns.Add("EnterpriseId", typeof(Guid));
                if (input.LstEnterpriseId != null && input.LstEnterpriseId.Count > 0)
                {
                    foreach (var guidValue in input.LstEnterpriseId)
                    {
                        dataTableEnterpriseId.Rows.Add(guidValue);
                    }
                }
                using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false))
                {
                    var query = $"EXEC sp_report_best_sale_by_product  @DateFrom, @DateTo, @EnterpriseIds, @StoreIds";

                    var dateFrm = new SqlParameter("@DateFrom", SqlDbType.DateTime);
                    var dateTo = new SqlParameter("@DateTo", SqlDbType.DateTime);
                    var epIds = new SqlParameter("@EnterpriseIds", SqlDbType.Structured);
                    var sIds = new SqlParameter("@StoreIds", SqlDbType.Structured);


                    dateFrm.Value = new DateTime(input.DateFrom.Value.Year, input.DateFrom.Value.Month, input.DateFrom.Value.Day, 0, 0, 0);
                    dateTo.Value = new DateTime(input.DateTo.Value.Year, input.DateTo.Value.Month, input.DateTo.Value.Day, 23, 59, 59);


                    epIds.TypeName = "dbo.GuidListEnterpriseId";
                    epIds.Value = dataTableEnterpriseId;
                    sIds.TypeName = "dbo.GuidListStoreId";
                    sIds.Value = dataTableStoreId;

                    //var data = await _entryRepository.GetDbContext().Database.ExecuteSqlRawAsync(query, listParam, cancellationToken);
                    var connection = _productRepository.GetDbContext().Database.GetDbConnection();
                    connection.Close();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = "sp_report_best_sale_by_product";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(dateFrm);
                    cmd.Parameters.Add(dateTo);
                    cmd.Parameters.Add(epIds);
                    cmd.Parameters.Add(sIds);
                    connection.Open();

                    var reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {

                            var val = new BestSaleByProductReportDto()
                            {
                                ProductName = reader["ProductName"] == null ? "" : reader["ProductName"].ToString(),
                                Code = reader["Code"] == null ? "" : reader["Code"].ToString(),
                                ProductId = reader["ProductId"] == null || reader["ProductId"] == DBNull.Value ? (Guid?)null : Guid.Parse(reader["ProductId"].ToString()),
                                StoreId = reader["StoreId"] == null || reader["StoreId"] == DBNull.Value ? (Guid?)null : Guid.Parse(reader["StoreId"].ToString()),
                                TenantId = reader["TenantId"] == null ? null : Guid.Parse(reader["TenantId"].ToString()),
                                Price = reader["Price"] == null ? 0 : decimal.Parse(reader["Price"].ToString())
                            };
                            retVal.Add(val);
                        }
                    }
                    connection.Close();
                }
                retVal = retVal.WhereIf(!string.IsNullOrEmpty(input.ProductName), x => x.ProductName.ToLower().Contains(input.ProductName.ToLower())).ToList();
                foreach (var item in retVal)
                {
                    var query = from wbp in warehousingBillProductGroupByProduct
                                join wb in warehousingBills on wbp.WarehousingBillId equals wb.Id
                                where wbp.ProductId == item.ProductId
                                select new
                                {
                                    Quantity = wbp.Quantity,
                                    BillType = wb.BillType,
                                    CreationTime = wbp.CreationTime,
                                    Price = wbp.Price,
                                    Code = wb.Code,
                                    StoreId = wb.StoreId,
                                    ProductId = wbp.ProductId,
                                };

                    var querySale = from bc in billCustomers
                                    join bcp in billCustomerProducts on bc.Id equals bcp.BillCustomerId
                                    where bcp.ProductId == item.ProductId
                                    select new
                                    {
                                        Quantity = bcp.Quantity,
                                        CustomerType = bc.CustomerType,
                                        CreationTime = bcp.CreationTime,
                                        StoreId = bc.StoreId,
                                        Price = bcp.Price,
                                        BillCustomerId = bc.Id,
                                        ProductId = bcp.ProductId,
                                        TenantId = bcp.TenantId,
                                    };

                    var queryReturn = from c in customers
                                      join cr in customerReturns on c.Id equals cr.CustomerId
                                      join crp in customerReturnProducts on cr.Id equals crp.CustomerReturnId
                                      where crp.ProductId == item.ProductId
                                      select new
                                      {
                                          Quantity = crp.Quantity,
                                          CustomerType = c.CustomerType,
                                          CreationTime = crp.CreationTime,
                                          StoreId = cr.StoreId,
                                          Price = crp.Price,
                                          ProductId = crp.ProductId,
                                          TenantId = crp.TenantId,
                                      };
                    var retailQuantity = querySale.Where(x => x.CustomerType == Enums.CustomerType.RetailCustomer && x.ProductId == item.ProductId
                    && input.LstStoreId.Contains(x.StoreId.Value)).Sum(p => p.Quantity);
                    var agencyQuantity = querySale.Where(x => x.CustomerType == Enums.CustomerType.Agency && x.ProductId == item.ProductId
                    && input.LstStoreId.Contains(x.StoreId.Value)).Sum(p => p.Quantity);
                    var spaQuantity = querySale.Where(x => x.CustomerType == Enums.CustomerType.SPACustomer && x.ProductId == item.ProductId
                    && input.LstStoreId.Contains(x.StoreId.Value)).Sum(p => p.Quantity);

                    var retailReturnQuantity = queryReturn.Where(x => x.CustomerType == Enums.CustomerType.RetailCustomer && x.ProductId == item.ProductId
                   && input.LstStoreId.Contains(x.StoreId.Value)).Sum(p => p.Quantity);
                    var agencyReturnQuantity = queryReturn.Where(x => x.CustomerType == Enums.CustomerType.Agency && x.ProductId == item.ProductId
                    && input.LstStoreId.Contains(x.StoreId.Value)).Sum(p => p.Quantity);
                    var spaReturnQuantity = queryReturn.Where(x => x.CustomerType == Enums.CustomerType.SPACustomer && x.ProductId == item.ProductId
                    && input.LstStoreId.Contains(x.StoreId.Value)).Sum(p => p.Quantity);

                    var wareImportBegin = query.Where(x => x.BillType == Enums.WarehousingBillType.Import && x.CreationTime.Date < input.DateFrom.Value.Date).Sum(x => x.Quantity);
                    var wareExportBegin = query.Where(x => x.BillType == Enums.WarehousingBillType.Export && x.CreationTime.Date < input.DateFrom.Value.Date).Sum(x => x.Quantity);
                    var sLImportPeriod = query.Where(x => x.BillType == Enums.WarehousingBillType.Import && x.CreationTime.Date >= input.DateFrom.Value.Date && x.CreationTime.Date <= input.DateTo.Value.Date &&
                    input.LstStoreId.Contains(x.StoreId)).Sum(x => x.Quantity);
                    var sLExportPeriod = query.Where(x => x.BillType == Enums.WarehousingBillType.Export && x.CreationTime.Date >= input.DateFrom.Value.Date && x.CreationTime.Date <= input.DateTo.Value.Date &&
                   input.LstStoreId.Contains(x.StoreId)).Sum(x => x.Quantity);
                    var lstWarehousingBillProduct = warehousingBillProductGroupByProduct.Where(x => x.ProductId == item.ProductId && x.CreationTime < input.DateFrom).FirstOrDefault();

                    if (lstWarehousingBillProduct != null)
                    {
                        item.SLBegin = (wareImportBegin - wareExportBegin) > 0 ? (wareImportBegin - wareExportBegin) : 0;
                    }
                    item.RetailQuantity = retailQuantity - retailReturnQuantity.Value;
                    item.SpaQuantity = spaQuantity - spaReturnQuantity.Value;
                    item.AgencyQuantity = agencyQuantity - agencyReturnQuantity.Value;
                    item.ImportQuatity = sLImportPeriod - sLExportPeriod;
                    item.TotalSLBeginAndImportQuatity = (sLImportPeriod - sLExportPeriod) + item.SLBegin;
                    item.SaleQuantity = item.RetailQuantity + item.SpaQuantity + item.AgencyQuantity;
                    item.SLEnd = item.TotalSLBeginAndImportQuatity - item.SaleQuantity;
                    item.SaleRate = item.TotalSLBeginAndImportQuatity > 0 ? Math.Round((item.SaleQuantity / item.TotalSLBeginAndImportQuatity), 0) : 0;
                }
                return new GenericActionResult(retVal.Count(), retVal);
            }
            catch (Exception ex)
            {
                var errM = ex.Message;
                return new GenericActionResult(400, false, "Lỗi xảy ra", null);
            }
        }


        public async Task<List<BestSaleByProductReportDto>> SearchFilter(BestSaleByProductReportRequest input)
        {
            var warehousingBillProducts = await _warehousingBillProductRepository.GetQueryableAsync();
            var warehousingBillProductGroupByProduct = warehousingBillProducts.OrderByDescending(x => x.CreationTime);
            var warehousingBills = await _warehousingBillRepository.GetQueryableAsync();
            var billCustomerProducts = await _billCustomerProductRepository.GetQueryableAsync();
            var billCustomers = await _billCustomerRepository.GetQueryableAsync();
            var customers = await _customerRepository.GetQueryableAsync();
            var customerReturns = await _customerReturnRepository.GetQueryableAsync();
            var customerReturnProducts = await _customerReturnProductRepository.GetQueryableAsync();

            var error = Validate(input);
            if (!string.IsNullOrEmpty(error))
            {
                return new List<BestSaleByProductReportDto>();
            }
            var retVal = new List<BestSaleByProductReportDto>();
            var dataTableStoreId = new DataTable();
            dataTableStoreId.Columns.Add("StoreId", typeof(Guid));
            if (input.LstStoreId != null && input.LstStoreId.Count > 0)
            {
                foreach (var guidValue in input.LstStoreId)
                {
                    dataTableStoreId.Rows.Add(guidValue);
                }
            }

            var dataTableEnterpriseId = new DataTable();
            dataTableEnterpriseId.Columns.Add("EnterpriseId", typeof(Guid));
            if (input.LstEnterpriseId != null && input.LstEnterpriseId.Count > 0)
            {
                foreach (var guidValue in input.LstEnterpriseId)
                {
                    dataTableEnterpriseId.Rows.Add(guidValue);
                }
            }
            using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false))
            {
                var query = $"EXEC sp_report_best_sale_by_product  @DateFrom, @DateTo, @EnterpriseIds, @StoreIds";

                var dateFrm = new SqlParameter("@DateFrom", SqlDbType.DateTime);
                var dateTo = new SqlParameter("@DateTo", SqlDbType.DateTime);
                var epIds = new SqlParameter("@EnterpriseIds", SqlDbType.Structured);
                var sIds = new SqlParameter("@StoreIds", SqlDbType.Structured);


                dateFrm.Value = new DateTime(input.DateFrom.Value.Year, input.DateFrom.Value.Month, input.DateFrom.Value.Day, 0, 0, 0);
                dateTo.Value = new DateTime(input.DateTo.Value.Year, input.DateTo.Value.Month, input.DateTo.Value.Day, 23, 59, 59);


                epIds.TypeName = "dbo.GuidListEnterpriseId";
                epIds.Value = dataTableEnterpriseId;
                sIds.TypeName = "dbo.GuidListStoreId";
                sIds.Value = dataTableStoreId;

                //var data = await _entryRepository.GetDbContext().Database.ExecuteSqlRawAsync(query, listParam, cancellationToken);
                var connection = _productRepository.GetDbContext().Database.GetDbConnection();
                connection.Close();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "sp_report_best_sale_by_product";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(dateFrm);
                cmd.Parameters.Add(dateTo);
                cmd.Parameters.Add(epIds);
                cmd.Parameters.Add(sIds);
                connection.Open();

                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var val = new BestSaleByProductReportDto()
                        {
                            ProductName = reader["ProductName"] == null ? "" : reader["ProductName"].ToString(),
                            Code = reader["Code"] == null ? "" : reader["Code"].ToString(),
                            ProductId = reader["ProductId"] == null ? null : Guid.Parse(reader["ProductId"].ToString()),
                            StoreId = reader["StoreId"] == null ? null : Guid.Parse(reader["StoreId"].ToString()),
                            TenantId = reader["TenantId"] == null ? null : Guid.Parse(reader["TenantId"].ToString()),
                        };
                        retVal.Add(val);
                    }
                }
                connection.Close();
            }
            foreach (var item in retVal)
            {
                var query = from wbp in warehousingBillProductGroupByProduct
                            join wb in warehousingBills on wbp.WarehousingBillId equals wb.Id
                            where wbp.ProductId == item.ProductId
                            select new
                            {
                                Quantity = wbp.Quantity,
                                BillType = wb.BillType,
                                CreationTime = wbp.CreationTime,
                                Price = wbp.Price,
                                Code = wb.Code,
                                StoreId = wb.StoreId,
                                ProductId = wbp.ProductId,
                            };

                var querySale = from bc in billCustomers
                                join bcp in billCustomerProducts on bc.Id equals bcp.BillCustomerId
                                where bcp.ProductId == item.ProductId
                                select new
                                {
                                    Quantity = bcp.Quantity,
                                    CustomerType = bc.CustomerType,
                                    CreationTime = bcp.CreationTime,
                                    StoreId = bc.StoreId,
                                    Price = bcp.Price,
                                    BillCustomerId = bc.Id,
                                    ProductId = bcp.ProductId,
                                    TenantId = bcp.TenantId,
                                };

                var queryReturn = from c in customers
                                  join cr in customerReturns on c.Id equals cr.CustomerId
                                  join crp in customerReturnProducts on cr.Id equals crp.CustomerReturnId
                                  where crp.ProductId == item.ProductId
                                  select new
                                  {
                                      Quantity = crp.Quantity,
                                      CustomerType = c.CustomerType,
                                      CreationTime = crp.CreationTime,
                                      StoreId = cr.StoreId,
                                      Price = crp.Price,
                                      ProductId = crp.ProductId,
                                      TenantId = crp.TenantId,
                                  };
                var retailQuantity = querySale.Where(x => x.CustomerType == Enums.CustomerType.RetailCustomer && x.ProductId == item.ProductId
                && input.LstStoreId.Contains(x.StoreId.Value)).Sum(p => p.Quantity);
                var agencyQuantity = querySale.Where(x => x.CustomerType == Enums.CustomerType.Agency && x.ProductId == item.ProductId
                && input.LstStoreId.Contains(x.StoreId.Value)).Sum(p => p.Quantity);
                var spaQuantity = querySale.Where(x => x.CustomerType == Enums.CustomerType.SPACustomer && x.ProductId == item.ProductId
                && input.LstStoreId.Contains(x.StoreId.Value)).Sum(p => p.Quantity);

                var retailReturnQuantity = queryReturn.Where(x => x.CustomerType == Enums.CustomerType.RetailCustomer && x.ProductId == item.ProductId
               && input.LstStoreId.Contains(x.StoreId.Value)).Sum(p => p.Quantity);
                var agencyReturnQuantity = queryReturn.Where(x => x.CustomerType == Enums.CustomerType.Agency && x.ProductId == item.ProductId
                && input.LstStoreId.Contains(x.StoreId.Value)).Sum(p => p.Quantity);
                var spaReturnQuantity = queryReturn.Where(x => x.CustomerType == Enums.CustomerType.SPACustomer && x.ProductId == item.ProductId
                && input.LstStoreId.Contains(x.StoreId.Value)).Sum(p => p.Quantity);

                var wareImportBegin = query.Where(x => x.BillType == Enums.WarehousingBillType.Import && x.CreationTime.Date < input.DateFrom.Value.Date).Sum(x => x.Quantity);
                var wareExportBegin = query.Where(x => x.BillType == Enums.WarehousingBillType.Export && x.CreationTime.Date < input.DateFrom.Value.Date).Sum(x => x.Quantity);
                var sLImportPeriod = query.Where(x => x.BillType == Enums.WarehousingBillType.Import && x.CreationTime.Date >= input.DateFrom.Value.Date && x.CreationTime.Date <= input.DateTo.Value.Date &&
                input.LstStoreId.Contains(x.StoreId)).Sum(x => x.Quantity);
                var lstWarehousingBillProduct = warehousingBillProductGroupByProduct.Where(x => x.ProductId == item.ProductId && x.CreationTime < input.DateFrom).FirstOrDefault();

                if (lstWarehousingBillProduct != null)
                {
                    item.SLBegin = (wareImportBegin - wareExportBegin) > 0 ? (wareImportBegin - wareExportBegin) : 0;
                }
                item.RetailQuantity = retailQuantity - retailReturnQuantity.Value;
                item.SpaQuantity = spaQuantity - spaReturnQuantity.Value;
                item.AgencyQuantity = agencyQuantity - agencyReturnQuantity.Value;
                item.ImportQuatity = sLImportPeriod;
                item.TotalSLBeginAndImportQuatity = sLImportPeriod + item.SLBegin;
                item.SaleQuantity = item.RetailQuantity + item.SpaQuantity + item.AgencyQuantity;
                item.SLEnd = item.TotalSLBeginAndImportQuatity - item.SaleQuantity;
                item.SaleRate = item.TotalSLBeginAndImportQuatity > 0 ? Math.Round((item.SaleQuantity / item.TotalSLBeginAndImportQuatity), 0) : 0;
            }
            return retVal;
        }

        private string Validate(BestSaleByProductReportRequest input)
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

        public async Task<byte[]> ExportBestSaleByProductAsync(BestSaleByProductReportRequest request)
        {
            var data = await SearchFilter(request);

            var exportData = new List<ExportBestSaleByProductResponse>();

            if (data != null && data.Count > 0)
            {
                var result = data.ToList();
                foreach (var item in result)
                {
                    exportData.Add(new ExportBestSaleByProductResponse()
                    {
                        Code = item.Code,
                        ProductName = item.ProductName,
                        Price = item.Price,
                        SLBegin = item.SLBegin,
                        ImportQuatity = item.ImportQuatity,
                        TotalSLBeginAndImportQuatity = item.TotalSLBeginAndImportQuatity,
                        RetailQuantity = item.RetailQuantity,
                        AgencyQuantity = item.AgencyQuantity,
                        SpaQuantity = item.SpaQuantity,
                        SaleQuantity = item.SaleQuantity,
                        SLEnd = item.SLEnd,
                        SaleRate = item.SaleRate,
                    });
                }
            }
            return ExcelHelper.ExportExcel(exportData);
        }
    }
}
