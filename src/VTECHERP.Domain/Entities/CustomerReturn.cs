using System;
using Volo.Abp.MultiTenancy;
using VTECHERP.Enums;

namespace VTECHERP.Entities
{
    public class CustomerReturn : BaseEntity<Guid>, IMultiTenant
    {
        public string Code { get; set; }
        public string? CustomerName { get; set; }

        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        /// <summary>
        /// Nhân viên chăm sóc
        /// </summary>
        public Guid? EmployeeCare { get; set; }
        public string? EmployeeCareName { get; set; }
        public Guid? EmployeeSell { get; set; }
        public string? EmployeeSellName { get; set; }
        public ExchangeEnum IsExchange { get; set; }
        public Guid? BillCustomerId { get; set; }
        public string BillCustomerCode { get; set; }
        public decimal? ReturnAmount { get; set; }
        public string PayNote { get; set; }
        public decimal? DiscountValue { get; set; }
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
        public decimal? TotalAmount { get; set; }
        public string? CreatorName { get; set; }
        public int isConfirmed { get; set; }
        public DateTime? DateConfirm { get; set; }
        public decimal? TotalDiscount { get; set; }
    }
}
