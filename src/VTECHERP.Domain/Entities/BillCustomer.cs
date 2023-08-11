using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using VTECHERP.Enums;
using VTECHERP.Enums.Bills;

namespace VTECHERP.Entities
{
    public class BillCustomer : BaseEntity<Guid>, IMultiTenant
    {
        public string Code { get; set; }
        public Guid? CustomerId { get; set; }
        /// <summary>
        /// Loại khách
        /// </summary>
        public CustomerType CustomerType { get; set; }
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

        public string? CarrierShippingCode { get; set; }

        #endregion
        public string NoteForProductBonus { get; set; }
        public string ReasonCancel { get; set; }
        /// <summary>
        /// Số tiền mặt được triết khấu của hóa đơn
        /// </summary>
        public decimal DiscountCash { get; set; }

        #region Thông tin voucher
        public Guid? VoucherId { get; set; }
        public string VoucherCode { get; set; }
        public DiscountUnit VoucherDiscountUnit { get; set; }
        /// <summary>
        /// Số tiền triết khấu được cấu hình trong voucher
        /// </summary>
        public decimal VoucherDiscountValue { get; set; }
        /// <summary>
        /// Số tiền voucher trừ trong hóa đơn
        /// </summary>
        public decimal BillVoucherDiscountValue { get; set; }
        public string? VoucherApplyStoreIds { get; set; }
        public decimal? VoucherBillMinValue { get; set; }
        public decimal? VoucherBillMaxValue { get; set; }
        public ApplyFor? VoucherApplyFor { get; set; }
        public string? VoucherApplyProductCategoryIds { get; set; }
        public string? VoucherApplyProductIds { get; set; }
        public bool VoucherNotApplyWithDiscount { get; set; }
        /// <summary>
        /// Số tiền triết khấu tối đa cấu hình trong voucher
        /// </summary>
        public decimal? VoucherMaxDiscountValue { get; set; }
        #endregion
    }
}
