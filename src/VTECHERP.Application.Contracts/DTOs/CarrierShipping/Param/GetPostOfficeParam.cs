using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.CarrierShipping.Param
{
    public class GetPostOfficeParam
    {
        public Guid? StoreId { get; set; }
        public Carrier Carrier { get; set; }
    }
}
