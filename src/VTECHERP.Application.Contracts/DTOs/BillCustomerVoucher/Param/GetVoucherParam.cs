using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.BillCustomerVoucher.Param
{
    public class GetVoucherParam
    {
        public string Code { get; set; }
        public Guid? StoreId { get; set; }
        /// <summary>
        /// Danh sách id sản phẩm trong hóa đơn
        /// </summary>
        public List<Guid> ProductIds { get; set; }
        /// <summary>
        /// Tổng tiền hóa đơn
        /// </summary>
        public decimal BillAmount { get; set; }
    }
}
