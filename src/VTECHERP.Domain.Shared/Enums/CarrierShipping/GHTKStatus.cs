using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Enums.CarrierShipping
{
    public enum GHTKStatus
    {
        /// <summary>
        /// Hủy đơn hàng
        /// </summary>
        CANCEL = -1,
        /// <summary>
        /// Chưa tiếp nhận
        /// </summary>
        NOT_RECEIVED = 1,
        /// <summary>
        /// Đã tiếp nhận
        /// </summary>
        RECEIVED = 2,
        /// <summary>
        /// Đã lấy hàng/Đã nhập kho
        /// </summary>
        PICKED_UP = 3,
        /// <summary>
        /// Đã điều phối giao hàng/Đang giao hàng
        /// </summary>
        DELIVERING = 4,
        /// <summary>
        /// Đã giao hàng/Chưa đối soát
        /// </summary>
        DELIVERED_NOT_CHECKED = 5,
        /// <summary>
        /// Đã đối soát
        /// </summary>
        CHECKED = 6,
        /// <summary>
        /// Không lấy được hàng
        /// </summary>
        NOT_PICKED_UP = 7,
        /// <summary>
        /// Hoãn lấy hàng
        /// </summary>
        DELAY_PICK_UP = 8,
        /// <summary>
        /// Không giao được hàng
        /// </summary>
        NOT_DELIVERY = 9,
        /// <summary>
        /// Delay giao hàng
        /// </summary>
        DELAY_DELIVERY = 10,
        /// <summary>
        /// Đã đối soát công nợ trả hàng
        /// </summary>
        CHECKED_RETURN = 11,
        /// <summary>
        /// Đã điều phối lấy hàng/Đang lấy hàng
        /// </summary>
        PICKING_UP = 12,
        /// <summary>
        /// Đơn hàng bồi hoàn
        /// </summary>
        REIMBURSEMENT = 13,
        /// <summary>
        /// Đang trả hàng (COD cầm hàng đi trả)
        /// </summary>
        RETURNING = 20,
        /// <summary>
        /// Đã trả hàng (COD đã trả xong hàng)
        /// </summary>
        RETURNED = 21,
        /// <summary>
        /// Shipper báo đã lấy hàng
        /// </summary>
        SHIPPER_PICKED = 123,
        /// <summary>
        /// Shipper (nhân viên lấy/giao hàng) báo không lấy được hàng
        /// </summary>
        SHIPPER_NOT_PICK = 127,
        /// <summary>
        /// Shipper báo delay lấy hàng
        /// </summary>
        SHIPPER_DELAY_PICK = 128,
        /// <summary>
        /// Shipper báo đã giao hàng
        /// </summary>
        SHIPPER_COMPLETE = 45,
        /// <summary>
        /// Shipper báo không giao được giao hàng
        /// </summary>
        SHIPPER_NOT_COMPLETE = 49,
        /// <summary>
        /// Shipper báo delay giao hàng
        /// </summary>
        SHIPPER_DELAY_DELIVERY = 410,
    }
}
