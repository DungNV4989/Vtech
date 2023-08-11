using System;
using System.Collections.Generic;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.Entries
{
    public class EntryLogRequest
    {
        public Guid EntryId { get; set; }
        public (Dictionary<string, object>, Dictionary<string, object>) Compare { get; set; }
        public EntityActions Action { get; set; }
    }
}
