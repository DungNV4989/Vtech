using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.OrderTransports;
using VTECHERP.DTOs.OrderTransports.Params;

namespace VTECHERP.Services
{
    public interface IOrderTransportService : IScopedDependency
    {
        Task<IQueryable<OrderTransportItemList>> GetList(GetListOrderTransportParm param);
        Task<Guid> Create(List<Guid> OrderSalesId);
        Task<byte[]> ExportAsync(GetListOrderTransportParm param);
        Task<IQueryable<OrderTransportItemList>> GetOrderTransportFull();
    }
}
