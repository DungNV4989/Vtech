using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.Entries
{
    public class EntryAccountDto : BaseDTO
    {
        public Guid EntryId { get; set; }
        public string? CreditAccountCode { get; set; }
        public string? DebtAccountCode { get; set; }
        public decimal? AmountVnd { get; set; }
        public decimal? AmountCny { get; set; }
        public string? Note { get; set; }
    }
}
