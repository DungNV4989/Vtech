using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Attachment;
using VTECHERP.DTOs.BillCustomers.Params;
using VTECHERP.Enums.Bills;

namespace VTECHERP.DTOs.BillCustomers.Respons
{
    /// <summary>
    /// Dto cho màn hình sửa
    /// </summary>
    public class BillCustomerDetail : BillCustomerCreateParam
    {
        public Guid BillCustomerId { get; set; }
        /// <summary>
        /// Giới hạn công nợ
        /// </summary>
        public decimal? DebtLimit { get; set; }

        #region Thông tin voucher
        public Guid? VoucherId { get; set; }
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
        public decimal? VoucherMaxDiscountValue { get; set; }
        #endregion

        /// <summary>
        /// Link sản phẩm
        /// </summary>
        public List<DetailAttachmentDto> Attachments { get; set; }
    }
}
