using System.Collections.Generic;
using System;
using VTECHERP.DTOs.Base;

namespace VTECHERP.DTOs.PriceTableProduct.Param
{
    public class SearchPriceProductByTableIdRequest : BasePagingRequest
    {
        // lọc theo mã cửa hàng
        public List<Guid> StoreIds { get; set; } = new List<Guid>();
        // khách hàng
        public List<Guid> CustomerIds { get; set; } = new List<Guid>();
        // bảng giá
        public Guid PriceTableId { get; set; }
        // mã sp / tên
        public string ProductCodeName { get; set; }
        // Danh mục SP
        public List<Guid> ProductCategoryIds { get; set; } = new();
    }
}
