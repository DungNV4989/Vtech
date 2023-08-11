using DocumentFormat.OpenXml.Math;
using System;

namespace VTECHERP.DTOs.Base
{
    public class MasterDataDTO
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Value { get
            {
                if (Code.IsNullOrEmpty() && Phone.IsNullOrEmpty())
                {
                    return $"{Name}";
                }
                if (Code.IsNullOrEmpty())
                {
                    return $"{Name} - {Phone}";
                }
                if (Phone.IsNullOrEmpty())
                {
                    return $"{Code} - {Name}";
                }
                return $"{Code} - {Name} - {Phone}";
            }
        }

    }
}