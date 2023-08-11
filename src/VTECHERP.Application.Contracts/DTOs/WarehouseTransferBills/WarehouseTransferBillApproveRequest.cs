using System.Collections.Generic;
using VTECHERP.DTOs.WarehouseTransferBillProducts;

namespace VTECHERP.DTOs.WarehouseTransferBills
{
    public class WarehouseTransferBillApproveRequest
    {
        public List<WarehouseTransferBillProductApproveRequest> WarehouseTransferBillProducts { get; set; }
    }
}