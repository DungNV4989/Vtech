using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.CarrierShipping;
using VTECHERP.DTOs.CarrierShipping.Param;

namespace VTECHERP.ServiceInterfaces
{
    public interface ICarrierShippingService : IScopedDependency
    {
        public Task<IActionResult> SendCodeShippingAsync(CarrierShippingInformationDTO dto);
        public Task<IActionResult> GHTKUpdateStatus(UpdateShippingGHTKDTO data);
        Task<(bool success, string message, VTPResTokenDTO data)> GetTokenViettelPost(Guid StoreId);
        Task<List<PostOfficeDTO>> GetPostOffice(GetPostOfficeParam param);
        Task<IActionResult> GetAllPriceVTPAsync(PriceAllDTO dto);
        Task<IActionResult> GetPriceVTPAsync(PriceDTO dto);
        Task<IActionResult> GetPriceVNPAsync(PriceVNPDTO dto);
        Task<VNPResponseTokenDTO> GetTokenVNPost(string username, string password, string customerCode);
    }
}
