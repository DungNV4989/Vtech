using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Attachment;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.SaleOrders
{
    public class SaleOrderDTO : SaleOrderCommonDto
    {
        /// <summary>
        /// Id cửa hàng
        /// </summary>
        public Guid? StoreId { get; set; }
        /// <summary>
        /// Tên cửa hàng
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// Ngày đặt hàng
        /// </summary>
        public DateTime? OrderDate { get; set; }
        /// <summary>
        /// Số lượng số sản phẩm
        /// </summary>
        public int? TotalProduct { get; set; }
        /// <summary>
        /// Tổng số lượng sản phẩm
        /// </summary>
        public int? TotalQuantity { get; set; }
        /// <summary>
        /// Tổng tiền Tệ
        /// </summary>
        //public double? Rate { get; set; }
        public decimal? TotalPriceNDT { get; set; }
        
        /// <summary>
        /// Id người tạo
        /// </summary>
        public Guid? CreatorId { get; set; }
        /// <summary>
        /// Tỉ giá NDT - VND
        /// </summary>
        public decimal? Rate { get; set; }
        /// <summary>
        /// Tên người tạo
        /// </summary>
        public string CreatorName { get; set; }
        /// <summary>
        /// Trạng thái duyệt
        /// </summary>
        public SaleOrder.Status Status { get; set; }
        /// <summary>
        /// Trạng thái xác nhận
        /// </summary>
        public bool IsConfirm { get; set; }
        /// <summary>
        /// Tổng số dượng duyệt
        /// </summary>
        public int? TotalApprove { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
        public DateTime? CreationTime { get; set; }
        public decimal? TotalPrice { get; set; }
        /// <summary>
        /// Nợ đến đơn
        /// </summary>
        public decimal DebtBeforeCNY { get; set; }
        /// <summary>
        /// Nợ sau đơn
        /// </summary>
        public decimal DebtFinalCNY { get; set; }
        /// <summary>
        /// Đang vận chuyển (đến đơn)
        /// </summary>
        public decimal DeliveryBeforeCNY { get; set; }
        /// <summary>
        /// Đang vận chuyển (cộng sau đơn)
        /// </summary>
        public decimal DeliveryFinalCNY { get; set; }
        /// <summary>
        /// Đã xác nhận(đến đơn)
        /// </summary>
        public decimal ConfirmedBeforeCNY { get; set; }
        /// <summary>
        /// Đã xác nhận (sau đơn)
        /// </summary>
        public decimal ConfirmedFinalCNY { get; set; }
        /// <summary>
        /// Đã thanh toán (đến đơn)
        /// </summary>
        public decimal PaidCNY { get; set; }

        /// <summary>
        /// Link sản phẩm
        /// </summary>
        public List<DetailAttachmentDto> Attachments { get; set; }
    }
}
