using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.VietNamPost.Places;

namespace VTECHERP.Services.Interface
{
    public interface IVietNamPostService : IScopedDependency
    {
        Task<ResponGetPlaceVietNamPost> GetPlaces(Guid Store);
        Task<List<ProvinceVietNamPost>> GetProvince(Guid Store);
        Task<List<WardVietNamPost>> GetWards(Guid Store, string districtCode);
        Task<List<DistrictVietNamPost>> GetDistrict(Guid Store, string ProvinceCode);

    }
}
