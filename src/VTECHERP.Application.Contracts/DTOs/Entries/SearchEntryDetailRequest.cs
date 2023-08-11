using System;
using System.Collections.Generic;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.Entries
{
    public class SearchEntryDetailRequest : BasePagingRequest
    {
        /// <summary>
        /// Id cửa hàng
        /// </summary>
        public List<Guid> StoreIds { get; set; }

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

        public DateTime? Start { get; set; } = null;

        public DateTime? End { get; set; } = null;

        /// <summary>
        /// Loại Đối tượng
        /// </summary>
        public AudienceTypes? AudienceType { get; set; }

        /// <summary>
        /// Đối tượng
        /// </summary>
        public string Audience { get; set; }
    }
}
