using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Attachment;
using VTECHERP.Enums;
using VTECHERP.Enums.Bills;

namespace VTECHERP.DTOs.TransportInformation
{
    public class TransportInformationResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string? FromStoreName { get; set; }
        public string ToStoreName { get; set; }
        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// Tên nhân viên giao hàng
        /// </summary>
        public string? ShipperName { get; set; }
        /// <summary>
        /// Số điện thoại khách hàng
        /// </summary>
        public string CustomerPhoneNumber { get; set; }
        /// <summary>
        /// Là chuyển kho
        /// </summary>
        public bool IsWarehouseTransfer { get; set; }
        /// <summary>
        /// Trạng thái giao vận
        /// </summary>
        public TransportStatus Status { get; set; }
        /// <summary>
        /// Tên đơn vị vận chuyển 
        /// </summary>
        public string TransportName { get; set; }
        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string TransportPhoneNumber { get; set; }
        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// Thời gian giao hàng
        /// </summary>
        public Guid? ShipperId { get; set; }
        /// <summary>
        /// Thời gian giao hàng
        /// </summary>
        public DateTime? ShipTime { get; set; }
        /// <summary>
        /// ghi chú 
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Có phải là phiểu chuyển kho không? True: Phải; false: Không;
        /// </summary>
        public bool IsIransferWarehouse { get; set; }

        /// <summary>
        /// Link sản phẩm
        /// </summary>
        public List<DetailAttachmentDto> Attachments { get; set; }
    }
}
