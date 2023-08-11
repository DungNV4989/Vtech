using System;
using VTECHERP.Enums.DraftTicket;
using VTECHERP.Enums.WarehouseTransferBill;

namespace VTECHERP.Entities
{
    /// <summary>
    /// Phiếu nháp
    /// </summary>
    public class DraftTicket : BaseEntity<Guid>
    {
        /// <summary>
        /// Mã phiếu nháp: tự sinh format 10 số
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Id cửa hàng/kho nguồn
        /// </summary>
        public Guid SourceStoreId { get; set; }
        /// <summary>
        /// Id cửa hàng/kho đích
        /// </summary>
        public Guid DestinationStoreId { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// Kiểu :
        /// </summary>
        public TransferBillType? TransferBillType { get; set; }
        /// <summary>
        /// Người phê duyệt phiếu nháp
        /// </summary>
        public Guid? DraftApprovedUserId { get; set; }
        /// <summary>
        /// Ngày phê duyệt phiếu nháp
        /// </summary>
        public DateTime? DraftApprovedDate { get; set; }
        /// <summary>
        /// Người xác nhận phiếu chuyển
        /// </summary>
        public Guid? DeliveryConfirmedUserId { get; set; }
        /// <summary>
        /// Ngày xác nhận phiếu chuyển
        /// </summary>
        public DateTime? DeliveryConfirmedDate { get; set; }
        /// <summary>
        /// ngày chuyển/ngày nhận hàng chuyển kho. 
        /// </summary>
        public DateTime? TranferDate { get; set; }


        /// <summary>
        /// Trạng thái phiếu nhám
        /// </summary>
        public Status Status { get; set; }
    }
}