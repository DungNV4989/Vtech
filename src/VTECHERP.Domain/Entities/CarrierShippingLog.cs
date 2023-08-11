using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTECHERP.Entities
{
    [Table("CarrierShippingLogs")]
    public class CarrierShippingLog : BaseEntity<Guid>
    {
        public string carrier { get; set; }
        public string dto { get; set; }
        public string data { get; set; }
        public int status { get; set; }
        public string response { get; set; }
        
    }
}
