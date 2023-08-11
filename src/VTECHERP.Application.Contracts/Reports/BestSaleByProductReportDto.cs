using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Reports
{
    public class BestSaleByProductReportDto
    {

        public string Code { get; set; }
        public string ProductName { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? StoreId { get; set; }
        public Guid? TenantId { get; set; }
        /// <summary>
        /// Giá bán lẻ
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Tồn đầu kỳ
        /// </summary>
        public decimal SLBegin { get; set; }
        /// <summary>
        /// Số lượng nhập
        /// </summary>
        public decimal ImportQuatity { get; set; }
        /// <summary>
        /// Bán spa
        /// </summary>
        public decimal SpaQuantity { get; set; }
        /// <summary>
        /// Bán sỉ
        /// </summary>
        public decimal AgencyQuantity { get; set; }
        public decimal TotalSLBeginAndImportQuatity { get; set; }
        /// <summary>
        /// Bán lẻ
        /// </summary>
        public decimal RetailQuantity { get; set; }
        /// <summary>
        /// Số lượng bán
        /// </summary>
        public decimal SaleQuantity { get; set; }
        /// <summary>
        /// Tồn cuối kỳ
        /// </summary>
        public decimal SLEnd { get; set; }
        /// <summary>
        /// Tỉ lệ bán ra
        /// </summary>
        public decimal SaleRate { get; set; }

    }
}
