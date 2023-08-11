using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums.Product;

namespace VTECHERP.Reports.Param
{
    public class ReportRevenueProductParam : BasePagingRequest
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public List<Guid> StoreIds { get; set; } = new List<Guid>();
        public List<Guid> CategoryIds { get; set; } = new List<Guid>();
        public Guid? ProductId { get; set; }
        public ProductStatus? ProductStatus { get; set; }
        public ProfitType? ProfitType { get; set; }
        public List<Guid> Tenants { get; set; } = new List<Guid>();
    }

    public enum ProfitType
    {
        Positive,
        Minus
    }
}
