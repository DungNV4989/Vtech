using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Enums.Product
{
    public enum ProductStatus
    {

        New, 
        /// <summary>
        /// Đang bán
        /// </summary>
        OnSale,
        /// <summary>
        /// Ngừng bán
        /// </summary>
        StopSelling,
        /// <summary>
        ///  Hết hàng
        /// </summary>
        OutOfStock,
        /// <summary>
        /// Thanh lý
        /// </summary>
        Liquidation
    }
}

