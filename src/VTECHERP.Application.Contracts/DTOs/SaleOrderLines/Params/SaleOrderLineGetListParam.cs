using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.SaleOrderLines.Params
{
    public class SaleOrderLineGetListParam : BasePagingRequest
    {
        public string SaleOrderLineCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        /// <summary>
        /// Tên người phụ trách
        /// </summary>
        public string ChargerName { get; set; }
        public string ProductName { get; set; }
        public string SuplierName { get; set; }
        public string InvoiceNumber { get; set; }
        public List<Guid?>? StoreId { get; set; }
        public List<Guid?>? ProductCategory { get; set; }
        /// <summary>
        /// Trạng thái phiếu
        /// </summary>
        public SaleOrder.Status? SaleOrderStatus { get; set; }
        public CategoryProductSearch? CategoryProductSearch { get; set; }
    }

    public enum CategoryProductSearch
    {
        /// <summary>
        /// Chưa thực thi
        /// </summary>
        NoteExecute = 1,
        /// <summary>
        /// Đã thực thi
        /// </summary>
        Executed = 2
    }
}
