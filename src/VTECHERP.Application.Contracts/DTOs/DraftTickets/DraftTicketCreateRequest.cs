using System;
using System.Collections.Generic;
using VTECHERP.DTOs.DraftTicketProducts;

namespace VTECHERP.DTOs.DraftTickets
{
    public class DraftTicketCreateRequest
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
        public IList<DraftTicketProductCreateRequest> DraftTicketProducts { get; set; }
    }
}