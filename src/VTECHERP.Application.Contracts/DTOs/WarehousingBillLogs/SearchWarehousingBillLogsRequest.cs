using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.WarehousingBillLogs
{
    public class SearchWarehousingBillLogsRequest : BasePagingRequest
    {
        /// <summary>
        /// Cửa hàng
        /// </summary>
        public List<Guid>? StoreIds { get; set; }
        /// <summary>
        /// Kiểu log
        /// </summary>
        public EntityActions? Action { get; set; }

        /// <summary>
        /// Id phiếu nhập/xuất kho
        /// </summary>
        public string? BillCode { get; set; }
        /// <summary>
        /// Loại phiếu nhập/xuất kho
        /// </summary>
        public WarehousingBillType? BillType { get; set; }
        /// <summary>
        /// Kiểu
        /// </summary>
        public List<DocumentDetailType>? DocumentDetailType { get; set; }
        /// <summary>
        /// Ngày sửa/xóa
        /// </summary>
        public DateTime? FromDate { get; set; }
        /// <summary>
        /// Ngày sửa/xóa
        /// </summary>
        public DateTime? ToDate { get; set; }
        /// <summary>
        /// Ngày phiếu
        /// </summary>
        public DateTime? FromDateBill { get; set; }
        /// <summary>
        /// Ngày phiếu
        /// </summary>
        public DateTime? ToDateBill { get; set; }
        /// <summary>
        /// Người sửa/xóa
        /// </summary>
        public string?  Creator { get; set; }
    }
}
