using System;
using System.Collections.Generic;
using VTECHERP.DTOs.Attachment;
using VTECHERP.DTOs.WarehouseTransferBillProducts;

namespace VTECHERP.DTOs.WarehouseTransferBills
{
    public class WarehouseTransferBillApproveDto
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Id cửa hàng/kho nguồn
        /// </summary>
        public Guid SourceStoreId { get; set; }

        /// <summary>
        /// Tên cửa hàng/kho nguồn
        /// </summary>
        public string SourceStoreName { get; set; }

        /// <summary>
        /// Id cửa hàng/kho đích
        /// </summary>
        public Guid DestinationStoreId { get; set; }

        /// <summary>
        /// Tên cửa hàng/kho đích
        /// </summary>
        public string DestinationStoreName { get; set; }

        public List<AttachmentShortDto> Attachments { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        public List<WarehouseTransferBillProductApproveDto> WarehouseTransferBillProducts { get; set; }
    }
}