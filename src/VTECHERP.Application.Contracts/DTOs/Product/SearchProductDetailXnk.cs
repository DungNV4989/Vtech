using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.Product
{
    public class SearchProductDetailXnk
    {
        /// <summary>
        /// Cửa hàng
        /// </summary>
        public List<Guid>? StoreIds { get; set; }
        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public Guid? SupplierIds { get; set; }
        /// <summary>
        /// Id long 
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Sản phẩm: Tên or Mã
        /// </summary>
        public string? ProductName { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// Id phiếu xnk | Id
        /// </summary>
        public string? BillId { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        //public List<Guid>? CustomerIds { get; set; }
        public Guid? CustomerIds { get; set; }
        /// <summary>
        /// Loại phiếu nhập/xuất kho
        /// </summary>
        public WarehousingBillType? BillType { get; set; }

        /// <summary>
        /// Kiểu phiếu
        /// </summary>
        public List<DocumentDetailType>? XnkTypes { get; set; }

        /// <summary>
        /// ID đơn hàng
        /// </summary>
        public string? OrderCode { get; set; }

        /// <summary>
        /// ID phiếu bảo hành
        /// </summary>
        public string? WarrantyCardCode { get; set; }

        /// <summary>
        /// ID phiếu kiểm kho
        /// </summary>
        public string? InventorySheetCode { get; set; }

        /// <summary>
        /// Id phiếu nháp
        /// </summary>
        public string? DraftTransferBillCode { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Note { get; set; }
        public int PageSize { get; set; } = 10;
        public int PageIndex { get; set; } = 1;
    }
}
