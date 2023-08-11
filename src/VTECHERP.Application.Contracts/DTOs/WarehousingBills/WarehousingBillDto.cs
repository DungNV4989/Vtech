using System;
using System.Collections.Generic;
using VTECHERP.DTOs.Attachment;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.WarehousingBills
{
    public class WarehousingBillDto: BaseDTO
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
        /// Loại phiếu nhập/xuất kho
        /// </summary>
        public string? BillTypeName { get; set; }
        /// <summary>
        /// Id cửa hàng/kho
        /// </summary>
        public Guid StoreId { get; set; }
        /// <summary>
        /// Tên cửa hàng
        /// </summary>
        public string StoreName { get; set; }

        public Guid? SourceId { get; set; }
        /// <summary>
        /// Loại đối tượng
        /// </summary>
        public AudienceTypes AudienceType { get; set; }
        /// <summary>
        /// Id NCC nếu có
        /// </summary>
        public Guid? AudienceId { get; set; }
        /// <summary>
        /// Mã NCC
        /// </summary>
        public string? AudienceCode { get; set; }
        /// <summary>
        /// Tên NCC
        /// </summary>
        public string? AudienceName { get; set; }
        /// <summary>
        /// SĐT NCC
        /// </summary>
        public string? AudiencePhone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        /// <summary>
        /// Kiểu chứng từ chi tiết
        /// </summary>
        public DocumentDetailType DocumentDetailType { get; set; }
        /// <summary>
        /// giá trị text Kiểu chứng từ chi tiết
        /// </summary>
        public string DocumentDetailTypeName { get; set; }
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
        /// Loại chiết khấu (Thanh toán): %/VND
        /// </summary>
        public MoneyModificationType? BillDiscountType { get; set; }
        /// <summary>
        /// Số tiền chiết khấu (trong mục thanh toán)
        /// </summary>
        public decimal? BillDiscountAmount { get; set; }
        /// <summary>
        /// Tổng số tiền chiết khấu (trong mục thanh toán hoặc danh sách sản phẩm)
        /// </summary>
        public decimal? TotalDiscountAmount { get; set; }
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
        /// Tổng tiền sản phẩm
        /// </summary>
        public decimal TotalPriceProduct { get; set; }
        /// <summary>
        /// Tổng tiền trước thuế
        /// </summary>
        public decimal TotalPriceBeforeTax { get; set; }
        /// <summary>
        /// Tổng tiền cuối cùng
        /// </summary>
        public decimal TotalPrice { get; set; }
        /// <summary>
        /// Số sản phẩm
        /// </summary>
        public int NumberOfProduct { get; set; }
        /// <summary>
        /// Tổng số lượng sản phẩm
        /// </summary>
        public int TotalProductAmount { get; set; }
        /// <summary>
        /// Phiếu XNK được tạo từ xác nhận đơn hàng
        /// </summary>
        public bool? IsFromOrderConfirmation { get; set; } = false;
        /// <summary>
        /// Phiếu XNK được tạo từ phiếu chuyển kho
        /// </summary>
        public bool? IsFromWarehouseTransfer { get; set; } = false;
        /// <summary>
        /// DS SP
        /// </summary>
        public List<WarehousingBillProductDto> Products { get; set; }
        /// <summary>
        /// Mã TK tiền quẹt thẻ
        /// </summary>
        public string? CardPaymentAccountCode { get; set; }
        /// <summary>
        /// Id tham chiếu đến nhân viên bán hàng
        /// </summary>
        public Guid? SalerId { get; set; }
        public bool IsPaymentModuleHidden { get
            {
                if(VATType != null && VATAmount > 0)
                {
                    return false;
                }
                if (!VATBillCode.IsNullOrWhiteSpace())
                {
                    return false;
                }
                if(VATBillDate != null)
                {
                    return false;
                }
                if (CashPaymentAccountCode != null && CashPaymentAmount > 0)
                {
                    return false;
                }
                if (BankPaymentAccountCode != null && BankPaymentAmount > 0)
                {
                    return false;
                }
                if(BillDiscountType != null && BillDiscountAmount > 0)
                {
                    return false;
                }
                return true;
            } 
        }
        /// <summary>
        /// Link sản phẩm
        /// </summary>
        public List<DetailAttachmentDto> Attachments { get; set; }
    }
}
