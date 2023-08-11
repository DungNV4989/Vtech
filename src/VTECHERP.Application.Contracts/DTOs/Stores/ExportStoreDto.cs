using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Domain.Shared.Helper.Excel.Attributes;

namespace VTECHERP.DTOs.Stores
{
    public class ExportStoreDto
    {
        [Header("Cửa hàng")]
        public string StoreName { get; set; }
        [Header("Doanh nghiệp")]
        public string EnterpriseName { get; set; }
        [Header("Mã tài khoản")]
        public string? AccountCode { get; set; }
        [Header("Tên tài khoản")]
        public string AccountName { get; set; }
        [Header("Số dư đầu kỳ")]
        public decimal amountBegin { get; set; }
        [Header("Tổng thu")]
        public decimal totalReceive { get; set; }
        [Header("Tổng chi")]
        public decimal? totalPay { get; set; }
        [Header("Số dư cuối kỳ")]
        public decimal? totalEnd { get; set; }

    }

    public class ExportStoreDetailDto
    {
        [Header("ID")]
        public string Code { get; set; }
        [Header("ID bút toán")]
        public string ParentCode { get; set; }
        [Header("Ngày")]
        public string? CreationTime { get; set; }
        [Header("Đối tượng")]
        public string AudienceName { get; set; }
        [Header("Tài khoản ghi nợ")]
        public string DebtAccountCode { get; set; }
        [Header("Tài khoản ghi có")]
        public string CreditAccountCode { get; set; }
        [Header("Số tiền nợ")]
        public decimal? DebtAmount { get; set; }
        [Header("Số tiền có")]
        public decimal? CreditAmount { get; set; }
        [Header("Chứng từ")]
        public string DocumentName { get; set; }
        [Header("Ghi chú")]
        public string Note { get; set; }

    }
}
