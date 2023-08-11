namespace VTECHERP.Enums
{
    /// <summary>
    /// Loại chứng từ
    /// </summary>
    public enum DocumentTypes
    {
        /// <summary>
        /// Đơn đặt hàng
        /// </summary>
        SupplierOrder = 0,
        /// <summary>
        /// Phiếu Nhập kho
        /// </summary>
        InventoryImport = 1,
        /// <summary>
        /// Phiếu Xuất kho
        /// </summary>
        InventoryExport = 2,
        /// <summary>
        /// Đơn vận chuyển
        /// </summary>
        ShippingNote = 3,
        /// <summary>
        /// Chứng từ ngoài
        /// </summary>
        Other = 4,
        /// <summary>
        /// Báo nợ
        /// </summary>
        DebitNote = 5,
        /// <summary>
        /// Báo có
        /// </summary>
        CreditNote = 6,
        /// <summary>
        /// Phiếu thu
        /// </summary>
        Receipt = 7,
        /// <summary>
        /// Phiếu chi
        /// </summary>
        PaymentVoucher = 8,
        /// <summary>
        /// Bút toán
        /// </summary>
        Entry = 9,
        /// <summary>
        /// Chuyển quỹ
        /// </summary>
        FundTransfer = 10,
        /// <summary>
        /// Bán hàng
        /// </summary>
        BillCustomer = 11,
        /// <summary>
        /// Hóa đơn trả hàng
        /// </summary>
        ReturnProduct = 12,
        /// <summary>
        /// Chuyển giá vốn
        /// </summary>
        ChangeCostPriceProduct = 13,
    }
}
