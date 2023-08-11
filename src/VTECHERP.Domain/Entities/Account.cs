using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.MultiTenancy;
using VTECHERP.Enums;

namespace VTECHERP.Entities
{
    /// <summary>
    /// Tài khoản kế toán
    /// </summary>
    [Table("Accounts")]
    public class Account : BaseEntity<Guid>, IMultiTenant
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string? ParentAccountCode { get; set; }
        public Guid? StoreId { get; set; }
        public AccountType? AccountType { get; set; }
        public int? Level { get; set; }
        /// <summary>
        /// Có phải tài khoản thiết lập mặc định không
        /// </summary>
        public bool? IsDefaultConfig { get; set; } = false;
        public Account()
        {

        }

        public Account(string code)
        {
            Code = code;
        }


        public Account(string code, string name):this(code)
        {
            Name = name;
        }
    }
}
