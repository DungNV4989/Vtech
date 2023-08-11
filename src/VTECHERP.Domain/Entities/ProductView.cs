using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace VTECHERP.Entities
{
    public class ProductView : IMultiTenant, IEntity<Guid>
    {
        public Guid? TenantId { get; set; }
        public Guid? StoreId { get; set; }
        public int TotalInventory { get; set; }
        public int Inventory { get; set; }
        public int Coming { get; set; }
        public int Booking { get; set; }
        public int Delivery { get; set; }
        public int TemporarilyHold { get; set; }
        public int SellNumber { get; set; }
        public Guid Id { get; set; }
        public object[] GetKeys()
        {
            throw new NotImplementedException();
        }
    }
}
