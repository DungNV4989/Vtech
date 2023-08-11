﻿using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Attachment;
using VTECHERP.Enums;
using VTECHERP.Enums.Bills;

namespace VTECHERP.DTOs.TransportInformation
{
    public class TransportInformationDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        /// <summary>
        /// Vận chuyển từ cửa hàng
        /// </summary>
        public Guid? FromStoreId { get; set; }
        public string? FromStoreName { get; set; }
        /// <summary>
        /// Vận chuyển đến cửa hàng
        /// </summary>
        public string? ToStoreId { get; set; }
        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid? CustomerId { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// Là chuyển kho
        /// </summary>
        public bool IsWarehouseTransfer { get; set; }
        /// <summary>
        /// có COD 
        /// </summary>
        public bool IsCOD { get; set; }
        /// <summary>
        /// có phải là tạo tự động 
        /// </summary>
        public bool IsAuto { get; set; }
        /// <summary>
        /// Tổng tiền
        /// </summary>
        public decimal? TotalAmount { get; set; }
        /// <summary>
        /// Hình thức vận chuyển
        /// </summary>
        public TransportForm TransportForm { get; set; }
        /// <summary>
        /// Trạng thái giao vận
        /// </summary>
        public TransportStatus Status { get; set; }
        /// <summary>
        /// Id đơn vị vận chuyển
        /// </summary>
        public Guid? TransportId { get; set; }
        /// <summary>
        /// Tên đơn vị vận chuyển 
        /// </summary>
        public string TransportName { get; set; }
        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string TransportPhoneNumber { get; set; }
        /// <summary>
        /// Phương thức giao vận
        /// </summary>
        public Carrier CarrierWay { get; set; }

        /// <summary>
        /// Link sản phẩm
        /// </summary>
        public List<DetailAttachmentDto> Attachments { get; set; }
        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// Thời gian giao hàng
        /// </summary>
        public DateTime? ShipTime { get; set; }
        /// <summary>
        /// ghi chú 
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// Số KM
        /// </summary>
        public decimal? Distance { get; set; }
        /// <summary>
        /// Danh sách hóa đơn đính kèm
        /// </summary>
        public List<BillDTO>? ListBillDTO{ get; set; }
    }
}
