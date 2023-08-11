using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.CarrierShipping
{
    #region VTPDTO
    public class VTPResTokenDTO
    {
        public int status { get; set; }
        public bool error { get; set; }
        public string? message { get; set; }
        public VTPResponseTokenDTO? data { get; set; }
    }
    public class VTPResOrderDTO
    {
        public int status { get; set; }
        public bool error { get; set; }
        public string? message { get; set; }
        public VTPResponseOrderDTO? data { get; set; }
    }
    public class VTPResponseOrderDTO
    {
        public string ORDER_NUMBER { get; set; }
        public int MONEY_COLLECTION { get; set; }
        public int EXCHANGE_WEIGHT { get; set; }
        public int MONEY_TOTAL { get; set; }
        public int MONEY_TOTAL_FEE { get; set; }
        public int MONEY_FEE { get; set; }
        public int MONEY_COLLECTION_FEE { get; set; }
        public int MONEY_OTHER_FEE { get; set; }
        public object? MONEY_VAS { get; set; }
        public int MONEY_VAT { get; set; }
        public double KPI_HT { get; set; }
    }
    public class VTPResGetAllPriceDTO
    {
        public string MA_DV_CHINH { get; set; }
        public string TEN_DICHVU { get; set; }
        public long GIA_CUOC { get; set; }
        public string THOI_GIAN { get; set; }
        public long EXCHANGE_WEIGHT { get; set; }
        public List<VTPExtraServiceDTO> EXTRA_SERVICE { get; set; }
    }
    public class VTPResGetPriceDTO
    {
        public int status { get; set; }
        public bool error { get; set; }
        public string? message { get; set; }
        public VTPResonGetPriceDTO? data { get; set; }
    }
    public class VTPResonGetPriceDTO
    {
        public long MONEY_TOTAL_OLD { get; set; }
        public long MONEY_TOTAL { get; set; }
        public long MONEY_TOTAL_FEE { get; set; }
        public long MONEY_FEE { get; set; }
        public long MONEY_COLLECTION_FEE { get; set; }
        public long MONEY_OTHER_FEE { get; set; }
        public long MONEY_VAS { get; set; }
        public long MONEY_VAT { get; set; }
        public double KPI_HT { get; set; }
    }
    public class VTPExtraServiceDTO
    {
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public string DESCRIPTION { get; set; }
    }
    public class VTPResponseTokenDTO
    {
        public int userId { get; set; }
        public string token { get; set; }
        public int partner { get; set; }
        public string phone { get; set; }
        public float expired { get; set; }
        public int source { get; set; }
    }
    #endregion
    #region VNPostDTO
    public class VNPResponseTokenDTO
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string ErrorMessage { get; set; }
    }
    //public class VNPResponsePriceDTO
    //{
    //    //public int Status { get; set; }
    //    public List<VNPResponsePrice>? Data { get; set; }
    //}
    public class VNPResponsePriceDTO
    {
        public string ServiceCode { get; set; }
        public decimal? WeightConvert { get; set; }
        public decimal? PriceWeight { get; set; }
        public decimal? MainFee { get; set; }
        public decimal? TotalFee { get; set; }
        public List<AddonServiceResponse>? AddonService { get; set; }
        public List<AdditionRequestResponse>? AdditionRequest { get; set; }
        public decimal? Vasfee { get; set; }
    }
    public class AddonServiceResponse
    {
        public string Code { get; set; }
        public decimal? Fee { get; set; }
    }
    public class AdditionRequestResponse
    {
        public string Code { get; set; }
        public decimal? Fee { get; set; }
    }
    public class VNPResponeOrderDTO
    {
        public string orderHdrID { get; set; }
        public string originalID { get; set; }
        public string itemCode { get; set; }
        public string originalItemCode { get; set; }
        public int status { get; set; }
        public string saleOrderCode { get; set; }
        public string senderCode { get; set; }
        public string senderContractNumber { get; set; }
        public string senderName { get; set; }
        public string senderPhone { get; set; }
        public string senderEmail { get; set; }
        public string senderAddress { get; set; }
        public string senderProvinceCode { get; set; }
        public string senderProvinceName { get; set; }
        public string senderDistrictCode { get; set; }
        public string senderDistrictName { get; set; }
        public string senderCommuneCode { get; set; }
        public string senderCommuneName { get; set; }
        public string senderPostcode { get; set; }
        public string receiverCode { get; set; }
        public string receiverName { get; set; }
        public string receiverContractNumber { get; set; }
        public string receiverAddress { get; set; }
        public string receiverProvinceCode { get; set; }
        public string receiverProvinceName { get; set; }
        public string receiverDistrictCode { get; set; }
        public string receiverDistrictName { get; set; }
        public string receiverCommCode { get; set; }
        public string receiverCommName { get; set; }
        public string receiverPostcode { get; set; }
        public string receiverPhone { get; set; }
        public string receiverEmail { get; set; }
        public string serviceCode { get; set; }
        public decimal? totalFee { get; set; }
        public decimal? mainFee { get; set; }
        public decimal? mainFeeBeforeTax { get; set; }
        public decimal? mainTax { get; set; }
        public decimal? vasFee { get; set; }
        public decimal? codAmount { get; set; }
        public decimal? weight { get; set; }
        public decimal? length { get; set; }
        public decimal? width { get; set; }
        public decimal? height { get; set; }
        public decimal? dimWeight { get; set; }
        public decimal? priceWeight { get; set; }
        public string deliveryInstruction { get; set; }
        public int quantity { get; set; }
        public string contentNote { get; set; }
        public List<ContentDetail> contentDetail { get; set; }
        public string sendType { get; set; }
        public string isBroken { get; set; }
        public string deliveryTime { get; set; }
        public string deliveryRequire { get; set; }
        public string vehicle { get; set; }
        public string awbNumber { get; set; }
        public string voucher { get; set; }
        public string orgCodeCollect { get; set; }
        public string orgCodeAccept { get; set; }
        public string createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public string updatedBy { get; set; }
        public DateTime? updatedDate { get; set; }
        public DateTime? acceptedDate { get; set; }
        public string sortingCode { get; set; }
        public string paymentStatus { get; set; }
        public int? caseID { get; set; }
        public int? caseTypeName { get; set; }
        public int? caseStatus { get; set; }
        public string source { get; set; }
        public string inputType { get; set; }
        public string inputMethod { get; set; }
        public List<Documents> documents { get; set; }
        public List<PackageInfo> packageInfo { get; set; }
        public List<AddonServiceOrder> addonService { get; set; }
        public List<AdditionRequestOrder> additionRequest { get; set; }
        public string timestamp { get; set; }
        public string error { get; set; }
        public string message { get; set; }
        public string description { get; set; }
        public string path { get; set; }
    }
    public class ContentDetail
    {
        public string nameVi { get; set; }
        public string nameEn { get; set; }
        public string hsCode { get; set; }
        public int? quantity { get; set; }
        public decimal? weight { get; set; }
        public decimal? netWeight { get; set; }
        public string originProduct { get; set; }
        public decimal? amountUSD { get; set; }
    }
    public class Documents
    {
        public string docType { get; set; }
        public string docNumber { get; set; }
    }
    public class PackageInfo
    {
        public string packageNo { get; set; }
        public decimal? weight { get; set; }
        public decimal? length { get; set; }
        public decimal? width { get; set; }
        public decimal? heigth { get; set; }
        public string isUsePalet { get; set; }

    }
    public class AddonServiceOrder
    {
        public string code { get; set; }
        public decimal? fee { get; set; }
        public decimal? feeBeforeTax { get; set; }
        public string tax { get; set; }
        public string taxRate { get; set; }
        public List<Proplist> proplist { get; set; }
    }
    public class AdditionRequestOrder
    {
        public string code { get; set; }
        public string fee { get; set; }
        public string feeBefore { get; set; }
        public string tax { get; set; }
        public string taxRate { get; set; }
        public List<Proplist> proplist { get; set; }
    }
    public class Proplist
    {
        public string propCode { get; set; }
        public string propValue { get; set; }
        public string propValueActual { get; set; }
    }
    #endregion
}
