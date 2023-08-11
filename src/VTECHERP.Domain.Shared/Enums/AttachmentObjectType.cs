using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Enums
{
    public enum AttachmentObjectType
    {
        /// <summary>
        /// Bút toán
        /// </summary>
        Entry = 0,
        /// <summary>
        /// Sản phẩm
        /// </summary>
        Product = 1,
        /// <summary>
        /// Chuyển kho
        /// </summary>
        WarehouseTransferBill = 2,
        /// <summary>
        /// Hóa đơn bán hàng
        /// </summary>
        BillCustomer = 3,
        /// <summary>
        /// Lịch sử vận đơn
        /// </summary>
        BillHistory = 4,
        /// <summary>
        /// Trả hàng
        /// </summary>
        CustomerReturn = 5,
        /// <summary>
        /// Thu chi
        /// </summary>
        PaymentReceipt = 6,
        /// <summary>
        /// Phiếu Xuất nhập kho
        /// </summary>
        WarehousingBill = 7,
        /// <summary>
        /// Phiếu đặt hàng
        /// </summary>
        SaleOrder = 8,
        /// <summary>
        /// Đơn vận chuyển TQ
        /// </summary>
        ShippingForm = 9,
        /// <summary>
        /// Giao hàng nội bộ
        /// </summary>
        TnternalDelivery = 10,
    }
}
