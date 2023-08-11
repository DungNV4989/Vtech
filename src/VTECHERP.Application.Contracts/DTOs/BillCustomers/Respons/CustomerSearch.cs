using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.BillCustomers.Respons
{
    public class CustomerSearch
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        /// <summary>
        /// Loại khách
        /// </summary>
        public CustomerType CustomerType { get; set; }
        /// <summary>
        /// Tỉnh/thành
        /// </summary>
        public Guid? ProvinceId { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public decimal DebtLimit { get; set; }
        public decimal DebtTotal { get; set; }
        public decimal CN { get; set; }
        public decimal LastCN { get; set; }
        public Guid? EmployeeCare { get; set; }
        public Guid? EmployeeRespon { get; set; }
        public string? Email { get; set; }
        public string? HandlerEmpName { get; set; }
        /// <summary>
        /// Phân loại công nợ
        /// </summary>
        public DebtGroup? DebtGroup { get; set; }
    }
}
