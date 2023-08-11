using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.Entries
{
    public class EntryUpdateRequest : EntryCreateRequest
    {
        public Guid Id { get; set; }
    }
}
