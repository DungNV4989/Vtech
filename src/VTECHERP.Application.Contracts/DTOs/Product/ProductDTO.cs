using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums.Product;

namespace VTECHERP.DTOs.Product
{
    public class ProductDTO
    {
        /// <summary>
        /// Id doanh nghiệp
        /// </summary>
        public Guid OrganizationId { get; set; }
        /// <summary>
        /// Mã Id Long tự tăng
        /// </summary>
        public string SequenceId { get; set; }

        /// <summary>
        /// Tên sản phẩm
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Mã sản phẩm
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Id sản phẩm cha
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// Mã sản phẩm cha
        /// </summary>
        public string ParentCode { get; set; }

        /// <summary>
        /// Tên sản phẩm cha
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// Loại sản phẩm
        /// </summary>
        public ProductType Type { get; set; }

        /// <summary>
        /// Mã vạch
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// Doanh nghiệp
        /// </summary>
        public string Enterprise { get; set; }

        /// <summary>
        /// Tên khác
        /// </summary>
        public string OtherName { get; set; }

        /// <summary>
        /// Link sản phẩm
        /// </summary>
        public string Attachments { get; set; }

        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Cân nặng cả vỏ hộp
        /// </summary>
        public double? Weight { get; set; }

        /// <summary>
        /// Số tháng bảo hành
        /// </summary>
        public int MonthOfWarranty { get; set; }

        /// <summary>
        /// Giá nhập
        /// </summary>
        public decimal? EntryPrice { get; set; }

        /// <summary>
        /// Giá bán
        /// </summary>
        public decimal? SalePrice { get; set; }

        /// <summary>
        /// VAT
        /// </summary>
        public decimal? VAT { get; set; }

        /// <summary>
        /// Giá cũ
        /// </summary>
        public decimal? OldPrice { get; set; }

        /// <summary>
        /// % lợi nhuận
        /// </summary>
        public decimal? Profit { get; set; }

        /// <summary>
        /// Giá vốn
        /// </summary>
        public decimal StockPrice { get; set; }

        /// <summary>
        /// Giá sỉ
        /// </summary>
        public decimal? WholeSalePrice { get; set; }

        /// <summary>
        /// Giá SPA
        /// </summary>
        public decimal? SPAPrice { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public ProductStatus? Status { get; set; }

        /// <summary>
        /// Id danh mục
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Id nhà cung cấp
        /// </summary>
        public Guid SupplierId { get; set; }

        /// <summary>
        /// Link trên website
        /// </summary>
        public string WebsiteLink { get; set; }

        /// <summary>
        /// Chiều cao
        /// </summary>
        public double? Height { get; set; }

        /// <summary>
        /// Chiều rộng
        /// </summary>
        public double? Width { get; set; }

        /// <summary>
        /// Chiều dài
        /// </summary>
        public double? Length { get; set; }
        #region Thông tin cache để tính lại giá vốn trong trường hợp cập nhật/xóa phiếu nhập kho
        /// <summary>
        /// Phiếu nhập kho cuối cùng ảnh hưởng đến giá vốn
        /// </summary>
        public Guid? LatestWarehousingBillId { get; set; }
        /// <summary>
        /// Giá vốn trước phiếu nhập kho cuối cùng
        /// </summary>
        public decimal StockPriceBeforeLatest { get; set; }
        /// <summary>
        /// Tổng tồn trước phiếu nhập kho cuối cùng
        /// </summary>
        public int TotalQuantityBeforeLatest { get; set; }
        #endregion
        public int TotalQuantity { get; set; }
        /// <summary>
        /// Giá cước
        /// </summary>
        public decimal? RatePrice { get; set; }
        /// <summary>
        /// Mô tả
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Câu hỏi thường gặp
        /// </summary>
        public string Question { get; set; }
        public Guid? StoreId { get; set; }
        public Guid? CreatorId { get; set; }
        public string CreatorName { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? LastModifierId { get; set; }
        public string LastModifierName { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}
