using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.InventoryByProductReport
{
    public class InventoryByProductReportRequest : BasePagingRequest
    {
        /// <summary>
        /// Mốc thời gian
        /// </summary>
        public int Period { get; set; } = 1;
        /// <summary>
        /// từ ngày
        /// </summary>
        public DateTime? DateFrom { get; set; }
        /// <summary>
        /// đến ngày
        /// </summary>
        public DateTime? DateTo { get; set; }
        /// <summary>
        /// danh sách Id doanh nghiệp
        /// </summary>
        public List<Guid>? LstEnterpriseId { get; set; }
        /// <summary>
        /// danh sách Id cửa hàng 
        /// </summary>
        public List<Guid>? LstStoreId { get; set; }
        /// <summary>
        /// Giá vốn
        /// </summary>
        public StockPrice StockPrice { get; set; } 
        /// <summary>
        /// Tồn hiện tại
        /// </summary>
        public CurrentInventory CurrentInventory { get; set; } 
        /// <summary>
        /// Phát sinh XNK
        /// </summary>
        public XNKArising XNKArising { get; set; } 
    }
}
