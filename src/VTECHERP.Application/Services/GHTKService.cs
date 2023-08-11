using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using VTECHERP.Constants;
using VTECHERP.DTOs.CarrierShipping;
using VTECHERP.DTOs.GHTK;
using VTECHERP.Entities;
using VTECHERP.Helper;
using VTECHERP.ServiceInterfaces;
using VTECHERP.Services.Interface;

namespace VTECHERP.Services
{
    public class GHTKService : IGHTKService
    {
        private readonly IRepository<StoreShippingInformation> _storeShippingInformationRepository;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Stores> _storeRepository;
        private readonly IRepository<Provinces> _provinceRepository;
        private readonly IRepository<Districts> _districtRepository;
        private readonly IRepository<Wards> _wardRepository;
        private readonly IRepository<GHTKPostOffice> _ghtkPostOfficeRepository;
        private readonly IDataFilter _dataFilter;
        private readonly IRepository<TransporstBills> _transportBillRepository;
        private readonly IRepository<BillCustomerProduct> _billCustomerProductRepository;
        public GHTKService(
             IRepository<StoreShippingInformation> storeShippingInformationRepository
            , IConfiguration configuration
            , IRepository<Stores> storeRepository
            , IRepository<Provinces> provinceRepository
            , IRepository<Districts> districtRepository
            , IRepository<Wards> wardRepository
            , IRepository<GHTKPostOffice> ghtkPostOfficeRepository
            , IDataFilter dataFilter
            , IRepository<TransporstBills> transportBillRepository
            , IRepository<BillCustomerProduct> billCustomerProductRepository
            )
        {
            _storeShippingInformationRepository = storeShippingInformationRepository;
            _configuration = configuration;
            _storeRepository = storeRepository;
            _provinceRepository = provinceRepository;
            _districtRepository = districtRepository;
            _wardRepository = wardRepository;
            _ghtkPostOfficeRepository = ghtkPostOfficeRepository;
            _dataFilter = dataFilter;
            _transportBillRepository = transportBillRepository;
            _billCustomerProductRepository = billCustomerProductRepository;
        }
        public async Task<(bool success, string message, string data, string dataJs, GHTKResponseDTO? res)> CreateOrderAsync(CarrierShippingInformationDTO param)
        {
            StoreShippingInformation GHTKInfor = null;

            GHTKInfor = await _storeShippingInformationRepository.FindAsync(x => x.StoreId == param.StoreId && x.Carrier == Enums.Carrier.GHTK);

            if (GHTKInfor == null || String.IsNullOrWhiteSpace(GHTKInfor.Token))
                return (false, "Cửa hàng chưa được cấu hình đơn vị vận chuyển", "", "", null);

            var hostApi = _configuration.GetValue<string>("GHTKAPI:Host.Api");
            var createShippingAPi = _configuration.GetValue<string>("GHTKAPI:CreateShipping");
            var apiGHTKCreate = hostApi + createShippingAPi;

            var ghtk = MappingToGHTK(param);
            if (!ghtk.success)
                return (false, ghtk.message, "", "", null);

            var dtoJson = JsonConvert.SerializeObject(param);
            var data = JsonConvert.SerializeObject(ghtk.data);

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, apiGHTKCreate);
            request.Headers.Add("Token", GHTKInfor.Token);

            var content = new StringContent(data, Encoding.UTF8, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            var status = response.StatusCode;
            var resContent = await response.Content.ReadAsStringAsync();
            var responseDTO = JsonConvert.DeserializeObject<GHTKResponseDTO>(resContent);

            return (true, "", data, dtoJson, responseDTO);
        }

        private (bool success, string message, TransportOrderGHTKDTO? data) MappingToGHTK(CarrierShippingInformationDTO dto)
        {
            var store = _storeRepository.GetAsync(x => x.Id == dto.StoreId).Result;

            var provinces = _provinceRepository.GetListAsync(x => x.Code == dto.ReceiverProvince || x.Id == store.ProvinceId).Result;
            var districts = _districtRepository.GetListAsync(x => x.Code == dto.ReceiverDistrict || x.Id == store.DistricId).Result;
            var wards = _wardRepository.GetListAsync(x => x.Code == dto.ReceiverWard || x.Id == store.WardId).Result;
            TransportOrderGHTKDTO shipping = new TransportOrderGHTKDTO();

            var storeProvince = provinces.FirstOrDefault(x => x.Id == store.ProvinceId);
            var storeDistrict = districts.FirstOrDefault(x => x.Id == store.DistricId);
            var storeWard = wards.FirstOrDefault(x => x.Id == store.WardId);

            var order = shipping.order = new OrderGHTKDTO();
            order.id = dto.VTShippingCode;
            order.pick_name = store.Name;
            order.pick_money = (int)dto.CODValue.GetValueOrDefault();

            if (dto.FormSend == Enums.CarrierShipping.FormSendReceive.Direction)
            {
                order.pick_address = store.Address;
                order.pick_street = store.Address;
                order.pick_province = storeProvince != null ? storeProvince.Name : "";
                order.pick_district = storeDistrict != null ? storeDistrict.Name : "";
                order.pick_ward = storeWard != null ? storeWard.Name : "";
                order.pick_tel = store.PhoneNumber;
            }

            if (dto.FormSend == Enums.CarrierShipping.FormSendReceive.ByPostOffice && !string.IsNullOrEmpty(dto.SenderPostOffice))
            {
                GHTKPostOffice postOffice;
                using (_dataFilter.Disable<IMultiTenant>())
                {
                    postOffice = _ghtkPostOfficeRepository.FirstOrDefaultAsync(x => x.Name == dto.SenderPostOffice).Result;
                    if (postOffice == null)
                        return (false, "Không tìm thấy thông tin bưu cục trong hệ thống", null);
                }

                order.pick_address = postOffice.Address;
                order.pick_street = "";
                order.pick_province = postOffice.ProvinceName;
                order.pick_district = postOffice.DistrictName;
                order.pick_ward = postOffice.WardName;
            }
            else if (dto.FormSend == Enums.CarrierShipping.FormSendReceive.ByPostOffice && string.IsNullOrEmpty(dto.SenderPostOffice))
                return (false, "Thiếu thông tin bưu cục gửi", null);

            var receiverProvince = provinces.FirstOrDefault(x => x.Code == dto.ReceiverProvince);
            var receiverDistrict = districts.FirstOrDefault(x => x.Code == dto.ReceiverDistrict);
            var receiverWard = wards.FirstOrDefault(x => x.Code == dto.ReceiverWard);

            order.name = dto.ReceiverName;
            order.tel = dto.ReceiverPhone;

            order.hamlet = "Khác";
            order.street = dto.ReceiverAddress;
            order.email = dto.ReceiverEmail;
            order.is_freeship = dto.ReceiverPayingShippingFee ? "0" : "1";
            order.pick_date = dto.PickDate.HasValue ? dto.PickDate.Value.ToShortDateString() : null;
            order.note = dto.Note;

            if (dto.FormReceive == Enums.CarrierShipping.FormSendReceive.Direction)
            {
                order.address = dto.ReceiverAddress;
                order.province = receiverProvince != null ? receiverProvince.Name : "";
                order.district = receiverDistrict != null ? receiverDistrict.Name : "";
                order.ward = receiverWard != null ? receiverWard.Name : "";
            }

            if (dto.FormReceive == Enums.CarrierShipping.FormSendReceive.ByPostOffice && !string.IsNullOrEmpty(dto.ReceiverPostOffice))
            {
                GHTKPostOffice postOffice;
                using (_dataFilter.Disable<IMultiTenant>())
                {
                    postOffice = _ghtkPostOfficeRepository.FirstOrDefaultAsync(x => x.Name == dto.ReceiverPostOffice).Result;
                    if (postOffice == null)
                        return (false, "Không tìm thấy thông tin bưu cục trong hệ thống", null);
                }

                order.address = postOffice.Address;
                order.province = postOffice.ProvinceName;
                order.district = postOffice.DistrictName;
                order.ward = postOffice.WardName;
            }
            else if (dto.FormReceive == Enums.CarrierShipping.FormSendReceive.ByPostOffice && string.IsNullOrEmpty(dto.ReceiverPostOffice))
                return (false, "Thiếu thông tin bưu cục nhận", null);

            var products = shipping.products = new List<ProductGHTKDTO>();

            products.Add(new ProductGHTKDTO()
            {
                name = dto.ShippingProductName,
                price = (int)dto.ProductValue.GetValueOrDefault(),
                weight = ((double)dto.Weight.GetValueOrDefault())/1000,
                quantity = dto.Quantity,
            });

            order.value = products.Sum(x => (x.price ?? 0) * (x.quantity ?? 0));
            return (true, "", shipping);
        }
    }
}
