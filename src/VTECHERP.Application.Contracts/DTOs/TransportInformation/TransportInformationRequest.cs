using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.TransportInformation
{
    public class TransportInformationRequest
    {
        public Guid? TransportInformationId { get; set; }
        public Guid? CustomerId { get; set; }
    }
}
