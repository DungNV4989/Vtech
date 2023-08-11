using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Attachment;
using VTECHERP.Enums;
using VTECHERP.Enums.WarehouseTransferBill;

namespace VTECHERP.DTOs.WarehouseTransferBills
{
    public class SearchWarehouseTransferResponse
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Mã Chuyển khod
        /// </summary>
        public string? WarehouseTransferCode { get; set; }

        /// <summary>
        /// Ngày Tạo
        /// </summary>
        public DateTime? WarehouseTransferCreatedTime { get; set; }

        /// <summary>
        /// Tên kho xuất
        /// </summary>
        public string ExportStoreName { get; set; }

        /// <summary>
        /// Tên kho nhập
        /// </summary>
        public string InputStoreName { get; set; }


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
        /// Chiết khấu
        /// </summary>
        public decimal Discount { get; set; } = 0;
        /// <summary>
        /// Kiểu chuyển kho
        /// </summary>
        public TransferBillType? TransferBillType { get; set; }

        /// <summary>
        /// Có file đính kèm hay không
        /// </summary>
        public bool? HasAttachment { get; set; }
        /// <summary>
        /// Người tạo
        /// </summary>
        public string CreatorName { get; set; }

        public string Note { get; set; }

        public List<WarehousingBill> WarehousingBills { get; set; }

        /// <summary>
        /// Link sản phẩm
        /// </summary>
        public List<DetailAttachmentDto> Attachments { get; set; }

        public class WarehousingBill
        {
            public Guid? WarehousingBillId { get; set; }
            /// <summary>
            /// Loại phiếu nhập/xuất kho
            /// </summary>
            public WarehousingBillType? BillType { get; set; }

            /// <summary>
            /// Mã phiếu chuyển kho: tự sinh format 10 số
            /// </summary>
            public string WarehousingBillCode { get; set; }
        }
    }


}
