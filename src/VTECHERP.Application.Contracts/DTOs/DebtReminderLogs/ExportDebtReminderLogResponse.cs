using VTECHERP.Domain.Shared.Helper.Excel.Attributes;

namespace VTECHERP.DTOs.DebtReminderLogs
{
    public class ExportDebtReminderLogResponse
    {
        [Header("ID")]
        public string Id { get; set; }

        [Header("NGƯỜI TẠO NHẮC NỢ")]
        public string CreateName { get; set; }

        [Header("NGÀY TẠO NHẮC NỢ")]
        public string CreateTime { get; set; }

        [Header("NGÀY HẸN TRẢ")]
        public string PayDate { get; set; }

        [Header("TÊN KHÁCH HÀNG")]
        public string CustomerName  { get; set; }

        [Header("SỐ ĐIỆN THOẠI")]
        public string CustomerPhone { get; set; }

        [Header("CỬA HÀNG PHỤ TRÁCH")]
        public string HandlerStoreNames { get; set; }

        [Header("NHÂN VIÊN PHỤ TRÁCH")]
        public string HandlerEmployeeName { get; set; }

        [Header("NỘI DUNG")]
        public string Content { get; set; }
    }
}