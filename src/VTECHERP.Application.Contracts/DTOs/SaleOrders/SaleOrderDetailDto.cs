using System;
using System.Collections.Generic;
using VTECHERP.DTOs.Attachment;
using VTECHERP.DTOs.SaleOrderLines;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.SaleOrders
{
    public class SaleOrderDetailDto
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Mã đơn hàng (Id)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Id cửa hàng
        /// </summary>
        public Guid StoreId { get; set; }

        /// <summary>
        /// Tên cửa hàng
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// Id nhà cung cấp
        /// </summary>
        public Guid SupplierId { get; set; }

        /// <summary>
        /// Tên nhà cung cấp
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// Số hóa đơn
        /// </summary>
        public string InvoiceNumber { get; set; }

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
        /// Tổng tiền
        /// </summary>
        public decimal? TotalPrice { get; set; }

        /// <summary>
        /// Id người tạo
        /// </summary>
        public Guid? CreatorId { get; set; }

        /// <summary>
        /// Tỉ giá NDT - VND
        /// </summary>
        public double? Rate { get; set; }

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
        /// Số kiện
        /// </summary>
        public int PackageRes { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        public List<SaleOrderLineDetailDto> SaleOrderLineDetailDtos { get; set; }

        /// <summary>
        /// Link sản phẩm
        /// </summary>
        public List<DetailAttachmentDto> Attachments { get; set; }
    }
}