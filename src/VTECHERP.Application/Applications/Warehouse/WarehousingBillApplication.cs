using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.TenantManagement;
using VTECHERP.Constants;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.ProductXnk;
using VTECHERP.DTOs.WarehousingBills;
using VTECHERP.Services;

namespace VTECHERP
{
    [Route("api/app/warehousing-bill/[action]")]
    [Authorize]
    public class WarehousingBillApplication : ApplicationService
    {
        private readonly IWarehousingBillService _warehousingBillService;
        private readonly ITenantRepository _tenantRepository;
        public WarehousingBillApplication(
            IWarehousingBillService warehousingBillService,
            ITenantRepository tenantRepository)
        {
            _warehousingBillService = warehousingBillService;
            _tenantRepository = tenantRepository;
        }

        [HttpPost]
        public async Task<PagingResponse<WarehousingBillDto>> Search(SearchWarehousingBillRequest request)
        {
            return await _warehousingBillService.SearchBills(request);
        }

        [HttpGet]
        public async Task<WarehousingBillDto> Get(Guid id)
        {
            return await _warehousingBillService.GetBill(id);
        }

        [HttpPost]
        public async Task<bool> Create(CreateWarehousingBillRequest request)
        {
            await _warehousingBillService.CreateBill(request);
            return true;
        }

        [HttpPut]
        public async Task<bool> Update(UpdateWarehousingBillRequest request)
        {
            await _warehousingBillService.UpdateBill(request);
            return true;
        }

        [HttpDelete]
        public async Task<bool> Delete(Guid id)
        {
            await _warehousingBillService.DeleteBill(id);
            return true;
        }

        [HttpPost]
        public async Task<PagingResponse<SearchProductXnkResponse>> SearchProductXnk(SearchProductXnkRequest request)
        {
            return await _warehousingBillService.SearchProductXnk(request);
        }


        [HttpPost]
        public async Task<bool> UpdateProductInBillProduct(UpdateProductInBillProductRequest request)
        {
            await _warehousingBillService.UpdateProductInBillProduct(request);
            return true;
        }

        [HttpGet]
        public async Task<GetUpdateProductInBillProductResponse> GetUpdateProductInBillProduct(Guid warehousingBillId)
        {
            return await _warehousingBillService.GetUpdateProductInBillProduct(warehousingBillId); 
        }


        [HttpDelete]
        public async Task<bool> DeleteProductInBillProduct(Guid id)
        {
            await _warehousingBillService.DeleteProductInBillProduct(id);   
            return true;
        }

        [HttpPost]
        public async Task<bool> DeleteListProductInBillProduct(DeleteListProductInBillProduct request)
        {
            await _warehousingBillService.DeleteListProductInBillProduct(request);
            return true;
        }

        //[HttpPut]
        //[AllowAnonymous]
        //public async Task<bool> RebuildProductStock()
        //{
        //    var tenants = await _tenantRepository.GetListAsync();
        //    foreach (var tenant in tenants)
        //    {
        //        using (CurrentTenant.Change(tenant.Id))
        //        {
        //            await _warehousingBillService.RebuildProductStock();
        //        }
        //    }
        //    return true;
        //}

        [HttpPost]
        public async Task<FileResult> ExportWarehousingBillAsync(SearchWarehousingBillRequest request)
        {
            try
            {
                var file = await _warehousingBillService.ExportWarehousingBill(request);
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"DanhSach_Phiếu xuất nhập kho_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
                };
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                throw;
            }
        }
        [HttpPost]
        public async Task<FileResult> ExportProductXnkAsync(SearchProductXnkRequest request)
        {
            try
            {
                var file = await _warehousingBillService.ExportProductXnk(request);
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"DanhSach_Sản phẩm xuất nhập kho_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
                };
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                throw;
            }
        }
    }
}
