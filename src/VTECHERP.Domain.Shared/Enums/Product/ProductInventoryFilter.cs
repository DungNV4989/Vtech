using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Enums.Product
{
    public enum ProductInventoryFilter
    {
        /// <summary>
        ///  Còn tồn: tồn lớn hơn 0 
        /// </summary>
        OnHold = 0,
        /// <summary>
        /// Có thể bán: có thể bán lớn hơn 0 
        /// Có thể bán = Tồn - Tạm giữ
        /// </summary>
        OnSale = 1
    }
}
