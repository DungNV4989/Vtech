using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VTECHERP.Entities
{
    /// <summary>
    /// DS Sản phẩm bảng giá
    /// </summary>
    [Table("PriceTableProducts")]
    public class PriceTableProduct: BaseEntity<Guid>
    {
        /// <summary>
        /// ID Bảng giá
        /// </summary>
        public Guid PriceTableId { get; set; }
        /// <summary>
        /// ID Sản phẩm
        /// </summary>
        public Guid ProductId { get; set; }
        /// <summary>
        /// Giá SP trong bảng giá
        /// </summary>
        public decimal Price { get; set; }
    }
}
