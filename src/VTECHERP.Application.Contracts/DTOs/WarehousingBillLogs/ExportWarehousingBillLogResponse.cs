using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Domain.Shared.Helper.Excel.Attributes;

namespace VTECHERP.DTOs.WarehousingBillLogs
{
    public class ExportWarehousingBillLogResponse
    {
        [Header("ID phiếu XNK")]
        public string Code { get; set; }
        [Header("ID sản phẩm XNK")]
        public string ProductCode { get; set; }
        [Header("Kiểu log")]
        public string ActionName { get; set; }
        [Header("Loại XNK")]
        public string BillTypeName { get; set; }
        [Header("Kiểu XNK")]
        public string DocumentDetailTypeName { get; set; }
        [Header("Sản phẩm")]
        public string ProductName { get; set; }
        [Header("Số lượng")]
        public decimal? CurrentStockQuantity { get; set; }
        [Header("Giá")]
        public decimal? TotalPriceBeforeTax { get; set; }
        [Header("Người thao tác")]
        public string UserAction { get; set; }
        [Header("Thời gian tạo")]
        public string CreationTime { get; set; }
    }
}
