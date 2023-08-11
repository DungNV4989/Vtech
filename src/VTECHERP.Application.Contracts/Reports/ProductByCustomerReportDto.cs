using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Reports
{
    public class ProductByCustomerReportDto
    {
        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// Tên doanh nghiệp
        /// </summary>
        public string TenantName { get; set; }
        /// <summary>
        /// Mã sản phẩm
        /// </summary>
        public string ProductCode { get; set; }
        /// <summary>
        /// Mã vạch
        /// </summary>
        public Guid? ProductId { get; set; }
        /// <summary>
        /// Tên Sản phẩm
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// Id nhân viên phụ trách
        /// </summary>
        public Guid? HandlerEmployeeId { get; set; }
        /// <summary>
        /// Tên nhân viên phụ trách
        /// </summary>
        public string? HandlerEmployeeName { get; set; }
        /// <summary>
        /// Nhân viên chăm sóc
        /// </summary>
        public Guid? EmployeeCare { get; set; }
        /// <summary>
        /// Tên nhân viên chăm sóc
        /// </summary>
        public string? EmployeeCareName { get; set; }

        public Guid? StoreId { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? CategoryId { get; set; }
        /// <summary>
        /// danh mục
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// Giá bán
        /// </summary>
        public decimal SalePrice { get; set; }
        /// <summary>
        /// SL bán
        /// </summary>
        public decimal SaleQuantity { get; set; }
        /// <summary>
        /// SL trả
        /// </summary>
        public decimal ReturnQuantity { get; set; }
        /// <summary>
        /// Chiếu khấu
        /// </summary>
        public decimal DiscountValue { get; set; }
        /// <summary>
        /// Doanh thu
        /// </summary>
        public decimal AmountAfterDiscount { get; set; }
        /// <summary>
        /// Giá vốn
        /// </summary>
        public decimal StockPrice { get; set; }
        /// <summary>
        /// Lợi nhuận
        /// </summary>
        public decimal Profit { get; set; }
        /// <summary>
        /// Ngày mua cuối
        /// </summary>
        public DateTime LastBuyDate { get; set; }
    }
}
