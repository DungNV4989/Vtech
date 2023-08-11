using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.Debts
{
    public class SearchDebtRequest : BasePagingRequest
    {
        /// <summary>
        /// Mã nhà cung cấp
        /// </summary>
        public Guid? SupplierId { get; set; }
        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Công nợ
        /// </summary>
        public DebtTypes? DebtType { get; set; }
        /// <summary>
        /// Từ ngày
        /// </summary>
        public DateTime? SearchDateFrom { get; set; }
        /// <summary>
        /// Đến ngày
        /// </summary>
        public DateTime? SearchDateTo { get; set; }
        /// <summary>
        /// Phân loại NCC
        /// </summary>
        public SupplierOrigin? Type { get; set; }
        /// <summary>
        /// Mã đối tượng hoặc tên đối tượng
        /// </summary>
        public string SupplierCode { get; set; }
        /// <summary>
        /// Nhân dân tệ
        /// </summary>
        public double? NDT { get; set; }

    }
}
