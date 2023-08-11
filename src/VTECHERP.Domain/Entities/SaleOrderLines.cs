using System;
using Volo.Abp.MultiTenancy;

namespace VTECHERP.Entities
{
    /// <summary>
    /// Sản phẩm trong đơn đăth hàng từ nhà cung cấp
    /// </summary>
    public class SaleOrderLines : BaseEntity<Guid>, IMultiTenant
    {
        public string Code { get; set; }

        /// <summary>
        /// Id SaleOrder
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// Id sản phẩm
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Số lượng yêu cầu
        /// </summary>
        public int? RequestQuantity { get; set; } = 0;

        /// <summary>
        /// Số lượng đã nhập
        /// </summary>
        public int? ImportQuantity { get; set; } = 0;

        /// <summary>
        /// Giá yêu cầu
        /// </summary>
        public decimal? RequestPrice { get; set; } = 0;

        /// <summary>
        /// Giá đề xuất
        /// </summary>
        public decimal? SuggestedPrice { get; set; } = 0;

        /// <summary>
        /// Tổng tiền tệ
        /// </summary>
        public decimal? TotalYuan { get; set; } = 0;

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
    }
}