using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VTECHERP.Entities
{
    public class ProductStockView: IMultiTenant, IEntity<Guid>
    {
        public Guid? TenantId { get; set; }
        public Guid? StoreId { get; set; }
        public int StockQuantity { get; set; }
        public int ComingQuantity { get; set; }
        public int CustomerOrderQuantity { get; set; }
        public int DeliveryQuantity { get; set; }
        public int HoldQuantity { get; set; }
        public int CanSellQuantity { get; set; }
        public Guid Id { get; set; }
        public object[] GetKeys()
        {
            throw new NotImplementedException();
        }
    }
}
