using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Enums.Product
{
    public enum ProductInventoryStatus
    {
        /// <summary>
        /// Đang giao hàng > 0 
        /// </summary>
        DeliveryInProgressGt0,
        /// <summary>
        /// Đang giao hàng <= 0 
        /// </summary>
        DeliveryInProgressLte0,
        /// <summary>
        /// Đang về > 0
        /// </summary>
        ComingGt0,
        /// <summary>
        /// Đang về <= 0 
        /// </summary>
        ComingLte0,
        /// <summary>
        /// Tạm giữ > 0 
        /// </summary>
        OnHoldGt0,
        /// <summary>
        /// Tạm giữ <= 0 
        /// </summary>
        OnHoldLte0,
        /// <summary>
        ///Có thể bán > 0 
        /// </summary>
        OnSaleGt0,
        /// <summary>
        /// Có thể bán <= 0 
        /// </summary>
        OnSaleLte0,
        /// <summary>
        /// Đặt trước > 0 
        /// </summary>
        PreOrderGt0,
        /// <summary>
        /// Đặt trước <= 0 
        /// </summary>
        PreOrderLte0,

    }
}
