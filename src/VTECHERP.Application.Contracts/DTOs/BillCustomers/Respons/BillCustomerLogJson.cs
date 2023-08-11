using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.BillCustomers.Params;
using VTECHERP.Enums;
using VTECHERP.Enums.Bills;

namespace VTECHERP.DTOs.BillCustomers.Respons
{
    public class BillCustomerLogJson
    {
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Guid? ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerNote { get; set; }
        public Guid? EmployeeCare { get; set; }
        public string EmployeeCareText { get; set; }
        public Guid? EmployeeSuport { get; set; }
        public string EmployeeSuportText { get; set; }

        public decimal? VatValue { get; set; }
        public MoneyModificationType? VatUnit { get; set; }
        public decimal? DiscountValue { get; set; }
        public MoneyModificationType? DiscountUnit { get; set; }

        public CustomerBillPayStatus? BillCustomerStatus { get; set; }
        public string BillCustomerStatusText { get; set; }
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
        /// <summary>
        /// Tài khoản kế toán tiền mặt
        /// </summary>
        public string AccountCode { get; set; }
        /// <summary>
        /// Tài khoản kế toán chuyển khoản
        /// </summary>
        public string AccountCodeBanking { get; set; }
        /// <summary>
        /// Tổng tiền sau triết khấu
        /// </summary>
        public decimal? AmountAfterDiscount { get; set; }
        /// <summary>
        /// Tổng tiền khách cần trả
        /// </summary>
        public decimal? AmountCustomerPay { get; set; }

        #region Giao hàng
        /// <summary>
        /// Hình thức vận chuyển
        /// </summary>
        public TransportForm TransportForm { get; set; }
        public string TransportFormText { get; set; }
        /// <summary>
        /// Ngày vận chuyển
        /// </summary>
        public DateTime? TransportDate { get; set; }
        public bool COD { get; set; }
        #endregion
        public string NoteForProductBonus { get; set; }
        public string ReasonCancel { get; set; }
        public List<BillCustomerProductItem> Products { get; set; }
    }
}
