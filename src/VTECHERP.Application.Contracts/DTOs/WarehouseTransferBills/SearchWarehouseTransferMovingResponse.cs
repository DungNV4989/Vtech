using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Domain.Shared.Helper.Excel.Attributes;
using VTECHERP.Enums;
using VTECHERP.Enums.WarehouseTransferBill;

namespace VTECHERP.DTOs.WarehouseTransferBills
{
    public class SearchWarehouseTransferMovingResponse
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

        /// <summary>
        /// Người tạo
        /// </summary>
        public string CreatorName { get; set; }

        public string Note { get; set; }

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

    public class ExportWarehouseTransferMovingResponse
    {
        [Header("ID")]
        public string Code { get; set; }
        [Header("Ngày")]
        public string CreatedTime { get; set; }
        [Header("Cửa hàng chuyển")]
        public string SourceStoreName { get; set; }
        [Header("Cửa hàng nhận")]
        public string DestinationStoreName { get; set; }
        [Header("Kiểu")]
        public string BillType { get; set; }
        [Header("SP")]
        public int? Sp { get; set; }
        [Header("SL")]
        public int? Quantity { get; set; }
        [Header("Người tạo")]
        public string CreatorName { get; set; }
        [Header("Người duyệt")]
        public string DraftApprovedUserName { get; set; }
        [Header("Ngày giờ duyệt")]
        public string DraftApprovedTime { get; set; }
        [Header("Ghi chú")]
        public string Note { get; set; }
    }
}
