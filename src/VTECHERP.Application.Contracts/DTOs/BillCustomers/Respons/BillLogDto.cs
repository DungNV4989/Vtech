using System;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.BillCustomers.Respons
{
    public class BillLogDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTime? CreationTime { get; set; }
        public BillLogType BillLogType { get; set; }
        public decimal? DiscountValue { get; set; }
        public MoneyModificationType? DiscountUnit { get; set; }
        public decimal? AmountCustomerPay { get; set; }
    }
}