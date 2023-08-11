using System;
using VTECHERP.Domain.Shared.Helper.Excel.Attributes;
using VTECHERP.Enums.WarehouseTransferBill;

namespace VTECHERP.DTOs.WarehouseTransferBills
{
    public class SearchWarehouseTransferComingResponse
    {
        public Guid Id { get; set; }
        /// <summary>
        /// ID dùng để show
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Ngày tạo phiếu
        /// </summary>
        public DateTime? CreatedTime { get; set; }

        /// <summary>
        /// Tên kho xuất/ Cửa hàng xuất
        /// </summary>
        public string SourceStoreName { get; set; }

        /// <summary>
        /// Tên kho của nhàng nhập
        /// </summary>
        public string DestinationStoreName { get; set; }

        /// <summary>
        /// Tổng số mã sản phẩm chuyển kho
        /// </summary>
        public int? Sp { get; set; }
        /// <summary>
        /// Tổng số lượng sản phẩm chuyển kho
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// Tổng tiền
        /// </summary>
        public decimal? TotalMoney { get; set; }

        /// <summary>
        /// Kiểu chuyển kho
        /// </summary>
        public TransferBillType? TransferBillType { get; set; }

        /// <summary>
        /// Là phiếu nháp và đã duyệt = true
        /// </summary>
        public bool? IsDraftApproved { get; set; }
        /// <summary>
        /// Người phê duyệt phiếu nháp
        /// </summary>
        public string DraftApprovedUserName { get; set; }
        /// <summary>
        /// Ngày phê duyệt phiếu nháp
        /// </summary>
        public DateTime? DraftApprovedDate { get; set; }
        public string DeliveryConfirmedUserName { get; set; }
        public DateTime? DeliveryConfirmedDate { get; set; }
        /// <summary>
        /// Người tạo
        /// </summary>
        public string CreatorName { get; set; }

        public string Note { get; set; }
    }

    public class ExportWarehouseTransferComingResponse
    {
        [Header("ID")]
        public string Code { get; set; }
        [Header("Ngày tạo phiếu")]
        public string CreatedTime { get; set; }
        [Header("Cửa hàng chuyển")]
        public string SourceStoreInfo{ get; set; }
        [Header("Cửa hàng nhận")]
        public string DestinationStoreInfo { get; set; }
        [Header("Kiểu phiếu")]
        public string BillType { get; set; }
        [Header("SP")]
        public int? SP { get; set; }
        [Header("SL")]
        public int? Quantity { get; set; }
        [Header("Người tạo")]
        public string CreatorName { get; set; }
        [Header("Người duyệt")]
        public string DraftApprovedUserName { get; set; }
        [Header("Ngày giờ duyệt")]
        public string DraftApprovedTime { get; set; }
        [Header("Người xác nhận")]
        public string DeliveryConfirmedUserName { get; set; }
        [Header("Ngày giờ xác nhận")]
        public string DeliveryConfirmedTime { get; set; }
        [Header("Ghi chú")]
        public string Note { get; set; }
    }

    public class ExportWarehouseTransferResponse
    {
        [Header("ID")]
        public string Code { get; set; }
        [Header("Ngày tạo")]
        public string CreatedTime { get; set; }
        
        [Header("Cửa hàng chuyển")]
        public string ExportStoreName { get; set; }
        [Header("Cửa hàng nhận")]
        public string InputStoreName { get; set; }
        [Header("Kiểu phiếu")]
        public string BillType { get; set; }
        [Header("SP")]
        public int? SP { get; set; }
        [Header("SL")]
        public int? Quantity { get; set; }
        [Header("Tổng tiền")]
        public decimal? TotalMoney { get; set; }
        [Header("Chiết khấu")]
        public decimal Discount { get; set; }
        [Header("Người tạo")]
        public string CreatorName { get; set; }
        [Header("Ghi chú")]
        public string Note { get; set; }
    }

    public class ExportDraftTicketResponse 
    {
        [Header("ID")]
        public string Code { get; set; }
        [Header("Ngày tạo")]
        public string CreatedTime { get; set; }

        [Header("Cửa hàng chuyển")]
        public string ExportStoreName { get; set; }
        [Header("Cửa hàng nhận")]
        public string InputStoreName { get; set; }
        [Header("Kiểu phiếu")]
        public string BillType { get; set; }
        [Header("SP")]
        public int? SP { get; set; }
        [Header("SL")]
        public int? Quantity { get; set; }
        [Header("Người tạo")]
        public string CreatorName { get; set; }
        [Header("Người duyệt")]
        public string DraftApprovedUserName { get; set; }
        [Header("Ngày giờ duyệt")]
        public string DraftApprovedDate { get; set; }
        [Header("Người xác nhận")]
        public string DeliveryConfirmedUserName { get; set; }
        [Header("Ngày giờ xác nhận")]
        public string DeliveryConfirmedDate { get; set; }
        [Header("Trạng thái")]
        public string TransferStatus { get; set; }
        [Header("Ghi chú")]
        public string Note { get; set; }
    }
}
