using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.CarrierShipping
{
    public class PriceDTO
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
        public int NationalType { get; set; }
        public string OrderService { get; set; }
        public string OrderServiceAdd { get; set; }
    }
    public class PriceVNPDTO
    {
        public Guid StoreId { get; set; }
        public string Scope { get; set; }
        public string CustomerCode { get; set; }
        public string ContractCode { get; set; }
        public PriceVNPData data { get; set; }
    }
}
