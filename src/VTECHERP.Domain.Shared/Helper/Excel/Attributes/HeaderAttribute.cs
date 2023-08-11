
namespace VTECHERP.Domain.Shared.Helper.Excel.Attributes
{
    public class HeaderAttribute : System.Attribute
    {
        public string HeaderName { get; set; }
        public HeaderAttribute(string name)
        {
            HeaderName = name;
        }
    }
}
