using System;

namespace VTECHERP.DTOs.DebtReminderLogs
{
    public class DebtReminderLogCreateRequest
    {
        /// <summary>
        /// Ngày hẹn trả
        /// </summary>
        public DateTime? PayDate { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid? CustomerId { get; set; }

    }
}