using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using static VTECHERP.Enums.SaleOrder;

namespace VTECHERP.DTOs.SaleOrders
{
    public class SearchSaleOrderRequest : BasePagingRequest
    {
        public string Code { get; set; }
        public string SupplierName { get; set; }
        public List<Guid?> StoreId { get; set; }
        public Status? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Số hóa đơn
        /// </summary>
        public string InvoiceNumber { get; set; }
    }
}
