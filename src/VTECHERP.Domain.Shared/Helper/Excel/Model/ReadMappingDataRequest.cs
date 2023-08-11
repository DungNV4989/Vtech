using System.ComponentModel.DataAnnotations.Schema;

namespace VTECHERP.Domain.Shared.Helper.Excel.Model
{
    public class ReadMappingDataRequest
    {
        public string? StartCell { get; set; } = string.Empty;
        public int? StartRow { get; set; }
        public int? StartColumn { get; set; }
        public string? EndCell { get; set; } = string.Empty;
        public int? EndRow { get; set; }
        public int? EndColumn { get; set; }
        public bool FirstRowKeys { get; set; } = true;
        public bool TryReadHeaderAbove { get; set; } = false;
        public Directions ReadDirection { get; set; } = Directions.Horizontal;
        [NotMapped]
        public string TemplateCell { get; set; } = string.Empty;
    }
}
