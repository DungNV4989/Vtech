using System;
using System.Collections.Generic;
using VTECHERP.DTOs.Base;

namespace VTECHERP.DTOs.DebtReminderLogs
{
    public class SearchDebtReminderLogRequest : BasePagingRequest
    {
        public DateTime? PayDateFrom { get; set; }
        public DateTime? PayDateTo { get; set; }
        public string Code { get; set; }
        public Guid? CustomerId { get; set; }
        public List<Guid> HandlerStoreIds { get; set; }
        public Guid? HandlerEmployeeId { get; set; }
        public Guid? CreatorId { get; set; }
        
    }
}