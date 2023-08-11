using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.CustomerReturnImport.Params
{
    public class CustomerReturnImportParam
    {
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        /// <summary>
        /// Nhân viên chăm sóc
        /// </summary>
        public Guid? EmployeeCare { get; set; }
        public Guid? EmployeeSell { get; set; }
        public ExchangeEnum IsExchange { get; set; }
        public Guid? BillCustomerId { get; set; }
        public Guid? BillCustomerCode { get; set; }
        public decimal? ReturnAmount { get; set; }
        public string PayNote { get; set; }
        public decimal? DiscountValue { get; set; } = 0;
        public MoneyModificationType? DiscountUnit { get; set; }
        /// <summary>
        /// Tài khoản kế toán tiền mặt
        /// </summary>
        public string AccountCode { get; set; }
        /// <summary>
        /// Tiền mặt
        /// </summary>
        public decimal? Cash { get; set; }
        /// <summary>
        /// Tài khoản kế toán chuyển khoản
        /// </summary>
        public string AccountCodeBanking { get; set; }
        /// <summary>
        /// Chuyển khoản
        /// </summary>
        public decimal? Banking { get; set; }
        public IFormFile File { get; set; }
    }
}
