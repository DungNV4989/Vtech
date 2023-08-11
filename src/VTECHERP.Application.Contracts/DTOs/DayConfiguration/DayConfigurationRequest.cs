using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.DayConfiguration
{
    public class DayConfigurationRequest
    {
        public int? DayNumbers { get; set; }
        public int? NumberOfDayAllowDeleteEntry { get; set; }
        public int? NumberOfDayAllowCreatePayRecieve { get; set; }
    }
}
