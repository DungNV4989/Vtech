using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.Entries
{
    public class EntryResponse
    {
        public EntryDTO Entry { get; set; }
        public IList<EntryAccountDto> EntryAccount { get; set; }   

    }
}
