using System;

namespace VTECHERP.DTOs.BillCustomers.Respons
{
    public class HistoryBillResponse
    {
        #region
        /// <summary>
        /// Ngày tạo hóa đơn bán hàng/Trả hàng (Ngày)
        /// </summary>
        public DateTime? CreationTime { get; set; }

        /// <summary>
        /// id hóa đơn bán hàng/Trả hàng
        /// </summary>
        public Guid BillId { get; set; }

        /// <summary>
        /// Mã hóa đơn bán hàng/Trả hàng
        /// </summary>
        public string BillCode { get; set; }

        /// <summary>
        /// Kiểu của hóa đơn (0:Bán hàng/1:Trả hàng)
        /// </summary>
        public BillLogType BillLogType { get; set; }

        /// <summary>
        /// Id cửa hàng
        /// </summary>
        public Guid? StoreId { get; set; }

        /// <summary>
        /// Mã cửa hàng
        /// </summary>
        public string StoreCode { get; set; }

        /// <summary>
        /// Tên cửa hàng
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// Id khách hàng
        /// </summary>
        public Guid? CustomerId { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Số điện thoại khách hàng
        /// </summary>
        public string CustomerPhone { get; set; }

        /// <summary>
        /// Số lượng sản phẩm (SP)
        /// </summary>
        public int AmountProduct { get; set; }

        /// <summary>
        /// Tổng số lượng sản phẩm (SL)
        /// </summary>
        public int TotalAmountProduct { get; set; }

        /// <summary>
        /// Số tiền khách phải trả (Tổng)
        /// </summary>
        public decimal? AmountCustomerPay { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
        #endregion
    }
}
