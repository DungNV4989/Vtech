using System.Collections.Generic;
using System.Linq;

namespace VTECHERP.Domain.Shared.Helper.Excel.Model
{
    public class CustomWorkBook
    {
        public List<CustomSheet> Sheets { get; set; } = new List<CustomSheet>();
        public CustomWorkBook(params CustomSheet[] sheets)
        {
            Sheets = sheets.ToList();
        }
    }
}
