using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.Customer;
using VTECHERP.DTOs.Entries;
using VTECHERP.DTOs.MasterDatas;
using VTECHERP.DTOs.SaleOrders;
using VTECHERP.DTOs.TransportInformation;
using VTECHERP.Enums;
using VTECHERP.Models;

namespace VTECHERP.Services
{
    public interface ITransportInformationService : IScopedDependency
    {
        Task<PagingResponse<TransportInformationResponse>> SearchTransportInformation(SearchTransportInformationRequest request);
        Task<PagingResponse<TransportInformation3RDResponse>> SearchTransportInformationBy3RD(SearchTransportInformationBy3RDRequest request);
        Task Delete(Guid id);
        Task<TransportInformationDTO> GetBillInformationById(Guid id);
        Task<TransportInformationDTO> Create(CreateTransportInformationDto input);
        Task<MasterDataDTO> GetBillInformationByCode(SearchTextRequest input);
        Task<bool> UpdateInternal(Guid id, UpdateTransportInformationDto input);
        Task<bool> UpdateShipper(Guid id, MasterDataDTO input);
        Task<bool> UpdateStatus(Guid id, TransportStatus input);
        Task<bool> UpdateDistance(Guid id, decimal? input);
        Task<List<MasterDataDTO>> SearchCustomer(SearchTextRequest request);
        Task<List<MasterDataDTO>> SearchBillInformation(SearchTextRequest request);
        Task<List<MasterDataDTO>> SearchShipper();
        Task<string> Validate(CreateTransportInformationDto input);
        Task<byte[]> ExportTransportInformationAsync(SearchTransportInformationRequest request);

        Task<PagingResponse<TransportHistoryInformationResponse>> SearchHistoryTransportInformation(SearchHistoryTransportInformationRequest request);
    }
}