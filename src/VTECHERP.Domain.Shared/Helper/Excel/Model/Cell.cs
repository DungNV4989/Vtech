using ClosedXML.Excel;

namespace VTECHERP.Domain.Shared.Helper.Excel.Model
{

    public class Cell
    {
        public Cell()
        {

        }
        public Cell(object? value)
        {
            Value = value;
        }
        public Cell(object? value, XLColor bgColor, XLColor color)
        {
            Value = value;
            BackgroundColor = bgColor;
            FontColor = color;
        }
        public Cell(object? value, string bgColor, string color)
        {
            Value = value;
            if (string.IsNullOrEmpty(bgColor))
            {
                BackgroundColor = XLColor.White;
            }
            else
            {
                BackgroundColor = XLColor.FromHtml(bgColor);
            }

            if (string.IsNullOrEmpty(color))
            {
                FontColor = XLColor.Black;
            }
            else
            {
                FontColor = XLColor.FromHtml(color);
            }
        }
        public XLAlignmentHorizontalValues HorizontalAlignment = XLAlignmentHorizontalValues.Left;
        public XLAlignmentVerticalValues VerticalAlignment = XLAlignmentVerticalValues.Center;
        public object? Value { get; set; }
        public XLColor BackgroundColor { get; set; } = XLColor.White;
        public XLColor FontColor { get; set; } = XLColor.Black;
        public bool Bold { get; set; } = false;
        public bool Italic { get; set; } = false;
        public bool Underline { get; set; } = false;
        public int RowSpan { get; set; } = 1;
        public int ColSpan { get; set; } = 1;
        public int FontSize = 10;
        public bool Editable { get; set; } = false;
        public CellFormula? CellFormula { get; set; }
        public int NumberFormatId { get; set; } = 0;
        public string Format { get; set; } = string.Empty;
        public ConditionalFormat? ConditionalFormat { get; set; }
    }

    public class CellFormula
    {
        public string Formula { get; set; } = string.Empty;
        public CellFormulaType Type { get; set; } = CellFormulaType.R1C1;
    }

    public enum CellFormulaType
    {
        A1,
        R1C1
    }

    public enum NumberFormats
    {
        General = 0,
        Interger = 1,
        Decimal = 2,
        Thousand = 3,
        ThousandDecimal = 4,
        IntPercentage = 9,
        DecimalPercentage = 10,
        Date = 14
    }

    public class ConditionalFormat
    {
        public CellCondition Condition { get; set; }
        public string Format { get; set; } = string.Empty;
    }

    public enum CellCondition
    {
        SelfTrunc = 1,
        Percentage = 2,
    }
}
