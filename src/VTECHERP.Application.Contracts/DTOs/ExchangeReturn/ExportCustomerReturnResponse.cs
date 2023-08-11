using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Domain.Shared.Helper.Excel.Attributes;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.ExchangeReturn
{
    public class ExportCustomerReturnResponse
    {
        [Header("Ngày tạo")]
        public string CreationTime { get; set; }
        [Header("ID")]
        public string Code { get; set; }
        [Header("NV nhận trả hàng")]
        public string HandlerEmployeeName { get; set; }
        [Header("NV bán hàng")]
        public string EmployeeSell { get; set; }
        [Header("Cửa hàng")]
        public string StoreName { get; set; }
        [Header("Tên khách hàng")]
        public string? CustomerName { get; set; }
        [Header("SĐT Khách hàng")]
        public string? CustomerPhone { get; set; }
        [Header("Địa chỉ Khách hàng")]
        public string CustomerAddress { get; set; }      
        [Header("Mã SP cha")]
        public string ParentProductCode { get; set; }
        [Header("Tên SP cha")]
        public string ParentProductName { get; set; }
        [Header("Mã sản phẩm")]
        public string ProductCode { get; set; }
        [Header("Tên sản phẩm")]
        public string ProductName { get; set; }
        [Header("Đơn vị tính")]
        public string Unit { get; set; }
        [Header("IMEI")]
        public string IMEI { get; set; }
        [Header("Giá trả")]
        public decimal ReturnPrice { get; set; }
        [Header("Giá vốn")]
        public decimal CostPrice { get; set; }
        [Header("Số lượng")]
        public decimal Quantity { get; set; }
        [Header("Chiết khấu")]
        public decimal DiscountValue { get; set; }
        [Header("Tiền trả")]
        public decimal ReturnMoney { get; set; }
        [Header("Trả lại")]
        public decimal ReturnAmount { get; set; }
        [Header("Phí")]
        public decimal Fee { get; set; }
        [Header("Tiền mặt")]
        public decimal Cash { get; set; }
        [Header("Tiền chuyển khoản")]
        public decimal Banking { get; set; }
        [Header("Tổng tiền")]
        public decimal AmountCustomerPay { get; set; }
        [Header("Ghi chú")]
        public string Note { get; set; }
        [Header("ID HĐBH đổi trả ngang")]
        public string Exchange { get; set; }
        [Header("Hãng vận chuyển")]
        public string Carrier { get; set; }
        [Header("Mã vận chuyển hãng")]
        public string CarrierShippingCode { get; set; }

    }
}
