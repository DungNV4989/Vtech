using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Validation;
using VTECHERP.Constants;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.BillCustomers.Params;
using VTECHERP.DTOs.DraftTickets;
using VTECHERP.DTOs.WarehouseTransferBills;
using VTECHERP.Enums.DraftTicket;
using VTECHERP.ServiceInterfaces;
using VTECHERP.Services;

namespace VTECHERP
{
    [Route("api/app/WarehouseTransferBill/[action]")]
    [Authorize]
    public class WarehouseTransferBillApplication : ApplicationService
    {
        private readonly IWarehouseTransferBillService _warehouseTransferBillService;
        private readonly IDraftTicketService _draftTicketService;
        private readonly ILogger<WarehouseTransferBillApplication> _logger;
        public WarehouseTransferBillApplication(
            IWarehouseTransferBillService warehouseTransferBillService,
            IDraftTicketService draftTicketService,
            ILogger<WarehouseTransferBillApplication> logger)
        {
            _warehouseTransferBillService = warehouseTransferBillService;
            _draftTicketService = draftTicketService;
            _logger = logger;
        }

        #region Phiếu nháp

        [HttpPost]
        public async Task<Guid> AddAsync(DraftTicketCreateRequest request)
        {
            return await _draftTicketService.AddDraftAsync(request);
        }

        [HttpGet]
        public async Task<object> GetByIdAsync(Guid id)
        {
            return await _draftTicketService.GetByIdAsync(id);
        }

        [HttpPost]
        public async Task<PagingResponse<DraftTicketDto>> GetListAsync(SearchDraftTicketRequest request)
        {
            return await _draftTicketService.GetListAsync(request);
        }

        [HttpPost]
        public async Task<FileResult> ExportDraftTicketAsync(SearchDraftTicketRequest request)
        {
            try
            {
                var file = await _draftTicketService.ExportDraftTicket(request);
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"DanhSach_Phiếu nháp_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
                };
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                throw;
            }
        }

        [HttpPost]
        public async Task<DraftTicketApproveDto> SetApproveByIdAsync(SearchDraftTicketApproveRequest request)
        {
            return await _draftTicketService.SetApproveByIdAsync(request);
        }

        /// <summary>
        /// Duyệt phiếu nháp 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DraftTicketApproveRequest> ApproveAsync(DraftTicketApproveRequest request)
        {
            return await _draftTicketService.ApproveAsync(request);
        }


        [HttpDelete]
        public async Task<Guid> DeleteAsync(Guid id)
        {
            var draftTicket = await _draftTicketService.GetByIdAsync(id);
            if (draftTicket.Status != Status.New && draftTicket.Status != Status.Cancel)
                throw new AbpValidationException(ErrorMessages.DraftTicket.NotDraft_CancelStatus, new List<ValidationResult>() { new ValidationResult(
                           ErrorMessages.DraftTicket.NotDraft_CancelStatus,
                           new List<string> { "id", $"{id}" })});
            return await _draftTicketService.DeleteAsync(id);
        }

        [HttpDelete]
        public async Task DeleteRangeAsync(List<Guid> ids)
        {
            await _draftTicketService.DeleteRangeAsync(ids);
        }

        #endregion Phiếu nháp

        [HttpGet]
        public async Task<object> GetTranferBillById(Guid id)
        {
            return await _warehouseTransferBillService.GetByIdAsync(id);
        }

        [HttpPost]
        public async Task<Guid> AddWarehouseTransferBillAsync(WarehouseTransferBillCreateRequest request)
        {
            return await _warehouseTransferBillService.AddWarehouseTransferBillAsync(request);
        }


        [HttpGet]
        public async Task<Guid> AcceptWarehouseTransferBillAsync(Guid id)
        {
            return await _warehouseTransferBillService.AcceptWarehouseTransferBill(id);
        }

        [HttpDelete]
        public async Task<Guid> DeleteWarehouseTransferBillAsync(Guid id)
        {
            return await _warehouseTransferBillService.DeleteWarehouseTransferBill(id, true);
        }

        [HttpPost]
        public async Task<PagingResponse<SearchWarehouseTransferResponse>> SearchWarehouseTransfer(SearchWarehouseTransferRequest request)
        {
            return await _warehouseTransferBillService.SearchWarehouseTransfer(request);
        }

        [HttpPost]
        public async Task<PagingResponse<SearchWarehouseTransferMovingResponse>> SearchWarehouseTransferMoving(SearchWarehouseTransferMovingRequest request)
        {
            return await _warehouseTransferBillService.SearchWarehouseTransferMoving(request);
        }

        [HttpPost]
        public async Task<PagingResponse<SearchWarehouseTransferComingResponse>> SearchWarehouseTransferComing(SearchWarehouseTransferComingRequest request)
        {
            return await _warehouseTransferBillService.SearchWarehouseTransferComing(request);
        }

        [HttpPost]
        public async Task<FileResult> ExportWarehouseTransferMoving(SearchWarehouseTransferMovingRequest request)
        {
            try
            {
                _logger.LogWarning("[VTECH] Start Export");
                var file = await _warehouseTransferBillService.ExportWarehouseTransferMoving(request);
                _logger.LogWarning($"[VTECH] Get Export File: File Length - {file.Length}");
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"ChuyenKho_DangDiChuyen_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
                };
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                _logger.LogError($"[VTECH]{e.Message}");
                throw;
            }
        }

        [HttpPost]
        public async Task<FileResult> ExportWarehouseTransferComing(SearchWarehouseTransferComingRequest request)
        {
            try
            {
                var file = await _warehouseTransferBillService.ExportWarehouseTransferComing(request);
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"ChuyenKho_SapChuyenDen_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
                };
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                throw;
            }
        }

        [HttpPost]
        public async Task<bool> ConfirmAsync(WarehouseTransferBillConfirmRequest request)
        {
            await _warehouseTransferBillService.ConfirmAsync(request);
            return true;
        }

        [HttpPost]
        public async Task<FileResult> ExportWarehouseTransfer(SearchWarehouseTransferRequest request)
        {
            try
            {
                _logger.LogWarning("[VTECH] Start Export");
                var file = await _warehouseTransferBillService.ExportWarehouseTransfer(request);
                _logger.LogWarning($"[VTECH] Get Export File: File Length - {file.Length}");
                return new FileContentResult(file, ContentTypes.SPREADSHEET)
                {
                    FileDownloadName = $"ChuyenKho_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
                };
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                _logger.LogError($"[VTECH]{e.Message}");
                throw;
            }
        }


        [HttpPost]
        public async Task<IActionResult> ImportWarehouseTransfer([FromForm] WarehouseTransferImportParam param)
        {
            if (param == null || param.File.Length <= 0)
                return new BadRequestObjectResult($"Dữ liệu không hợp lệ");

            // Kiểm tra phần mở rộng tệp
            var fileExtension = Path.GetExtension(param.File.FileName);
            if (fileExtension.ToLower() != ".xlsx" && fileExtension.ToLower() != ".xls")
                return new ObjectResult("Sai định dạng file")
                {
                    StatusCode = 500,
                };

            var responCreate = await _warehouseTransferBillService.ImportBillCustomer(param);

            if(!responCreate.success)
                return new ObjectResult(responCreate.message)
                {
                    StatusCode = 500,
                };

            return new OkObjectResult(new
            {
                Data = responCreate.data,
                Success = responCreate.success,
                Message = responCreate.message,
                FileReturn = responCreate.fileRespon
            });
        }

        [HttpGet]
        public async Task<IActionResult> DownloadTemplateImport()
        {
            var file = await _warehouseTransferBillService.DownloadTemplateImport();
            return new FileContentResult(file, ContentTypes.SPREADSHEET)
            {
                FileDownloadName = $"Mau_import_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
            };
        }
    }
}