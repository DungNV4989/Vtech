using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.Reports
{
    public class RequestDetail 
    {
        /// <summary>
        /// danh sách Id doanh nghiệp
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// danh sách Id cửa hàng 
        /// </summary>
        public TicketTypesSearch? Type { get; set; }
    }
}
