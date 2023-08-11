namespace VTECHERP.Enums
{
    #region Suppliers Enum
    public enum SupplierStatus
    {
    }
    public enum SupplierOrigin
    {
        CN = 0,
        VN = 1
    }


    #endregion Suppliers Enum

    #region ExchangeEnum
    public enum ExchangeEnum
    {
        Return = 0,
        Exchange = 1
    }
    #endregion



    #region ProductCategories Enum

    public class ProductCategory
    {
        public enum Status
        {
            //Hoạt động
            Active = 0,
            //Không hoạt động
            InActive = 1,
        }
    }

    #endregion ProductCategories Enum

    #region SaleOrders Enum

    public class SaleOrder
    {
        /// <summary>
        /// Trạng thái đơn hàng
        /// </summary>
        public enum Status
        {
            /// <summary>
            /// Chưa hoàn thành
            /// </summary>
            Unfinished,

            /// <summary>
            /// Đã hoàn thành
            /// </summary>
            Finished
        }

        /// <summary>
        /// Xác nhận đơn hàng
        /// </summary>
        public enum Confirm
        {
            /// <summary>
            /// Xác nhận thiếu
            /// </summary>
            Lack,

            /// <summary>
            /// Xác nhận đủ
            /// </summary>
            Enough,

            /// <summary>
            /// Xác nhận thừa
            /// </summary>
            Excess
        }

        public enum ConpleteType
        {

            
            Manually,   
            Auto
        }
    }

    #endregion SaleOrders Enum

    public enum OrderTransportStatus
    {
        /// <summary>
        /// Chưa nhận
        /// </summary>
        NotReceive,
        /// <summary>
        /// Đã nhận
        /// </summary>
        Received
    }
    #region PriceTable Enum
    public enum PriceTableStatus
    {
        /// <summary>
        /// Không hoạt động
        /// </summary>
        InActive,
        /// <summary>
        /// Hoạt động
        /// </summary>
        Active
    }
    #endregion PriceTable Enum
}