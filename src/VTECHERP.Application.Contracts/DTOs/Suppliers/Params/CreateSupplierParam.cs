using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.Suppliers.Params
{
    public class CreateSupplierParam
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Nguồn gốc NCC
        /// </summary>
        public SupplierOrigin Type { get; set; }
    }
}
