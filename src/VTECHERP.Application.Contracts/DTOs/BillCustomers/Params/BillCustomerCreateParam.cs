using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Attachment;
using VTECHERP.DTOs.BillCustomers.Respons;
using VTECHERP.Enums;
using VTECHERP.Enums.Bills;

namespace VTECHERP.DTOs.BillCustomers.Params
{
    public class BillCustomerCreateParam : BillCustomerDto
    {
        public string CustomerName { get; set; }
        /// <summary>
        /// Loại khách
        /// </summary>
        public CustomerType? CustomerType { get; set; }
        /// <summary>
        /// Tỉnh/thành
        /// </summary>
        public Guid? ProvinceId { get; set; }
        public string CustomerPhone { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public string NoteForProductBonus { get; set; }
        public List<BillCustomerProductDto> BillCustomerProducts { get; set; } = new List<BillCustomerProductDto>();
        public string VoucherCode { get; set; }
    }

    public class BillCustomerProductDto : BillCustomerProductItem
    {
        public List<BillCustomerProductBonusDto> ProductBonus { get; set; } = new List<BillCustomerProductBonusDto>();
        public List<BillCustomerProductItem> ProductChildren { get; set; } = new List<BillCustomerProductItem>();
        public List<DetailAttachmentDto> Attachments { get; set; }
    }

    public class BillCustomerProductItem
    {
        public Guid? Id { get; set; }
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public decimal? Price { get; set; }
        public int Quantity { get; set; }
        public decimal? DiscountValue { get; set; }
        public MoneyModificationType? DiscountUnit { get; set; }
        public Guid? ParentId { get; set; }
        /// <summary>
        /// Số lượng tồn
        /// </summary>
        public int Inventory { get; set; } = 0;
        /// <summary>
        /// Giá vốn
        /// </summary>
        public decimal? CostPrice { get; set; } = 0;
        public string Note { get; set; }
    }
}
