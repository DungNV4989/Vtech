using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.CarrierShipping;
using VTECHERP.DTOs.GHTK;

namespace VTECHERP.ServiceInterfaces
{
    public interface IGHTKService : IScopedDependency
    {
        Task<(bool success, string message, string data, string dataJs, GHTKResponseDTO? res)> CreateOrderAsync(CarrierShippingInformationDTO param);
    }
}
