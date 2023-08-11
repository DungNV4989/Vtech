﻿using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.BillCustomers.Params;

namespace VTECHERP.DTOs.BillCustomers.Respons
{
    public class BillCustomerProductImportRespon : BillCustomerProductItem
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; }
        public string ColExcel1 { get; set; }
        public string ColExcel2 { get; set; }
        public string ColExcel3 { get; set; }
        public string ColExcel4 { get; set; }
        public string ColExcel5 { get; set; }
    }
}
