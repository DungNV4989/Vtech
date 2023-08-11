using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace VTECHERP.Entities
{
    public class BatchStatus : Entity<Guid>
    {
        public string BatchName { get; set; }

        public int Status { get; set; }
    }
}
