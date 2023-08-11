using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace VTECHERP.Entities
{
    [Table("Employees")]
    public class Employee : BaseEntity<Guid>, IMultiTenant
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public Guid? UserId { get; set; }
    }
}
