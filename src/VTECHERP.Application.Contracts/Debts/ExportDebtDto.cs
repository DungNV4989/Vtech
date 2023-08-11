using VTECHERP.Domain.Shared.Helper.Excel.Attributes;
using VTECHERP.Enums;

namespace VTECHERP.Debts
{
    public class ExportDebtDto
    {
        [Header("Mã đối tượng")]
        public string? SupplierCode { get; set; }
        [Header("Nhà cung cấp")]
        public string? SupplierName { get; set; }
        [Header("Số điện thoại")]
        public string? PhoneNumber { get; set; }
       
        [Header("Nợ[phải thu] đầu kì")]
        public string BeginDebt { get; set; }
        [Header("Có[phải trả] đầu kì")]
        public string BeginCredit { get; set; }
        
        [Header("Nợ trong kì")]
        public string Debt { get; set; }
        [Header("Có trong kì")]
        public string Credit { get; set; }
        
        [Header("Nợ[phải thu] Cuối kì")]
        public string EndDebt { get; set; }
        [Header("Có[phải thu] cuối kì")]
        public string EndCredit { get; set; }
        //public SupplierOrigin? SupplierType { get; set; }
    }
}
