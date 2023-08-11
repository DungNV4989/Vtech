using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.Product
{
    public class ProductDetailInventory
    {
        public Guid StoreId { get; set; }
        public string StoreName { get; set; }
        /// <summary>
        /// Tồn
        /// </summary>
        public decimal Inventory { get; set; }
        /// <summary>
        /// Đang giao hàng
        /// </summary>
        public decimal Delivery { get; set; }
        /// <summary>
        /// Đã xuất chuyển kho
        /// </summary>
        public decimal Exported { get; set; }
        /// <summary>
        /// Chờ nhập chuyển kho
        /// </summary>
        public decimal WaitImport { get; set; }
        /// <summary>
        /// Có thể bán
        /// </summary>
        public decimal AbleSell { get; set; }
        /// <summary>
        /// Đang về
        /// </summary>
        public decimal Comming { get; set; }
        /// <summary>
        /// Tạm giữ
        /// </summary>
        public decimal Hold { get; set; }
        /// <summary>
        /// Đặt trước
        /// </summary>
        public decimal Ordered { get; set; }
    }
}
