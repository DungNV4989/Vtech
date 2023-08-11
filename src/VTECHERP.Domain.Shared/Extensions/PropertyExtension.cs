using System.Linq;
using System.Reflection;
using VTECHERP.Domain.Shared.Helper.Excel.Attributes;

namespace VTECHERP.Extensions
{
    public static class PropertyExtension
    {
        public static string HeaderName(this PropertyInfo property)
        {
            var headerName = property.Name;
            var headerAttr = property.GetCustomAttributes(true).FirstOrDefault(p => p is HeaderAttribute);
            if (headerAttr != null)
            {
                headerName = (headerAttr as HeaderAttribute)?.HeaderName;
            }
            if (headerName != null)
            {
                return headerName;
            }
            return "";
        }
    }
}
