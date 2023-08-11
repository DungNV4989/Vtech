using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.SaleOrderLines
{
    public class SaleOrderLineDto
    {
        public Guid Id { get; set; }
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
        public int? RequestQuantity { get; set; }

        /// <summary>
        /// Số lượng đã nhập
        /// </summary>
        public int? ImportQuantity { get; set; }

        /// <summary>
        /// Giá yêu cầu
        /// </summary>
        public decimal? RequestPrice { get; set; }

        /// <summary>
        /// Giá đề xuất
        /// </summary>
        public decimal? SuggestedPrice { get; set; }

        /// <summary>
        /// Tổng tiền tệ
        /// </summary>
        public decimal? TotalYuan { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
    }
}
