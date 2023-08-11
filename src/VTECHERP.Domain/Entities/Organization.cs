using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VTECHERP.Entities
{
    /// <summary>
    /// Doanh nghiệp
    /// </summary>
    [Table("Organizations")]
    public class Organization: BaseEntity<Guid>
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
