using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.OrderTransports.Params;
using VTECHERP.DTOs.PriceTableProduct.Param;

namespace VTECHERP.ServiceInterfaces
{
    public interface IPriceTableProductService : IScopedDependency
    {
        Task<byte[]> DownloadTemplateImport();
        Task<byte[]> ImportPriceTableProductData(PriceTableProductImportParam param);
    }
}
