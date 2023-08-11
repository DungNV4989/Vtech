using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums.Product;

namespace VTECHERP.DTOs.Product
{
    public class SearchProductRequest : BasePagingRequest
    {
        /// <summary>
        /// Cửa hàng
        /// </summary>
        public List<Guid>? StoreId { get; set; }

        /// <summary>
        /// Danh mục
        /// </summary>
        public List<Guid>? ProductCategoryIds { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public string? BarCode { get; set; }
        public string? SequenceId { get; set; }

        /// <summary>
        /// Tồn filter
        /// </summary>
        public ProductInventoryFilter? InventoryFilter { get; set; }
        /// <summary>
        /// Trạng thái sản phẩm/ Trạng thái bán
        /// </summary>
        public ProductStatus? Status { get; set; }    
        /// <summary>
        /// Trạng thái tồn
        /// </summary>
        public Enums.Product.ProductInventoryStatus? InventoryStatus{ get; set; }

    }

    
}
