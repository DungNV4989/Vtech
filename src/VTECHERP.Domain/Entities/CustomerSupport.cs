using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VTECHERP.Entities
{
    [Table("CustomerSupports")]
    public class CustomerSupport: BaseEntity<Guid>
    {
        /// <summary>
        /// Id khách hàng
        /// </summary>
        public Guid CustomerId { get; set; }
        /// <summary>
        /// Id nhân viên hỗ trợ
        /// </summary>
        public Guid SupportEmployeeId { get; set; }
        /// <summary>
        /// Cửa hàng hỗ trợ
        /// </summary>
        public Guid SupportStoreId { get; set; }
        /// <summary>
        /// Ngày hỗ trợ
        /// </summary>
        public DateTime SupportDate { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
    }
}
