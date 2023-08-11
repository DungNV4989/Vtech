using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTECHERP.Entities
{
    /// <summary>
    /// Bảng giá - khách hàng
    /// </summary>
    [Table("PriceTableCustomer")]
    public class PriceTableCustomer : BaseEntity<Guid>
    {
        /// <summary>
        /// ID Bảng giá
        /// </summary>
        public Guid PriceTableId { get; set; }
        /// <summary>
        /// ID khách hàng
        /// </summary>
        public Guid CustomerId { get; set; }
    }
}
