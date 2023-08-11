using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using VTECHERP.Constants;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.SaleOrderLines;
using VTECHERP.DTOs.SaleOrderLines.Params;
using VTECHERP.Entities;
using VTECHERP.Helper;
using VTECHERP.Services;

namespace VTECHERP
{
    [Authorize]
    public class SaleOrderLineApplication : ApplicationService
    {
        private readonly ISaleOrderLineService _saleOrderLineService;
        private readonly IRepository<SaleOrderLines> _saleOrderLineRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IIdentityUserRepository _userRepository;
        public SaleOrderLineApplication(ISaleOrderLineService saleOrderLineService
            , IRepository<SaleOrderLines> saleOrderLineRepository
            , ICurrentUser currentUser
            , IIdentityUserRepository userRepository
            )
        {
            _saleOrderLineService = saleOrderLineService;
            _saleOrderLineRepository = saleOrderLineRepository;
            _currentUser = currentUser;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<object> GetList(SaleOrderLineGetListParam param)
        {
            var saleOrders = await _saleOrderLineService.GetListFull(param);

            var count = saleOrders.Count();
            var result = saleOrders.Skip((param.PageIndex - 1) * param.PageSize).Take(param.PageSize).ToList();
            var currentUser = (await _userRepository.FindAsync(_currentUser.GetId()));
            //return new PagingResponse<SaleOrderListItem>(count, result);
            return new { IsVTech = currentUser.GetProperty("IsVTech", false), Total = count, Data = result };
        }
        [HttpPost]
        public async Task<IActionResult> Export(SaleOrderLineGetListParam param)
        {
            try
            {
                var file = await _saleOrderLineService.ExportAsync(param);
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"danhsachsanphamphieudathang_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
                };
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid SaleOrderLineId, SaleOrderLineUpdateParam param)
        {
            var saleOrderLine = await _saleOrderLineRepository.FindAsync(x => x.Id == SaleOrderLineId);
            if (saleOrderLine == null)
                return new GenericActionResult(400, false, $"Không tìm thấy sản phẩm phiếu đặt hàng id là ${SaleOrderLineId}");

            saleOrderLine.SuggestedPrice = param.SuggestPrice;
            await CurrentUnitOfWork.SaveChangesAsync();

            return new GenericActionResult(200, true, $"Cập nhật thành công", saleOrderLine);
        }
    }
}