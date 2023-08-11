using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Attachment;
using VTECHERP.Enums;
using VTECHERP.Enums.Bills;

namespace VTECHERP.DTOs.TransportInformation
{
    public class TransportHistoryInformationResponse
    {
        /// <summary>
        /// Id lịch sử vận đơn
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Mã lịch sử vận đơn
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Ngày (Thời gian tạo)
        /// </summary>
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// Cửa hàng (Tên cửa hàng tạo đơn)
        /// </summary>
        public string FromStoreName { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Nhà vận chuyển
        /// </summary>
        public string TransportName { get; set; }

        /// <summary>
        /// Chuyển kho (Cửa hàng được chuyển đến)
        /// </summary>
        public string ToStoreName { get; set; }

        /// <summary>
        /// Có phải là chuyển kho không (True:có/False: không)
        /// </summary>
        public bool IsWarehouseTransfer { get; set; }


        /// <summary>
        /// Trạng thái chuyển kho
        /// </summary>
        public TransportStatus? TransferStatus { get; set; }

        /// <summary>
        /// Id người giao hàng
        /// </summary>
        public Guid? ShipperId { get; set; }

        /// <summary>
        /// Người giao hàng
        /// </summary>
        public string? ShipperName { get; set; }

        /// <summary>
        /// Số Km
        /// </summary>
        public decimal? Distance { get; set; }

        /// <summary>
        /// Trạng thái vận chuyển
        /// </summary>
        public TransportStatus? TransportStatus { get; set; }

        /// <summary>
        /// ghi chú 
        /// </summary>
        public string Note { get; set; }


        /// <summary>
        /// Link sản phẩm
        /// </summary>
        public List<DetailAttachmentDto> Attachments { get; set; }

    }
}
