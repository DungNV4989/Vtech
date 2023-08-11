using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.TenantManagement;
using VTECHERP.Constants;
using VTECHERP.DTOs.SaleOrders;
using VTECHERP.Entities;
using VTECHERP.Helper;
using VTECHERP.Services;

namespace VTECHERP
{
    [Route("api/app/SaleOrder/[action]")]
    [Authorize]
    public class SaleOrderApplication : ApplicationService
    {
        private readonly ISaleOrderService _saleOrderService;
        private readonly IRepository<SaleOrders> _saleOrderRepository;
        private readonly IRepository<OrderTransportSale> _orderTransportSaleRepository;
        private readonly IRepository<Suppliers> _suplierRepository;
        private readonly ISaleOrderLineService _saleOrderLineService;
        private readonly ITenantRepository _tenantRepository;

        public SaleOrderApplication(ISaleOrderService saleOrderService
            , ISaleOrderLineService saleOrderLineService
            , IRepository<SaleOrders> saleOrderRepository
            , IRepository<Suppliers> suplierRepository
            , ITenantRepository tenantRepository
            , IRepository<OrderTransportSale> orderTransportSaleRepository
            )
        {
            _saleOrderService = saleOrderService;
            _saleOrderLineService = saleOrderLineService;
            _saleOrderRepository = saleOrderRepository;
            _suplierRepository = suplierRepository;
            _tenantRepository = tenantRepository;
            _orderTransportSaleRepository = orderTransportSaleRepository;
        }

        [HttpPost]
        public async Task<object> AddAsync(SaleOrderCreateRequest request)
        {
            return await _saleOrderService.AddAsync(request);
        }

        //Get danh sách đơn hàng
        [HttpPost]
        public async Task<object> GetListAsync(SearchSaleOrderRequest request)
        {
            return await _saleOrderService.GetListAsync(request);
        }

        [HttpPost]
        public async Task<IActionResult> Export(SearchSaleOrderRequest request)
        {
            var file = await _saleOrderService.ExportAsync(request);
            return new FileContentResult(file, ContentTypes.SPREADSHEET)
            {
                FileDownloadName = $"Don_dat_hang_NCC_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
            };
        }

        [HttpGet]
        public async Task<object> GetByIdAsync(Guid id)
        {
            var x = await _saleOrderService.GetByIdAsync(id);
            return x;
        }

        [HttpPut]
        public async Task<object> UpdateAsync(SaleOrderUpdateRequest request)
        {
            return await _saleOrderService.UpdateAsync(request);
        }

        [HttpDelete]
        public async Task<bool> Delete(Guid id)
        {
            await _saleOrderService.Delete(id);
            return true;
        }

        [HttpGet]
        public async Task<bool> Complete(Guid id)
        {
            await _saleOrderService.Complete(id);
            return true;
        }

        [HttpGet]
        public async Task<GetDetailConfirmByIdResponse> GetDetailConfirmById(Guid id)
        {
            return await _saleOrderService.GetDetailConfirmById(id);
        }

        [HttpPost]
        public async Task<bool> Confirm(SaleOrderConfirmRequest request)
        {
            await _saleOrderService.Confirm(request);
            return true;
        }

        [HttpGet]
        public async Task<IActionResult> SearchByCode(string code, Guid? OrderTransportId)
        {
            var result = new List<SaleOrderDropList>();

            var saleOrderId = new List<Guid>();
            if (OrderTransportId.HasValue)
            {
                var saleOrder = await _orderTransportSaleRepository.GetListAsync(x => x.OrderTransportId == OrderTransportId);
                saleOrderId = saleOrder.Select(x => x.OrderSaleId).ToList();
            }

            var saleOrders = await _saleOrderRepository.GetQueryableAsync();
            var supliers = await _suplierRepository.GetQueryableAsync();

            result = (from sale in saleOrders

                      join sup in supliers
                      on sale.SupplierId equals sup.Id
                      into supGr
                      from sup in supGr.DefaultIfEmpty()

                      where !saleOrderId.Contains(sale.Id)

                      select new SaleOrderDropList
                      {
                          Id = sale.Id,
                          Code = sale.Code,
                          InvoiceNumber = sale.InvoiceNumber,
                          SuplierText = sup.Name
                      })
                     .WhereIf(!string.IsNullOrEmpty(code), x => x.Code.Contains(code))
                     .ToList();

            return new GenericActionResult(200, true, "", result);
        }

        [HttpPut]
        [AllowAnonymous]
        public async Task<bool> RebuildSupplierOrderReport()
        {
            var tenants = await _tenantRepository.GetListAsync();
            foreach (var tenant in tenants)
            {
                using (CurrentTenant.Change(tenant.Id))
                {
                    await _saleOrderService.RebuildSupplierOrderReport();
                }
            }

            return true;
        }
    }
}