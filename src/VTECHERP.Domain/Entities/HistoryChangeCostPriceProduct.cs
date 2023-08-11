using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using VTECHERP.Enums;

namespace VTECHERP.Entities
{
    public class HistoryChangeCostPriceProduct : BaseEntity<Guid>, IMultiTenant
    {
        public Guid ProductId { get; set; }
        public decimal CostPriceNew { get; set; }
        public decimal CostPriceOld { get; set; }
        public decimal ProfitDecrease { get; set; }
        public ChangeCostPriceProductType? Type { get; set; }
    }
}
