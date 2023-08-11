using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace VTECHERP.DTOs.IntegratedCasso
{
    public class IntegratedCassoDTO
    {
        public int id { get; set; }
        public string? when { get; set; }
        public long amount { get; set; }
        public string description { get; set; }
        public long cusum_balance { get; set; }
        public string tid { get; set; }
        public string subAccId { get; set; }
        public string bank_sub_acc_id { get; set; }
        public string virtualAccount { get; set; }
        public string virtualAccountName { get; set; }
        public string corresponsiveName { get; set; }
        public string corresponsiveAccount { get; set; }
        public string corresponsiveBankId { get; set; }
        public string corresponsiveBankName { get; set; }
    }
}
