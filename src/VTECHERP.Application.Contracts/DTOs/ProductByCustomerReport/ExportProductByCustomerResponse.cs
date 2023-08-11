using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Domain.Shared.Helper.Excel.Attributes;

namespace VTECHERP.DTOs.ProductByCustomerReport
{
    public class ExportProductByCustomerResponse
    {
        [Header("Khách hàng")]
        public string CustomerName { get; set; }
        [Header("DN")]
        public string TenantName { get; set; }
        [Header("NVPT")]
        public string HandlerEmployeeName { get; set; }
        [Header("NVCS")]
        public string EmployeeCareName { get; set; }
        [Header("Mã sản phẩm")]
        public string ProductCode { get; set; }
        [Header("Sản phẩm")]
        public string ProductName { get; set; }
        [Header("Danh mục")]
        public string CategoryName { get; set; }
        [Header("Giá bán ")]
        public decimal SalePrice { get; set; }
        [Header("SL bán ")]
        public decimal SaleQuantity { get; set; }
        [Header("SL trả ")]
        public decimal ReturnQuantity { get; set; }
        [Header("Chiết khấu ")]
        public decimal DiscountValue { get; set; }
        [Header("Doanh thu ")]
        public decimal AmountAfterDiscount { get; set; }
        [Header("Giá vốn")]
        public decimal StockPrice { get; set; }
        [Header("Lợi nhuận ")]
        public decimal Profit { get; set; }
        [Header("Ngày mua cuối ")]
        public DateTime LastBuyDate { get; set; }
    }
}
