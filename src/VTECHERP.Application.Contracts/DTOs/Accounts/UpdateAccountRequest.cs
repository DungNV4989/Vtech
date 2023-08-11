using System;

namespace VTECHERP.DTOs.Accounts
{
    public class UpdateAccountRequest
    {
        public Guid Id { get; set; }
        public Guid? StoreId { get; set; }
        public string Code { get; set; }
        public string? ParentAccountCode { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
