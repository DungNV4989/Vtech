using ClosedXML.Excel;

namespace VTECHERP.Domain.Shared.Helper.Excel.Constants
{
    public class ExcelColors
    {
        public const string BLUE_LIGHTER = "#DDEBF7";
        public const string BLUE_LIGHT = "#9BC2E6";
        public const string BLUE = "#0070C0";
        public const string BLACK = "#000000";
        public const string YELLOW = "#FFFF00";
        public const string Y_GREEN = "#92D050";
        public const string LIGHT_GRAY = "#D9D9D9";

        public static XLColor GetColor(string color)
        {
            return XLColor.FromHtml(color);
        }
    }
}
