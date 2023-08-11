using Microsoft.AspNetCore.Routing.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using VTECHERP.Enums;
using VTECHERP.Enums.Bills;

namespace VTECHERP.Entities
{
    public class BillCustomerProduct : BaseEntity<Guid>, IMultiTenant
    {
        public string Code { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? BillCustomerId { get; set; }
        public decimal? Price { get; set; }
        public int Quantity { get; set; }
        public Guid? TablePriceId { get; set; }
        public decimal? DiscountValue { get; set; }
        public MoneyModificationType? DiscountUnit { get; set; }
        public Guid? ParentId { get; set; }
        /// <summary>
        /// Giá vốn
        /// </summary>
        public decimal? CostPrice { get; set; }
        public decimal DiscountValueCash { get; set; }
        public decimal VoucherValueCash { get; set; }
        public Guid? ProductCategoryId { get; set; }
        public string Note { get; set; }
    }
}
