using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VTECHERP.DTOs.CarrierShipping;
using VTECHERP.DTOs.VietNamPost.Places;
using VTECHERP.DTOs.ViettelPost.Places;
using VTECHERP.Entities;
using VTECHERP.Services.Interface;

namespace VTECHERP.Services
{
    public class VietNamPostService : IVietNamPostService
    {
        private readonly IConfiguration _configuration;
        public VietNamPostService(
            IConfiguration configuration
            )
        {
            _configuration = configuration;
        }
        public async Task<ResponGetPlaceVietNamPost> GetPlaces(Guid Store)
        {
            var result = new ResponGetPlaceVietNamPost();
            var hostApi = _configuration.GetValue<string>("VNPostAPI:Host.Api");
            var getTokenShippingAPi = _configuration.GetValue<string>("VNPostAPI:GetToken");
            var apiVNPGetToken = hostApi + getTokenShippingAPi;

            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, apiVNPGetToken);
                var content = new StringContent("{\"USERNAME\":\"" + "bongdeptrai0201@gmail.com" + "\",\"PASSWORD\":\"" + "Ll@!0932378195" +
                    "\",\"customerCode\":\"" + "C000366474" + "\"}", Encoding.UTF8, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                var resContent = await response.Content.ReadAsStringAsync();
                var responseDTO = JsonConvert.DeserializeObject<VNPResponseTokenDTO>(resContent);
                if (responseDTO.Success)
                {
                    var getWardsApi = hostApi + _configuration.GetValue<string>("VNPostAPI:GetWards");
                    var getDistrictsApi = hostApi + _configuration.GetValue<string>("VNPostAPI:GetDistricts");
                    var getProvinceApi = hostApi + _configuration.GetValue<string>("VNPostAPI:GetProvince");

                    var requestWard = new HttpRequestMessage(HttpMethod.Get, getWardsApi);
                    requestWard.Headers.Add("Token", responseDTO.Token);
                    var responseWard = await client.SendAsync(requestWard);
                    var resContentWard = await responseWard.Content.ReadAsStringAsync();
                    result.Wards = JsonConvert.DeserializeObject<List<WardVietNamPost>>(resContentWard);

                    var requestDistrict = new HttpRequestMessage(HttpMethod.Get, getDistrictsApi);
                    requestDistrict.Headers.Add("Token", responseDTO.Token);
                    var responseDistrict = await client.SendAsync(requestDistrict);
                    var resContentDistrict = await responseDistrict.Content.ReadAsStringAsync();
                    result.Districts = JsonConvert.DeserializeObject<List<DistrictVietNamPost>>(resContentDistrict);

                    var requestCities = new HttpRequestMessage(HttpMethod.Get, getProvinceApi);
                    requestCities.Headers.Add("Token", responseDTO.Token);
                    var responseCities = await client.SendAsync(requestCities);
                    var resContentCities = await responseCities.Content.ReadAsStringAsync();
                    result.Provinces = JsonConvert.DeserializeObject<List<ProvinceVietNamPost>>(resContentCities);
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<List<ProvinceVietNamPost>> GetProvince(Guid Store)
        {
            var result = new List<ProvinceVietNamPost>();
            var hostApi = _configuration.GetValue<string>("VNPostAPI:Host.Api");
            var getTokenShippingAPi = _configuration.GetValue<string>("VNPostAPI:GetToken");
            var apiVNPGetToken = hostApi + getTokenShippingAPi;

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, apiVNPGetToken);
            var content = new StringContent("{\"USERNAME\":\"" + "bongdeptrai0201@gmail.com" + "\",\"PASSWORD\":\"" + "Ll@!0932378195" +
                "\",\"customerCode\":\"" + "C000366474" + "\"}", Encoding.UTF8, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            var resContent = await response.Content.ReadAsStringAsync();
            var responseDTO = JsonConvert.DeserializeObject<VNPResponseTokenDTO>(resContent);
            if (responseDTO.Success)
            {
                var getProvinceApi = hostApi + _configuration.GetValue<string>("VNPostAPI:GetProvince");

                var requestCities = new HttpRequestMessage(HttpMethod.Get, getProvinceApi);
                requestCities.Headers.Add("Token", responseDTO.Token);
                var responseCities = await client.SendAsync(requestCities);
                var resContentCities = await responseCities.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<List<ProvinceVietNamPost>>(resContentCities);
            }
            return result;
        }

        public async Task<List<WardVietNamPost>> GetWards(Guid Store, string districtCode)
        {
            var result = new List<WardVietNamPost>();
            var hostApi = _configuration.GetValue<string>("VNPostAPI:Host.Api");
            var getTokenShippingAPi = _configuration.GetValue<string>("VNPostAPI:GetToken");
            var apiVNPGetToken = hostApi + getTokenShippingAPi;

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, apiVNPGetToken);
            var content = new StringContent("{\"USERNAME\":\"" + "bongdeptrai0201@gmail.com" + "\",\"PASSWORD\":\"" + "Ll@!0932378195" +
                "\",\"customerCode\":\"" + "C000366474" + "\"}", Encoding.UTF8, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            var resContent = await response.Content.ReadAsStringAsync();
            var responseDTO = JsonConvert.DeserializeObject<VNPResponseTokenDTO>(resContent);
            if (responseDTO.Success)
            {
                var getWardsApi = hostApi + _configuration.GetValue<string>("VNPostAPI:GetWards");

                var requestWard = new HttpRequestMessage(HttpMethod.Get, getWardsApi);
                requestWard.Headers.Add("Token", responseDTO.Token);
                var responseWard = await client.SendAsync(requestWard);
                var resContentWard = await responseWard.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<List<WardVietNamPost>>(resContentWard);
                result = result.Where(x => x.districtCode == districtCode).ToList();
            }

            return result;
        }

        public async Task<List<DistrictVietNamPost>> GetDistrict(Guid Store, string ProvinceCode)
        {
            var result = new List<DistrictVietNamPost>();
            var hostApi = _configuration.GetValue<string>("VNPostAPI:Host.Api");
            var getTokenShippingAPi = _configuration.GetValue<string>("VNPostAPI:GetToken");
            var apiVNPGetToken = hostApi + getTokenShippingAPi;

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, apiVNPGetToken);
            var content = new StringContent("{\"USERNAME\":\"" + "bongdeptrai0201@gmail.com" + "\",\"PASSWORD\":\"" + "Ll@!0932378195" +
                "\",\"customerCode\":\"" + "C000366474" + "\"}", Encoding.UTF8, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            var resContent = await response.Content.ReadAsStringAsync();
            var responseDTO = JsonConvert.DeserializeObject<VNPResponseTokenDTO>(resContent);
            if (responseDTO.Success)
            {
                var getDistrictsApi = hostApi + _configuration.GetValue<string>("VNPostAPI:GetDistricts");

                var requestDistrict = new HttpRequestMessage(HttpMethod.Get, getDistrictsApi);
                requestDistrict.Headers.Add("Token", responseDTO.Token);
                var responseDistrict = await client.SendAsync(requestDistrict);
                var resContentDistrict = await responseDistrict.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<List<DistrictVietNamPost>>(resContentDistrict);
                result = result.Where(x => x.provinceCode == ProvinceCode).ToList();
            }

            return result;
        }

    }
}
