using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.Suppliers.Respons
{
    public class SupplierImportRespon
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Type { get; set; }
        public string Message { get; set; } = "";
        public bool Success { get; set; } = true;
    }
}
