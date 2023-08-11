using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.SaleOrders
{
    public class SaleOrderCommonDto
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Mã đơn hàng (Id)
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Id nhà cung cấp
        /// </summary>
        public Guid? SupplierId { get; set; }
        /// <summary>
        /// Tên nhà cung cấp
        /// </summary>
        public string SupplierName { get; set; }
        /// <summary>
        /// Số hóa đơn
        /// </summary>
        public string InvoiceNumber { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
