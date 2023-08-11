using System;
using System.Collections.Generic;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.WarehousingBills
{
    public class CreateWarehousingBillRequest
    {
        /// <summary>
        /// Loại phiếu nhập/xuất kho
        /// </summary>
        public WarehousingBillType BillType { get; set; }
        /// <summary>
        /// Id cửa hàng/kho
        /// </summary>
        public Guid StoreId { get; set; }
        /// <summary>
        /// Loại đối tượng
        /// </summary>
        public AudienceTypes AudienceType { get; set; }
        /// <summary>
        /// Id đối tượng
        /// </summary>
        public Guid? AudienceId { get; set; }
        /// <summary>
        /// Kiểu
        /// </summary>
        public DocumentDetailType DocumentDetailType { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Note { get; set; }
        /// <summary>
        /// Loại VAT: %/VND
        /// </summary>
        public MoneyModificationType? VATType { get; set; }
        /// <summary>
        /// Số tiền VAT
        /// </summary>
        public decimal? VATAmount { get; set; }
        /// <summary>
        /// Số hóa đơn VAT
        /// </summary>
        public string? VATBillCode { get; set; }
        /// <summary>
        /// Ngày xuất VAT
        /// </summary>
        public DateTime? VATBillDate { get; set; }
        /// <summary>
        /// Loại chiết khấu (Thanh toán): %/VND
        /// </summary>
        public MoneyModificationType? BillDiscountType { get; set; }
        /// <summary>
        /// Số tiền chiết khấu
        /// </summary>
        public decimal? BillDiscountAmount { get; set; }
        /// <summary>
        /// Mã TK tiền mặt
        /// </summary>
        public string? CashPaymentAccountCode { get; set; }
        /// <summary>
        /// Số tiền mặt
        /// </summary>
        public decimal? CashPaymentAmount { get; set; }
        /// <summary>
        /// Mã TK tiền chuyển khoản
        /// </summary>
        public string? BankPaymentAccountCode { get; set; }
        /// <summary>
        /// Số tiền chuyển khoản
        /// </summary>
        public decimal? BankPaymentAmount { get; set; }
        /// <summary>
        /// Id nguồn dữ liệu tạo phiếu XNK (phiếu chuyển kho, xác nhận đơn hàng)
        /// </summary>
        public Guid? SourceId { get; set; }
        /// <summary>
        /// Phiếu XNK được tạo từ phiếu chuyển kho
        /// </summary>
        public bool? IsFromWarehouseTransfer { get; set; }=false;
        /// <summary>
        /// Phiếu XNK được tạo từ xác nhận đơn hàng nhà cung cấp TQ
        /// </summary>
        public bool? IsFromOrderConfirmation { get; set; } = false;
        public bool? IsFromBillCustomer { get; set; } = false;
        public bool? IsFromCustomerReturn { get; set; } = false;
        /// <summary>
        /// DS sản phẩm
        /// </summary>
        public List<WarehousingBillProductRequest> Products { get; set; }
    }
}
