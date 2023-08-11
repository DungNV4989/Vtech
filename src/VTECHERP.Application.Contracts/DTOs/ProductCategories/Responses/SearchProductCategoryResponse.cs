using System;
using VTECHERP.DTOs.Base;
using static VTECHERP.Enums.ProductCategory;

namespace VTECHERP.DTOs.ProductCategories.Responses
{
    /// <summary>
    /// Đầu ra danh sách danh mục sản phẩm
    /// </summary>
    public class SearchProductCategoryResponse
    {
        /// <summary>
        /// Id danh mục
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// (Id)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// (Tên danh mục)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// (Mã danh mục)
        /// </summary>
        public string CategoryCode { get; set; }

        /// <summary>
        /// Số sản phẩm
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Id người phụ trách
        /// </summary>
        public Guid? ManagerId { get; set; }

        /// <summary>
        /// Tên người phụ trách (Người phụ trách)
        /// </summary>
        public string ManagerName { get; set; }

        /// <summary>
        /// Tỷ lệ
        /// </summary>
        public double? Ratio { get; set; }

        /// <summary>
        /// (Trạng thái: 0:Hoạt động/1:Không hoạt động)
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// Có phải là danh múc cha không (null:phải)
        /// </summary>
        public Guid? ParentId { get; set; }
    }
}