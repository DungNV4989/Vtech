﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTECHERP.Enums.CarrierShipping;

namespace VTECHERP.Entities
{
    public class CarrierShippingInformation : BaseEntity<Guid>
    {
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
        /// <summary>
        /// Hình thức gửi hàng
        /// </summary>
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
        /// <summary>
        /// Hình thức nhận hàng
        /// </summary>
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
        public bool? IsCOD { get; set; }
        public decimal? CODValue { get; set; }
        public string? Note { get; set; }
        public bool ReceiverPayingShippingFee { get; set; } = true;
    }
}
