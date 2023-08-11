using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using VTECHERP.Enums;

namespace VTECHERP.Entities
{
    public class StoreShippingInformation : BaseEntity<Guid>, IMultiTenant
    {
        public Carrier? Carrier { get; set; }
        public string UserNameCarriers { get; set; }
        public string PasswordCarries { get; set; }
        public Guid? BankId { get; set; }
        /// <summary>
        /// Số tài khoản ngân hàng
        /// </summary>
        public string AccountBankNumber { get; set; }
        /// <summary>
        /// Tên chủ tài khoản ngân hàng
        /// </summary>
        public string OwnerAccountBank { get; set; }
        public Guid StoreId { get; set; }
        public string Token { get; set; }
        /// <summary>
        /// Mã khách hàng dùng cho VN Post
        /// </summary>
        public string CustomerCode { get; set; }
    }
}
