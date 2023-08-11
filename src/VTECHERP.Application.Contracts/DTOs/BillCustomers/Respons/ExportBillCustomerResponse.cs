using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Domain.Shared.Helper.Excel.Attributes;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.BillCustomers.Respons
{
    public class ExportBillCustomerResponse
    {
        [Header("Ngày tạo")]
        public string CreationTime { get; set; }
        [Header("ID")]
        public string Code { get; set; }
        [Header("NV chăm sóc")]
        public string EmployeeCare { get; set; }
        [Header("NV phụ trách")]
        public string HandlerEmployeeName { get; set; }
        [Header("NV bán hàng")]
        public string EmployeeSell { get; set; }
        [Header("Cửa hàng")]
        public string StoreName { get; set; }
        [Header("Tên khách hàng")]
        public string? CustomerName { get; set; }
        [Header("Ngày sinh")]
        public string DateOfBirth { get; set; }
        [Header("SĐT Khách hàng")]
        public string? CustomerPhone { get; set; }
        [Header("Địa chỉ Khách hàng")]
        public string  CustomerAddress{ get; set; }
        [Header("Mã vạch")]
        public string BarCode { get; set; }
        [Header("Mã sản phẩm")]
        public string ProductCode { get; set; }
        [Header("Mã SP cha")]
        public string ParentProductCode { get; set; }
        [Header("Tên SP cha")]
        public string ParentProductName { get; set; }
        [Header("Tên sản phẩm")]
        public string ProductName { get; set; }
        [Header("Quà tặng kèm")]
        public string ProductBonusCode { get; set; }
        [Header("Giá vốn quà tặng")]
        public decimal CostPriceBonus { get; set; }
        [Header("Đơn vị tính")]
        public string Unit { get; set; }
        [Header("IMEI")]
        public string IMEI { get; set; }
        [Header("Giá bán")]
        public decimal SalePrice { get; set; }
        [Header("Giá vốn")]
        public decimal CostPrice { get; set; }
        [Header("Số lượng")]
        public decimal Quantity { get; set; }
        [Header("VAT")]
        public decimal VAT { get; set; }
        [Header("Doanh thu SP sau chiết khấu")]
        public decimal AfterDiscount { get; set; }
        [Header("Chiết khấu")]
        public decimal DiscountValue { get; set; }
        [Header("Tiền mặt")]
        public decimal Cash { get; set; }
        [Header("Tài khoản chuyển khoản")]
        public string AccountCodeBanking { get; set; }
        [Header("Tổng tiền")]
        public decimal AmountCustomerPay { get; set; }
        [Header("Ghi chú")]
        public string Note { get; set; }
        [Header("Lợi nhuận")]
        public decimal Profit { get; set; }
        [Header("Voucher")]
        public string VoucherCode { get; set; }
        [Header("Danh mục sản phẩm")]
        public string  ProductCategoryName { get; set; }
        [Header("Còn nợ")]
        public decimal StillDebt { get; set; }
        [Header("ID Hóa đơn trả hàng")]
        public string RefundBillCode { get; set; }
        [Header("Bảng giá")]
        public string TablePriceId { get; set; }
        [Header("Hãng vận chuyển")]
        public Carrier? Carrier { get; set; }
        [Header("Mã vận chuyển hãng")]
        public string CarrierShippingCode { get; set; }
        [Header("Số KM ( vận chuyển nội bộ)")]
        public decimal Distance { get; set; }
    }
}
