using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System;
using System.Collections.Generic;
using VTECHERP.DTOs.Base;
using static VTECHERP.Enums.ProductCategory;

namespace VTECHERP.DTOs.ProductCategories.Requests
{
    /// <summary>
    /// Đầu vào danh sách danh mục sản phẩm
    /// </summary>
    public class SearchProductCategoryRequest : BasePagingRequest
    {
        /// <summary>
        /// Từ ngày (Thời gian tạo)
        /// </summary>
        public DateTime? From { get; set; }

        /// <summary>
        /// Đến ngày (Thời gian tạo)
        /// </summary>
        public DateTime? To { get; set; }

        /// <summary>
        /// (Id danh mục)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// (Tên danh mục)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// (Trạng thái: 0:hoạt động/1:không hoạt động)
        /// </summary>
        public Status? Status { get; set; }

        public List<Guid> ManagerIds { get; set; } = new List<Guid>();
    }
}