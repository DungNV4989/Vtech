using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VTECHERP.DTOs.CarrierShipping;
using VTECHERP.DTOs.ViettelPost;
using VTECHERP.DTOs.ViettelPost.Places;
using VTECHERP.Entities;
using VTECHERP.ServiceInterfaces;
using VTECHERP.Services.Interface;

namespace VTECHERP.Services
{
    public class ViettelPostService : IViettelPostService
    {
        private readonly IConfiguration _configuration;
        private readonly ICarrierShippingService _carrierShippingService;
        public ViettelPostService(
            IConfiguration configuration
            , ICarrierShippingService carrierShippingService
            )
        {
            _configuration = configuration;
            _carrierShippingService = carrierShippingService;
        }

        public async Task<(bool success, string message, List<WardViettelPost> wards, List<DistrictViettelPost> districts, List<CityViettelPost> cities)> GetPlaces(Guid storeId)
        {
            var wards = new List<WardViettelPost>();
            var districts = new List<DistrictViettelPost>();
            var cities = new List<CityViettelPost>();
            var hostApi = _configuration.GetValue<string>("ViettelPostAPI:Host.Api");
            var getWardsApi = hostApi + _configuration.GetValue<string>("ViettelPostAPI:GetWards");
            var getDistrictsApi = hostApi + _configuration.GetValue<string>("ViettelPostAPI:GetDistricts");
            var getCitiesApi = hostApi + _configuration.GetValue<string>("ViettelPostAPI:GetCities");

            var getTokenResult = await _carrierShippingService.GetTokenViettelPost(storeId);
            if (getTokenResult.success && getTokenResult.data.status == 200)
            {
                var apiKey = getTokenResult.data.data.token;

                using (HttpClient client = new HttpClient())
                {
                    var requestWard = new HttpRequestMessage(HttpMethod.Get, getWardsApi);
                    requestWard.Headers.Add("Token", apiKey);
                    var responseWard = await client.SendAsync(requestWard);
                    var resContentWard = await responseWard.Content.ReadAsStringAsync();
                    var responseDTOWard = JsonConvert.DeserializeObject<WardViettelPostRespon>(resContentWard);
                    wards = responseDTOWard.data.ToList();

                    var requestDistrict = new HttpRequestMessage(HttpMethod.Get, getDistrictsApi);
                    requestDistrict.Headers.Add("Token", apiKey);
                    var responseDistrict = await client.SendAsync(requestDistrict);
                    var resContentDistrict = await responseDistrict.Content.ReadAsStringAsync();
                    var responseDTODistrict = JsonConvert.DeserializeObject<DistrictViettelPostRespon>(resContentDistrict);
                    districts = responseDTODistrict.data.ToList();

                    var requestCities = new HttpRequestMessage(HttpMethod.Get, getCitiesApi);
                    requestCities.Headers.Add("Token", apiKey);
                    var responseCities = await client.SendAsync(requestCities);
                    var resContentCities = await responseCities.Content.ReadAsStringAsync();
                    var responseDTOCities = JsonConvert.DeserializeObject<CityViettelPostRespon>(resContentCities);
                    cities = responseDTOCities.data.ToList();
                }
            }
            else return (false, getTokenResult.message, null, null, null);
            return (true, "", wards, districts, cities);
        }

        public async Task<(bool success, string message, List<CityViettelPost> data)> GetProvince(Guid storeId)
        {
            var cities = new List<CityViettelPost>();
            var hostApi = _configuration.GetValue<string>("ViettelPostAPI:Host.Api");
            var getCitiesApi = hostApi + _configuration.GetValue<string>("ViettelPostAPI:GetCities") + $"?provinceId=-1";

            var getTokenResult = await _carrierShippingService.GetTokenViettelPost(storeId);
            if (getTokenResult.success && getTokenResult.data.status == 200)
            {
                var apiKey = getTokenResult.data.data.token;

                using (HttpClient client = new HttpClient())
                {
                    var requestCities = new HttpRequestMessage(HttpMethod.Get, getCitiesApi);
                    requestCities.Headers.Add("Token", apiKey);
                    var responseCities = await client.SendAsync(requestCities);
                    var resContentCities = await responseCities.Content.ReadAsStringAsync();
                    var responseDTOCities = JsonConvert.DeserializeObject<CityViettelPostRespon>(resContentCities);
                    cities = responseDTOCities.data.ToList();
                }
            }
            else return (false, getTokenResult.message, null);
            return (true, "", cities);
        }

        public async Task<(bool success, string message, List<DistrictViettelPost> data)> GetDistrict(Guid storeId, int ProvinceId)
        {
            var districts = new List<DistrictViettelPost>();
            var hostApi = _configuration.GetValue<string>("ViettelPostAPI:Host.Api");
            var getDistrictsApi  = hostApi + _configuration.GetValue<string>("ViettelPostAPI:GetDistricts") + $"?provinceId={ProvinceId}";

            var getTokenResult = await _carrierShippingService.GetTokenViettelPost(storeId);
            if (getTokenResult.success && getTokenResult.data.status == 200)
            {
                var apiKey = getTokenResult.data.data.token;

                using (HttpClient client = new HttpClient())
                {
                    var requestDistrict = new HttpRequestMessage(HttpMethod.Get, getDistrictsApi);
                    requestDistrict.Headers.Add("Token", apiKey);
                    var responseDistrict = await client.SendAsync(requestDistrict);
                    var resContentDistrict = await responseDistrict.Content.ReadAsStringAsync();
                    var responseDTODistrict = JsonConvert.DeserializeObject<DistrictViettelPostRespon>(resContentDistrict);
                    districts = responseDTODistrict.data.ToList();
                }
            }
            else return (false, getTokenResult.message, null);
            return (true, "", districts);
        }

        public async Task<(bool success, string message, List<WardViettelPost> data)> GetWards(Guid storeId, int DistrictId)
        {
            var wards = new List<WardViettelPost>();
            var hostApi = _configuration.GetValue<string>("ViettelPostAPI:Host.Api");
            var getWardsApi = hostApi + _configuration.GetValue<string>("ViettelPostAPI:GetWards") + $"?districtId={DistrictId}";

            var getTokenResult = await _carrierShippingService.GetTokenViettelPost(storeId);
            if (getTokenResult.success && getTokenResult.data.status == 200)
            {
                var apiKey = getTokenResult.data.data.token;

                using (HttpClient client = new HttpClient())
                {
                    var requestWard = new HttpRequestMessage(HttpMethod.Get, getWardsApi);
                    requestWard.Headers.Add("Token", apiKey);
                    var responseWard = await client.SendAsync(requestWard);
                    var resContentWard = await responseWard.Content.ReadAsStringAsync();
                    var responseDTOWard = JsonConvert.DeserializeObject<WardViettelPostRespon>(resContentWard);
                    wards = responseDTOWard.data.ToList();
                }
            }
            else return (false, getTokenResult.message, null);
            return (true, "", wards);
        }
    }
}
