using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.WarehouseTransferBillProducts;

namespace VTECHERP.DTOs.WarehouseTransferBills
{
    public class WarehouseTransferBillProductImportRespon : WarehouseTransferBillProductCreateRequest
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; }
        public string ColExcel1 { get; set; }
        public string ColExcel2 { get; set; }
        public string ColExcel3 { get; set; }
    }
}
