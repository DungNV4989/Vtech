using System;

namespace VTECHERP.DTOs.WarehouseTransferBills
{
    public class SearchWarehouseTransferBillApproveRequest
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
    }
}