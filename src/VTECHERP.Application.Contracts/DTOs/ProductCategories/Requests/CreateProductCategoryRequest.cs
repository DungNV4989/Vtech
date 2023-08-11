using System;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.ProductCategories.Requests
{
    /// <summary>
    /// Đầu vào tạo danh mục sản phẩm
    /// </summary>
    public class CreateProductCategoryRequest
    {
        /// <summary>
        /// Id danh mục cha
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Tên danh mục
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Mã danh mục
        /// </summary>
        public string CategoryCode { get; set; }

        /// <summary>
        /// Tỷ lệ
        /// </summary>
        public double? Ratio { get; set; }

        /// <summary>
        /// Id người phụ trách
        /// </summary>
        public Guid? ManagerId { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public ProductCategory.Status? Status { get; set; }
    }
}
