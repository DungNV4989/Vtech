using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums;

namespace VTECHERP.Debts
{
    public class CreateOrUpdateDebtDto
    {
        /// <summary>
        /// Id giao dịch
        /// </summary>
        public Guid TransactionId { get; set; }
        /// <summary>
        /// Mã KH
        /// </summary>
        public Guid? CustomerId { get; set; }
        /// <summary>
        /// Mã nhà cung cấp
        /// </summary>
        public Guid? SupplierId { get; set; }
        /// <summary>
        /// Người phụ trách
        /// </summary>
        public Guid EmployeeId { get; set; }
        /// <summary>
        /// Có
        /// </summary>
        public decimal Credits { get; set; }
        /// <summary>
        /// Nợ
        /// </summary>
        public decimal Debts { get; set; }
        /// <summary>
        /// Chứng từ
        /// </summary>
        public string Document { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// Đối tượng
        /// </summary>
        public AudienceTypes? AudienceType { get; set; }
        /// <summary>
        /// Ngày thu chi
        /// </summary>
        public DateTime TransactionDate { get; set; }
        /// <summary>
        /// Loại phiếu
        /// </summary>
        public TicketTypes? TicketType { get; set; }
    }
}
