using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTECHERP.Enums;
using VTECHERP.Enums.Product;

namespace VTECHERP.Entities
{
    [Table("PaymentReceipts")]
    public class PaymentReceipt: BaseEntity<Guid>
    {
        /// <summary>
        /// Mã tự sinh
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Id cửa hàng
        /// </summary>
        public Guid StoreId { get; set; }
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
        /// Tài khoản kế toán
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
        public ActionSources Source { get; set; }
        public Guid? SourceId { get; set; }
        public DocumentTypes? DocumentType { get; set; }
        public DocumentDetailType? DocumentDetailType { get; set; }
        public string? DocumentCode { get; set; }
        public bool IsFromWarehousingBill { get; set; } = false;
        public bool IsFromManualEntry { get; set; } = false;
        public AccountingTypes AccountingType { get; set; }
        /// <summary>
        /// Trạng thái thanh khoản(True: Đã thanh khoản; False: chưa thanh khoản)
        /// </summary>
        public bool IsLiquidity { get; set; } = false;
    }
}
