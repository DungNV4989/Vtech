using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.CarrierShipping
{
    public class PriceAllDTO
    {
        public Guid StoreId { get; set; }
        public int SenderProvince { get; set; }
        public int SenderDistrict { get; set; }
        public int ReceiverDistrict { get; set; }
        public int ReceiverProvince { get; set; }
        public string ProductType { get; set; }
        public int ProductWeight { get; set; }
        public int? ProductPrice { get; set; }
        public int MoneyCollection { get; set; }
        public int ProductLengt { get; set; }
        public int ProductWidth { get; set; }
        public int ProductHeight { get; set; }
        public int Type { get; set; }
    }
}
