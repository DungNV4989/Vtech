using ClosedXML.Excel;

namespace VTECHERP.Domain.Shared.Helper.Excel.Model
{
    public class FixedHeader
    {
        public string Range { get; set; } = string.Empty;
        public int StartRowIndex { get; set; }
        public int EndRowIndex { get; set; }
        public int StartColumnIndex { get; set; }
        public int EndColumnIndex { get; set; }
        public string Value { get; set; } = string.Empty;
        public XLColor BackgroundColor { get; set; } = XLColor.FromTheme(XLThemeColor.Accent1);
        public XLColor FontColor { get; set; } = XLColor.White;
        public int FontSize { get; set; } = 10;
        public bool Bold { get; set; } = false;
        public bool Italic { get; set; } = false;
        public bool Underline { get; set; } = false;
    }
}
