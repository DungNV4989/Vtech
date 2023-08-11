using System;
using System.Collections.Generic;
using VTECHERP.DTOs.Attachment;
using VTECHERP.DTOs.DraftTicketProducts;
using VTECHERP.Enums.DraftTicket;
using VTECHERP.Enums.WarehouseTransferBill;

namespace VTECHERP.DTOs.DraftTickets
{
    public class DraftTicketDetailDto
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

        /// <summary>
        /// Trạng thái chuyển kho: Nháp/Đang vận chuyển/Đã vận chuyển
        /// </summary>
        public TransferStatuses TransferStatus { get; set; }

        public Status Status { get; set; }

        public List<DraftTicketProductDetailDto> DraftTicketProducts { get; set; }
    }
}