using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.GHTK
{
    public class CreateOrderGHTKResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public OrderGHTKResponse? order { get; set; }
    }
    public class OrderGHTKResponse
    {
        public string? partner_id { get; set; }
        public string? label { get; set; }
        public string? area { get; set; }
        public string? ghtk_label { get; set; }
        public string? fee { get; set; }
        public string? insurance_fee { get; set; }
        public string? estimated_pick_time { get; set; }
        public string? estimated_deliver_time { get; set; }
        public string? created { get; set; }
        public List<object>? products { get; set; }
        public int? status_id { get; set; }
        public int? status { get; set; }

    }
}
