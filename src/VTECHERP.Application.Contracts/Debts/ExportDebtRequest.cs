using System.Collections.Generic;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.Debts
{
    public class ExportDebtRequest: BaseSearchRequest
    {
        /// <summary>
        /// Mã tài khoản
        /// </summary>
        public string? Code { get; set; }
        /// <summary>
        /// Tên tài khoản
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Tên cửa hàng
        /// </summary>
        public string? StoreName { get; set; }
        /// <summary>
        /// Loại tài khoản
        /// </summary>
        public List<AccountType>? AccountType { get; set; }
    }
}
