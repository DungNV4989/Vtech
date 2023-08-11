using System;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.Accounts
{
    public class AccountDto: BaseDTO
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string? ParentAccountCode { get; set; }
        public string? ParentAccountName { get; set; }
        public int Level { get; set; } = 1;
        public Guid? StoreId { get; set; }
        public string? StoreName { get; set; }
        public AccountType AccountType { get; set; }
        public bool? IsDefaultConfig { get; set; }
    }
}
