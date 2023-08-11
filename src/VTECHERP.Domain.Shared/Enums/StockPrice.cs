using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Enums
{
    public enum StockPrice
    {
        /// <summary>
        /// Theo giá vốn cuối kỳ
        /// </summary>
        PriceByEnd = 0,
        /// <summary>
        /// Theo giá vốn trung bình trong kỳ
        /// </summary>
        PriceByPeriod = 1
    }

    public enum CurrentInventory
    {
        /// <summary>
        /// Tất cả sản phẩm
        /// </summary>
        AllProduct = 0,
        /// <summary>
        /// Còn tồn hiện tại (Mặc định) 
        /// </summary>
        CurrentStock = 1
    }
    public enum XNKArising
    {
        /// <summary>
        /// Tất cả 
        /// </summary>
        All = 0,
        /// <summary>
        /// Có phát sinh XNK
        /// </summary>
        Arising = 1
    }
    
}
