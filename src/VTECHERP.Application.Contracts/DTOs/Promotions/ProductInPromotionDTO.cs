using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.Promotions
{
    public class ProductInPromotionDTO
    {
        public Guid Id { get; set; }
        public string SequenceId { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
    }
}
