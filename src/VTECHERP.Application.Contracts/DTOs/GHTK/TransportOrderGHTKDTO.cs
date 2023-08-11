using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.GHTK
{
    public class TransportOrderGHTKDTO
    {
        public OrderGHTKDTO order { get; set; }
        public List<ProductGHTKDTO> products { get; set; }
    }
    public class OrderGHTKDTO
    {
        public string id { get; set; }
        public string pick_name { get; set; }
        /// <summary>
        /// Số tiền thu COD
        /// </summary>
        public int pick_money { get; set; }
        /// <summary>
        /// String - Địa chỉ ngắn gọn để lấy nhận hàng hóa
        /// </summary>
        public string pick_address { get; set; }
        public string? pick_address_id { get; set; }
        /// <summary>
        /// Địa chỉ lấy hàng (Tỉnh/thành)
        /// </summary>
        public string pick_province { get; set; }
        /// <summary>
        /// Địa chỉ lấy hàng (Quận/huyện)
        /// </summary>
        /// 
        public string pick_district { get; set; }
        /// <summary>
        /// Địa chỉ lấy hàng (Phường/Xã)
        /// </summary>
        public string? pick_ward { get; set; }
        /// <summary>
        /// Địa chỉ lấy hàng (Đường)
        /// </summary>
        public string? pick_street { get; set; }
        /// <summary>
        /// Số điện thoại nơi lấy hàng
        /// </summary>
        public string pick_tel { get; set; }
        /// <summary>
        /// Email nơi lấy hàng
        /// </summary>
        public string? pick_email { get; set; }
        /// <summary>
        /// Số điện thoại người nhận hàng
        /// </summary>
        public string tel { get; set; }
        /// <summary>
        /// Tên người nhận hàng
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Địa chỉ người nhận
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// Địa chỉ nhận hàng (Tỉnh/thành)
        /// </summary>
        public string province { get; set; }
        /// <summary>
        /// Địa chỉ nhận hàng (Quận/huyện)
        /// </summary>
        /// 
        public string district { get; set; }
        /// <summary>
        /// Địa chỉ nhận hàng (Phường/Xã)
        /// </summary>
        public string? ward { get; set; }
        /// <summary>
        /// Địa chỉ nhận hàng (Đường)
        /// </summary>
        public string? street { get; set; }
        /// <summary>
        /// Địa chỉ nhận hàng chi tiết
        /// </summary>
        public string hamlet { get; set; } = "Khác";
        /// <summary>
        /// Email người nhận hàng
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// Freeship cho người nhận hàng(0/1)
        /// </summary>
        public string? is_freeship { get; set; }
        /// <summary>
        /// String YYYY/MM/DD - Hẹn ngày lấy hàng
        /// </summary>
        public string? pick_date { get; set; }
        public string? note { get; set; }
        /// <summary>
        /// nhận một trong hai giá trị gram và kilogram, 
        /// mặc định là kilogram, 
        /// đơn vị khối lượng của các sản phẩm có trong gói hàng
        /// </summary>
        public string? weight_option { get; set; }
        /// <summary>
        /// Tổng khối lượng của đơn hàng, 
        /// mặc định sẽ tính theo products.weight nếu không truyền giá trị này
        /// </summary>
        public double? total_weight { get; set; }
        public int value { get; set; }
        public string? transport { get; set; }
        /// <summary>
        /// Nhận một trong hai giá trị cod và post, 
        /// mặc định là cod, 
        /// biểu thị lấy hàng bởi COD hoặc Shop sẽ gửi tại bưu cục
        /// </summary>
        public string? pick_option { get; set; }
        public string? deliver_option { get; set; }
        public int? pick_session { get; set; }
        public List<int>? tags { get; set; }

    }

    public class ProductGHTKDTO
    {
        public string name { get; set; }
        public int? price { get; set; }
        public double weight { get; set; }
        public int? quantity { get; set; }
        public int? product_code { get; set; }
    }
}
