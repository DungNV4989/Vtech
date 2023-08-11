using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.CarrierShipping;
using VTECHERP.DTOs.ViettelPost.Places;

namespace VTECHERP.Services.Interface
{
    public interface IViettelPostService : IScopedDependency
    {
        Task<(bool success, string message, List<WardViettelPost> wards, List<DistrictViettelPost> districts, List<CityViettelPost> cities)> GetPlaces(Guid storeId);
        Task<(bool success, string message, List<CityViettelPost> data)> GetProvince(Guid storeId);
        Task<(bool success, string message, List<DistrictViettelPost> data)> GetDistrict(Guid storeId, int ProvinceId);
        Task<(bool success, string message, List<WardViettelPost> data)> GetWards(Guid storeId, int DistrictId);
    }
}
