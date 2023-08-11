using System.Collections.Generic;
using VTECHERP.DTOs.Base;

namespace VTECHERP.DTOs.PriceTableProduct.Respon
{
    public class SearchProductPriceResponse: PagingResponse<ProductPriceDetail>
    {
        public List<MasterDataDTO> PriceTables { get; set; }
    }
}
