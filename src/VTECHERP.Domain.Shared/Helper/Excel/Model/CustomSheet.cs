using System.Collections.Generic;
using System.Linq;

namespace VTECHERP.Domain.Shared.Helper.Excel.Model
{
    public class CustomSheet
    {
        public CustomSheet(params CustomDataTable[] tables)
        {
            Tables = tables.ToList();
        }
        public CustomSheet(string sheetName, params CustomDataTable[] tables)
        {
            SheetName = sheetName;
        }
        public string SheetName { get; set; } = string.Empty;
        public List<FixedHeader> FixedHeaders { get; set; } = new List<FixedHeader>();
        public List<CustomDataTable> Tables { get; set; } = new List<CustomDataTable>();
        public bool IsFreezeRow { get; set; }
        public int FreezeRow { get; set; }
        public bool IsFreezeColumn { get; set; }
        public int FreezeColumn { get; set; }
    }
}
