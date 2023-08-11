using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Attachment;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.ExchangeReturn
{
    public class CustomerReturnProductDTO
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? CustomerReturnId { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public decimal? DiscountValue { get; set; } = 0;
        public MoneyModificationType? DiscountUnit { get; set; }
        public bool? IsError { get; set; }
        /// <summary>
        /// Link sản phẩm
        /// </summary>
        public List<DetailAttachmentDto> Attachments { get; set; }
        public decimal StockPrice { get; set; }
        public string Note { get; set; }
        public decimal? TotalPriceAfterDiscount { get; set; }
    }
}
