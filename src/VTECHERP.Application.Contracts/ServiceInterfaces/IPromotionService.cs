using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.Promotions;

namespace VTECHERP.ServiceInterfaces
{
    public interface IPromotionService: IScopedDependency
    {
        Task<PagingResponse<PromotionDTO>> SearchPromotin(SearchPromotionRequest request);
        Task<PagingResponse<VoucherDTO>> SearchVoucherByPromotinId(Guid promotionId, SearchVoucherRequest request);
        Task<PagingResponse<ProductInPromotionDTO>> SearchProductByPromotinId(Guid promotionId, SearchProductInPromotionRequest request);
        Task<DetailPromotionDTO> GetPromotionById(Guid Id);
        Task CreatePromotion(CreatePromotionRequest request);
        Task UpdatePromotion(UpdatePromotionRequest request);
        Task<bool> DeletePromotion(Guid id);
    }
}
