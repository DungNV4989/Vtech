using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.BillCustomers.Respons
{
    /// <summary>
    /// Dto cho màn hình xem chi tiết
    /// </summary>
    public class BillCustomerDetailById
    {
        /// <summary>
        /// Id hóa đơn
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Mã hóa đơn
        /// </summary>
        public string BillCustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        /// <summary>
        /// Tổng tiền
        /// </summary>
        public decimal? AmountTotal { get; set; }
        /// <summary>
        /// Khách còn nợ
        /// </summary>
        public decimal? CustomerDebt { get; set; }
        /// <summary>
        /// Tên cửa hàng
        /// </summary>
        public string StoreText { get; set; }
        /// <summary>
        /// Người tạo hóa đơn
        /// </summary>
        public string CreatorText { get; set; }
        public DateTime? CreateTime { get; set; }
        public List<TransportInformationLogDto> TransportInformationLogs { get; set; } = new List<TransportInformationLogDto>();
        public List<BillCustomerAttachment> Attachments { get; set; } = new List<BillCustomerAttachment>();
        /// <summary>
        /// Số tiền voucher trừ trong hóa đơn
        /// </summary>
        public decimal BillVoucherDiscountValue { get; set; }
        public string VoucherCode { get; set; }
    }

    public class BillCustomerProductDetail
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountValue { get; set; }
        public MoneyModificationType? DiscountUnit { get; set; }
        public decimal? Total { get; set; }
    }

    public class BillCustomerEntries
    {
        /// <summary>
        /// Mã bút toán
        /// </summary>
        public string EntryCode { get; set; }
        /// <summary>
        /// Ngày giao dịch
        /// </summary>
        public DateTime? TransactionDate { get; set; }
        public TicketTypes TicketType { get; set; }
        /// <summary>
        /// Tên loại phiếu
        /// </summary>
        public string? TicketTypeName { get => Datas.MasterDatas.TicketTypes.First(x => x.Id == TicketType).Name; }
        /// <summary>
        /// Tổng tiền
        /// </summary>
        public decimal? Amount { get; set; }
        /// <summary>
        /// Tài khoản nợ
        /// </summary>
        public string Debt { get; set; }
        /// <summary>
        /// Tài khoản có
        /// </summary>
        public string Credit { get; set; }
        public string Note { get; set; }
    }

    public class TransportInformationLogDto
    {
        public Guid? ShipperId { get; set; }
        /// <summary>
        /// Tên shipper
        /// </summary>
        public string ShipperText { get; set; }
        public TransportStatus Status { get; set; }
        /// <summary>
        /// Trạng thái đơn vận chuyển
        /// </summary>
        public string StatusText { get; set; }
        /// <summary>
        /// Mã đơn vận chuyển
        /// </summary>
        public string TransportInformationCode { get; set; }
        /// <summary>
        /// Thời gian giao hàng
        /// </summary>
        public DateTime? ShipTime { get; set; }
        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime? CreateTime { get; set; }
    }

    public class BillCustomerAttachment
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string TypeFile { get; set; }

    }

    public class TypeFile
    {
        private TypeFile() {}

        public const string Pdf = "application/pdf";
        public const string Excel = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public const string Word = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
    }    
}
