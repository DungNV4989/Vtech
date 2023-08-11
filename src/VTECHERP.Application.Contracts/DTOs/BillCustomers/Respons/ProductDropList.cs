using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Attachment;

namespace VTECHERP.DTOs.BillCustomers.Respons
{
    public class ProductDropList : ProductBasicInfo
    {
        /// <summary>
        /// Tồn kho
        /// </summary>
        public int Inventory{ get; set; }
        public string ProductName { get; set; }
        /// <summary>
        /// Link sản phẩm
        /// </summary>
        public List<DetailAttachmentDto> Attachments { get; set; }
    }
}
