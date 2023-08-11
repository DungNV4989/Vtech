using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VTECHERP.Entities
{
    [Table("DebtReminderLogs")]
    public class DebtReminderLog : BaseEntity<Guid>
    {
        public string Code { get; set; }

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