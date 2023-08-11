using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;
using VTECHERP.Enums.Debt;

namespace VTECHERP.DTOs.DebtCustomer
{
    public class SearchDebtCustomerRequest : BasePagingRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid? CustomerId { get; set; }
        /// <summary>
        /// Công nợ
        /// </summary>
        public DebtTypes? DebtType { get; set; }
        public CustomerType? CustomerType { get; set; }
        /// <summary>
        /// Nhân viên phụ trách
        /// </summary>
        public Guid? EmployeeId { get; set; }
        /// <summary>
        /// Cửa hàng phụ trách
        /// </summary>
        public Guid? StoreId { get; set; }
        /// <summary>
        /// Tỉnh của KH
        /// </summary>
        public List<Guid>? ProvinceId { get; set; }
        public DebtLimitEnums? DebtLimit { get; set; }

        /// <summary>
        /// COD: COD
        /// </summary>
        public DebtHasCodeEnums? HasCod { get; set; }
        /// <summary>
        /// Tìm kiếm đơn hàng gần nhất
        /// </summary>
        public int? LastOrder { get; set; }
        /// <summary>
        /// Số ngày nợ
        /// </summary>
        public int? NumberOfDaysOwed { get; set; }
        public decimal? DebtFrom { get; set; }
        public decimal? DebtTo { get; set; }
        
    }
}
