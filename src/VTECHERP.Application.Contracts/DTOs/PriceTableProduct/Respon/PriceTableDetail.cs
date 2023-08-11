using System;
using System.Collections.Generic;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.PriceTableProduct.Respon
{
    public class PriceTableDetail
    {
        public Guid Id { get; set; }
        public string STT { get; set; }
        public string PriceTableName { get; set; }
        public string Code { get; set; }
        public Guid? ParentId { get; set; }
        public string PriceTableParentName { get; set; }
        public PriceTableStatus Status { get; set; }
        public string StatusText { get; set; }
        public List<PriceTableCustomerDetail>? Customers { get; set; } = new();
        public List<PriceTableStoreDetail>? Stores { get; set; } = new();
        public DateTime AppliedFrom { get; set; }
        public DateTime? AppliedTo { get; set; }
        public Guid? CreatorId { get; set; }
        public string CreatorName { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? LastModifierId { get; set; }
        public string LastModifierName { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string Note { get; set; }
    }

    public class PriceTableCustomerDetail
    {
        public Guid PriceTableCustomerId { get; set; }
        public Guid PriceTableId { get; set; }
        public string CustomerName { get; set; }
        public Guid CustomerId { get; set; }
    }

    public class PriceTableStoreDetail
    {
        public Guid PriceTableStoreId { get; set; }
        public Guid PriceTableId { get; set; }
        public string StoreName { get; set; }
        public Guid StoreId { get; set; }
    }
}
