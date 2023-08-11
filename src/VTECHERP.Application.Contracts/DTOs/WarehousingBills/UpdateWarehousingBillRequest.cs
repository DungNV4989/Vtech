using System;

namespace VTECHERP.DTOs.WarehousingBills
{
    public class UpdateWarehousingBillRequest: CreateWarehousingBillRequest
    {
        public Guid Id { get; set; }
    }
}
