using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Domain.Shared.Helper.Excel.Attributes;

namespace VTECHERP.DTOs.Product
{
    public class ExportProductResponse
    {
        [Header("ID")]
        public string SequenceId { get; set; }
        [Header("ID Sản phẩm cha")]
        public string ParentID { get; set; }
        [Header("Mã Sản phẩm cha")]
        public string ParentCode { get; set; }
        [Header("Tên Sản phẩm cha")]
        public string ParentName { get; set; }
        [Header("Mã vạch")]
        public string BarCode { get; set; }
        [Header("Doanh nghiệp")]
        public string Enterprise { get; set; }
        [Header("Mã sản phẩm")]
        public string Code { get; set; }
        [Header("Tên sản phẩm")]
        public string Name { get; set; }
        [Header("Tên khác")]
        public string OtherName { get; set; }
        [Header("Đơn vị tính")]
        public string Unit { get; set; }
        [Header("Giá vốn")]
        public decimal StockPrice { get; set; }
        [Header("Giá nhập")]
        public decimal EntryPrice { get; set; }
        [Header("Giá bán")]
        public decimal SalePrice { get; set; }
        [Header("%Lãi giá bán")]
        public decimal? SalePricePercent { get; set; }
        [Header("Giá sỉ")]
        public decimal WholeSalePrice { get; set; }
        [Header("% Lãi giá sỉ")]
        public decimal? WholeSalePricePercent { get; set; }
        [Header("Giá spa")]
        public decimal SpaPrice { get; set; }
        [Header("% Lãi giá spa")]
        public decimal? SpaPricePercent { get; set; }
        [Header("Giá cước ")]
        public decimal? RatePrice { get; set; }
        [Header("Tồn")]
        public decimal? Inventory { get; set; }
        [Header("Tổng tồn")]
        public decimal? TotalInventory { get; set; }
        [Header("Lỗi")]
        public string Description { get; set; }
        [Header("Đang giao hàng")]
        public decimal? Delivery { get; set; }
        [Header("Tạm giữ")]
        public decimal? TemporarilyHold { get; set; }
        [Header("Có thể bán")]
        public decimal? SellNumber { get; set; }
        [Header("Đang về")]
        public decimal? Coming { get; set; }
        [Header("Đặt trước")]
        public decimal? Booking { get; set; }
        [Header("Trạng thái ")]
        public string Status { get; set; }
        [Header("Mã danh mục")]
        public string CategoryCode { get; set; }
        [Header("Danh mục")]
        public string CategoryName { get; set; }
        [Header("Nhà cung cấp")]
        public string SupplierName { get; set; }
        [Header("Ngày tạo")]
        public string CreationTime { get; set; }
    }
}
