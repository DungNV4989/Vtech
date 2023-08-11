using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.Suppliers.Params
{
    public class SearchParam : BasePagingRequest
    {
        public string Squence { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public SupplierOrigin? Type { get; set; }
    }
}
