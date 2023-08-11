using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.DebtCustomer
{
    public class TotalDebtCustomerResponse
    {
        /// <summary>
        /// Tổng phải thu
        /// </summary>
        public decimal Credit { get; set; }
        /// <summary>
        /// Tổng phải trả
        /// </summary>
        public decimal Debt { get; set; }
    }
}
