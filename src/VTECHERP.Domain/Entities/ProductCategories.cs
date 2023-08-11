using System;
using Volo.Abp.MultiTenancy;
using VTECHERP.Enums;

namespace VTECHERP.Entities
{
    /// <summary>
    /// Danh mục sản phẩm
    /// </summary>
    public class ProductCategories : BaseEntity<Guid>, IMultiTenant
    {
        /// <summary>
        /// Id danh mục cha
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Mã tự tăng (ID)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Tên danh mục
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Mã danh mục
        /// </summary>
        public string CategoryCode { get; set; }

        /// <summary>
        /// Thông tin BH
        /// </summary>
        public string Insuarance { get; set; }

        /// <summary>
        /// Số SP
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public ProductCategory.Status Status { get; set; } = ProductCategory.Status.Active;

        /// <summary>
        /// Id người phụ trách
        /// </summary>
        public Guid ManagerId { get; set; }

        /// <summary>
        /// Tỷ lệ
        /// </summary>
        public double? Ratio { get; set; }
    }
}