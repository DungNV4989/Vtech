using System;
using System.Collections.Generic;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums.Product;

namespace VTECHERP.DTOs.Product
{
    public class SearchProductStockRequest: BasePagingRequest
    {
        /// <summary>
        /// Cửa hàng
        /// </summary>
        public List<Guid>? StoreIds { get; set; }
        /// <summary>
        /// Danh mục
        /// </summary>
        public Guid? ProductCategoryIds { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }    
        /// <summary>
        /// Tồn filter
        /// </summary>
        public ProductInventoryFilter? InventoryFilter { get; set; }
        /// <summary>
        /// Trạng thái tồn
        /// </summary>
        public ProductInventoryStatus? InventoryStatus { get; set; }
    }

    public class SearchProductExcelRequest : SearchProductStockRequest
    {
        public List<Guid>? ReponseIds { get; set; }
    }
}
