using System;
using Volo.Abp.MultiTenancy;

namespace VTECHERP.Entities
{

    /// <summary>
    /// Chi tiết các lần confirm 
    /// </summary>
    public class SaleOrderLineConfirm : BaseEntity<Guid>, IMultiTenant
    {
        public Guid SupplierId { get; set; }
        public Guid? SaleOrderId { get; set; }
        public Guid? SaleOrderLineId { get; set; }

        /// <summary>
        /// Số lượng đã nhập
        /// </summary>
        public int? Quantity { get; set; }
        /// <summary>
        /// Tổng số đã nhập trên đơn
        /// </summary>
        public int UpdatedImportedQuantity { get; set; }
        /// <summary>
        /// Số yêu cầu trên đơn
        /// </summary>
        public int RequestQuantity { get; set; }
        /// <summary>
        /// Số lượng nhập thừa
        /// </summary>
        public int SurplusQuantity { get; set; }
        /// <summary>
        /// Xác nhận thừa không
        /// </summary>
        public bool IsSurplus { get; set; }
        /// <summary>
        /// Giá nhập
        /// </summary>
        public decimal? EntryPrice { get; set; }
        /// <summary>
        /// Giá cước
        /// </summary>
        public decimal? RatePrice { get; set; }

        public string Note { get; set; }
        public DateTime ConfirmDate { get; set; }
    }
}
