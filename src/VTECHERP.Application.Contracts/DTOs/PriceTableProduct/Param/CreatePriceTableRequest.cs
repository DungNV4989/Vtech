using System;
using System.Collections.Generic;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.PriceTableProduct.Param
{

    public class CreatePriceTableRequest
    {
        // Tên bảng giá
        public string Name { get; set; }
        public string STT { get; set; }
        // Áp dụng từ
        public DateTime AppliedFrom { get; set; }
        // Áp dụng đến
        public DateTime? AppliedTo { get; set; }
        // List cửa hàng
        public List<Guid> StoreIds { get; set; } = new List<Guid>();
        // List khách hàng
        public List<Guid> CustomerIds { get; set; } = new List<Guid>();
        // Id bảng giá cha
        public Guid? ParentPriceTableId { get; set; }
        // trạng thái
        // 0 - inactive
        // 1 - active
        public PriceTableStatus Status { get; set; }
        // ghi chú
        public string Note { get; set; }
    }
}
