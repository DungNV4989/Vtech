using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTECHERP.Entities
{
    /// <summary>
    /// Bảng giá - cửa hàng
    /// </summary>
    [Table("PriceTableStore")]
    public class PriceTableStore : BaseEntity<Guid>
    {
        /// <summary>
        /// ID Bảng giá
        /// </summary>
        public Guid PriceTableId { get; set; }
        /// <summary>
        /// ID cửa hàng
        /// </summary>
        public Guid StoreId { get; set; }
    }
}
