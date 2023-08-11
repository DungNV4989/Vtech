using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums.Product;
using VTECHERP.Models;

namespace VTECHERP.DTOs.Product
{
    public class CreateProductDto
    {
        public string ProductName { get; set; }
        /// <summary>
        /// Tên khác
        /// </summary>
        public string OtherName { get; set; }
        /// <summary>
        /// Mã sản phẩm
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Id sản phẩm cha
        /// </summary>
        public Guid ParentId { get; set; }
        /// <summary>
        /// Id danh mục
        /// </summary>
        public Guid CategoryId { get; set; }
        /// <summary>
        /// Giá bán
        /// </summary>
        public decimal? SalePrice { get; set; }
        /// <summary>
        /// Trạng thái
        /// </summary>
        public ProductStatus? Status { get; set; }
        /// <summary>
        /// Giá sỉ
        /// </summary>
        public decimal? WholeSalePrice { get; set; }
        /// <summary>
        /// Giá SPA
        /// </summary>
        public decimal? SPAPrice { get; set; }
        /// <summary>
        /// giá cước
        /// </summary>
        public decimal? RatePrice { get; set; }
        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string Unit { get; set; }

        public IEnumerable<IFormFile>? formFiles { get; set; }
        /// <summary>
        /// Mô tả
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Câu hỏi thường gặp
        /// </summary>
        public string Question { get; set; }
    }
}
