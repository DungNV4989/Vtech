using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace VTECHERP.Entities
{
    [Table("DayConfigurations")]
    public class DayConfiguration : BaseEntity<Guid>, IMultiTenant
    {
        /// <summary>
        /// số ngày cho phép nhân viên sửa bút toán
        /// </summary>
        public int DayNumbers { get; set; }
        /// <summary>
        /// số ngày cho phép nhân viên xóa bút toán
        /// </summary>
        public int NumberOfDayAllowDeleteEntry { get; set; }
        /// <summary>
        /// số ngày quá khứ nhân viên được chọn khi lập phiếu thu chi
        /// </summary>
        public int NumberOfDayAllowCreatePayRecieve { get; set; }
    }
}
