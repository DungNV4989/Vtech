using System;

namespace VTECHERP.DTOs.Product
{
    public class MapStockQuantityDto
    {
        /// <summary>
        /// Id Sản phẩm
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Mã vạch
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// Tên sản phẩm
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Số lượng tồn
        /// </summary>
        public int StockQuantity { get; set; } = 0;
    }
}