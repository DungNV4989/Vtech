using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VTECHERP.DTOs.SaleOrders
{
    public class SaleOrderConfirmRequest
    {
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Số hóa đơn
        /// </summary>
        public string? InvoiceNumber { get; set; }

        /// <summary>
        /// Số kiện
        /// </summary>
        public int? Package { get; set; } = 0;

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Note { get; set; }

        [Required]
        public List<SaleOrderProductCompleteRequest> ProductDeatails { get; set; }
    }

    public class SaleOrderProductCompleteRequest
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Số lượng đã nhập
        /// </summary>
        [Required]
        public int? Quantity { get; set; }

        /// <summary>
        /// Giá cước
        /// </summary>
        [Required]
        public decimal? RatePrice { get; set; }

        public string Note { get; set; }

    }
}
