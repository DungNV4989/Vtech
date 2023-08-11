namespace VTECHERP.Enums
{
    /// <summary>
    /// Nguồn gốc tạo bút toán
    /// </summary>
    public enum ActionSources
    {
        /// <summary>
        /// Tạo đơn hàng
        /// </summary>
        OrderCreate = 0,
        /// <summary>
        /// Xác nhận đơn hàng - Tạo phiếu XNK tự động
        /// </summary>
        OrderConfirmation = 1,
        /// <summary>
        /// Hoàn thành đơn hàng
        /// </summary>
        OrderCompleted = 2,
        /// <summary>
        /// Tạo thủ công
        /// </summary>
        ManualCreateEntry = 3,
        /// <summary>
        /// Tạo phiếu XNK
        /// </summary>
        WarehousingBillCreate = 4,
        /// <summary>
        /// Tạo phiếu thu chi
        /// </summary>
        CreatePaymentReceipt = 5,
        /// <summary>
        /// Tạo hóa đơn bán hàng
        /// </summary>
        CreateBillCustomer = 6,
        ReturnProduct = 7,
        /// <summary>
        /// Chỉnh sửa giá vốn sản phẩm
        /// </summary>
        ChangeCostPriceProduct = 8
    }
}
