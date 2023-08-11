using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Attachment;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums.Product;

namespace VTECHERP.DTOs.Product
{
    public class SearchProductResponse 
    {
        public Guid Id { get; set; }
        public bool? HasPicture { get; set; }
        /// <summary>
        /// Doanh nghiệp
        /// </summary>
        public string Enterprise { get; set; }

        /// <summary>
        /// Mã Id Long tự tăng
        /// </summary>
        public string SequenceId { get; set; }
        /// <summary>
        /// Id sản phẩm cha
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Mã sản phẩm cha
        /// </summary>
        public string ParentCode { get; set; }

        /// <summary>
        /// Tên sản phẩm cha
        /// </summary>
        public string ParentName { get; set; }
        /// <summary>
        /// Mã vạch
        /// </summary>
        public string BarCode { get; set; }
        /// <summary>
        /// Tên khác
        /// </summary>
        public string OtherName { get; set; }
        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string Unit { get; set; }
        public string ProudctCode { get; set; }    
        public string ProudctName { get; set; }     
        /// <summary>
        /// Giá vốn
        /// </summary>
        public decimal StockPrice { get; set; }
        /// <summary>
        /// Giá vốn
        /// </summary>
        public decimal? SpaPrice { get; set; }
        /// <summary>
        /// Giá bán
        /// </summary>
        public decimal? SalePrice { get; set; }    
        /// <summary>
        /// Giá nhập
        /// </summary>
        public decimal? EntryPrice { get; set; }
        /// <summary>
        /// Giá sỉ
        /// </summary>
        public decimal? WholeSalePrice { get; set; }

        /// <summary>
        ///  Tồn kho
        /// </summary>
        public int? Inventory { get; set; }
        /// <summary>
        /// Tổng tồn
        /// </summary>
        public int? TotalInventory { get; set; }    
        /// <summary>
        /// Đang giao hàng
        /// </summary>
        public int? Delivery { get; set; }    
        /// <summary>
        /// Tồn trong kho
        /// </summary>
        public int? InStock { get; set; }    
        /// <summary>
        /// Tạm giũ
        /// </summary>
        public int? TemporarilyHold { get; set; }    

        /// <summary>
        /// có thể bán
        /// </summary>
        public int? SellNumber { get; set; }
        /// <summary>
        /// Đang về
        /// </summary>
        public int? Coming { get; set; }
        /// <summary>
        /// Đặt trước
        /// </summary>
        public int? Booking { get; set; }

        /// <summary>
        /// Trạng thái sản phẩm/ Trạng thái bán /Bán
        /// </summary>
        public ProductStatus? Status { get; set; }

        /// <summary>
        /// Link sản phẩm
        /// </summary>
        public List<DetailAttachmentDto> Attachments { get; set; }
        /// <summary>
        /// Id danh mục
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Id nhà cung cấp
        /// </summary>
        public Guid SupplierId { get; set; }
        /// <summary>
        /// Giá cước
        /// </summary>
        public decimal? RatePrice { get; set; }
        /// <summary>
        /// Mô tả
        /// </summary>
        public string Description { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
