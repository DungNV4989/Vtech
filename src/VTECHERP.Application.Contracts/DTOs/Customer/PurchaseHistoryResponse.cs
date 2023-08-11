﻿using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Vml;
using System;
using System.Collections.Generic;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.BillCustomers.Respons;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.Customer
{
    public class PurchaseHistoryResponse
    {
        #region
        /// <summary>
        /// id hóa đơn bán hàng/Trả hàng
        /// </summary>
        public Guid BillId { get; set; }

        /// <summary>
        /// Mã hóa đơn bán hàng/Trả hàng
        /// </summary>
        public string BillCode { get; set; }

        /// <summary>
        /// Kiểu của hóa đơn (Bán hàng/Trả hàng)
        /// </summary>
        public BillLogType BillLogType { get; set; }

        /// <summary>
        /// Id cửa hàng
        /// </summary>
        public Guid StoreId { get; set; }

        /// <summary>
        /// Mã cửa hàng
        /// </summary>
        public string StoreCode { get; set; }

        /// <summary>
        /// Tên cửa hàng
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// Id khách hàng
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Số điện thoại khách hàng
        /// </summary>
        public string CustomerPhone { get; set; }

        /// <summary>
        /// Ngày tạo hóa đơn bán hàng/Trả hàng
        /// </summary>
        public DateTime? CreationTime { get; set; }
        #endregion

        public List<ProductPurchaseHistory> ProductPurchaseHistorys { get; set; }
    }

    public class ProductPurchaseHistory
    {

        /// <summary>
        /// Id sản phẩm
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Mã sản phẩm
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Tên sản phẩm
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Giá sản phẩm
        /// </summary>
        public decimal? Price { get; set; } = 0;

        /// <summary>
        /// Số lượng sản phẩm
        /// </summary>
        public int? Quantity { get; set; } = 0;

        /// <summary>
        /// Giá trị của chiết khẩu (VND/%)
        /// </summary>
        public decimal? DiscountValue { get; set; }

        /// <summary>
        /// Kiểu của chiết khấu
        /// </summary>
        public MoneyModificationType? DiscountUnit { get; set; }

        /// <summary>
        /// Tổng tiền trước chiết khấu
        /// </summary>
        public decimal PreDiscountTotal { get; set; } = 0;

        /// <summary>
        /// Tổng tiền sau chiết khấu
        /// </summary>
        public decimal AfterDiscountTotal { get; set; } = 0;
    }

    public class PagingPurchaseHistoryResponse : PagingResponse<PurchaseHistoryResponse>
    {
        public decimal? TotalMoney { get; set; }

        public PagingPurchaseHistoryResponse(int total, IEnumerable<PurchaseHistoryResponse> data, decimal? totalMoney)
        {
            Total = total;
            Data = data;
            TotalMoney = totalMoney;
        }
    }
}
