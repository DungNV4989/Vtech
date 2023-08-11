using System;
using VTECHERP.Domain.Shared.Helper.Excel.Attributes;

namespace VTECHERP.DTOs.Entries
{
    public class ExportEntryResponse
    {
        [Header("ID")]
        public string Id { get; set; }

        [Header("Ngày")]
        public string TransactionDate { get; set; }

        [Header("Doanh nghiệp")]
        public string Enterprise { get; set; }

        [Header("Cửa hàng")]
        public string StoreName { get; set; }

        [Header("Loại phiếu")]
        public string TicketName { get; set; }

        [Header("Mã đối tượng")]
        public string AudienceCode { get; set; }

        [Header("Tên đối tượng")]
        public string AudienceName { get; set; }

        [Header("Chứng từ")]
        public string Document { get; set; }

        [Header("ID chứng từ")]
        public string DocumentCode { get; set; }

        [Header("Số tiền")]
        public string Money { get; set; }

        [Header("Nợ")]
        public string Debt { get; set; }

        [Header("Có")]
        public string Credit { get; set; }

        [Header("Ghi chú")]
        public string Note { get; set; }

        [Header("Người tạo")]
        public string Creator { get; set; }

        [Header("Ngày Tạo")]
        public string CreateTime { get; set; }
    }
}