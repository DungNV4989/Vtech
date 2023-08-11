using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VTECHERP.DTOs.BillCustomerVoucher.Param;
using VTECHERP.Services.Interface;

namespace VTECHERP.Applications.BillCustomerVoucher
{
    public class BillCustomerVoucherApplication : ApplicationService
    {
        private readonly IBillCustomerVoucherService _billCustomerVoucherService;
        public BillCustomerVoucherApplication(
            IBillCustomerVoucherService billCustomerVoucherService
            )
        {
            _billCustomerVoucherService = billCustomerVoucherService;
        }

        [HttpPost]
        public async Task<IActionResult> GetVoucher(GetVoucherParam param)
        {
            var result = _billCustomerVoucherService.GetVoucher(param);
            if (!result.Item3)
            {
                return new BadRequestObjectResult(new
                {
                    success = false,
                    message = result.Item2
                });
            }

            return new OkObjectResult(new
            {
                success = true,
                message = result.Item2,
                data = result.Item1
            });
        }

        [HttpGet]
        public async Task<IActionResult> ProductsApplyVoucher(string ProductVoucher, List<Guid> ProductIds)
        {
            var result = _billCustomerVoucherService.ProductsApplyVoucher(ProductVoucher, ProductIds);
            return new OkObjectResult(new
            {
                success = true,
                data = result
            });
        }

        [HttpGet]
        public async Task<IActionResult> ProductsApplyVoucherByCategory(string CategoryProductVoucher, List<Guid> ProductIds)
        {
            var result = _billCustomerVoucherService.ProductsApplyVoucherByCategory(CategoryProductVoucher, ProductIds);
            return new OkObjectResult(new
            {
                success = true,
                data = result
            });
        }
    }
}
