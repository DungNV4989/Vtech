using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Enums.Bills
{
    public enum CustomerBillPayStatus
    {
        /// <summary>
        /// Khách đặt hàng
        /// </summary>
        CustomerOrder = 0,
        /// <summary>
        /// Chờ gọi hàng
        /// </summary>
        WaitCall = 1,
        /// <summary>
        /// Lên đơn
        /// </summary>
        Confirm = 2,
        /// <summary>
        /// Đã nhặt hàng
        /// </summary>
        Taked = 3,
        /// <summary>
        /// Đã kiểm xong
        /// </summary>
        Checked = 4,
        /// <summary>
        /// Đang giao hàng
        /// </summary>
        Delivery = 5,
        /// <summary>
        /// Thành công
        /// </summary>
        Success = 6,
        /// <summary>
        /// Hủy
        /// </summary>
        Cancel = 7
    }
}
