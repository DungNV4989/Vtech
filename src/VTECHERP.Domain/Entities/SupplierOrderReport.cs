using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VTECHERP.Entities
{
    /* 
     * Phục vụ report dữ liệu đơn hàng nhà cung cấp
     * Khi phát sinh đơn hàng -> tổng hợp thông tin Nợ - Thanh toán - Di chuyển - Đã về tại ngày phát sinh đơn hàng
     *** Đầu kỳ
     * Dư nợ đầu kỳ = (Dư nợ cuối kỳ trước - Thanh toán giữa 2 kỳ (số thực tế -> dương nếu còn nợ, âm nếu thanh toán thừa)
     * Nợ đầu kỳ hiển thị = Dư nợ đầu kỳ (set = 0 nếu < 0)
     * Thanh toán đầu kỳ = tổng thanh toán phát sinh giữa 2 kỳ (0 nếu không phát sinh thanh toán)
     * Di chuyển đầu kỳ = Di chuyển cuối kỳ trước - tổng xác nhận đơn đặt hàng phát sinh giữa 2 kỳ (set = 0 nếu < 0)
     * Đã về đầu kỳ = Đã về cuối kỳ trước + tổng xác nhận đơn đặt hàng phát sinh giữa 2 kỳ
     *** Trong kỳ
     * Nợ trong kỳ = tổng tiền đặt hàng
     * Thanh toán trong kỳ = tổng phiếu chi/báo nợ trong kỳ
     * Di chuyển trong kỳ = Nợ trong kỳ
     * Đã về trong kỳ = tổng xác nhận đơn đặt hàng trong kỳ
     *** Cuối kỳ
     * Dư nợ cuối kỳ = Dư nợ đầu kỳ + Nợ trong kỳ - Thanh toán trong kỳ (số thực tế -> dương nếu còn nợ, âm nếu thanh toán thừa)
     * Nợ cuối kỳ hiển thị = Dư nợ đầu kỳ (set = 0 nếu < 0)
     * Di chuyển cuối kỳ = Di chuyển đầu kỳ + Di chuyển trong kỳ - Đã về trong kỳ (set = 0 nếu < 0)
     * Đã về cuối kỳ = Đã về đầu kỳ + Đã về trong kỳ
    */
    [Table("SupplierOrderReports")]
    public class SupplierOrderReport: BaseEntity<Guid>
    {
        /// <summary>
        /// ID NCC
        /// </summary>
        public Guid SupplierId { get; set; }
        /// <summary>
        /// Ngày tổng hợp thông tin
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Dư nợ đầu kỳ
        /// </summary>
        public decimal DebtToDateCNY { get; set; }
        /// <summary>
        /// Dư nợ cuối kỳ
        /// </summary>
        public decimal DebtFinalCNY { get; set; }
        /// <summary>
        /// Nợ trong kỳ
        /// </summary>
        public decimal DebtInDateCNY { get; set; }
        /// <summary>
        /// Đã trả phát sinh giữa 2 kỳ
        /// </summary>
        public decimal PaidSinceLastReportCNY { get; set; }
        /// <summary>
        /// Đã trả trong kỳ
        /// </summary>
        public decimal PaidInDateCNY { get; set; }
        /// <summary>
        /// Đang trên đường đến kỳ
        /// </summary>
        public decimal DeliveryToDateCNY { get; set; }
        /// <summary>
        /// Đang trên đường phát sinh
        /// </summary>
        public decimal DeliveryInDateCNY { get; set; }
        /// <summary>
        /// Đang trên đường cuối kỳ
        /// </summary>
        public decimal DeliveryFinalCNY { get; set; }
        /// <summary>
        /// Đã về phát sinh giữa 2 kỳ
        /// </summary>
        public decimal ConfirmedSinceLastReportCNY { get; set; }
        /// <summary>
        /// Đã về đến kỳ
        /// </summary>
        public decimal ConfirmedToDateCNY { get; set; }
        /// <summary>
        /// Đã về trong kỳ
        /// </summary>
        public decimal ConfirmedInDateCNY { get; set; }
        /// <summary>
        /// Đã về cuối kỳ
        /// </summary>
        public decimal ConfirmedFinalCNY { get; set; }
        //public decimal DebtBeforeVND { get; set; }
        //public decimal DebtVND { get; set; }
        //public decimal PaidBeforeVND { get; set; }
        //public decimal PaidVND { get; set; }
        //public decimal PaidAccumulatedVND { get; set; }
        //public decimal PaidFinalVND { get; set; }
        //public decimal DebtFinalVND { get; set; }
        //public decimal InDeliveryBeforeVND { get; set; }
        //public decimal DeliveryVND { get; set; }
        //public decimal InDeliveryFinalVND { get; set; }
        //public decimal ConfirmedBeforeVND { get; set; }
        //public decimal ConfirmedVND { get; set; }
        //public decimal ConfirmedFinalVND { get; set; }
    }
}
