namespace VTECHERP.Enums
{
    /// <summary>
    /// Loại phiếu
    /// </summary>
    public enum TicketTypes
    {
        /// <summary>
        /// Phiếu nhập
        /// </summary>
        Import = 0,
        /// <summary>
        /// Phiếu xuất
        /// </summary>
        Export = 1,
        /// <summary>
        /// Báo nợ
        /// </summary>
        DebitNote = 2,
        /// <summary>
        /// Báo có
        /// </summary>
        CreditNote = 3,
        /// <summary>
        /// Phiếu thu
        /// </summary>
        Receipt = 4,
        /// <summary>
        /// Phiếu chi
        /// </summary>
        PaymentVoucher = 5,
        /// <summary>
        /// Kết chuyển
        /// </summary>
        ClosingEntry = 6,
        /// <summary>
        /// Khác
        /// </summary>
        Other = 7,
        /// <summary>
        /// Bán hàng
        /// </summary>
        Sales = 8,
        /// <summary>
        /// Trả hàng
        /// </summary>
        Return = 9,
        /// <summary>
        /// Chuyển quỹ
        /// </summary>
        FundTransfer = 10,
        /// <summary>
        /// Chuyển giá vốn
        /// </summary>
        ChangeCostPriceProduct = 11,
    }

    public enum TicketTypesSearch
    {
        None = 0,
        /// <summary>
        /// Phiếu thu
        /// </summary>
        Receipt = 1,
        /// <summary>
        /// Phiếu chi
        /// </summary>
        PaymentVoucher = 2,
    }
}
