using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTECHERP.Entities
{
    [Table("IntegratedCassoLogs")]
    public class IntegratedCassoLog : BaseEntity<Guid>
    {
        public Guid? AudienceId { get; set; }
        public string Data { get; set; }
        public int Status { get; set; }
        public string? Log { get; set; }
        public int CassoId { get; set; }
        public DateTime? When { get; set; }
        public string? tid { get; set; }
        public string? amount { get; set; }
        public string? description { get; set; }
    }
}
