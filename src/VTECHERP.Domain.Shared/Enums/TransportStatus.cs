using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Enums
{
    public enum TransportStatus
    {
        /// <summary>
        /// Mới
        /// </summary>
        New = 0,
        /// <summary>
        /// Chờ giao hàng
        /// </summary>
        WaitingDelivery = 1,
        /// <summary>
        /// Đang giao
        /// </summary>
        Delivering = 2,
        /// <summary>
        /// Đã chuyển kho 
        /// </summary>
        Moved = 3,
        /// <summary>
        /// Xác nhận chuyển kho
        /// </summary>
        Confirm = 4,
        /// <summary>
        /// Thành công
        /// </summary>
        Done = 5,
        /// <summary>
        /// Hủy
        /// </summary>
        Cancel = 6
    }
}
