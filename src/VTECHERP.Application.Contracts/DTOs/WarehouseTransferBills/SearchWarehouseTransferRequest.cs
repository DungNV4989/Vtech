using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;
using VTECHERP.Enums.WarehouseTransferBill;

namespace VTECHERP.DTOs.WarehouseTransferBills
{
    public class SearchWarehouseTransferRequest : BasePagingRequest
    {
        /// <summary>
        /// Danh sách cửa hàng chuyển kho: / Từ kho
        /// </summary>
        public List<Guid> ToStoreIds { get; set; }

        /// <summary>
        /// Danh sách cửa hàng chuyển kho: / Đến kho
        /// </summary>
        public List<Guid> FromStoreIds { get; set; }

        /// <summary>
        /// Id phiếu nhập / Mã phiếu nhập
        /// </summary>
        public string WarehouseTransferCode { get; set; }

        /// <summary>
        /// Loại phiếu nhập/xuất kho
        /// </summary>
        public TransferBillType? TransferBillType { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

    }
}
