using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.WarehouseTransferBillProducts;

namespace VTECHERP.DTOs.WarehouseTransferBills
{
    public class WarehouseTransferBillConfirmRequest
    {
        public Guid Id { get; set; }
        public List<WarehouseTransferBillProductConfirmRequest> WarehouseTransferBillProducts { get; set; }
    }

    public class WarehouseTransferBillProductConfirmRequest
    {
        public Guid Id { get; set; }
        public int ConfirmQuantity { get; set; }
    }
}
