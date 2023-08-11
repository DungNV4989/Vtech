using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Domain.Shared.Helper.Excel.Attributes;

namespace VTECHERP.DTOs.WarehousingBills
{
    public class ExportProductXnkResponse
    {
        [Header("ID")]
        public string Code { get; set; }
        [Header("Ngày")]
        public string CreationTime { get; set; }
        [Header("Cửa hàng")]
        public string StoreName { get; set; }
        [Header("Mã sản phẩm")]
        public string ProductCode { get; set; }
        [Header("Sản phẩm")]
        public string ProductName { get; set; }
        [Header("Đơn vị tính")]
        public string Unit { get; set; }
        [Header("Loại phiếu")]
        public string BillTypeName { get; set; }
        [Header("Số lượng")]
        public int Quantity { get; set; }
        [Header("Tồn")]
        public int Inventory { get; set; }
        [Header("Giá")]
        public decimal Price { get; set; }
        [Header("Giá vốn")]
        public decimal CostPrice { get; set; }
        [Header("Tiền")]
        public decimal Money { get; set; }
        [Header("Tổng tiền")]
        public decimal TotalPrice { get; set; }
        [Header("Chiếu khấu")]
        public decimal DiscountAmount { get; set; }
        [Header("Người tạo")]
        public string CreatorName { get; set; }
        [Header("Ghi chú")]
        public string Note { get; set; }
    }
}
