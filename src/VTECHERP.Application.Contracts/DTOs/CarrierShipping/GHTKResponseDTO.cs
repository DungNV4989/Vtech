using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.CarrierShipping
{
    public class GHTKResponseDTO
    {
        public bool success { get; set; }
        public string? message { get; set; }
        public GHTKResponseOrderDTO? order { get; set; }
        public GHTKResponseErrorDTO? error { get; set; }

    }
    public class GHTKResponseOrderDTO 
    {
        public string partner_id { get; set; }
        public string label { get; set; }
        public string area { get; set; }
        public string fee { get; set; }
        public string insurance_fee { get; set; }
        public string estimated_pick_time { get; set; }
        public string estimated_deliver_time { get; set; }
        public string status_id { get; set; }
    }
    public class GHTKResponseErrorDTO
    {
        public string code { get; set; }
        public string partner_id { get; set; }
        public string ghtk_label { get; set; }
        public string created { get; set; }
        public string status { get; set; }
    }
}
