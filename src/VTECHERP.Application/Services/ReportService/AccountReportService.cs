using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VTECHERP.Entities;
using VTECHERP.Reports;
using VTECHERP.ServiceInterfaces;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VTECHERP.Helper;
using Vinpearl.Modelling.Library.Utility.Excel;
using VTECHERP.DTOs.Stores;
using Microsoft.Extensions.Configuration;

namespace VTECHERP.Services.ReportService
{
    public class AccountReportService: IAccountReportService
    {
        private readonly IRepository<Entry> _entryRepository;
        private readonly IConfiguration _configuration;
        public AccountReportService(IRepository<Entry> entryRepository, IConfiguration configuration)
        {
            _entryRepository = entryRepository;
            _configuration = configuration;
        }
        // tà áo năm xưa còn đây nhưng em đâu rồi người ơi
        // đò sang bên sông có nhớ đến bến này đợi mong
        // người nói đi câu vội chia ly ai nỡ sao đành
        // vậy mà ngày ai đã nói em chờ đợi anh?
        public IActionResult SearchAccountReportAsync(SearchAccountReport input, CancellationToken cancellationToken = default)
        {
            try
            {
                var dataTableEnterpriseId = new DataTable();
                dataTableEnterpriseId.Columns.Add("EnterpriseId", typeof(Guid));
                if (input.LstEnterpriseId != null && input.LstEnterpriseId.Count > 0)
                {
                    foreach (var guidValue in input.LstEnterpriseId)
                    {
                        dataTableEnterpriseId.Rows.Add(guidValue);
                    }
                }
                var epIds = new SqlParameter("@EnterpriseIds", SqlDbType.Structured);
                epIds.TypeName = "dbo.GuidListEnterpriseId";
                epIds.Value = dataTableEnterpriseId;
                var dateFrm = new SqlParameter("@fromDate", SqlDbType.DateTime);
                var status = new SqlParameter("@Status", SqlDbType.Bit);
                status.Value = input.IsActive ?? null;
                var dateTo = new SqlParameter("@toDate", SqlDbType.DateTime);
                var code = new SqlParameter("@Code", SqlDbType.NVarChar);
                code.Value = input.Code??"";
                var name = new SqlParameter("@Name", SqlDbType.NVarChar);
                name.Value = input.Name??"";
                if (input.DateFrom == null)
                {
                    return new GenericActionResult(400, false, "Từ ngày bắt buộc phải nhập", null);
                }
                if (input.DateTo == null)
                {
                    return new GenericActionResult(400, false, "Đến ngày bắt buộc phải nhập", null);
                }
                else
                {
                    var date = input.DateFrom.Value.AddHours(-10);
                    dateFrm.Value = new DateTime(input.DateFrom.Value.Year, input.DateFrom.Value.Month, input.DateFrom.Value.Day, 0, 0, 0); ;
                }
                if (input.DateTo == null)
                {
                    dateTo.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                }
                else
                {
                    dateTo.Value = new DateTime(input.DateTo.Value.Year, input.DateTo.Value.Month, input.DateTo.Value.Day, 23, 59, 59);
                }
                var connection = _entryRepository.GetDbContext().Database.GetDbConnection();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "AccountReport";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(epIds);
                cmd.Parameters.Add(status);
                cmd.Parameters.Add(dateFrm);
                cmd.Parameters.Add(dateTo);
                cmd.Parameters.Add(code);
                cmd.Parameters.Add(name);
                connection.Open();

                var reader = cmd.ExecuteReader();


                var retVal = new List<AccountReportDto>();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var val = new AccountReportDto()
                        {
                            Code = reader["Code"].ToString(),
                            Name = reader["Name"].ToString(),
                            ParentAccountCode= reader["ParentAccountCode"].ToString(),
                            TenantId = Guid.Parse(reader["TenantId"].ToString()),
                            Lvl = Int32.Parse(reader["Lvl"].ToString()),
                            EnterpriseName = reader["EnterpriseName"].ToString(),
                            BeforeCreditAmount = Convert.ToDecimal(reader["BeforeCreditAmount"].ToString()),
                            BeforeDebitAmount = Convert.ToDecimal(reader["BeforeDebitAmount"].ToString()),
                            CreditAmount = Convert.ToDecimal(reader["CreditAmount"].ToString()),
                            DebitAmount = Convert.ToDecimal(reader["DebitAmount"].ToString()),
                            AfterCreditAmount = Convert.ToDecimal(reader["AfterCreditAmount"].ToString()),
                            AfterDebitAmount = decimal.Parse(reader["AfterDebitAmount"].ToString()),
                        };
                        retVal.Add(val);
                    }
                }
                connection.Close();

                if (retVal.Count > 0)
                {
                    return new GenericActionResult(200, true, "Success", retVal);
                }
                else
                {
                    return new GenericActionResult(200, true, "No Data", null);
                }

            }
            catch (Exception ex)
            {
                var errM = ex.Message;
                return new GenericActionResult(400, false, "Lỗi xảy ra", null);
            }

        }
        class AccountReportDto
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public Guid TenantId { get; set; }
            public string EnterpriseName { get; set; }
            public string ParentAccountCode { get; set; }
            public bool IsActive { get; set; }
            public int Lvl { get; set; }
            public decimal BeforeCreditAmount { get; set; }
            public decimal BeforeDebitAmount { get; set; }
            public decimal CreditAmount { get; set; }
            public decimal DebitAmount { get; set; }
            public decimal AfterCreditAmount { get; set; }
            public decimal AfterDebitAmount { get; set; }
        }
    }
}
