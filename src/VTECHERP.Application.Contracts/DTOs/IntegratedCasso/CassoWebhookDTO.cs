using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.IntegratedCasso
{
    public class CassoWebhookDTO
    {
        public int error { get; set; }
        public List<IntegratedCassoDTO> data { get; set; }
    }
}
