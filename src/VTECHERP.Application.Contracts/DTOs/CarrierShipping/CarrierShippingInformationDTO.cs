using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.Enums.CarrierShipping;

namespace VTECHERP.DTOs.CarrierShipping
{
    public class CarrierShippingInformationDTO
    {
        public Guid? Id { get; set; }
        public Guid TransportInformationId { get; set; }
        public Guid StoreId { get; set; }
        #region Sender
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string SenderProvince { get; set; }
        public string SenderProvinceName { get; set; }
        public string SenderDistrict { get; set; }
        public string SenderDistrictName { get; set; }
        public string SenderWard { get; set; }
        public string SenderWardName { get; set; }
        public string SenderAddress { get; set; }
        public string SenderPostOffice { get; set; }
        public DateTime? PickDate { get; set; }
        public FormSendReceive FormSend { get; set; }
        #endregion
        #region Receiver
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string ReceiverEmail { get; set; }
        public string ReceiverProvince { get; set; }
        public string ReceiverProvinceName { get; set; }
        public string ReceiverDistrict { get; set; }
        public string ReceiverDistrictName { get; set; }
        public string ReceiverWard { get; set; }
        public string ReceiverWardName { get; set; }
        public string ReceiverAddress { get; set; }
        public string ReceiverPostOffice { get; set; }
        public FormSendReceive FormReceive { get; set; }
        #endregion

        #region Product
        public string VTShippingCode { get; set; }
        public string? CarrierShippingCode { get; set; }
        public string ShippingProductName { get; set; }
        public int? Quantity { get; set; }
        public decimal? Weight { get; set; }
        public decimal? ProductValue { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        #endregion
        public string OrderService { get; set; }
        public bool? IsCOD { get; set; }
        public decimal? CODValue { get; set; }
        public string? Note { get; set; }
        public string SaleOrderCodeVNP { get; set; }
        public bool ReceiverPayingShippingFee { get; set; } = true;
    }
}
