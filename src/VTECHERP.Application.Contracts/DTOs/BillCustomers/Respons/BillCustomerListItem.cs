using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Attachment;
using VTECHERP.Enums;
using VTECHERP.Enums.Bills;

namespace VTECHERP.DTOs.BillCustomers.Respons
{
    public class BillCustomerListItem
    {
        /// <summary>
        /// Id hóa đơn
        /// </summary>
        public Guid BillCustomerId { get; set; }
        /// <summary>
        /// Id người tạo
        /// </summary>
        public Guid? CreatorId { get; set; }
        /// <summary>
        /// Tên người tạo
        /// </summary>
        public string CreatorText { get; set; }
        /// <summary>
        /// Mã hóa đơn
        /// </summary>
        public string BillCustomerCode { get; set; }
        /// <summary>
        /// Id cửa hàng
        /// </summary>
        public Guid? StoreId { get; set; }
        /// <summary>
        /// Tên cửa hàng
        /// </summary>
        public string StoreText { get; set; }
        /// <summary>
        /// Id khách hàng
        /// </summary>
        public Guid? CustomerId { get; set; }
        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string CustomerText { get; set; }
        /// <summary>
        /// Trạng thái hóa đơn
        /// </summary>
        public CustomerBillPayStatus? BillCustomerPayStatus { get; set; }
        /// <summary>
        /// Trạng thái hóa đơn text
        /// </summary>
        public string BillCustomerPayStatusText { get; set; }
        /// <summary>
        /// vat 
        /// </summary>
        public decimal? VatValue { get; set; }
        /// <summary>
        /// đơn vị vat
        /// </summary>
        public MoneyModificationType? VatUnit { get; set; }
        /// <summary>
        /// triết khẩu 
        /// </summary>
        public decimal? DiscountValue { get; set; }
        /// <summary>
        /// Đơn vị triết khấu
        /// </summary>
        public MoneyModificationType? DiscountUnit { get; set; }
        public decimal AmountTotal { get; set; }
        public string PayNote { get; set; }
        /// <summary>
        /// Danh sách sản phẩm
        /// </summary>
        public List<BillCustomerProductDto> BillCustomerProducts { get; set; }
        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// Tổng tiền sau triết khấu
        /// </summary>
        public decimal? AmountAfterDiscount { get; set; }
        /// <summary>
        /// Tổng tiền khách cần trả
        /// </summary>
        public decimal? AmountCustomerPay { get; set; }
        public decimal Cash { get; set; }
        public decimal Banking { get; set; }
        /// <summary>
        /// Hình thức vận chuyển
        /// </summary>
        public TransportForm TransportForm { get; set; }
        public string EmployeeNote { get; set; }
        /// <summary>
        /// Số tiền mặt được triết khấu của hóa đơn
        /// </summary>
        public decimal DiscountCash { get; set; }

        /// <summary>
        /// Link sản phẩm
        /// </summary>
        public List<DetailAttachmentDto> Attachments { get; set; }
    }
}
