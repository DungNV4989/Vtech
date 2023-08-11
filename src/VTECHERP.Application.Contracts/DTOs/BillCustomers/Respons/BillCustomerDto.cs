using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums;
using VTECHERP.Enums.Bills;

namespace VTECHERP.DTOs.BillCustomers.Respons
{
    public class BillCustomerDto
    {
        public Guid? Id { get; set; }
        public string Code { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public string EmployeeNote { get; set; }

        /// <summary>
        /// Bảng giá áp dụng
        /// </summary>
        public Guid? TablePriceId { get; set; }

        /// <summary>
        /// Nhân viên chăm sóc
        /// </summary>
        public Guid? EmployeeCare { get; set; }
        public Guid? EmployeeSell { get; set; }

        #region Tổng tiền hàng
        public decimal? VatValue { get; set; }
        public MoneyModificationType? VatUnit { get; set; }
        public decimal? DiscountValue { get; set; }
        public MoneyModificationType? DiscountUnit { get; set; }
        public CustomerBillPayStatus? CustomerBillPayStatus { get; set; }
        /// <summary>
        /// Tiền mặt
        /// </summary>
        public decimal? Cash { get; set; }
        /// <summary>
        /// Chuyển khoản
        /// </summary>
        public decimal? Banking { get; set; }
        public decimal? Coupon { get; set; }
        public string PayNote { get; set; }
        public string AccountCode { get; set; }
        /// <summary>
        /// Tài khoản kế toán chuyển khoản
        /// </summary>
        public string AccountCodeBanking { get; set; }
        #endregion

        #region Giao hàng
        /// <summary>
        /// Hình thức vận chuyển
        /// </summary>
        public TransportForm TransportForm { get; set; }
        /// <summary>
        /// Ngày vận chuyển
        /// </summary>
        public DateTime? TransportDate { get; set; }
        public bool COD { get; set; }
        #endregion
    }
}
