using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.BillCustomers.Params
{
    public class CustomerNewParam
    {
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
        /// <summary>
        /// Loại khách
        /// </summary>
        public CustomerType? CustomerType { get; set; }
        /// <summary>
        /// Tỉnh/thành
        /// </summary>
        public Guid? ProvinceId { get; set; }
        public string CustomerPhone { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        /// <summary>
        /// Nhân viên chăm sóc của khách hàng
        /// </summary>
        public Guid? EmployeeCare { get; set; }
        /// <summary>
        /// Nhân viên phụ trách của khách hàng
        /// </summary>
        public Guid? EmployeeSell { get; set; }
    }
}
