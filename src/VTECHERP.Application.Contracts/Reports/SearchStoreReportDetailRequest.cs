using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.Reports
{
    public class SearchStoreReportDetailRequest 
    {
        /// <summary>
        /// Id doanh nghiệp
        /// </summary>
        public List<Guid>? TenantId { get; set; }
        /// <summary>
        /// Id cửa hàng
        /// </summary>
        public List<Guid>? StoreId { get; set; }
        /// <summary>
        /// Loại phiếu
        /// </summary>
        public TicketTypes? TicketType { get; set; }

        /// <summary>
        /// Id bút toán cha
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Id bút toán cha
        /// </summary>
        public string ParentCode { get; set; }

        /// <summary>
        /// Id bút toán con
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Mã tham chiếu đến chứng từ (phiếu nhập, phiếu xuất,...)
        /// </summary>
        public string DocumentCode { get; set; }
        /// <summary>
        /// Mã tài khoản
        /// </summary>
        public string AccountCode { get; set; }
        /// <summary>
        /// Loại đối tượng
        /// </summary>
        public AudienceTypes? AudienceType { get; set; }


        public string Audience { get; set; }

        public DateTime? Start { get; set; } = null;

        public DateTime? End { get; set; } = null;

        public TicketTypesSearch? Type { get; set; }

    }
}
