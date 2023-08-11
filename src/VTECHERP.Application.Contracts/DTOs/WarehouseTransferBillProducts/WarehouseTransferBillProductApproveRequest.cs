using System;

namespace VTECHERP.DTOs.WarehouseTransferBillProducts
{
    public class WarehouseTransferBillProductApproveRequest
    {
        public Guid Id { get; set; }
        public int ApproveQuantity { get; set; }
    }
}