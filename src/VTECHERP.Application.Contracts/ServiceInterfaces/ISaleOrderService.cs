using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.SaleOrders;

namespace VTECHERP.Services
{
    public interface ISaleOrderService : IScopedDependency
    {
        Task<object> AddAsync(SaleOrderCreateRequest request);
        Task<object> GetListAsync(SearchSaleOrderRequest input);
        Task<object> GetByIdAsync(Guid id);
        Task<object> UpdateAsync(SaleOrderUpdateRequest request);
        Task Delete(Guid id);
        Task<GetDetailConfirmByIdResponse> GetDetailConfirmById(Guid id);
        Task Confirm(SaleOrderConfirmRequest request);
        Task Complete(Guid id);
        Task<byte[]> ExportAsync(SearchSaleOrderRequest request);
        Task RebuildSupplierOrderReport();
    }
}