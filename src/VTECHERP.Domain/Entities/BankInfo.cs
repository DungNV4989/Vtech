using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace VTECHERP.Entities
{
    public class BankInfo : BaseEntity<Guid>, IMultiTenant
    {
        /// <summary>
        /// Mã ngân hàng
        /// </summary>
        public string BankCode { get; set; }
        public string FullName { get; set; }
        /// <summary>
        /// Tên giao dịch
        /// </summary>
        public string TransactionName { get; set; }
    }
}
