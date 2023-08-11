using System;
using System.Collections.Generic;
using VTECHERP.Enums.Bills;

namespace VTECHERP.DTOs.Promotions
{
    public class UpdatePromotionRequest
    {
        // -- Promotion -- 
        public Guid Id { get; set; }
        // Tên Chương trình khuyến mãi
        public string Name { get; set; }
        // Cửa hàng áp dụng
        public List<Guid>? ApplyStoreIds { get; set; }
        // Áp dụng cho
        // Không chọn = null
        // Danh mục = 1,
        // Sản phẩm = 2
        public ApplyFor? ApplyFor { get; set; }
        // Danh mục sản phẩm áp dụng
        public List<Guid>? ApplyProductCategoryIds { get; set; }
        // Sản phẩm áp dụng
        public List<Guid>? ApplyProductIds { get; set; }
        // Từ ngày
        public DateTime FromDate { get; set; }
        // Đến ngày
        public DateTime ToDate { get; set; }
        // Giá trị hóa đơn tối thiểu
        public decimal? BillMinValue { get; set; }
        // Giá trị hóa đơn tối đa
        public decimal? BillMaxValue { get; set; }
        // Trạng thái
        //Chưa hoạt dộng = 0,
        //Hoạt động = 1,
        //Dừng hoạt động = 2,
        public PromotionStatus Status { get; set; }
        // -- Voucher -- 
        // Ghi chú
        public string? Note { get; set; }
    }
}
