using System.Collections.Generic;
using System.Linq;

namespace VTECHERP.Domain.Shared.Helper.Excel.Model
{
    public class DataRow
    {
        public List<Cell> Cells { get; set; } = new List<Cell>();

        public DataRow(params Cell[] cells)
        {
            Cells = cells.ToList();
        }
    }
}
