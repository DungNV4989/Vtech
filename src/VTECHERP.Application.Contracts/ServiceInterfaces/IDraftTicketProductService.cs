using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.DraftTicketProducts;

namespace VTECHERP.Services
{
    public interface IDraftTicketProductService : IScopedDependency
    {
        Task AddRangeAsync(Guid DraftTicketId, List<DraftTicketProductCreateRequest> requests);

        Task<List<DraftTicketProductDetailDto>> GetByIdsAsync(List<Guid> ids);

        Task<List<DraftTicketProductDetailDto>> GetByDraftTicketId(Guid draftTicketId);

        Task<List<DraftTicketProductDetailDto>> GetByDraftTicketIds(List<Guid> draftTicketIds);

        Task<List<DraftTicketProductApproveDto>> SetApproveByDraftTicketIdAsync(Guid draftTicketId, Guid storeId, string productName);

        Task ApprovesAsync(List<DraftTicketProductApproveRequest> requests);

        Task DeleteRangeAsync(Guid draftTicketId);
    }
}