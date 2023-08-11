using System;
using System.ComponentModel.DataAnnotations.Schema;
using VTECHERP.Enums.WarehouseTransferBill;

namespace VTECHERP.Entities
{
    [Table("WarehouseTransferBills")]
    /// <summary>
    /// Phiếu Chuyển kho
    /// </summary>
    public class WarehouseTransferBill: BaseEntity<Guid>
    {
        /// <summary>
        /// Mã phiếu chuyển kho: tự sinh format 10 số
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
        /// Trạng thái chuyển kho: Chuyển kho/Đang vận chuyển/Đã vận chuyển
        /// </summary>
        public TransferStatuses TransferStatus { get; set; }
        /// <summary>
        /// Check phiếu chuyển nguồn gốc từ 
        /// </summary>
        public WarehouseTransferBillCreatedFrom CreatedFrom { get; set; }

        /// <summary>
        /// Id của phiếu nháp trong trường hợp tạo từ phiếu nháp
        /// </summary>
        public Guid? DraftTicketId { get; set; }

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

    }
}
