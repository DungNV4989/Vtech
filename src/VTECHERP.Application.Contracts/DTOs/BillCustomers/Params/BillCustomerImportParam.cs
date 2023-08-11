using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.BillCustomers.Respons;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.BillCustomers.Params
{
    public class BillCustomerImportParam : BillCustomerDto
    {
        public string CustomerName { get; set; }
        /// <summary>
        /// Loại khách
        /// </summary>
        public CustomerType? CustomerType { get; set; }
        /// <summary>
        /// Tỉnh/thành
        /// </summary>
        public Guid? ProvinceId { get; set; }
        public string CustomerPhone { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public string NoteForProductBonus { get; set; }
        public IFormFile File { get; set; }
    }
}
