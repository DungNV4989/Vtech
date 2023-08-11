using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.Stores
{
    public class StoreListItem
    {
        public Guid StoreId { get; set; }
        public string StoreCode { get; set; }
        public uint Order { get; set; }
        public string StoreName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime? ExpireDate { get; set; }
        public bool Status { get; set; }
        public string StatusText { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public string CreatorName { get; set; }
    }
}
