using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VTECHERP.DTOs.SaleOrderLines;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.SaleOrders
{
    public class GetDetailConfirmByIdResponse
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
        /// Tổng VND YC
        /// </summary>
        public decimal? TotalRequestVnd { get; set; }

        /// <summary>
        /// Tổng VND đã nhập
        /// </summary>
        public decimal? TotalInputVnd { get; set; }

        /// <summary>
        /// Số kiện
        /// </summary>
        public int PackageRes { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        public List<GetDetailConfirmProudctByIdResponse> DetailProudcts { get; set; }
    }

    public class GetDetailConfirmProudctByIdResponse
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Id SaleOrder
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// Id sản phẩm
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Id 
        /// </summary>
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        /// <summary>
        /// Số lượng yêu cầu
        /// </summary>
        public int? RequestQuantity { get; set; }

        /// <summary>
        /// Số lượng đã nhập
        /// </summary>
        public int? ImportQuantity { get; set; }

        /// <summary>
        /// Giá yêu cầu
        /// </summary>
        public decimal? RequestPrice { get; set; }

        /// <summary>
        /// Tổng VND YC
        /// </summary>
        public decimal? TotalRequestVnd { get; set; }

        /// <summary>
        /// Tổng VND đã nhập
        /// </summary>
        public decimal? TotalInputVnd { get; set; }

        /// <summary>
        /// Đơn giá VND
        /// </summary>
        public decimal? UnitPriceVnd { get; set; }

        /// <summary>
        /// Giá cước
        /// </summary>
        public decimal? RatePrice { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }



    }
}
