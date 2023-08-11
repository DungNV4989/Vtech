using VTECHERP.Domain.Shared.Helper.Excel.Attributes;

namespace VTECHERP.Debts
{
    public class ExportDebtDetailDto
    {
        [Header("Id")]
        public string? SupplierCode { get; set; }
        [Header("Nhà cung cấp")]
        public string? SupplierName { get; set; }
        [Header("Số điện thoại")]
        public string? PhoneNumber { get; set; }
        [Header("Có[phải trả] đầu kì")]
        public decimal BeginCredit { get; set; }
        [Header("Nợ[phải thu] đầu kì")]
        public decimal BeginDebt { get; set; }
        [Header("Có trong kì")]
        public decimal Credit { get; set; }
        [Header("Nợ trong kì")]
        public decimal Debt { get; set; }
        [Header("Có[phải thu] cuối kì")]
        public decimal EndCredit { get; set; }
        [Header("Nợ[phải thu] Cuối kì")]
        public decimal EndDebt { get; set; }
    }
}
