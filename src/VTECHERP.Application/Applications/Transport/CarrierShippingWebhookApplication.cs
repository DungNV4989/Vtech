using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;
using VTECHERP.DTOs.CarrierShipping;
using VTECHERP.Helper;
using VTECHERP.ServiceInterfaces;

namespace VTECHERP.Applications.Transport
{
    [Route("api/app/shipping/[action]")]
    public class CarrierShippingWebhookApplication : ApplicationService
    {
        private readonly ICarrierShippingService _carrierShippingService;
        public CarrierShippingWebhookApplication(ICarrierShippingService carrierShippingService)
        {
            _carrierShippingService = carrierShippingService;
        }

        [HttpPost]
        [CustomAuthFilter]
        public async Task<IActionResult> GHTKUpdateBill(UpdateShippingGHTKDTO data)
        {
            var response = await _carrierShippingService.GHTKUpdateStatus(data);
            return response;
        }

        [HttpPost]
        public async Task<IActionResult> VTPUpdateBill(UpdateShippingGHTKDTO data)
        {
            var response = await _carrierShippingService.GHTKUpdateStatus(data);
            return response;
        }
    }
}
