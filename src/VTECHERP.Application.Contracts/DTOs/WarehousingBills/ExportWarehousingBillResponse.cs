using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Domain.Shared.Helper.Excel.Attributes;

namespace VTECHERP.DTOs.WarehousingBills
{
    public class ExportWarehousingBillResponse
    {
        [Header("ID")]
        public string Code { get; set; }
        [Header("ID đơn hàng")]
        public string AudienceCode { get; set; }
        [Header("Cửa hàng")]
        public string StoreName { get; set; }
        [Header("ID phiếu bảo hành")]
        public string WarrantyCardCode { get; set; }
        [Header("Id phiếu kiểm kho")]
        public string InventorySheetCode { get; set; }
        [Header("Id phiếu nháp")]
        public string SourceCode { get; set; }
        [Header("Kiểu")]
        public string BillTypeName { get; set; }
        [Header("Nhà cung cấp")]
        public string SupplierName { get; set; }
        [Header("Số SP")]
        public int NumberOfProduct { get; set; }
        [Header("Tổng SL")]
        public int TotalProductAmount { get; set; }
        [Header("Tổng tiền")]
        public decimal TotalPrice { get; set; }
        [Header("Chiếu khấu ")]
        public decimal TotalDiscountAmount { get; set; }
        [Header("Người lập phiếu")]
        public string CreatorName { get; set; }
        [Header("Khách hàng")]
        public string AudienceName { get; set; }
        [Header("Ngày sinh")]
        public string DateOfBirth { get; set; }
        [Header("Điện thoại")]
        public string? AudiencePhone { get; set; }
        [Header("Ghi chú ")]
        public string Note { get; set; }
        [Header("Số hóa đơn VAT")]
        public string VATBillCode { get; set; }
        [Header("Ngày tạo")]
        public string CreationTime { get; set; }
    }
}
