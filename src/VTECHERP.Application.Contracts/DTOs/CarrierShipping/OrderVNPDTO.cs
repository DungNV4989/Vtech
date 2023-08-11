using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.CarrierShipping
{
    public class OrderVNPDTO
    {
        public bool OrderCreationStatus { get; set; }
        public string Type { get; set; }
        public string CustomerCode { get; set; }
        public string ContractCode { get; set; }
        public InformationOrderVNP InformationOrder { get; set; }
    }
    public class InformationOrderVNP
    {
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string SenderMail { get; set; }
        public string SenderAddress { get; set; }
        public string SenderProvinceCode { get; set; }
        public string SenderProvinceName { get; set; }
        public string SenderDistrictCode { get; set; }
        public string SenderDistrictName { get; set; }
        public string SenderCommuneCode { get; set; }
        public string SenderCommuneName { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverAddress { get; set; }
        public string ReceiverProvinceCode { get; set; }
        public string ReceiverProvinceName { get; set; }
        public string ReceiverDistrictCode { get; set; }
        public string ReceiverDistrictName { get; set; }
        public string ReceiverCommuneCode { get; set; }
        public string ReceiverCommuneName { get; set; }
        public string ReceiverPhone { get; set; }
        public string ReceiverEmail { get; set; }
        public string ServiceCode { get; set; }
        public string OrgCodeCollect { get; set; }
        public string OrgCodeAccept { get; set; }
        public string SaleOrderCode { get; set; }
        public string ContentNote { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public decimal? Height { get; set; }
        public string Vehicle { get; set; }
        public string SendType { get; set; }
        public string IsBroken { get; set; }
        public string DeliveryTime { get; set; }
        public string DeliveryRequire { get; set; }
        public string DeliveryInstruction { get; set; }
        public List<VNPAddonService> AddonService { get; set; }
        public List<VNPAdditionRequest> AdditionRequest { get; set; }
    } 
    public class PriceVNPRequestDTO
    {
        public string Scope { get; set; }
        public string CustomerCode { get; set; }
        public string ContractCode { get; set; }
        public PriceVNPData data { get; set; }

    }
    public class PriceVNPData
    {
        public string SenderProvinceCode { get; set; }
        public string SenderProvinceName { get; set; }
        public string SenderDistrictName { get; set; }
        public string SenderDistrictCode { get; set; }
        public string SenderCommuneCode { get; set; }
        public string SenderCommuneName { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverAddress { get; set; }
        public string ReceiverProvinceCode { get; set; }
        public string ReceiverProvinceName { get; set; }
        public string ReceiverDistrictCode { get; set; }
        public string ReceiverDistrictName { get; set; }
        public string ReceiverCommuneCode { get; set; }
        public string ReceiverCommuneName { get; set; }
        public string ReceiverNational { get; set; }
        public string ReceiverCity { get; set; }
        public string OrgCodeAccept { get; set; }
        public string ReceiverPostCode { get; set; }
        public decimal Weight { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public decimal Height { get; set; }
        public string ServiceCode { get; set; }
        public List<VNPAddonService> AddonService { get; set; }
        public List<VNPAdditionRequest> AdditionRequest { get; set; }
        public string Vehicle { get; set; }
    }
    public class VNPAddonService
    {
        public string Code { get; set; }
        public string PropValue { get; set; }
    }
    public class VNPAdditionRequest
    {
        public string Code { get; set; }
        public string PropValue { get; set; }
    }

}
