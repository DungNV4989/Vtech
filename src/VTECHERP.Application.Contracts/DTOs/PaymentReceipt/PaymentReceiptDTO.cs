using System;
using System.Collections.Generic;
using System.Linq;
using VTECHERP.DTOs.Attachment;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.PaymentReceipt
{
    public class PaymentReceiptDTO: BaseDTO
    {
        public string Code { get; set; }
        /// <summary>
        /// Loại phiếu
        /// </summary>
        public TicketTypes TicketType { get; set; }
        /// <summary>
        /// Tên loại phiếu
        /// </summary>
        public string? TicketTypeName { get => Datas.MasterDatas.TicketTypes.First(x => x.Id == TicketType).Name; }
        /// <summary>
        /// Loại đối tượng
        /// </summary>
        public AudienceTypes AudienceType { get; set; }
        /// <summary>
        /// Id đối tượng
        /// </summary>
        public Guid? AudienceId { get; set; }
        public string AudienceCode { get; set; }
        public string AudienceName { get; set; }
        public string AudiencePhone { get; set; }
        public string? AudienceTypeName { get => Datas.MasterDatas.AudienceTypes.First(x => x.Id == AudienceType).Name; }
        /// <summary>
        /// Tài khoản
        /// </summary>
        public string AccountCode { get; set; }
        /// <summary>
        /// Tên tài khoản
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// Tài khoản đối ứng
        /// </summary>
        public string ReciprocalAccountCode { get; set; }
        /// <summary>
        /// Tên tài khoản đối ứng
        /// </summary>
        public string ReciprocalAccountName { get; set; }
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
        public AccountingTypes AccountingType { get; set; }
        public DocumentTypes? DocumentType { get; set; }
        public string? DocumentTypeName { get => Datas.DocumentTypesData.Datas.Where(x => x.DocumentTypes == this.DocumentType.Value).First().Name; }
        public DocumentDetailType? DocumentDetailType { get; set; }
        public string? DocumentDetailTypeName { get => DocumentDetailType != null ? Datas.DocumentDetailTypeData.Datas.First(x => x.DocumentDetailType == DocumentDetailType).Name : null; }
        public string? DocumentCode { get; set; }
        /// <summary>
        /// Link sản phẩm
        /// </summary>
        public List<DetailAttachmentDto> Attachments { get; set; }
    }
}
