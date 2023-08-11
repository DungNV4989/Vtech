using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.Promotions;
using VTECHERP.Helper;
using VTECHERP.ServiceInterfaces;

namespace VTECHERP.Applications.CustomerSale
{
    [Route("api/app/promotion/[action]")]
    [Authorize]
    public class PromotionApplication: ApplicationService
    {
        private readonly IPromotionService _promotionService;
        public PromotionApplication(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpPost]
        public async Task<PagingResponse<PromotionDTO>> Search(SearchPromotionRequest request)
        {
            return await _promotionService.SearchPromotin(request);
        }
        [HttpPost]
        public async Task<PagingResponse<VoucherDTO>> SearchVoucherByPromotinId(Guid promotionId,SearchVoucherRequest request)
        {
            return await _promotionService.SearchVoucherByPromotinId(promotionId, request);
        }
        [HttpPost]
        public async Task<PagingResponse<ProductInPromotionDTO>> SearchProductByPromotinId(Guid promotionId, SearchProductInPromotionRequest request)
        {
            return await _promotionService.SearchProductByPromotinId(promotionId, request);
        }
        [HttpGet]
        public async Task<DetailPromotionDTO> GetPromotionById(Guid Id)
        {
            return await _promotionService.GetPromotionById(Id);
        }

        [HttpPost]
        public async Task<bool> Create(CreatePromotionRequest request)
        {
            await _promotionService.CreatePromotion(request);
            return true;
        }

        [HttpPost]
        public async Task<bool> Update(UpdatePromotionRequest request)
        {
            await _promotionService.UpdatePromotion(request);
            return true;
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var resuilt= await _promotionService.DeletePromotion(id);
            if(resuilt)
            {
                return new GenericActionResult();
            }
            else
            {
                return new GenericActionResult(200, false,"Không thể xóa chương trình do đã có voucher được sử dụng",null);
            } 
                
        }
    }
}
