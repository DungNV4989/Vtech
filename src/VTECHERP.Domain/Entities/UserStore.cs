using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VTECHERP.Entities
{
    [Table("UserStores")]
    public class UserStore: BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public Guid StoreId { get; set; }
        public bool IsDefault { get; set; }
    }
}
