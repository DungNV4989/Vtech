using System;
using System.Collections.Generic;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.WarehousingBills
{
    public class SearchWarehousingBillRequest:BasePagingRequest
    {
        /// <summary>
        /// Cửa hàng
        /// </summary>
        public List<Guid>? StoreIds { get; set; }
        /// <summary>
        /// Id phiếu nhập/xuất kho
        /// </summary>
        public string? BillCode { get; set; }
        /// <summary>
        /// Loại phiếu nhập/xuất kho
        /// </summary>
        public WarehousingBillType? BillType { get; set; }
        /// <summary>
        /// Ngày tạo từ
        /// </summary>
        public DateTime? DateFrom { get; set; }
        /// <summary>
        /// Ngày tạo đến
        /// </summary>
        public DateTime? DateTo { get; set; }
        /// <summary>
        /// ID đơn hàng
        /// </summary>
        public string? OrderCode { get; set; }
        /// <summary>
        /// Loại đối tượng
        /// </summary>
        public AudienceTypes? AudienceType { get; set; }
        /// <summary>
        /// ID đối tượng (search chọn)
        /// </summary>
        public Guid? AudienceId { get; set; }
        /// <summary>
        /// Kiểu
        /// </summary>
        public List<DocumentDetailType>? DocumentDetailType { get; set; }
        /// <summary>
        /// Sản phẩm (tên/mã)
        /// </summary>
        public Guid? ProductId { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Note { get; set; }
        /// <summary>
        /// Id phiếu nhập
        /// </summary>
        public string? DraftTransferBillCode { get; set; }
        //TODO: Id phiếu kiểm kho, Id phiếu bảo hành
    }
}
