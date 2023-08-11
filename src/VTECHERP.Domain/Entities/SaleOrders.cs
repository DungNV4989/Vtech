using System;
using Volo.Abp.MultiTenancy;
using VTECHERP.Enums;

namespace VTECHERP.Entities
{
    /// <summary>
    /// Đơn đặt hàng từ nhà cung cấp
    /// </summary>
    public class SaleOrders : BaseEntity<Guid>, IMultiTenant
    {
        /// <summary>
        /// Mã đơn hàng
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Id cửa hàng
        /// </summary>
        public Guid StoreId { get; set; }

        /// <summary>
        /// Id NCC
        /// </summary>
        public Guid SupplierId { get; set; }

        /// <summary>
        /// Số hóa đơn
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Ngày đặt hàng
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Tỉ giá NDT - VND
        /// </summary>
        public decimal? Rate { get; set; } = 1;

        /// <summary>
        /// Trạng thái
        /// </summary>
        public SaleOrder.Status Status { get; set; }
        public SaleOrder.ConpleteType? ConpleteType { get; set; }


        /// <summary>
        /// Xác nhận
        /// </summary>
        public SaleOrder.Confirm? Confirm { get; set; }

        public DateTime? ConfirmDate { get; set; }

        public Guid? ConfirmBy { get; set; }

        public DateTime? CompleteDate { get; set; }

        public Guid? CompleteBy { get; set; }

        /// <summary>
        /// Số kiện
        /// </summary>
        public int Package { get; set; } = 0;

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// Tổng số lượng được duyệt
        /// </summary>
        public int? TotalApprove { get; set; }
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
    }
}