using System;

namespace VTECHERP.DTOs.PaymentReceipt
{
    public class UpdatePaymentReceiptRequest: CreatePaymentReceiptRequest
    {
        public Guid Id { get; set; }
    }
}
