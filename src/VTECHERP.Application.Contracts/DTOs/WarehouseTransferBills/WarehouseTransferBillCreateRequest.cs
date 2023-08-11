using System;
using System.Collections.Generic;
using VTECHERP.DTOs.WarehouseTransferBillProducts;

namespace VTECHERP.DTOs.WarehouseTransferBills
{
    public class WarehouseTransferBillCreateRequest
    {
        /// <summary>
        /// Id cửa hàng/kho nguồn
        /// </summary>
        public Guid SourceStoreId { get; set; }

        /// <summary>
        /// Id cửa hàng/kho đích
        /// </summary>
        public Guid DestinationStoreId { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Danh sách sản phẩm của phiếu nháp
        /// </summary>
        public IList<WarehouseTransferBillProductCreateRequest> WarehouseTransferBillProducts { get; set; }
    }
}