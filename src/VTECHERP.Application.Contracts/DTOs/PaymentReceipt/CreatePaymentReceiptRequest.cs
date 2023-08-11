using System;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.PaymentReceipt
{
    public class CreatePaymentReceiptRequest
    {
        /// <summary>
        /// Loại phiếu
        /// </summary>
        public TicketTypes TicketType { get; set; }
        /// <summary>
        /// Loại đối tượng
        /// </summary>
        public AudienceTypes AudienceType { get; set; }
        /// <summary>
        /// Id đối tượng
        /// </summary>
        public Guid? AudienceId { get; set; }
        /// <summary>
        /// Tài khoản
        /// </summary>
        public string AccountCode { get; set; }
        /// <summary>
        /// Tài khoản đối ứng
        /// </summary>
        public string ReciprocalAccountCode { get; set; }
        /// <summary>
        /// Số tiền VN
        /// </summary>
        public decimal? AmountVND { get; set; }
        /// <summary>
        /// Số tiền TQ
        /// </summary>
        public decimal? AmountCNY { get; set; }
        /// <summary>
        /// Ngày giao dịch
        /// </summary>
        public DateTime TransactionDate { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
    }
}
