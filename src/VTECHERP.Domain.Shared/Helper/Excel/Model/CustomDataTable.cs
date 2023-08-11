using System.Collections.Generic;
using System.Linq;

namespace VTECHERP.Domain.Shared.Helper.Excel.Model
{
    public class CustomDataTable
    {
        public string StartCell { get; set; } = string.Empty;
        public int? StartRowIndex { get; set; }
        public int? StartColumnIndex { get; set; }
        public CustomDataTable(params DataRow[] rows)
        {
            Rows = rows.ToList();
        }

        public CustomDataTable(string startCell, params DataRow[] rows)
        {
            StartCell = startCell;
            Rows = rows.ToList();
        }

        public CustomDataTable(int startRow, int startColumn, params DataRow[] rows)
        {
            StartRowIndex = startRow;
            StartColumnIndex = startColumn;
            Rows = rows.ToList();
        }

        public CustomDataTable(int startRow, int startColumn, Directions direction, params DataRow[] rows)
        {
            StartRowIndex = startRow;
            StartColumnIndex = startColumn;
            Rows = rows.ToList();
            RowDirection = direction;
        }

        public Directions RowDirection { get; set; } = Directions.Horizontal;
        public List<DataRow> Rows { get; set; } = new List<DataRow>();
    }
}
