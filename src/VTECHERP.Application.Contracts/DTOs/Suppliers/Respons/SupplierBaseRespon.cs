using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.Suppliers.Respons
{
    public class SupplierBaseRespon
    {
        public Guid Id { get; set; }
        public string Squence { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string TypeText { get; set; }
        public SupplierOrigin Type { get; set; }
    }
}
