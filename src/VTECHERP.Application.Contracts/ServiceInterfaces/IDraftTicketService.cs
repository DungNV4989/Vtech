using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.DraftTickets;
using VTECHERP.Enums.DraftTicket;

namespace VTECHERP.Services
{
    public interface IDraftTicketService : IScopedDependency
    {
        Task<Guid> AddDraftAsync(DraftTicketCreateRequest request);

        Task<DraftTicketDetailDto> GetByIdAsync(Guid id);

        Task<PagingResponse<DraftTicketDto>> GetListAsync(SearchDraftTicketRequest request);
        Task<byte[]> ExportDraftTicket(SearchDraftTicketRequest request);

        Task<DraftTicketApproveDto> SetApproveByIdAsync(SearchDraftTicketApproveRequest request);

        Task<DraftTicketApproveRequest> ApproveAsync(DraftTicketApproveRequest request);

        Task<Guid> DeleteAsync(Guid id);

        Task DeleteRangeAsync(List<Guid> ids);

        Task ChangeStatus(Guid? id, Status status);
    }
}