using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.Product;

namespace VTECHERP.ServiceInterfaces
{
    public interface IProductStockService : IScopedDependency
    {
        Task<byte[]> ExportProductStockAsync(SearchProductExcelRequest request);
        Task<SearchProductStockResponse> Search(SearchProductStockRequest request);
    }
}
