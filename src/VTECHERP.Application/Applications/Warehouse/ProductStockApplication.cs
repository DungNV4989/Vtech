using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using VTECHERP.Constants;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.Customer;
using VTECHERP.DTOs.Product;
using VTECHERP.Entities;
using VTECHERP.Enums.Product;
using VTECHERP.ServiceInterfaces;

namespace VTECHERP
{
    [Route("api/app/product-stock/[action]")]

    public class ProductStockApplication : ApplicationService
    {
        private readonly IProductStockService _productStockService;

        public ProductStockApplication(
            IProductStockService productStockService
            )
        {
            _productStockService = productStockService;
        }

        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(PagingResponse<SearchProductStockResponse>))]
        public async Task<SearchProductStockResponse> Search(SearchProductStockRequest request)
        {
            try
            {
                var result = await _productStockService.Search(request);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

        }

        [HttpPost]
        public async Task<FileResult> ExportProductStockAsync(SearchProductExcelRequest request)
        {
            try
            {
                var file = await _productStockService.ExportProductStockAsync(request);
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"DanhSach_Tồn kho_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
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
