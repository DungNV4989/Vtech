using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.OrderTransports.Params;
using VTECHERP.DTOs.SaleOrderLines;
using VTECHERP.DTOs.SaleOrderLines.Params;

namespace VTECHERP.Services
{
    public interface ISaleOrderLineService : IScopedDependency
    {
        Task<object> AddRangeAsync(Guid orderId, List<SaleOrderLineCreateRequest> requests);
        Task<object> UpdateRangeAsync(Guid orderId, List<SaleOrderLineUpdateRequest> requests);
        Task<SaleOrderLineDto> GetByIdAsync(Guid id);
        Task<List<SaleOrderLineDetailDto>> GetByOrderIdAsync(Guid orderId);
        Task<List<SaleOrderLineDto>> ListByOrderIdAsync(Guid orderId);
        Task<IQueryable<SaleOrderListItem>> GetList();
        Task<IQueryable<SaleOrderListItem>> GetListFull(SaleOrderLineGetListParam param);
        Task<byte[]> ExportAsync(SaleOrderLineGetListParam param);
    }
}