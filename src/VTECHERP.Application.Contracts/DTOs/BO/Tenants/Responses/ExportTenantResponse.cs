using VTECHERP.Domain.Shared.Helper.Excel.Attributes;

namespace VTECHERP.DTOs.BO.Tenants.Responses
{
    public class ExportTenantResponse
    {

        [Header("ID")]
        public string Id { get; set; }


        [Header("Tên doanh nghiệp/đại lý")]
        public string Name { get; set; }


        [Header("Kiểu")]
        public string Type { get; set; }


        [Header("Số điện thoại")]
        public string PhoneNumber { get; set; }


        [Header("Địa chỉ")]
        public string Address { get; set; }


        [Header("Thành phố")]
        public string Province { get; set; }


        [Header("Quận huyện")]
        public string District { get; set; }


        [Header("Số cửa hàng")]
        public string StoreCount { get; set; }


        [Header("Ngày hết hạn")]
        public string Expiration { get; set; }


        [Header("Trạng thái")]
        public string Status { get; set; }

        [Header("Ngày tạo")]
        public string CreationTime { get; set; }

        [Header("Người tạo")]
        public string CreatorName { get; set; }

    }
}