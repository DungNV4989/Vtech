using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.WarehousingBills;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.WarehousingBillLogs
{
    public class WarehousingBillLogsDTO
    {
        public Guid Id { get; set; }
        public Guid ActionId { get; set; }
        /// <summary>
        /// Mã phiếu XNK
        /// </summary>
        public Guid WarehousingBillId { get; set; }
        public string FromValue { get; set; }
        public string ToValue { get; set; }
        public EntityActions Action { get; set; }
        public string ActionName { get; set; }
        public DateTime? CreationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public string Creator { get; set; }
        public WarehousingBillDto WarehousingBillDto { get; set; }
    }
}
