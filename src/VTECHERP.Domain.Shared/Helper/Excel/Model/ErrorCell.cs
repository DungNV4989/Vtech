using ClosedXML.Excel;
using VTECHERP.Domain.Shared.Helper.Excel.Model;

namespace Vinpearl.Modelling.Library.Utility.Excel.Model
{
    public class ErrorCell : Cell
    {
        public ErrorCell()
        {
            BackgroundColor = XLColor.Red;
            FontColor = XLColor.White;
        }
        public ErrorCell(string value): base(value)
        {
            BackgroundColor = XLColor.Red;
            FontColor = XLColor.White;
        }
    }
}
