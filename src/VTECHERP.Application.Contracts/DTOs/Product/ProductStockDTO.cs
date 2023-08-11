
using System;
using System.Collections.Generic;
using VTECHERP.DTOs.Base;

namespace VTECHERP.DTOs.Product
{
    public class SearchProductStockResponse: PagingResponse<StoreProductResponse>
    {
        public List<MasterDataDTO> Stores { get; set; }  
    }
    public class StoreProductResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<StoreProductStock> Stocks { get; set; }
    }
    public class StoreProductStock
    {
        public Guid StoreId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public int DeliveryQuantity { get; set; }
        public int CanSellQuantity { get; set; }
        public int HoldQuantity { get; set; }
        public int ComingQuantity { get; set; }
        public int PreOrderQuantity { get; set; }
    }
}
