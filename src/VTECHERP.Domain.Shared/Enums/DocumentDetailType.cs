namespace VTECHERP.Enums
{
    /// <summary>
    /// Kiểu chi tiết của chứng từ
    /// </summary>
    public enum DocumentDetailType
    {
        /// <summary>
        /// Nhập - Khách hàng
        /// </summary>
        ImportCustomer = 0,
        /// <summary>
        /// Nhập - Nhà cung cấp
        /// </summary>
        ImportSupplier = 1,
        /// <summary>
        /// Nhập - Chuyển kho
        /// </summary>
        ImportTransfer = 2,
        /// <summary>
        /// Nhập - Bù trừ kiểm kho
        /// </summary>
        ImportStockCheck = 3,
        /// <summary>
        /// Nhập - sản xuất
        /// </summary>
        ImportProduce = 4,
        /// <summary>
        /// Nhập - chuyển mã
        /// </summary>
        ImportCodeChange = 5,
        /// <summary>
        /// Nhập - Khác
        /// </summary>
        ImportOther = 6,
        /// <summary>
        /// Xuất - Khách hàng
        /// </summary>
        ExportCustomer = 7,
        /// <summary>
        /// Xuất - Nhà cung cấp
        /// </summary>
        ExportSupplier = 8,
        /// <summary>
        /// Xuất - Chuyển kho
        /// </summary>
        ExportTransfer = 9,
        /// <summary>
        /// Xuất - Bù trừ kiểm kho
        /// </summary>
        ExportStockCheck = 10,
        /// <summary>
        /// Xuất - Sán xuất
        /// </summary>
        ExportProduce = 11,
        /// <summary>
        /// Xuất - chuyển mã
        /// </summary>
        ExportCodeChange = 12,
        /// <summary>
        /// Xuất - Bảo hành
        /// </summary>
        ExportMaintain = 13,
        /// <summary>
        /// Xuất - Hủy
        /// </summary>
        ExportCancel = 14,
        /// <summary>
        /// Xuất - Quà tặng
        /// </summary>
        ExportGift = 15,
        /// <summary>
        /// Xuất - Khác
        /// </summary>
        ExportOther = 16,
        /// <summary>
        /// Xuất VAT
        /// </summary>
        ExportVAT = 17,
        /// <summary>
        /// Nhập VAT
        /// </summary>
        ImportVAT = 18,
        /// <summary>
        /// Phiếu đặt hàng
        /// </summary>
        Order = 19,
        /// <summary>
        /// Đơn vận chuyển
        /// </summary>
        DeliveryNote = 20,
        /// <summary>
        /// Báo nợ
        /// </summary>
        DebitNote = 21,
        /// <summary>
        /// Báo có
        /// </summary>
        CreditNote = 22,
        /// <summary>
        /// Phiếu thu
        /// </summary>
        Receipt = 23,
        /// <summary>
        /// Phiếu chi
        /// </summary>
        PaymentVoucher = 24,
        /// <summary>
        /// Chuyển quỹ
        /// </summary>
        FundTransfer = 25,
        /// <summary>
        /// Trả hàng
        /// </summary>
        ReturnProduct = 26,
    }
}
