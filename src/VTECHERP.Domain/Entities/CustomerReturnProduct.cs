using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using VTECHERP.Enums;

namespace VTECHERP.Entities
{
    public class CustomerReturnProduct : BaseEntity<Guid>, IMultiTenant
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? CustomerReturnId { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public decimal? DiscountValue { get; set; }
        public MoneyModificationType? DiscountUnit { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? TotalPriceAfterDiscount { get; set; }
        /// <summary>
        /// Đánh dấu hàng lỗi
        /// </summary>
        public bool? IsError { get; set; }
        public decimal? StockPrice { get; set; }
        public string Note { get; set; }
    }
}
