using System;
using System.ComponentModel.DataAnnotations.Schema;
using VTECHERP.Enums;

namespace VTECHERP.Entities
{
    /// <summary>
    /// Phiếu Xuất nhập kho
    /// </summary>
    [Table("WarehousingBills")]
    public class WarehousingBill : BaseEntity<Guid>
    {
        /// <summary>
        /// Mã phiếu XNK: tự sinh format 10 số
        /// </summary>
        public string Code { get; set; }

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
        /// Id đối tượng nếu có
        /// </summary>
        public Guid? AudienceId { get; set; }

        /// <summary>
        /// Kiểu chứng từ chi tiết
        /// </summary>
        public DocumentDetailType DocumentDetailType { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

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
        /// Ngày nhập VAT > 0
        /// </summary>
        public DateTime? VATHaveValueDate { get; set; }

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
        /// Ngày nhập số tiền thanh toán tiền mặt > 0
        /// </summary>
        public DateTime? CashPaymentHaveValueDate { get; set; }

        /// <summary>
        /// Mã TK tiền chuyển khoản
        /// </summary>
        public string? BankPaymentAccountCode { get; set; }

        /// <summary>
        /// Số tiền chuyển khoản
        /// </summary>
        public decimal? BankPaymentAmount { get; set; }

        /// <summary>
        /// Ngày nhập số tiền thanh toán chuyển khoản > 0
        /// </summary>
        public DateTime? BankPaymentHaveValueDate { get; set; }

        /// <summary>
        /// Tổng tiền sản phẩm
        /// </summary>
        public decimal TotalPriceProduct { get; set; }

        /// <summary>
        /// Tổng giá vốn của đơn
        /// </summary>
        public decimal TotalStockPrice { get; set; }

        /// <summary>
        /// Tổng tiền trước thuế
        /// </summary>
        public decimal TotalPriceBeforeTax { get; set; }

        /// <summary>
        /// Tổng tiền cuối cùng
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Phiếu XNK được tạo từ xác nhận đơn hàng
        /// </summary>
        public bool? IsFromOrderConfirmation { get; set; } = false;

        /// <summary>
        /// Phiếu XNK được tạo từ phiếu chuyển kho
        /// </summary>
        public bool? IsFromWarehouseTransfer { get; set; } = false;

        /// <summary>
        /// Phiếu XNK được tạo từ phiếu bán hàng.
        /// </summary>
        public bool? IsFromBillCustomer { get; set; } = false;

        /// <summary>
        /// Phiếu XNK được tạo từ phiếu trả hàng.
        /// </summary>
        public bool? IsFromCustomerReturn { get; set; } = false;

        /// <summary>
        /// Id tham chiếu đến nguồn tạo phiếu
        /// </summary>
        public Guid? SourceId { get; set; }

        /// <summary>
        /// Mã TK tiền quẹt thẻ
        /// </summary>
        public string? CardPaymentAccountCode { get; set; }

        /// <summary>
        /// Id tham chiếu đến nhân viên bán hàng
        /// </summary>
        public Guid? SalerId { get; set; }
    }
}