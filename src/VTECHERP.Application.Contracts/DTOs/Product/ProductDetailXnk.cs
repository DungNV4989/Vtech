using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.Product
{
    public class ProductDetailXnk : BaseDTO
    {
        /// <summary>
        /// Id int của sản phẩm trong phiếu xnk
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Id cửa hàng/kho
        /// </summary>
        public Guid? StoreId { get; set; }

        /// <summary>
        /// Tên kho hàng / Tên của hàng
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// Tên sản phẩm cha
        /// </summary>
        public string ProductParentName { get; set; }
        /// <summary>
        /// Mã sản phẩm cha
        /// </summary>
        public string ProductParentCode { get; set; }

        public Guid? ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        /// <summary>
        /// Số lượng nhập/xuất
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// Giá
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// Giá vốn
        /// </summary>
        public decimal? CostPrice { get; set; }
        /// <summary>
        /// Tồn
        /// </summary>
        public int? Inventory { get; set; }
        /// <summary>
        /// Đơn vị
        /// </summary>
        public string Unit { get; set; }
        public WarehousingBillType? BillType { get; set; }
        /// <summary>
        /// Tiền
        /// </summary>
        public decimal? Money { get; set; }
        /// <summary>
        /// Tổng tiền
        /// </summary>
        public decimal? TotalMoney { get; set; }
        /// <summary>
        /// Chiết khấu
        /// </summary>
        public decimal? DiscountAmount { get; set; }
        public string Note { get; set; }
        /// <summary>
        /// Kiểu chứng từ chi tiết
        /// </summary>
        public DocumentDetailType DocumentDetailType { get; set; }
        public string DocumentDetailTypeName { get => Datas.MasterDatas.DocumentDetailTypes.First(x => x.Id == DocumentDetailType).Name; }
    }
}
