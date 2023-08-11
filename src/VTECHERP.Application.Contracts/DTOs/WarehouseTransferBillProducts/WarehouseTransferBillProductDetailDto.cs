using System;

namespace VTECHERP.DTOs.WarehouseTransferBillProducts
{
    public class WarehouseTransferBillProductDetailDto
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
        /// Số lượng
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Số lượng có thể chuyển tại thời điểm tạo mới/chỉnh sửa
        /// </summary>
        public int CanTransfer { get; set; }
    }
}