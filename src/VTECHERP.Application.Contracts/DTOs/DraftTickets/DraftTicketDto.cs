using System;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;
using VTECHERP.Enums.DraftTicket;
using VTECHERP.Enums.WarehouseTransferBill;

namespace VTECHERP.DTOs.DraftTickets
{
    public class DraftTicketDto : BaseDTO
    {
        /// <summary>
        /// Mã phiếu chuyển kho: tự sinh format 10 số
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Id phiếu xuất kho
        /// </summary>
        public Guid? WarehousingBillIdPXK { get; set; }

        /// <summary>
        /// Mã phiếu xuất kho: tự sinh format 10 số
        /// </summary>
        public string WarehousingBillCodePXK { get; set; }

        /// <summary>
        /// Id phiếu nhập kho
        /// </summary>
        public Guid? WarehousingBillIdPNK { get; set; }

        /// <summary>
        /// Mã phiếu Nhập kho: tự sinh format 10 số
        /// </summary>
        public string WarehousingBillCodePNK { get; set; }

        /// <summary>
        /// Id Phiếu chuyển kho
        /// </summary>
        public Guid? WarehouseTransferBillId { get; set; }

        /// <summary>
        /// Mã phiếu chuyển kho
        /// </summary>
        public string WarehouseTransferBillCode { get; set; }

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

        public Status Status { get; set; }

    }
}