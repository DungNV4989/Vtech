using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using VTECHERP.DTOs.CarrierShipping;
using VTECHERP.DTOs.CarrierShipping.Param;
using VTECHERP.Entities;
using VTECHERP.Helper;
using VTECHERP.ServiceInterfaces;
using VTECHERP.Services.Interface;

namespace VTECHERP.Applications.Transport
{
    [Route("api/app/carrier-shipping/[action]")]
    [Authorize]
    public class CarrierShippingApplication : ApplicationService
    {
        private readonly ICarrierShippingService _carrierShippingService;
        private readonly IRepository<CarrierShippingInformation> _carrierShippingInformationRepository;
        private readonly IViettelPostService _viettelPostService;
        private readonly IVietNamPostService _vietNamPostService;
        public CarrierShippingApplication(
            ICarrierShippingService carrierShippingService
            , IRepository<CarrierShippingInformation> carrierShippingInformationRepository
            , IViettelPostService viettelPostService
            , IVietNamPostService vietNamPostService
            ) 
        { 
            _carrierShippingService = carrierShippingService;
            _carrierShippingInformationRepository = carrierShippingInformationRepository;
            _viettelPostService = viettelPostService;
            _vietNamPostService = vietNamPostService;
        }

        [HttpPost]
        public async Task<IActionResult> SendCodeAsync(CarrierShippingInformationDTO dto)
        {
            try
            {
                var response = await _carrierShippingService.SendCodeShippingAsync(dto);
                return response;
            }catch (Exception ex)
            {
                return new GenericActionResult(500, false);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetAllPriceVTPAsync(PriceAllDTO dto)
        {
            try
            {
                var response = await _carrierShippingService.GetAllPriceVTPAsync(dto);
                return response;
            }
            catch (Exception ex)
            {
                return new GenericActionResult(500, false);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetPriceVTPAsync(PriceDTO dto)
        {
            try
            {
                var response = await _carrierShippingService.GetPriceVTPAsync(dto);
                return response;
            }
            catch (Exception ex)
            {
                return new GenericActionResult(500, false);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetPriceVNPAsync(PriceVNPDTO dto)
        {
            try
            {
                var response = await _carrierShippingService.GetPriceVNPAsync(dto);
                return response;
            }
            catch (Exception ex)
            {
                return new GenericActionResult(500, false);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPostOffice(GetPostOfficeParam param)
        {
            var response = await _carrierShippingService.GetPostOffice(param);
            return new OkObjectResult(response);
        }
        [HttpPost]
        public async Task<IActionResult> GetTokenVNPost(string username,string password, string customerCode)
        {
            var response = await _carrierShippingService.GetTokenVNPost(username,password,customerCode);
            return new OkObjectResult(response);
        }
        [HttpPost]
        public async Task<IActionResult> SaveOrderTemporary(CarrierShippingInformationDTO dto)
        {
            var carrierShipping = new CarrierShippingInformation()
            {
                CarrierShippingCode= dto.CarrierShippingCode,
                CODValue= dto.CODValue,
                FormReceive = dto.FormReceive,
                FormSend= dto.FormSend,
                Height= dto.Height,
                IsCOD= dto.IsCOD,   
                Length= dto.Length, 
                Note= dto.Note,
                PickDate= dto.PickDate,
                ProductValue= dto.ProductValue,
                Quantity= dto.Quantity,
                ReceiverAddress= dto.ReceiverAddress,   
                ReceiverDistrict= dto.ReceiverDistrict,
                ReceiverDistrictName= dto.ReceiverDistrictName,
                ReceiverEmail= dto.ReceiverEmail,
                ReceiverName= dto.ReceiverName,
                ReceiverPayingShippingFee= dto.ReceiverPayingShippingFee,
                ReceiverPostOffice= dto.ReceiverPostOffice,
                ReceiverPhone = dto.ReceiverPhone,
                ReceiverProvince= dto.ReceiverProvince,
                ReceiverProvinceName= dto.ReceiverProvinceName,
                ReceiverWardName= dto.ReceiverWardName,
                ReceiverWard = dto.ReceiverWard,
                SenderAddress= dto.SenderAddress,
                SenderDistrict= dto.SenderDistrict,
                SenderDistrictName= dto.SenderDistrictName,
                SenderName = dto.SenderName,
                SenderPhone= dto.SenderPhone,
                SenderPostOffice= dto.SenderPostOffice,
                SenderProvince= dto.SenderProvince,
                SenderProvinceName= dto.SenderProvinceName,
                SenderWard= dto.SenderWard,
                SenderWardName = dto.SenderWardName,
                ShippingProductName= dto.ShippingProductName,
                StoreId= dto.StoreId,
                TransportInformationId= dto.TransportInformationId,
                VTShippingCode= dto.VTShippingCode,
                Weight= dto.Weight,
                Width= dto.Width
            };

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> GetPlacesViettelPost(Guid StoreId)
        {
            var result = await _viettelPostService.GetPlaces(StoreId);
            if(!result.success)
            {
                return new BadRequestObjectResult(new
                {
                    success = false,
                    message = result.message
                });
            } 
            
            return new OkObjectResult(new
            {
                success = true,
                message = "",
                wards = result.wards,
                districts = result.districts,
                province = result.cities
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetPlacesVietNamPost(Guid StoreId)
        {
            var result = await _vietNamPostService.GetPlaces(StoreId);
            if (!result.Success)
            {
                return new BadRequestObjectResult(new
                {
                    success = false,
                    message = result.Message
                });
            }

            return new OkObjectResult(new
            {
                success = true,
                message = "",
                wards = result.Wards,
                districts = result.Districts,
                province = result.Provinces
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetProvinceVietNamPost(Guid StoreId)
        {
            var result = await _vietNamPostService.GetProvince(StoreId);

            return new OkObjectResult(new
            {
                success = true,
                message = "",
                data = result
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetDistrictVietNamPost(Guid StoreId, string ProvinceCode)
        {
            var result = await _vietNamPostService.GetDistrict(StoreId, ProvinceCode);
            return new OkObjectResult(new
            {
                success = true,
                message = "",
                data = result
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetWardsVietNamPost(Guid StoreId, string DistrictCode)
        {
            var result = await _vietNamPostService.GetWards(StoreId, DistrictCode);

            return new OkObjectResult(new
            {
                success = true,
                message = "",
                data = result
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetProvinceViettelPost(Guid StoreId)
        {
            var result = await _viettelPostService.GetProvince(StoreId);

            return new OkObjectResult(new
            {
                success = result.success,
                message = result.message,
                data = result.data
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetDistrictViettelPost(Guid StoreId, int ProvinceId)
        {
            var result = await _viettelPostService.GetDistrict(StoreId, ProvinceId);
            return new OkObjectResult(new
            {
                success = result.success,
                message = result.message,
                data = result.data
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetWardsViettelPost(Guid StoreId, int DistrictId)
        {
            var result = await _viettelPostService.GetWards(StoreId, DistrictId);

            return new OkObjectResult(new
            {
                success = result.success,
                message = result.message,
                data = result.data
            });
        }
    }
}
