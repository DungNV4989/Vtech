using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums.Bills;

namespace VTECHERP.DTOs.Promotions
{
    public class DetailPromotionDTO
    {
        public Guid Id { get; set; }
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
        // Không áp dụng cùng chương trình chiết khấu?
        // Check = true
        public bool NotApplyWithDiscount { get; set; }
        // -- Voucher -- 
        // Tiền tố
        public string? Prefix { get; set; }
        // Hậu tố
        public string? Suffix { get; set; }
        // Số ký tự sinh
        public int GenCodeNum { get; set; }
        // Số lượng voucher
        public int VoucherNum { get; set; }
        // Kiểu giảm giá
        // % = 1,
        // Tiền mặt = 2
        public DiscountUnit DiscountUnit { get; set; }
        // Giá trị giảm giá
        public decimal? DiscountValue { get; set; }
        // Kiểu giảm giá tối đa
        // % = 1,
        // Tiền mặt = 2
        public DiscountUnit MaxDiscountUnit { get; set; }
        // Giá trị giảm tối đa
        public decimal? MaxDiscountValue { get; set; }
        // Ghi chú
        public string? Note { get; set; }
    }
}
