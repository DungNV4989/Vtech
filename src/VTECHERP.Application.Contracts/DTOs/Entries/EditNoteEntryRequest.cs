using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.Entries
{
    public class EditNoteEntryRequest
    {
        public Guid Id { get; set; }
        public string Note { get; set; }
    }
}
