using System;
using System.Collections.Generic;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.PriceTableProduct.Param
{
    public class SearchPriceTableRequest: BasePagingRequest
    {
        // lọc theo mã cửa hàng
        public List<Guid> StoreIds { get; set; } = new List<Guid>();
        public List<Guid> CustomerIds { get; set; } = new List<Guid>();
        // Lọc theo mã bảng giá
        public string Id { get; set;}
        public string Name { get; set;}
        public DateTime? AppliedFrom { get; set; }
        public DateTime? AppliedTo { get; set; }
        public PriceTableStatus? Status { get; set; }
        public Guid? ParentPriceTableId { get; set; }
    }
}
