using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums.CarrierShipping;

namespace VTECHERP.DTOs.CarrierShipping
{
    public class UpdateShippingGHTKDTO
    {
        public string label_id {  get; set; }
        public string partner_id {  get; set; }
        public GHTKStatus status_id {  get; set; }
        public string action_time {  get; set; }
        public string reason_code {  get; set; }
        public string reason {  get; set; }
        public decimal weight {  get; set; }
        public int fee {  get; set; }
        public int return_part_package {  get; set; }
    }
}
