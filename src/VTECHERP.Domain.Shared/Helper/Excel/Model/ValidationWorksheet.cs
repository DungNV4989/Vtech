using ClosedXML.Excel;
using System.Collections.Generic;

namespace VTECHERP.Domain.Shared.Helper.Excel.Model
{
    public class ExcelValidationData
    {
        public string Code { get; set; } = string.Empty;
        public List<string> Values { get; set; } = new List<string>();
        public string DataRange { get; set; } = string.Empty;
    }
}
