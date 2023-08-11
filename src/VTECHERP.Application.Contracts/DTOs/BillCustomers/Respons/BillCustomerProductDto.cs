using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums.Product;

namespace VTECHERP.DTOs.BillCustomers.Respons
{
    public class BillCustomerProductDto
    {
        /// <summary>
        /// Id của bản ghi
        /// </summary>
        public Guid BillCustomerProductId { get; set; }
        /// <summary>
        /// Id sản phẩm
        /// </summary>
        public Guid ProductId { get; set; }
        /// <summary>
        /// Tên sản phẩm
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// Giá 
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Số lượng
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// Đơn vị
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// Id danh mục sản phẩm
        /// </summary>
        public Guid? ProductCategory { get; set; }
        /// <summary>
        /// Giá vốn
        /// </summary>
        public decimal? CostPrice { get; set; }
    }
}
