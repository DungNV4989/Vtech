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
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.InventoryByProductReport;
using VTECHERP.DTOs.WarehousingBills;
using VTECHERP.Entities;
using VTECHERP.Helper;
using VTECHERP.Reports;
using VTECHERP.ServiceInterfaces;

namespace VTECHERP.Services.ReportService
{
    public class InventoryByProductReportService : IInventoryByProductReportService
    {
        private readonly IRepository<Products> _productRepository;
        private readonly IRepository<WarehousingBillProduct> _warehousingBillProductRepository;
        private readonly IRepository<WarehousingBill> _warehousingBillRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICommonService _commonService;
        public InventoryByProductReportService(IRepository<Products> productRepository, IUnitOfWorkManager unitOfWorkManager
            , ICommonService commonService
            , IRepository<WarehousingBillProduct> warehousingBillProductRepository
            , IRepository<WarehousingBill> warehousingBillRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _productRepository = productRepository;
            _warehousingBillProductRepository = warehousingBillProductRepository;
            _commonService = commonService;
            _warehousingBillRepository = warehousingBillRepository;
        }
        public async Task<IActionResult> SearchReportAsync(InventoryByProductReportRequest input, CancellationToken cancellationToken = default)
        {
            try
            {
                var warehousingBillProducts = await _warehousingBillProductRepository.GetQueryableAsync();
                var warehousingBillProductGroupByProduct = warehousingBillProducts.OrderByDescending(x => x.CreationTime);
                var warehousingBills = await _warehousingBillRepository.GetQueryableAsync();

                var error = Validate(input);
                if (!string.IsNullOrEmpty(error))
                {
                    return  null;
                }
                if (input.Period == 2)
                {
                    input.DateFrom = input.DateFrom.Value.AddDays((-input.DateFrom.Value.Day) + 1);
                    input.DateTo = input.DateTo.Value.AddMonths(1);
                    input.DateTo = input.DateTo.Value.AddDays(-(input.DateTo.Value.Day));
                }
                var retVal = new List<InventoryByProductReportDto>();
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
                    var query = $"EXEC sp_report_inventory_by_product  @DateFrom, @DateTo, @EnterpriseIds, @StoreIds";
                    //var query = $"EXEC sp_report_inventory_by_product @Period, @DateFrom, @DateTo, @EnterpriseIds, @StoreIds, @StockPrice, @CurrentInventory, @XNKArising";
                    
                    var dateFrm = new SqlParameter("@DateFrom", SqlDbType.DateTime);
                    var dateTo = new SqlParameter("@DateTo", SqlDbType.DateTime);
                    var epIds = new SqlParameter("@EnterpriseIds", SqlDbType.Structured);
                    var sIds = new SqlParameter("@StoreIds", SqlDbType.Structured);
                    //var period = new SqlParameter("@Period", SqlDbType.Int);
                    //var stockPrice = new SqlParameter("@StockPrice", SqlDbType.Int);
                    //var currentInventory = new SqlParameter("@CurrentInventory", SqlDbType.Int);
                    //var xNKArising = new SqlParameter("@XNKArising", SqlDbType.Int);

                    dateFrm.Value = new DateTime(input.DateFrom.Value.Year, input.DateFrom.Value.Month, input.DateFrom.Value.Day, 0, 0, 0);
                    dateTo.Value = new DateTime(input.DateTo.Value.Year, input.DateTo.Value.Month, input.DateTo.Value.Day, 23, 59, 59);
                    //period.Value = input.Period;
                    //stockPrice.Value = input.StockPrice;
                    //currentInventory.Value = input.CurrentInventory;
                    //xNKArising.Value = input.XNKArising;

                    epIds.TypeName = "dbo.GuidListEnterpriseId";
                    epIds.Value = dataTableEnterpriseId;
                    sIds.TypeName = "dbo.GuidListStoreId";
                    sIds.Value = dataTableStoreId;

                    //var data = await _entryRepository.GetDbContext().Database.ExecuteSqlRawAsync(query, listParam, cancellationToken);
                    var connection = _productRepository.GetDbContext().Database.GetDbConnection();
                    connection.Close();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = "sp_report_inventory_by_product";
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    cmd.Parameters.Add(dateFrm);
                    cmd.Parameters.Add(dateTo);
                    cmd.Parameters.Add(epIds);
                    cmd.Parameters.Add(sIds);
                    //cmd.Parameters.Add(period);
                    //cmd.Parameters.Add(stockPrice);
                    //cmd.Parameters.Add(currentInventory);
                    //cmd.Parameters.Add(xNKArising);
                    connection.Open();

                    var reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var val = new InventoryByProductReportDto()
                            {
                                ProductName = reader["ProductName"].ToString(),
                                Code = reader["Code"].ToString(),
                                BarCode = reader["BarCode"].ToString(),
                                TotalInventory = Convert.ToDecimal(reader["TotalInventory"].ToString()),
                                Inventory = Convert.ToDecimal(reader["Inventory"].ToString()),
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
                    var wareImportBegin = query.Where(x => x.BillType == Enums.WarehousingBillType.Import && x.CreationTime.Date < input.DateFrom.Value.Date &&
                    input.LstStoreId.Contains(x.StoreId)).Sum(x => x.Quantity);
                    var wareExportBegin = query.Where(x => x.BillType == Enums.WarehousingBillType.Export && x.CreationTime.Date < input.DateFrom.Value.Date &&
                    input.LstStoreId.Contains(x.StoreId)).Sum(x => x.Quantity);

                    var sLImportPeriod = query.Where(x => x.BillType == Enums.WarehousingBillType.Import && x.CreationTime.Date >= input.DateFrom.Value.Date && x.CreationTime.Date <= input.DateTo.Value.Date &&
                    input.LstStoreId.Contains(x.StoreId)).Sum(x => x.Quantity);
                    var sLExportPeriod = query.Where(x => x.BillType == Enums.WarehousingBillType.Export && x.CreationTime.Date >= input.DateFrom.Value.Date && x.CreationTime.Date <= input.DateTo.Value.Date &&
                    input.LstStoreId.Contains(x.StoreId)).Sum(x => x.Quantity);
                    var lstWarehousingBillProduct = warehousingBillProductGroupByProduct.Where(x => x.ProductId == item.ProductId && x.CreationTime < input.DateFrom).FirstOrDefault();
                   
                    if (lstWarehousingBillProduct != null)
                    {
                        item.StockPriceBegin = lstWarehousingBillProduct.Price;
                        item.SLBegin = (wareImportBegin - wareExportBegin) > 0 ? (wareImportBegin - wareExportBegin) : 0;
                        item.MoneyBegin = item.StockPriceBegin * item.SLBegin;
                    }
                    item.SLImportPeriod = sLImportPeriod;
                    item.SLExportPeriod = sLExportPeriod;
                    item.SLEnd = item.SLBegin + sLExportPeriod - sLExportPeriod;
                    if (input.StockPrice == Enums.StockPrice.PriceByPeriod)
                    {
                        var importPeriod = query.Where(x => x.BillType == Enums.WarehousingBillType.Import && x.CreationTime.Date >= input.DateFrom.Value.Date && x.CreationTime.Date <= input.DateTo.Value.Date);
                        decimal totalAverage = 0;
                        decimal average = 0;
                        foreach (var lst in importPeriod)
                        {
                            totalAverage = totalAverage + (lst.Quantity * lst.Price);
                        }

                        if (totalAverage == 0)
                        {
                            average = item.StockPriceBegin;
                        }
                        else 
                        {
                            average = Math.Round(totalAverage / sLImportPeriod, 2); 
                            item.StockPriceImportPeriod = average;
                            item.MoneyImportPeriod = item.StockPriceImportPeriod * item.SLImportPeriod;
                            item.StockPriceExportPeriod = average;
                            item.MoneyExportPeriod = item.StockPriceExportPeriod * item.SLExportPeriod;
                        }
                        item.MoneyEnd = item.SLEnd * average;
                    }
                    else
                    {
                        item.StockPriceImportPeriod = item.StockPriceBegin;
                        item.MoneyImportPeriod = item.StockPriceBegin * item.SLImportPeriod;
                        item.StockPriceExportPeriod = item.StockPriceBegin;
                        item.MoneyExportPeriod = item.StockPriceBegin * item.SLExportPeriod;
                        item.MoneyEnd = item.SLEnd * item.StockPriceBegin;
                    }
                }
                var result =   retVal.Skip(input.Offset).Take(input.PageSize).ToList();
                if (retVal != null && retVal.Count > 0)
                {
                    var rowTotal = new
                    {
                        TotalBegin = result.Sum(x => x.SLBegin),
                        PriceBegin = result.Sum(x => x.StockPriceBegin),
                        MoneyBegin = result.Sum(x => x.MoneyBegin),
                        TotalImportPeriod = result.Sum(x => x.SLImportPeriod),
                        PriceImportPeriod = result.Sum(x => x.StockPriceImportPeriod),
                        MoneyImportPeriod = result.Sum(x => x.MoneyImportPeriod),
                        TotalExportPeriod = result.Sum(x => x.SLExportPeriod),
                        PriceExportPeriod = result.Sum(x => x.StockPriceExportPeriod),
                        MoneyExportPeriod = result.Sum(x => x.MoneyExportPeriod),
                        TotalEnd = result.Sum(x => x.SLEnd),
                        PriceEnd = result.Sum(x => x.StockPriceEnd),
                        MoneyEnd = result.Sum(x => x.MoneyEnd)
                    };

                    return new OkObjectResult(new
                    {
                        data = result,
                        success = true,
                        rowTotal = rowTotal,
                        total = retVal.Count()
                    });
                }
                else
                {
                    var rowTotal = new
                    { };
                    return new OkObjectResult(new
                    {
                        data = result,
                        success = true,
                        rowTotal = rowTotal,
                        total = retVal.Count()
                    });
                }
                
                //return new GenericActionResult(retVal.Count(), response);
            }
            catch (Exception ex)
            {
                var errM = ex.Message;
                return new GenericActionResult(400, false, "Lỗi xảy ra", null);
            }
        }


        public async Task<List<InventoryByProductReportDto>> SearchFilter(InventoryByProductReportRequest input) 
        {
            var warehousingBillProducts = await _warehousingBillProductRepository.GetQueryableAsync();
            var warehousingBillProductGroupByProduct = warehousingBillProducts.OrderByDescending(x => x.CreationTime);
            var warehousingBills = await _warehousingBillRepository.GetQueryableAsync();

            var error = Validate(input);
            if (!string.IsNullOrEmpty(error))
            {
                return null;
            }
            if (input.Period == 2)
            {
                input.DateFrom = input.DateFrom.Value.AddDays((-input.DateFrom.Value.Day) + 1);
                input.DateTo = input.DateTo.Value.AddMonths(1);
                input.DateTo = input.DateTo.Value.AddDays(-(input.DateTo.Value.Day));
            }
            var retVal = new List<InventoryByProductReportDto>();
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
                var query = $"EXEC sp_report_inventory_by_product  @DateFrom, @DateTo, @EnterpriseIds, @StoreIds";
                //var query = $"EXEC sp_report_inventory_by_product @Period, @DateFrom, @DateTo, @EnterpriseIds, @StoreIds, @StockPrice, @CurrentInventory, @XNKArising";

                var dateFrm = new SqlParameter("@DateFrom", SqlDbType.DateTime);
                var dateTo = new SqlParameter("@DateTo", SqlDbType.DateTime);
                var epIds = new SqlParameter("@EnterpriseIds", SqlDbType.Structured);
                var sIds = new SqlParameter("@StoreIds", SqlDbType.Structured);
                //var period = new SqlParameter("@Period", SqlDbType.Int);
                //var stockPrice = new SqlParameter("@StockPrice", SqlDbType.Int);
                //var currentInventory = new SqlParameter("@CurrentInventory", SqlDbType.Int);
                //var xNKArising = new SqlParameter("@XNKArising", SqlDbType.Int);

                dateFrm.Value = new DateTime(input.DateFrom.Value.Year, input.DateFrom.Value.Month, input.DateFrom.Value.Day, 0, 0, 0);
                dateTo.Value = new DateTime(input.DateTo.Value.Year, input.DateTo.Value.Month, input.DateTo.Value.Day, 23, 59, 59);
                //period.Value = input.Period;
                //stockPrice.Value = input.StockPrice;
                //currentInventory.Value = input.CurrentInventory;
                //xNKArising.Value = input.XNKArising;

                epIds.TypeName = "dbo.GuidListEnterpriseId";
                epIds.Value = dataTableEnterpriseId;
                sIds.TypeName = "dbo.GuidListStoreId";
                sIds.Value = dataTableStoreId;

                //var data = await _entryRepository.GetDbContext().Database.ExecuteSqlRawAsync(query, listParam, cancellationToken);
                var connection = _productRepository.GetDbContext().Database.GetDbConnection();
                connection.Close();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "sp_report_inventory_by_product";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(dateFrm);
                cmd.Parameters.Add(dateTo);
                cmd.Parameters.Add(epIds);
                cmd.Parameters.Add(sIds);
                //cmd.Parameters.Add(period);
                //cmd.Parameters.Add(stockPrice);
                //cmd.Parameters.Add(currentInventory);
                //cmd.Parameters.Add(xNKArising);
                connection.Open();

                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var val = new InventoryByProductReportDto()
                        {
                            ProductName = reader["ProductName"].ToString(),
                            Code = reader["Code"].ToString(),
                            BarCode = reader["BarCode"].ToString(),
                            TotalInventory = Convert.ToDecimal(reader["TotalInventory"].ToString()),
                            Inventory = Convert.ToDecimal(reader["Inventory"].ToString()),
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
                var wareImportBegin = query.Where(x => x.BillType == Enums.WarehousingBillType.Import && x.CreationTime.Date < input.DateFrom.Value.Date).Sum(x => x.Quantity);
                var wareExportBegin = query.Where(x => x.BillType == Enums.WarehousingBillType.Export && x.CreationTime.Date < input.DateFrom.Value.Date).Sum(x => x.Quantity);

                var sLImportPeriod = query.Where(x => x.BillType == Enums.WarehousingBillType.Import && x.CreationTime.Date >= input.DateFrom.Value.Date && x.CreationTime.Date <= input.DateTo.Value.Date).Sum(x => x.Quantity);
                var sLExportPeriod = query.Where(x => x.BillType == Enums.WarehousingBillType.Export && x.CreationTime.Date >= input.DateFrom.Value.Date && x.CreationTime.Date <= input.DateTo.Value.Date).Sum(x => x.Quantity);
                var lstWarehousingBillProduct = warehousingBillProductGroupByProduct.Where(x => x.ProductId == item.ProductId && x.CreationTime < input.DateFrom).FirstOrDefault();

                if (lstWarehousingBillProduct != null)
                {
                    item.StockPriceBegin = lstWarehousingBillProduct.Price;
                    item.SLBegin = (wareImportBegin - wareExportBegin) > 0 ? (wareImportBegin - wareExportBegin) : 0;
                    item.MoneyBegin = item.StockPriceBegin * item.SLBegin;
                }
                item.SLImportPeriod = sLImportPeriod;
                item.SLExportPeriod = sLExportPeriod;
                item.SLEnd = item.SLBegin + sLExportPeriod - sLExportPeriod;
                if (input.StockPrice == Enums.StockPrice.PriceByPeriod)
                {
                    var importPeriod = query.Where(x => x.BillType == Enums.WarehousingBillType.Import && x.CreationTime.Date >= input.DateFrom.Value.Date && x.CreationTime.Date <= input.DateTo.Value.Date);
                    decimal totalAverage = 0;
                    decimal average = 0;
                    foreach (var lst in importPeriod)
                    {
                        totalAverage = totalAverage + (lst.Quantity * lst.Price);
                    }

                    if (totalAverage == 0)
                    {
                        average = item.StockPriceBegin;
                    }
                    else
                    {
                        average = Math.Round(totalAverage / sLImportPeriod, 2);
                        item.StockPriceImportPeriod = average;
                        item.MoneyImportPeriod = item.StockPriceImportPeriod * item.SLImportPeriod;
                        item.StockPriceExportPeriod = average;
                        item.MoneyExportPeriod = item.StockPriceExportPeriod * item.SLExportPeriod;
                    }
                    item.MoneyEnd = item.SLEnd * average;
                }
                else
                {
                    item.StockPriceImportPeriod = item.StockPriceBegin;
                    item.MoneyImportPeriod = item.StockPriceBegin * item.SLImportPeriod;
                    item.StockPriceExportPeriod = item.StockPriceBegin;
                    item.MoneyExportPeriod = item.StockPriceBegin * item.SLExportPeriod;
                    item.MoneyEnd = item.SLEnd * item.StockPriceBegin;
                }
            }
            return retVal;
        }

        private string Validate(InventoryByProductReportRequest input)
        {
            var error = "";
            if (input.LstEnterpriseId == null || input.LstEnterpriseId.Count == 0)
            {
                error = error +  "doanh nghiệp là bắt buộc,";
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

        public async Task<byte[]> ExportInventoryByProductAsync(InventoryByProductReportRequest request)
        {
            var data = await SearchFilter(request);
            var exportData = new List<ExportInventoryByProductResponse>();

            if (data != null && data.Count > 0)
            {
                foreach (var item in data)
                {
                    exportData.Add(new ExportInventoryByProductResponse()
                    {
                        Code = item.Code,
                        BarCode = item.BarCode,
                        ProductName = item.ProductName,
                        Inventory = item.Inventory,
                        TotalInventory = item.TotalInventory,
                        SLBegin = item.SLBegin,
                        StockPriceBegin = item.StockPriceBegin,
                        MoneyBegin = item.MoneyBegin,
                        SLImportPeriod = item.SLImportPeriod,
                        StockPriceImportPeriod = item.StockPriceImportPeriod,
                        MoneyImportPeriod = item.MoneyImportPeriod,
                        SLExportPeriod = item.SLExportPeriod,
                        StockPriceExportPeriod = item.StockPriceExportPeriod,
                        MoneyExportPeriod = item.MoneyExportPeriod,
                        SLEnd = item.SLEnd,
                        StockPriceEnd = item.StockPriceEnd,
                        MoneyEnd = item.MoneyEnd
                    });
                }
            }
            return ExcelHelper.ExportExcel(exportData);
        }
    }
}
