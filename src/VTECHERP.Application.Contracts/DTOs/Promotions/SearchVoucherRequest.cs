using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums.Bills;

namespace VTECHERP.DTOs.Promotions
{
    public class SearchVoucherRequest: BasePagingRequest
    {
        public string Code { get; set; }
        public VoucherStatus? Status { get; set; }
    }
}
