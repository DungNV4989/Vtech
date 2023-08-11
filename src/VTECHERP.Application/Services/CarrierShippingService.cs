using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using VTECHERP.Constants;
using VTECHERP.DTOs.CarrierShipping;
using VTECHERP.DTOs.CarrierShipping.Param;
using VTECHERP.DTOs.GHTK;
using VTECHERP.DTOs.ViettelPost;
using VTECHERP.Entities;
using VTECHERP.Helper;
using VTECHERP.ServiceInterfaces;
using static VTECHERP.Constants.ErrorMessages;

namespace VTECHERP.Services
{
    public class CarrierShippingService : ICarrierShippingService
    {
        private readonly IRepository<CarrierShippingInformation> _carrierShippingRepository;
        private readonly IRepository<CarrierShippingLog> _carrierShippingLogRepository;
        private readonly IRepository<TransportInformation> _transportInformationRepository;
        private readonly IRepository<TransporstBills> _transportBillRepository;
        private readonly IRepository<BillCustomer> _billCustomerRepository;
        private readonly IRepository<Stores> _storeRepository;
        private readonly IRepository<StoreShippingInformation> _storeShippingInformationRepository;
        private readonly IRepository<Provinces> _provinceRepository;
        private readonly IRepository<Districts> _districtRepository;
        private readonly IRepository<Wards> _wardRepository;
        private readonly IConfiguration _configuration;
        private readonly IRepository<GHTKPostOffice> _ghtkPostOfficeRepository;
        private readonly IDataFilter _dataFilter;
        private readonly IGHTKService _ghtkService;
        public CarrierShippingService(IConfiguration configuration,
            IRepository<CarrierShippingInformation> carrierShippingRepository,
            IRepository<CarrierShippingLog> carrierShippingLogRepository,
            IRepository<TransportInformation> transportInformationRepository,
            IRepository<TransporstBills> transportBillRepository,
            IRepository<BillCustomer> billCustomerRepository,
            IRepository<Stores> storeRepository,
            IRepository<StoreShippingInformation> storeShippingInformationRepository,
            IRepository<Provinces> provinceRepository,
            IRepository<Districts> districtRepository,
            IRepository<Wards> wardRepository,
            IRepository<GHTKPostOffice> ghtkPostOfficeRepository,
            IDataFilter dataFilter
            ,IGHTKService ghtkService
            )
        {
            _configuration = configuration;
            _carrierShippingRepository = carrierShippingRepository;
            _carrierShippingLogRepository = carrierShippingLogRepository;
            _transportInformationRepository = transportInformationRepository;
            _transportBillRepository = transportBillRepository;
            _billCustomerRepository = billCustomerRepository;
            _storeRepository = storeRepository;
            _storeShippingInformationRepository = storeShippingInformationRepository;
            _provinceRepository = provinceRepository;
            _districtRepository = districtRepository;
            _wardRepository = wardRepository;
            _ghtkPostOfficeRepository = ghtkPostOfficeRepository;
            _dataFilter = dataFilter;
            _ghtkService = ghtkService;
            _ghtkService = ghtkService;
        }
        public async Task<IActionResult> GHTKUpdateStatus(UpdateShippingGHTKDTO data)
        {
            try
            {
                var status = MapBillGHTKStatus(data.status_id);
                if (status != null)
                {
                    var listBill = await _billCustomerRepository.GetListAsync(x => x.CarrierShippingCode == data.label_id);
                    listBill.ForEach(x =>
                    {
                        x.CustomerBillPayStatus = status;
                    });
                }

                return new GenericActionResult(200, true, "Success");
            }
            catch (Exception ex)
            {
                return new GenericActionResult(500, false, "Có lỗi xảy ra");
            }
        }

        public async Task<IActionResult> SendCodeShippingAsync(CarrierShippingInformationDTO dto)
        {
            var transportInfo = await _transportInformationRepository.FindAsync(x => x.Id == dto.TransportInformationId);
            if (transportInfo == null)
            {
                return new GenericActionResult(400, false, "Không tồn tại đơn vận chuyển");
            }

            var listTransportBill = await _transportBillRepository.GetListAsync(x => x.TransportInformationId == transportInfo.Id);
            if (listTransportBill.Count == 0)
            {
                return new GenericActionResult(400, false, "Không tồn tại đơn vận chuyển");
            }
            var listBill = await _billCustomerRepository.GetListAsync(x => listTransportBill.Select(y => y.BillCustomerId).ToList().Contains(x.Id));
            if (listBill.Count == 0)
            {
                return new GenericActionResult(400, false, "Không tồn tại đơn vận chuyển");
            }

            var store = await _storeRepository.GetAsync(x => x.Id == dto.StoreId);
            if (store == null)
            {
                return new GenericActionResult(400, false, "Cửa hàng không tồn tại");
            }

            if (transportInfo.TransportForm == Enums.Bills.TransportForm.Production)
            {
                if (transportInfo.CarrierWay == Enums.Carrier.GHTK)
                {
                    try
                    {
                        var responseDTO = await _ghtkService.CreateOrderAsync(dto);
                        if (!responseDTO.res.success)
                            return new GenericActionResult(400, false, responseDTO.res.message);

                        var res = JsonConvert.SerializeObject(responseDTO.res);
                        if (responseDTO.success == true)
                        {
                            await WriteLogCarrierShipping("GHTK", responseDTO.data, responseDTO.dataJs, 1, res);

                            listBill.ForEach(x =>
                            {
                                x.CarrierShippingCode = responseDTO.res.order.label;
                                x.CustomerBillPayStatus = Enums.Bills.CustomerBillPayStatus.Delivery;
                            });

                            transportInfo.Status = Enums.TransportStatus.Delivering;
                            transportInfo.CarrierShippingCode = responseDTO.res.order.label;
                            await _transportInformationRepository.UpdateAsync(transportInfo);
                            await _billCustomerRepository.UpdateManyAsync(listBill);
                            var data = JsonConvert.DeserializeObject<GHTKResponseDTO>(res);
                            return new GenericActionResult(200, true, "", data);
                        }
                        else
                        {
                            await WriteLogCarrierShipping("GHTK", responseDTO.data, responseDTO.dataJs, 0, res);
                            return new GenericActionResult(400, false, ErrorMessages.CarrierShipping.IntegrationError, responseDTO);
                        }
                    }
                    catch (Exception ex)
                    {
                        await WriteLogCarrierShipping("GHTK", "", JsonConvert.SerializeObject(dto), 0, ex.Message + " --- " + ex.StackTrace);
                        return new GenericActionResult(400, false, ErrorMessages.CarrierShipping.IntegrationError, ex.Message);
                    }
                }
                else if (transportInfo.CarrierWay == Enums.Carrier.ViettelPost)
                {
                    var VTPInfor = await _storeShippingInformationRepository.FindAsync(x => x.StoreId == store.Id && x.Carrier == Enums.Carrier.ViettelPost);
                    if (VTPInfor == null || String.IsNullOrWhiteSpace(VTPInfor.UserNameCarriers) || String.IsNullOrWhiteSpace(VTPInfor.PasswordCarries))
                    {
                        return new GenericActionResult(400, false, "Cửa hàng chưa được cấu hình đơn vị vận chuyển");
                    }
                    var hostApi = _configuration.GetValue<string>("ViettelPostAPI:Host.Api");
                    var createShippingAPi = _configuration.GetValue<string>("ViettelPostAPI:CreateShipping");
                    var getTokenShippingAPi = _configuration.GetValue<string>("ViettelPostAPI:GetToken");
                    var apiVTPCreate = hostApi + createShippingAPi;
                    var apiVTPGetToken = hostApi + getTokenShippingAPi;

                    var ghtk = MappingToVTP(dto);
                    var dtoJson = JsonConvert.SerializeObject(dto);
                    var data = JsonConvert.SerializeObject(ghtk);
                    try
                    {
                        var client = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Post, apiVTPGetToken);
                        //request.Headers.Add("Token", GHTKInfor.Token);

                        var content = new StringContent("{\"USERNAME\":\"" + VTPInfor.UserNameCarriers + "\",\"PASSWORD\":\"" + VTPInfor.PasswordCarries + "\"}", Encoding.UTF8, "application/json");
                        request.Content = content;
                        var response = await client.SendAsync(request);
                        var resContent = await response.Content.ReadAsStringAsync();
                        var responseDTO = JsonConvert.DeserializeObject<VTPResTokenDTO>(resContent);
                        var res = JsonConvert.SerializeObject(responseDTO);
                        if (responseDTO.error == false)
                        {
                            var clientCreateVTP = new HttpClient();
                            var requestCreate = new HttpRequestMessage(HttpMethod.Post, apiVTPCreate);
                            requestCreate.Headers.Add("Token", responseDTO.data.token);

                            var contentCreate = new StringContent(data, Encoding.UTF8, "application/json");
                            requestCreate.Content = contentCreate;
                            var responseCreate = await clientCreateVTP.SendAsync(requestCreate);
                            var statusCreate = responseCreate.StatusCode;
                            var resContentCreate = await responseCreate.Content.ReadAsStringAsync();
                            var responseDTOCreate = JsonConvert.DeserializeObject<VTPResOrderDTO>(resContentCreate);
                            var resCreate = JsonConvert.SerializeObject(responseDTOCreate);
                            if (responseDTOCreate.error == false)
                            {
                                await WriteLogCarrierShipping("VTP", data, dtoJson, 1, resCreate);

                                listBill.ForEach(x =>
                                {
                                    x.CarrierShippingCode = responseDTOCreate.data.ORDER_NUMBER;
                                    x.CustomerBillPayStatus = Enums.Bills.CustomerBillPayStatus.Delivery;
                                });

                                transportInfo.Status = Enums.TransportStatus.Delivering;
                                transportInfo.CarrierShippingCode = responseDTOCreate.data.ORDER_NUMBER;
                                await _transportInformationRepository.UpdateAsync(transportInfo);
                                await _billCustomerRepository.UpdateManyAsync(listBill);
                                return new GenericActionResult(200, true, "", responseDTOCreate);
                            }
                            else
                            {
                                await WriteLogCarrierShipping("VTP", data, dtoJson, 0, resCreate);
                                return new GenericActionResult(400, false, ErrorMessages.CarrierShipping.IntegrationError, responseDTOCreate);

                            }
                        }
                        else
                        {
                            await WriteLogCarrierShipping("VTP", data, dtoJson, 0, res);
                            return new GenericActionResult(400, false, ErrorMessages.CarrierShipping.IntegrationError, responseDTO);

                        }
                    }
                    catch (Exception ex)
                    {
                        await WriteLogCarrierShipping("VTP", data, dtoJson, 0, ex.Message + " --- " + ex.StackTrace);
                        return new GenericActionResult(400, false, ErrorMessages.CarrierShipping.IntegrationError, ex.Message);
                    }
                }
                else if (transportInfo.CarrierWay == Enums.Carrier.VNPost)
                {
                    //var VTPInfor = await _storeShippingInformationRepository.GetAsync(x => x.StoreId == store.Id && x.Carrier == Enums.Carrier.ViettelPost);
                    //if (VTPInfor == null || String.IsNullOrWhiteSpace(VTPInfor.UserNameCarriers) || String.IsNullOrWhiteSpace(VTPInfor.PasswordCarries))
                    //{
                    //    return new GenericActionResult(400, false, "Cửa hàng chưa được cấu hình đơn vị vận chuyển");
                    //}
                    var hostApi = _configuration.GetValue<string>("VNPostAPI:Host.Api");
                    var createShippingAPi = _configuration.GetValue<string>("VNPostAPI:CreateShipping");
                    var getTokenShippingAPi = _configuration.GetValue<string>("VNPostAPI:GetToken");
                    var apiVNPCreate = hostApi + createShippingAPi;
                    var apiVNPGetToken = hostApi + getTokenShippingAPi;

                    var ghtk = MappingToOrderVNPDTO(dto);
                    var dtoJson = JsonConvert.SerializeObject(dto);
                    var data = JsonConvert.SerializeObject(ghtk);
                    try
                    {
                        var client = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Post, apiVNPGetToken);
                        var content = new StringContent("{\"username\":\"" + "bongdeptrai0201@gmail.com" + "\",\"password\":\"" + "Ll@!0932378195" + "\",\"customerCode\":\"" + "C000366474" + "\"}", Encoding.UTF8, "application/json");
                        request.Content = content;
                        var response = await client.SendAsync(request);
                        var resContent = await response.Content.ReadAsStringAsync();
                        var responseDTO = JsonConvert.DeserializeObject<VNPResponseTokenDTO>(resContent);
                        var res = JsonConvert.SerializeObject(responseDTO);
                        if (responseDTO.Success == true)
                        {
                            var clientCreateVTP = new HttpClient();
                            var requestCreate = new HttpRequestMessage(HttpMethod.Post, apiVNPCreate);
                            requestCreate.Headers.Add("Token", responseDTO.Token);

                            var contentCreate = new StringContent(data, Encoding.UTF8, "application/json");
                            requestCreate.Content = contentCreate;
                            var responseCreate = await clientCreateVTP.SendAsync(requestCreate);
                            var statusCreate = responseCreate.StatusCode;
                            var resContentCreate = await responseCreate.Content.ReadAsStringAsync();
                            var responseDTOCreate = JsonConvert.DeserializeObject<VNPResponeOrderDTO>(resContentCreate);
                            var resCreate = JsonConvert.SerializeObject(responseDTOCreate);
                            if (statusCreate == System.Net.HttpStatusCode.OK)
                            {
                                await WriteLogCarrierShipping("VTP", data, dtoJson, 1, resCreate);

                                listBill.ForEach(x =>
                                {
                                    x.CarrierShippingCode = responseDTOCreate.orderHdrID;
                                    x.CustomerBillPayStatus = Enums.Bills.CustomerBillPayStatus.Delivery;
                                });

                                transportInfo.Status = Enums.TransportStatus.Delivering;
                                transportInfo.CarrierShippingCode = responseDTOCreate.orderHdrID;
                                await _transportInformationRepository.UpdateAsync(transportInfo);
                                await _billCustomerRepository.UpdateManyAsync(listBill);
                                return new GenericActionResult(200, true, "", responseDTOCreate);
                            }
                            else
                            {
                                await WriteLogCarrierShipping("VNP", data, dtoJson, 0, resCreate);
                                return new GenericActionResult(400, false, ErrorMessages.CarrierShipping.IntegrationError, responseDTOCreate);

                            }
                        }
                        else
                        {
                            await WriteLogCarrierShipping("VNP", data, dtoJson, 0, res);
                            return new GenericActionResult(400, false, ErrorMessages.CarrierShipping.IntegrationError, responseDTO);

                        }
                    }
                    catch (Exception ex)
                    {
                        await WriteLogCarrierShipping("VNP", data, dtoJson, 0, ex.Message + " --- " + ex.StackTrace);
                        return new GenericActionResult(400, false, ErrorMessages.CarrierShipping.IntegrationError, ex.Message);
                    }
                }
                else
                {
                    return new GenericActionResult(400, false, ErrorMessages.CarrierShipping.WrongCarrier);
                }
            }
            else
            {
                return new GenericActionResult(400, false, ErrorMessages.CarrierShipping.WrongCarrier);
            }
            return new GenericActionResult(200, true, "", null);
        }
        public async Task<IActionResult> GetAllPriceVTPAsync(PriceAllDTO dto)
        {
            var store = await _storeRepository.GetAsync(x => x.Id == dto.StoreId);
            if (store == null)
            {
                return new GenericActionResult(400, false, "Cửa hàng không tồn tại");
            }
            var VTPInfor = await _storeShippingInformationRepository.GetAsync(x => x.StoreId == store.Id && x.Carrier == Enums.Carrier.ViettelPost);
            if (VTPInfor == null || String.IsNullOrWhiteSpace(VTPInfor.UserNameCarriers) || String.IsNullOrWhiteSpace(VTPInfor.PasswordCarries))
            {
                return new GenericActionResult(400, false, "Cửa hàng chưa được cấu hình đơn vị vận chuyển");
            }
            var priceAll=MappingToPriceAllVTPDTO(dto);
            var hostApi = _configuration.GetValue<string>("ViettelPostAPI:Host.Api");
            var createGetAllPriceAPi = _configuration.GetValue<string>("ViettelPostAPI:GetAllPrice");
            var getTokenShippingAPi = _configuration.GetValue<string>("ViettelPostAPI:GetToken");
            var apiVTPCreate = hostApi + createGetAllPriceAPi;
            var apiVTPGetToken = hostApi + getTokenShippingAPi;
            var dtoJson = JsonConvert.SerializeObject(dto);
            var data = JsonConvert.SerializeObject(priceAll);
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, apiVTPGetToken);
                //request.Headers.Add("Token", GHTKInfor.Token);

                var content = new StringContent("{\"USERNAME\":\"" + VTPInfor.UserNameCarriers + "\",\"PASSWORD\":\"" + VTPInfor.PasswordCarries + "\"}", Encoding.UTF8, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                var resContent = await response.Content.ReadAsStringAsync();
                var responseDTO = JsonConvert.DeserializeObject<VTPResTokenDTO>(resContent);
                var res = JsonConvert.SerializeObject(responseDTO);
                if (responseDTO.error == false)
                {
                    var clientCreateVTP = new HttpClient();
                    var requestCreate = new HttpRequestMessage(HttpMethod.Post, apiVTPCreate);
                    requestCreate.Headers.Add("Token", responseDTO.data.token);

                    var contentCreate = new StringContent(data, Encoding.UTF8, "application/json");
                    requestCreate.Content = contentCreate;
                    var responseCreate = await clientCreateVTP.SendAsync(requestCreate);
                    var statusCreate = responseCreate.StatusCode;
                    var resContentCreate = await responseCreate.Content.ReadAsStringAsync();
                    var responseDTOCreate = JsonConvert.DeserializeObject<List<VTPResGetAllPriceDTO>>(resContentCreate);
                    var resCreate = JsonConvert.SerializeObject(responseDTOCreate);
                    if (responseDTOCreate != null)
                    {
                        await WriteLogCarrierShipping("VTP_GetAllPice", data, dtoJson, 1, resCreate);
                        return new GenericActionResult(200, true, "", responseDTOCreate);
                    }
                    else
                    {
                        await WriteLogCarrierShipping("VTP_GetAllPice", data, dtoJson, 0, resCreate);
                        return new GenericActionResult(400, false, ErrorMessages.CarrierShipping.IntegrationError, responseDTOCreate);

                    }
                }
                else
                {
                    await WriteLogCarrierShipping("VTP_GetAllPice", data, dtoJson, 0, res);
                    return new GenericActionResult(400, false, ErrorMessages.CarrierShipping.IntegrationError, responseDTO);

                }
            }
            catch (Exception ex)
            {
                await WriteLogCarrierShipping("VTP_GetAllPice", data, dtoJson, 0, ex.Message + " --- " + ex.StackTrace);
                return new GenericActionResult(400, false, ErrorMessages.CarrierShipping.IntegrationError, ex.Message);
            }
        }
        public async Task<IActionResult> GetPriceVTPAsync(PriceDTO dto)
        {
            var store = await _storeRepository.GetAsync(x => x.Id == dto.StoreId);
            if (store == null)
            {
                return new GenericActionResult(400, false, "Cửa hàng không tồn tại");
            }
            var VTPInfor = await _storeShippingInformationRepository.GetAsync(x => x.StoreId == store.Id && x.Carrier == Enums.Carrier.ViettelPost);
            if (VTPInfor == null || String.IsNullOrWhiteSpace(VTPInfor.UserNameCarriers) || String.IsNullOrWhiteSpace(VTPInfor.PasswordCarries))
            {
                return new GenericActionResult(400, false, "Cửa hàng chưa được cấu hình đơn vị vận chuyển");
            }
            var price = MappingToPriceVTPDTO(dto);
            var hostApi = _configuration.GetValue<string>("ViettelPostAPI:Host.Api");
            var createGetAllPriceAPi = _configuration.GetValue<string>("ViettelPostAPI:GetPrice");
            var getTokenShippingAPi = _configuration.GetValue<string>("ViettelPostAPI:GetToken");
            var apiVTPCreate = hostApi + createGetAllPriceAPi;
            var apiVTPGetToken = hostApi + getTokenShippingAPi;
            var dtoJson = JsonConvert.SerializeObject(dto);
            var data = JsonConvert.SerializeObject(price);
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, apiVTPGetToken);
                //request.Headers.Add("Token", GHTKInfor.Token);

                var content = new StringContent("{\"USERNAME\":\"" + VTPInfor.UserNameCarriers + "\",\"PASSWORD\":\"" + VTPInfor.PasswordCarries + "\"}", Encoding.UTF8, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                var resContent = await response.Content.ReadAsStringAsync();
                var responseDTO = JsonConvert.DeserializeObject<VTPResTokenDTO>(resContent);
                var res = JsonConvert.SerializeObject(responseDTO);
                if (responseDTO.error == false)
                {
                    var clientCreateVTP = new HttpClient();
                    var requestCreate = new HttpRequestMessage(HttpMethod.Post, apiVTPCreate);
                    requestCreate.Headers.Add("Token", responseDTO.data.token);

                    var contentCreate = new StringContent(data, Encoding.UTF8, "application/json");
                    requestCreate.Content = contentCreate;
                    var responseCreate = await clientCreateVTP.SendAsync(requestCreate);
                    var statusCreate = responseCreate.StatusCode;
                    var resContentCreate = await responseCreate.Content.ReadAsStringAsync();
                    var responseDTOCreate = JsonConvert.DeserializeObject<VTPResGetPriceDTO>(resContentCreate);
                    var resCreate = JsonConvert.SerializeObject(responseDTOCreate);
                    if (responseDTOCreate.error == false)
                    {
                        await WriteLogCarrierShipping("VTP_GetPice", data, dtoJson, 1, resCreate);
                        return new GenericActionResult(200, true, "", responseDTOCreate);
                    }
                    else
                    {
                        await WriteLogCarrierShipping("VTP_GetPice", data, dtoJson, 0, resCreate);
                        return new GenericActionResult(400, false, ErrorMessages.CarrierShipping.IntegrationError, responseDTOCreate);

                    }
                }
                else
                {
                    await WriteLogCarrierShipping("VTP_GetPice", data, dtoJson, 0, res);
                    return new GenericActionResult(400, false, ErrorMessages.CarrierShipping.IntegrationError, responseDTO);

                }
            }
            catch (Exception ex)
            {
                await WriteLogCarrierShipping("VTP_GetPice", data, dtoJson, 0, ex.Message + " --- " + ex.StackTrace);
                return new GenericActionResult(400, false, ErrorMessages.CarrierShipping.IntegrationError, ex.Message);
            }
        }
        public async Task<IActionResult> GetPriceVNPAsync(PriceVNPDTO dto)
        {
            var store = await _storeRepository.GetAsync(x => x.Id == dto.StoreId);
            if (store == null)
            {
                return new GenericActionResult(400, false, "Cửa hàng không tồn tại");
            }
            var price = MappingToPriceVNPRequestDTO(dto);
            var hostApi = _configuration.GetValue<string>("VNPostAPI:Host.Api");
            var createGetAllPriceAPi = _configuration.GetValue<string>("VNPostAPI:GetPrice");
            var getTokenShippingAPi = _configuration.GetValue<string>("VNPostAPI:GetToken");
            var apiVTPCreate = hostApi + createGetAllPriceAPi;
            var apiVNPGetToken = hostApi + getTokenShippingAPi;
            var dtoJson = JsonConvert.SerializeObject(dto);
            var data = JsonConvert.SerializeObject(price);
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
                var res = JsonConvert.SerializeObject(responseDTO);
                if (responseDTO.Success == true)
                {
                    var clientCreateVNP = new HttpClient();
                    var requestCreate = new HttpRequestMessage(HttpMethod.Post, apiVTPCreate);
                    requestCreate.Headers.Add("Token", responseDTO.Token);
                    var contentCreate = new StringContent(data, Encoding.UTF8, "application/json");
                    requestCreate.Content = contentCreate;
                    var responseCreate = await clientCreateVNP.SendAsync(requestCreate);
                    var statusCreate = responseCreate.StatusCode;
                    var resContentCreate = await responseCreate.Content.ReadAsStringAsync();
                    var responseDTOCreate = JsonConvert.DeserializeObject<List<VNPResponsePriceDTO>>(resContentCreate);
                    var resCreate = JsonConvert.SerializeObject(responseDTOCreate);
                    if (responseDTOCreate!=null && responseDTOCreate.Count>0)
                    {
                        await WriteLogCarrierShipping("VNP_GetPice", data, dtoJson, 1, resCreate);
                        return new GenericActionResult(200, true, "", responseDTOCreate);
                    }
                    else
                    {
                        await WriteLogCarrierShipping("VNP_GetPice", data, dtoJson, 0, resCreate);
                        return new GenericActionResult(400, false, ErrorMessages.CarrierShipping.IntegrationError, responseDTOCreate);

                    }
                }
                else
                {
                    await WriteLogCarrierShipping("VNP_GetPice", data, dtoJson, 0, res);
                    return new GenericActionResult(400, false, ErrorMessages.CarrierShipping.IntegrationError, responseDTO);

                }
            }
            catch (Exception ex)
            {
                await WriteLogCarrierShipping("VTP_GetPice", data, dtoJson, 0, ex.Message + " --- " + ex.StackTrace);
                return new GenericActionResult(400, false, ErrorMessages.CarrierShipping.IntegrationError, ex.Message);
            }
        }

        private TransportOrderVTPDTO MappingToVTP(CarrierShippingInformationDTO dto)
        {
            var store = _storeRepository.GetAsync(x => x.Id == dto.StoreId).Result;

            var provinces = _provinceRepository.GetListAsync(x => x.Code == dto.ReceiverProvince || x.Id == store.ProvinceId).Result;
            var districts = _districtRepository.GetListAsync(x => x.Code == dto.ReceiverDistrict || x.Id == store.DistricId).Result;
            var wards = _wardRepository.GetListAsync(x => x.Code == dto.ReceiverWard || x.Id == store.WardId).Result;
            TransportOrderVTPDTO shipping = new TransportOrderVTPDTO();

            var storeProvince = provinces.FirstOrDefault(x => x.Id == store.ProvinceId);
            var storeDistrict = districts.FirstOrDefault(x => x.Id == store.DistricId);
            var storeWard = wards.FirstOrDefault(x => x.Id == store.WardId);

            shipping.ORDER_NUMBER = dto.VTShippingCode;
            shipping.GROUPADDRESS_ID = 5818802;
            shipping.CUS_ID = 123;
            shipping.DELIVERY_DATE = dto.PickDate.HasValue ? dto.PickDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : null;
            shipping.SENDER_FULLNAME = store.Name;
            //shipping.SENDER_ADDRESS = store.Address;
            shipping.SENDER_ADDRESS = "Số 5A ngách 22 ngõ 282 Kim Giang, Đại Kim (0967.363.789), Quận Hoàng Mai, Hà Nội";
            shipping.SENDER_PHONE = store.PhoneNumber;
            shipping.SENDER_EMAIL = store.Email;
            shipping.SENDER_WARD = 0;
            shipping.SENDER_DISTRICT = 4;
            shipping.SENDER_PROVINCE = 1;
            shipping.SENDER_LATITUDE = 0;
            shipping.SENDER_LONGITUDE = 0;

            var receiverProvince = provinces.FirstOrDefault(x => x.Code == dto.ReceiverProvince);
            var receiverDistrict = districts.FirstOrDefault(x => x.Code == dto.ReceiverDistrict);
            var receiverWard = wards.FirstOrDefault(x => x.Code == dto.ReceiverWard);

            shipping.RECEIVER_FULLNAME = dto.ReceiverName;
            //shipping.RECEIVER_ADDRESS = dto.ReceiverAddress;
            shipping.RECEIVER_ADDRESS = "1 NKKN P.Nguyễn Thái Bình, Quận 1, TP Hồ Chí Minh";
            shipping.RECEIVER_PHONE = dto.ReceiverPhone;
            shipping.RECEIVER_EMAIL = dto.ReceiverEmail;
            shipping.RECEIVER_WARD = Convert.ToInt32(dto.ReceiverWard);
            shipping.RECEIVER_DISTRICT = Convert.ToInt32(dto.ReceiverDistrict);
            shipping.RECEIVER_PROVINCE = Convert.ToInt32(dto.ReceiverProvince);
            shipping.RECEIVER_LATITUDE = 0;
            shipping.RECEIVER_LONGITUDE = 0;


            shipping.PRODUCT_NAME = dto.ShippingProductName;
            shipping.PRODUCT_QUANTITY = dto.Quantity.GetValueOrDefault();
            shipping.PRODUCT_PRICE = dto.ProductValue.HasValue ? (int)dto.ProductValue : 0;
            shipping.PRODUCT_WEIGHT = (int)(dto.Weight??0);
            shipping.PRODUCT_LENGTH = (int)(dto.Length??0);
            shipping.PRODUCT_WIDTH = (int)(dto.Width??0);
            shipping.PRODUCT_HEIGHT = (int)(dto.Height??0);
            shipping.PRODUCT_TYPE = "HH";

            int payment = 1;
            if (dto.ReceiverPayingShippingFee)
            {
                if (dto.IsCOD.HasValue && dto.IsCOD.Value)
                {
                    payment = 2;
                }
                else
                {
                    payment = 4;
                }
            }
            else
            {
                if (dto.IsCOD.HasValue && dto.IsCOD.Value)
                {
                    payment = 3;
                }
                else
                {
                    payment = 1;
                }
            }

            shipping.ORDER_PAYMENT = payment;
            shipping.ORDER_NOTE = dto.Note;
            shipping.MONEY_COLLECTION = (dto.CODValue.HasValue && (payment == 2 || payment == 4)) ? (int)dto.CODValue : 0;
            shipping.ORDER_SERVICE = "LCOD";

            var listItem = shipping.LIST_ITEM = new List<ProductVTPDTO>();
            listItem.Add(new ProductVTPDTO()
            {
                PRODUCT_NAME = dto.ShippingProductName,
                PRODUCT_PRICE = dto.ProductValue.HasValue ? (int)dto.ProductValue : 0,
                PRODUCT_QUANTITY = dto.Quantity.GetValueOrDefault(),
                PRODUCT_WEIGHT = (int)dto.Weight
            });
            return shipping;
        }
        private PriceAllVTPDTO MappingToPriceAllVTPDTO(PriceAllDTO dto)
        {
            PriceAllVTPDTO shipping = new PriceAllVTPDTO();
            shipping.SENDER_DISTRICT = 4;
            shipping.SENDER_PROVINCE = 1;
            shipping.RECEIVER_DISTRICT = 43;
            shipping.RECEIVER_PROVINCE = 2;
            shipping.PRODUCT_PRICE = dto.ProductPrice.HasValue ? (int)dto.ProductPrice : 0;
            shipping.PRODUCT_WEIGHT = (int)dto.ProductWeight;
            shipping.PRODUCT_LENGTH = (int)dto.ProductLengt;
            shipping.PRODUCT_WIDTH = (int)dto.ProductWidth;
            shipping.PRODUCT_HEIGHT = (int)dto.ProductHeight;
            shipping.PRODUCT_TYPE = "HH";
            shipping.MONEY_COLLECTION = (int)dto.MoneyCollection;
            shipping.TYPE=dto.Type;
            return shipping;
        }
        private PriceVTPDTO MappingToPriceVTPDTO(PriceDTO dto)
        {
            PriceVTPDTO shipping = new PriceVTPDTO();
            shipping.SENDER_DISTRICT = 4;
            shipping.SENDER_PROVINCE = 1;
            shipping.RECEIVER_DISTRICT = 43;
            shipping.RECEIVER_PROVINCE = 2;
            shipping.PRODUCT_PRICE = dto.ProductPrice.HasValue ? (int)dto.ProductPrice : 0;
            shipping.PRODUCT_WEIGHT = (int)dto.ProductWeight;
            shipping.PRODUCT_LENGTH = (int)dto.ProductLengt;
            shipping.PRODUCT_WIDTH = (int)dto.ProductWidth;
            shipping.PRODUCT_HEIGHT = (int)dto.ProductHeight;
            shipping.PRODUCT_TYPE = "HH";
            shipping.MONEY_COLLECTION = (int)dto.MoneyCollection;
            shipping.NATIONAL_TYPE = dto.NationalType;
            shipping.ORDER_SERVICE = dto.OrderService;
            shipping.ORDER_SERVICE_ADD = dto.OrderServiceAdd;
            return shipping;
        }
        private OrderVNPDTO MappingToOrderVNPDTO(CarrierShippingInformationDTO dto)
        {
            OrderVNPDTO orderVNPDTO = new OrderVNPDTO();
            var store = _storeRepository.GetAsync(x => x.Id == dto.StoreId).Result;
            orderVNPDTO.OrderCreationStatus = true;
            orderVNPDTO.Type = "GUI";
            orderVNPDTO.CustomerCode = "C000366474";
            orderVNPDTO.ContractCode = null;
            orderVNPDTO.InformationOrder=new InformationOrderVNP();
            orderVNPDTO.InformationOrder.SenderName = "Trần Đức Lương";
            orderVNPDTO.InformationOrder.SenderPhone = "0392332583";
            orderVNPDTO.InformationOrder.SenderMail = "bongdeptrai0201@gmail.com";
            orderVNPDTO.InformationOrder.SenderAddress = "Ngõ 82 Duy Tân, Dịch Vọng Hậu, Cầu Giấy, Hà Nội";
            orderVNPDTO.InformationOrder.SenderProvinceCode = dto.SenderProvince;
            orderVNPDTO.InformationOrder.SenderProvinceName = dto.SenderProvinceName;
            orderVNPDTO.InformationOrder.SenderDistrictCode = dto.SenderDistrict;
            orderVNPDTO.InformationOrder.SenderDistrictName = dto.SenderDistrictName;
            orderVNPDTO.InformationOrder.SenderCommuneCode = dto.SenderWard;
            orderVNPDTO.InformationOrder.SenderCommuneName = dto.SenderWardName;
            orderVNPDTO.InformationOrder.ReceiverName = dto.ReceiverName;
            orderVNPDTO.InformationOrder.ReceiverAddress = dto.ReceiverAddress;
            orderVNPDTO.InformationOrder.ReceiverProvinceCode = dto.ReceiverProvince;
            orderVNPDTO.InformationOrder.ReceiverProvinceName = dto.ReceiverProvinceName;
            orderVNPDTO.InformationOrder.ReceiverDistrictCode = dto.ReceiverDistrict;
            orderVNPDTO.InformationOrder.ReceiverDistrictName = dto.ReceiverDistrictName;
            orderVNPDTO.InformationOrder.ReceiverCommuneCode = dto.ReceiverWard;
            orderVNPDTO.InformationOrder.ReceiverCommuneName = dto.ReceiverWardName;
            orderVNPDTO.InformationOrder.ReceiverPhone = dto.ReceiverPhone;
            orderVNPDTO.InformationOrder.ReceiverEmail = null;
            orderVNPDTO.InformationOrder.ServiceCode = "ETN011";
            orderVNPDTO.InformationOrder.OrgCodeCollect = null;
            orderVNPDTO.InformationOrder.OrgCodeAccept = "134800";
            orderVNPDTO.InformationOrder.SaleOrderCode = dto.SaleOrderCodeVNP;
            orderVNPDTO.InformationOrder.ContentNote = dto.Note;
            orderVNPDTO.InformationOrder.Weight = dto.Weight??0;
            orderVNPDTO.InformationOrder.Width = dto.Width;
            orderVNPDTO.InformationOrder.Length = dto.Length;
            orderVNPDTO.InformationOrder.Height = dto.Height;
            orderVNPDTO.InformationOrder.Vehicle = "BO";
            orderVNPDTO.InformationOrder.SendType = "1";
            orderVNPDTO.InformationOrder.IsBroken = "0";
            orderVNPDTO.InformationOrder.DeliveryTime = "N";
            orderVNPDTO.InformationOrder.DeliveryRequire = "1";
            orderVNPDTO.InformationOrder.DeliveryInstruction = "";
            orderVNPDTO.InformationOrder.AddonService = new List<VNPAddonService>();
            orderVNPDTO.InformationOrder.AdditionRequest = new List<VNPAdditionRequest>();

            return orderVNPDTO;
        }
        private PriceVNPRequestDTO MappingToPriceVNPRequestDTO(PriceVNPDTO dto)
        {
            PriceVNPRequestDTO request=new PriceVNPRequestDTO();
            request.data=new PriceVNPData();
            request.Scope = "1";
            request.CustomerCode = "C000366474";
            request.ContractCode = null;
            request.data.SenderProvinceCode = "10";
            request.data.SenderProvinceName = "Hà Nội";
            request.data.SenderDistrictCode = "1100";
            request.data.SenderDistrictName = "Hoàn kiếm";
            request.data.SenderCommuneCode = "11022";
            request.data.SenderCommuneName = "Tràng tiền"; ;
            request.data.ReceiverProvinceCode = "66";
            request.data.ReceiverProvinceName = "Lâm đồng";
            request.data.ReceiverDistrictCode = "6680";
            request.data.ReceiverDistrictName = "Đức trọng";
            request.data.ReceiverCommuneCode = "66812";
            request.data.ReceiverCommuneName = "Phú hội";
            request.data.ReceiverNational = "VN";
            request.data.ReceiverCity = null;
            request.data.OrgCodeAccept = "110020";
            request.data.ReceiverPostCode = "";
            request.data.Weight = 1000;
            request.data.Width = 20;
            request.data.Length = 100;
            request.data.Height = 15;
            request.data.ServiceCode = "";
            request.data.AddonService = new List<VNPAddonService>();
            request.data.AdditionRequest = new List<VNPAdditionRequest>();
            request.data.Vehicle = null;
            return request;
        }
        private Enums.Bills.CustomerBillPayStatus? MapBillGHTKStatus(Enums.CarrierShipping.GHTKStatus status)
        {
            if (status == Enums.CarrierShipping.GHTKStatus.CANCEL)
                return Enums.Bills.CustomerBillPayStatus.Cancel;
            if (status == Enums.CarrierShipping.GHTKStatus.DELIVERED_NOT_CHECKED || status == Enums.CarrierShipping.GHTKStatus.CHECKED)
                return Enums.Bills.CustomerBillPayStatus.Success;

            return null;
        }

        private async Task WriteLogCarrierShipping(string carrier, string data, string dto, int status, string response)
        {
            CarrierShippingLog log = new CarrierShippingLog()
            {
                carrier = carrier,
                data = data,
                dto = dto,
                status = status,
                response = response
            };

            await _carrierShippingLogRepository.InsertAsync(log);
        }

        public async Task<(bool success, string message, VTPResTokenDTO data)> GetTokenViettelPost(Guid StoreId)
        {
            var VTPInfor = await _storeShippingInformationRepository.GetAsync(x => x.StoreId == StoreId && x.Carrier == Enums.Carrier.ViettelPost);
            if (VTPInfor == null || String.IsNullOrWhiteSpace(VTPInfor.UserNameCarriers) || String.IsNullOrWhiteSpace(VTPInfor.PasswordCarries))
                return (false, "Cửa hàng chưa được cấu hình đơn vị vận chuyển", null);

            var hostApi = _configuration.GetValue<string>("ViettelPostAPI:Host.Api");
            var getTokenShippingAPi = _configuration.GetValue<string>("ViettelPostAPI:GetToken");
            var apiVTPGetToken = hostApi + getTokenShippingAPi;

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, apiVTPGetToken);

            var content = new StringContent("{\"USERNAME\":\"" + VTPInfor.UserNameCarriers + "\",\"PASSWORD\":\"" + VTPInfor.PasswordCarries + "\"}", Encoding.UTF8, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            var resContent = await response.Content.ReadAsStringAsync();
            var responseDTO = JsonConvert.DeserializeObject<VTPResTokenDTO>(resContent);
            return (true, "", responseDTO);
        }

        private async Task<List<PostOfficeDTO>> GetPostOfficeViettelPost(Guid StoreId)
        {
            var result = new List<PostOfficeDTO>();
            var hostApi = _configuration.GetValue<string>("ViettelPostAPI:Host.Api");
            var getPostOffice = _configuration.GetValue<string>("ViettelPostAPI:GetPostOffice");
            var apiVTPGetPostOffice = hostApi + getPostOffice;

            var getTokenResult = await GetTokenViettelPost(StoreId);
            if (getTokenResult.success && getTokenResult.data.status == 200)
            {
                var apiKey = getTokenResult.data.data.token;
                using (HttpClient client = new HttpClient())
                {
                    var requestCreate = new HttpRequestMessage(HttpMethod.Get, apiVTPGetPostOffice);
                    requestCreate.Headers.Add("Token", apiKey);
                    var responseCreate = await client.SendAsync(requestCreate);
                    var statusCreate = responseCreate.StatusCode;
                    var resContentCreate = await responseCreate.Content.ReadAsStringAsync();
                    var responseDTOCreate = JsonConvert.DeserializeObject<PostOfficeRespon>(resContentCreate);
                    result = responseDTOCreate.data.Select(x => new PostOfficeDTO
                    {
                        Name = x.TEN_BUUCUC,
                        Address = x.DIA_CHI,
                        Province = x.TEN_TINH,
                        Ward = x.TEN_PHUONGXA,
                        District = x.TEN_QUANHUYEN
                    }).ToList();
                }
            }

            return result;
        }

        private async Task<List<PostOfficeDTO>> GetPostOfficeGHTK()
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var result = (await _ghtkPostOfficeRepository.GetListAsync()).Select(x => new PostOfficeDTO
                {
                    Name = x.Name,
                    Address = x.Address,
                    District = x.DistrictName,
                    Province = x.ProvinceName,
                    Ward = x.WardName
                }).ToList();

                return result;
            }
        }

        public async Task<List<PostOfficeDTO>> GetPostOffice(GetPostOfficeParam param)
        {
            var result = new List<PostOfficeDTO>();
            if (param.Carrier == Enums.Carrier.GHTK)
                result = await GetPostOfficeGHTK();

            if (param.Carrier == Enums.Carrier.ViettelPost && param.StoreId.HasValue)
                result = await GetPostOfficeViettelPost(param.StoreId.Value);

            return result;
        }
        public async Task<VNPResponseTokenDTO> GetTokenVNPost(string username, string password, string customerCode)
        {
            var hostApi = _configuration.GetValue<string>("VNPostAPI:Host.Api");
            var getTokenShippingAPi = _configuration.GetValue<string>("VNPostAPI:GetToken");
            var apiVTPGetToken = hostApi + getTokenShippingAPi;
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, apiVTPGetToken);
                var content = new StringContent("{\"username\":\"" + username + "\",\"password\":\"" + password +
                    "\",\"customerCode\":\"" + customerCode + "\"}", Encoding.UTF8, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                var resContent = await response.Content.ReadAsStringAsync();
                var responseDTO = JsonConvert.DeserializeObject<VNPResponseTokenDTO>(resContent);
                var res = JsonConvert.SerializeObject(responseDTO);
                return responseDTO;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
