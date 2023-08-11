using ClosedXML.Excel;

namespace VTECHERP.Domain.Shared.Helper.Excel.Model
{
    public class SuccessCell : Cell
    {
        public SuccessCell()
        {
            BackgroundColor = XLColor.Green;
            FontColor = XLColor.White;
        }
        public SuccessCell(string value) : base(value)
        {
            BackgroundColor = XLColor.Green;
            FontColor = XLColor.White;
        }
    }
}
