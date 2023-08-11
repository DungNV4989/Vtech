using System;

namespace VTECHERP.DTOs.Accounts
{
    public class UpdateAccountStatusRequest
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}
