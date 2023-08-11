using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.MultiTenancy;
using VTECHERP.Enums;

namespace VTECHERP.Entities
{
    public class StoreReportView : BaseEntity<Guid>, IMultiTenant
    {
        public Guid? StoreId { get; set; }
        /// <summary>
        /// Năm
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// Quý
        /// </summary>
        public int? Quarter { get; set; }
        /// <summary>
        /// Tháng
        /// </summary>
        public int? Month { get; set; }
        /// <summary>
        /// Số dư đầu kỳ
        /// </summary>
        public decimal BeginBalance { get; set; }
        /// <summary>
        /// Tổng thu
        /// </summary>
        public decimal TotalRecieve { get; set; }
        /// <summary>
        /// Tổng chi
        /// </summary>
        public decimal TotalPay { get; set; }
        /// <summary>
        /// Số dư cuối kỳ
        /// </summary>
        public decimal EndBalance { get; set; }
        /// <summary>
        /// Mã nhà cung cấp // Khách hàng
        /// </summary>
        public Guid? AudienceId { get; set; } = Guid.Empty;
    }
}
