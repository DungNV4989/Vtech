using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.Debts
{
    public class SearchDebtDetailRequest : BasePagingRequest
    {
        /// <summary>
        /// Id nhà cung cấp
        /// </summary>
        public Guid? SupplierId { get; set; }
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

        public DateTime? Start { get; set; } = null;

        public DateTime? End { get; set; } = null;


    }
}
