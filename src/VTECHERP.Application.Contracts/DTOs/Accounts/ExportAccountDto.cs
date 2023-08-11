using VTECHERP.Domain.Shared.Helper.Excel.Attributes;

namespace VTECHERP.DTOs.Accounts
{
    public class ExportAccountDto
    {
        [Header("Code")]
        public string Code { get; set; }
        [Header("Tên")]
        public string Name { get; set; }
        [Header("Cửa hàng")]
        public string? StoreName { get; set; }
        [Header("Tình trạng")]
        public string IsActiveName { get; set; }
        [Header("Người tạo")]
        public string CreatorName { get; set; }
        [Header("Ngày tạo")]
        public string CreationTime { get; set; }
    }
}
