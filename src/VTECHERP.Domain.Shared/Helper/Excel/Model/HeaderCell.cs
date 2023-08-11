using ClosedXML.Excel;

namespace VTECHERP.Domain.Shared.Helper.Excel.Model
{
    public class HeaderCell: Cell
    {
        public HeaderCell()
        {
            BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);
            FontColor = XLColor.White;
            Bold = true;
        }
        public HeaderCell(object value): base(value)
        {
            BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);
            FontColor = XLColor.White;
            Bold = true;
        }
        public HeaderCell(object value, XLColor bgColor, XLColor color) : base(value)
        {
            BackgroundColor = bgColor;
            FontColor = XLColor.White;
            FontColor = color;
            Bold = true;
        }
        public HeaderCell(object value, string bgColor, string color)
        {
            Value = value;
            Bold = true;
            if (string.IsNullOrEmpty(bgColor))
            {
                BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);
            }
            else
            {
                BackgroundColor = XLColor.FromHtml(bgColor);
            }

            if (string.IsNullOrEmpty(color))
            {
                FontColor = XLColor.White;
            }
            else
            {
                FontColor = XLColor.FromHtml(color);
            }
        }

        public new XLAlignmentHorizontalValues HorizontalAlignment = XLAlignmentHorizontalValues.Center;
        public new XLAlignmentVerticalValues VerticalAlignment = XLAlignmentVerticalValues.Center;
    }
}
