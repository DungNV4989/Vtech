using System;
using System.Collections.Generic;

namespace VTECHERP.DTOs.DebtReminderLogs
{
    public class DebtReminderLogDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public Guid CreatorId { get; set; }
        public string CreateName { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? PayDate { get; set; }
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public Guid? HandlerStoreIds { get; set; }
        public string HandlerStoreNames { get; set; }
        public Guid? HandlerEmployeeId { get; set; }
        public string HandlerEmployeeName { get; set; }
        public string Content { get; set; }
    }
}