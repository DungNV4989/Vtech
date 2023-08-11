using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.ViettelPost
{
    public class TranportOrderViettelPostDTO
    {
        public string ORDER_NUMBER { get; set; }
        public int GROUPADDRESS_ID { get; set; }
        public int CUS_ID { get; set; }
        public string DELIVERY_DATE { get; set; }
        public string SENDER_FULLNAME { get; set; }
        public string SENDER_ADDRESS { get; set; }
        public string SENDER_PHONE { get; set; }
        public string SENDER_EMAIL { get; set; }
        public int SENDER_WARD { get; set; }
        public int SENDER_DISTRICT { get; set; }
        public int SENDER_PROVINCE { get; set; }
        public int SENDER_LATITUDE { get; set; }
        public int SENDER_LONGITUDE { get; set; }
        public string RECEIVER_FULLNAME { get; set; }
        public string RECEIVER_ADDRESS { get; set; }
        public string RECEIVER_PHONE { get; set; }
        public string RECEIVER_EMAIL { get; set; }
        public int RECEIVER_WARD { get; set; }
        public int RECEIVER_DISTRICT { get; set; }
        public int RECEIVER_PROVINCE { get; set; }
        public int RECEIVER_LATITUDE { get; set; }
        public int RECEIVER_LONGITUDE { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string PRODUCT_DESCRIPTION { get; set; }
        public int PRODUCT_QUANTITY { get; set; }
        public int PRODUCT_PRICE { get; set; }
        public int PRODUCT_WEIGHT { get; set; }
        public int PRODUCT_LENGTH { get; set; }
        public int PRODUCT_WIDTH { get; set; }
        public int PRODUCT_HEIGHT { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public int ORDER_PAYMENT { get; set; }
        public string ORDER_SERVICE { get; set; }
        public string ORDER_SERVICE_ADD { get; set; }
        public string ORDER_VOUCHER { get; set; }
        public string ORDER_NOTE { get; set; }
        public int MONEY_COLLECTION { get; set; }
        public int MONEY_TOTALFEE { get; set; }
        public int MONEY_FEECOD { get; set; }
        public int MONEY_FEEVAS { get; set; }
        public int MONEY_FEEINSURRANCE { get; set; }
        public int MONEY_FEE { get; set; }
        public int MONEY_FEEOTHER { get; set; }
        public int MONEY_TOTALVAT { get; set; }
        public int MONEY_TOTAL { get; set; }
        public List<LISTITEM> LIST_ITEM { get; set; }
    }
    public class LISTITEM
    {
        public string PRODUCT_NAME { get; set; }
        public int PRODUCT_PRICE { get; set; }
        public int PRODUCT_WEIGHT { get; set; }
        public int PRODUCT_QUANTITY { get; set; }
    }
}
