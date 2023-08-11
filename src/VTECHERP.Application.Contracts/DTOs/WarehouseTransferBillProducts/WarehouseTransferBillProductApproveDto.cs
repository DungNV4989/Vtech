using System;

namespace VTECHERP.DTOs.WarehouseTransferBillProducts
{
    public class WarehouseTransferBillProductApproveDto
    {
        public Guid Id { get; set; }

        public Guid WarehouseTransferBillId { get; set; }

        /// <summary>
        /// Mã sản phẩm
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Mã vạch sản phẩm
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// Tên sản phẩm
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Có thể chuyển
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// Số lượng yêu cầu
        /// </summary>
        public int RequestQuantity { get; set; }

        /// <summary>
        /// Số lượng duyệt
        /// </summary>
        public int ApproveQuantity { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
    }
}