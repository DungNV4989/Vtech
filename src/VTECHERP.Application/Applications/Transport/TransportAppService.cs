using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VTECHERP.Services;
using VTECHERP.DTOs.TransportInformation;
using Volo.Abp;
using VTECHERP.Constants;
using VTECHERP.Helper;
using VTECHERP.Entities;
using Volo.Abp.Domain.Repositories;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.MasterDatas;
using VTECHERP.Enums;
using Microsoft.Extensions.Logging;

namespace VTECHERP.Applications.Transport
{
    [Route("api/app/transport/[action]")]
    [Authorize]
    public class TransportAppService : ApplicationService
    {
        private readonly ITransportInformationService _informationService;
        private readonly IRepository<Customer> _customerRepository;
        private readonly ILogger<TransportAppService> _logger;

        public TransportAppService(ITransportInformationService informationService, IRepository<Customer> customerRepository, ILogger<TransportAppService> logger)
        {
            _informationService = informationService;
            _customerRepository = customerRepository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<PagingResponse<TransportInformationResponse>> Search(SearchTransportInformationRequest request)
        {
            return await _informationService.SearchTransportInformation(request);
        }
        [HttpPost]
        public async Task<PagingResponse<TransportInformation3RDResponse>> Search3RD(SearchTransportInformationBy3RDRequest request)
        {
            return await _informationService.SearchTransportInformationBy3RD(request);
        }

        [HttpPost]
        public async Task<PagingResponse<TransportHistoryInformationResponse>> SearchHistory(SearchHistoryTransportInformationRequest request)
        {
            return await _informationService.SearchHistoryTransportInformation(request);
        }

        [HttpGet]
        public async Task<TransportInformationDTO> GetById(Guid id)
        {
            return await _informationService.GetBillInformationById(id);
        }

        [HttpDelete]
        public async Task Delete(Guid id)
        {
            await _informationService.Delete(id);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateTransportInformationDto input)
        {
            try
            {

                if (input != null)
                {
                    var validate = await _informationService.Validate(input);
                    if (!string.IsNullOrEmpty(validate))
                    {
                        return new GenericActionResult(400, false, string.Format(ErrorMessages.Common.Required, validate) , null);
                    }
                }
                else
                {
                    return new GenericActionResult(400, false, "Dữ liệu không hợp lệ ", null);
                }
                var result = await _informationService.Create(input);
                if (result != null)
                {
                    var customer = await _customerRepository.FindAsync(x => x.Id == result.CustomerId.GetValueOrDefault());
                    if (customer != null)
                    {
                        result.CustomerName = customer.Name;
                    }
                }
                return new GenericActionResult(200, true, "", result);
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> GetBillInformationByCode(SearchTextRequest input)
        {
            try
            {
                var result = await _informationService.GetBillInformationByCode(input);
                
                return new GenericActionResult(200, true, "", result);
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }

        }

        [HttpPost]
        public async Task<IActionResult> SearchCustomer(SearchTextRequest request)
        {
            try
            {
                var result = await _informationService.SearchCustomer(request);

                return new GenericActionResult(200, true, "", result);
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
            
        }
        //[HttpGet]
        //public async Task<IActionResult> SearchTransportInformation(SearchTextRequest request)
        //{
        //    try
        //    {
        //        var result = await _informationService.SearchTransportInformation(request);

        //        return new GenericActionResult(200, true, "", result);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
        //    }

        //}
        [HttpPut]
        public async Task<IActionResult> UpdateTransportInformationInternal(Guid id, UpdateTransportInformationDto input)
        {
            try
            {
                if (input == null)
                {
                    return new GenericActionResult(400, false, "dữ liệu không hợp lệ", null);
                }
                var result = await _informationService.UpdateInternal(id,input);
                if (result == false)
                {
                    return new GenericActionResult(400, false, "cập nhật không thành công", id);
                }
                return new GenericActionResult();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }

        }

        [HttpPost]
        public async Task<IActionResult> SearchShipper()
        {
            try
            {
                var result = await _informationService.SearchShipper();

                return new GenericActionResult(200, true, "", result);
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
            
        }

        [HttpPut]
        public async Task<IActionResult> UpdateShipper(Guid id, MasterDataDTO input)
        {
            try
            {
                var result = await _informationService.UpdateShipper(id, input);
                if (result == false)
                {
                    return new GenericActionResult(400, false, "cập nhật không thành công", id);
                }
                return new GenericActionResult();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }

        }
        [HttpPut]
        public async Task<IActionResult> UpdateStatus(Guid id, TransportStatus input)
        {
            try
            {
                var result = await _informationService.UpdateStatus(id, input);
                if (result == false)
                {
                    return new GenericActionResult(400, false, "cập nhật không thành công", id);
                }
                return new GenericActionResult();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }

        }

        [HttpPut]
        public async Task<IActionResult> UpdateDistance(Guid id, decimal? input)
        {
            try
            {
                var result = await _informationService.UpdateDistance(id, input);
                if (result == false)
                {
                    return new GenericActionResult(400, false, "cập nhật không thành công", id);
                }
                return new GenericActionResult();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }

        }

        [HttpPost]
        public async Task<FileResult> ExportTransportInformationAsync(SearchTransportInformationRequest request)
        {
            try
            {
                _logger.LogWarning("[VTECH] Start Export");
                var file = await _informationService.ExportTransportInformationAsync(request);
                _logger.LogWarning($"[VTECH] Get Export File: File Length - {file.Length}");
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"DanhSach_DonVanChuyen_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
                };
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                _logger.LogError($"[VTECH]{e.Message}");
                throw;
            }
        }
    }
}
