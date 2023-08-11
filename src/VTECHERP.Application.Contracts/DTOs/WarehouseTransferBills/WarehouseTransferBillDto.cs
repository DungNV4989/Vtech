using System;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;
using VTECHERP.Enums.WarehouseTransferBill;

namespace VTECHERP.DTOs.WarehouseTransferBills
{
    public class WarehouseTransferBillDto : BaseDTO
    {
        /// <summary>
        /// Mã phiếu chuyển kho: tự sinh format 10 số
        /// </summary>
        public string Code { get; set; }

        public Guid? WarehousingBillId { get; set; }

        /// <summary>
        /// Loại phiếu nhập/xuất kho
        /// </summary>
        public WarehousingBillType BillType { get; set; }

        /// <summary>
        /// Mã phiếu chuyển kho: tự sinh format 10 số
        /// </summary>
        public string WarehousingBillCode { get; set; }

        /// <summary>
        /// Check phiếu chuyển nguồn gốc từ phiếu nháp
        /// </summary>
        public bool IsFromDraft { get; set; } = false;

        /// <summary>
        /// Id cửa hàng/kho nguồn
        /// </summary>
        public Guid SourceStoreId { get; set; }

        /// <summary>
        /// Tên cửa hàng/kho nguồn
        /// </summary>
        public string SourceStoreName { get; set; }

        /// <summary>
        /// Id cửa hàng/kho đích
        /// </summary>
        public Guid DestinationStoreId { get; set; }

        /// <summary>
        /// Tên cửa hàng/kho đích
        /// </summary>
        public string DestinationStoreName { get; set; }

        /// <summary>
        /// Tổng số mã sản phẩm
        /// </summary>
        public int TotalProductCode { get; set; }

        /// <summary>
        /// Tổng số lượng sản phẩm chuyển 
        /// </summary>
        public int TotalNumberProduct { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Trạng thái chuyển kho: Nháp/Đang vận chuyển/Đã vận chuyển
        /// </summary>
        public TransferStatuses TransferStatus { get; set; }

        /// <summary>
        /// Người phê duyệt phiếu nháp
        /// </summary>
        public Guid? DraftApprovedUserId { get; set; }

        /// <summary>
        /// Tên người phê duyệt phiếu nháp
        /// </summary>
        public string DraftApprovedName { get; set; }

        /// <summary>
        /// Ngày phê duyệt phiếu nháp
        /// </summary>
        public DateTime? DraftApprovedDate { get; set; }

        /// <summary>
        /// Người xác nhận phiếu chuyển
        /// </summary>
        public Guid? DeliveryConfirmedUserId { get; set; }

        /// <summary>
        /// Tên người xác nhận phiếu chuyển
        /// </summary>
        public string DeliveryConfirmedName { get; set; }

        /// <summary>
        /// Ngày xác nhận phiếu chuyển
        /// </summary>
        public DateTime? DeliveryConfirmedDate { get; set; }

    }
}