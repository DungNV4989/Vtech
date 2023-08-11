using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Volo.Abp.AutoMapper;
using VTECHERP.Debts;
using VTECHERP.DTOs.Accounts;
using VTECHERP.DTOs.Attachment;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.BO;
using VTECHERP.DTOs.BO.Tenants.Responses;
using VTECHERP.DTOs.Customer;
using VTECHERP.DTOs.DebtReminderLogs;
using VTECHERP.DTOs.DebtReports;
using VTECHERP.DTOs.DraftTicketProducts;
using VTECHERP.DTOs.DraftTickets;
using VTECHERP.DTOs.Entries;
using VTECHERP.DTOs.ExchangeReturn;
using VTECHERP.DTOs.PaymentReceipt;
using VTECHERP.DTOs.Permissions;
using VTECHERP.DTOs.PriceTableProduct.Param;
using VTECHERP.DTOs.Product;
using VTECHERP.DTOs.ProductCategories.Requests;
using VTECHERP.DTOs.ProductCategories.Responses;
using VTECHERP.DTOs.Promotions;
using VTECHERP.DTOs.SaleOrderLines;
using VTECHERP.DTOs.SaleOrders;
using VTECHERP.DTOs.StoreProducts;
using VTECHERP.DTOs.Stores;
using VTECHERP.DTOs.Suppliers;
using VTECHERP.DTOs.TransportInformation;
using VTECHERP.DTOs.WarehouseTransferBillProducts;
using VTECHERP.DTOs.WarehouseTransferBills;
using VTECHERP.DTOs.WarehousingBillLogs;
using VTECHERP.DTOs.WarehousingBills;
using VTECHERP.Entities;
using VTECHERP.Reports;

namespace VTECHERP;

public class VTECHERPApplicationAutoMapperProfile : Profile
{
    public VTECHERPApplicationAutoMapperProfile()
    {
        #region Entry

        CreateMap<Entry, EntryDTO>()
            .Ignore(source => source.Attachments);
        CreateMap<EntryAccount, EntryAccountDto>();

        #endregion Entry

        #region Customer

        CreateMap<CreateCustomerRequest, Customer>();
        CreateMap<UpdateCustomerRequest, Customer>().Ignore(source => source.Id);
        CreateMap<Customer, CustomerDTO>();
        CreateMap<Customer, CustomerResponse>()
            .ForMember(dto => dto.HandlerStoreIds, act => act.MapFrom(e => e.HandlerStoreId)); ;

        #endregion Customer

        #region Debt

        CreateMap<CreateOrUpdateDebtDto, Debt>();
        CreateMap<Debt, DebtDto>()
            .ForMember(desc => desc.Debt, src => src.MapFrom(src => src.Debts))
            .ForMember(desc => desc.Credit, src => src.MapFrom(src => src.Credits));
        CreateMap<Debt, DebtDetailDto>();
        CreateMap<DebtReport, DebtReportDto>();
        CreateMap<DebtDto, ExportDebtDto>();
        CreateMap<DebtDetailDto, ExportDebtDetailDto>();

        #endregion Debt

        #region Account

        CreateMap<Account, AccountDto>();
        CreateMap<AccountDto, ExportAccountDto>();
        CreateMap<CreateAccountRequest, Account>();

        #endregion Account

        #region SaleOrder

        CreateMap<SaleOrderCreateRequest, SaleOrders>();
        CreateMap<SaleOrderLineCreateRequest, SaleOrderLines>();
        CreateMap<SaleOrders, SaleOrderDetailDto>();
        CreateMap<SaleOrders, GetDetailConfirmByIdResponse>();

        #endregion SaleOrder

        #region SaleOrderLine

        CreateMap<SaleOrderLines, SaleOrderLineDto>();
        CreateMap<SaleOrderLines, SaleOrderLineDetailDto>();
        CreateMap<SaleOrderLineUpdateRequest, SaleOrderLineCreateRequest>();

        #endregion SaleOrderLine

        #region Supplier

        CreateMap<Suppliers, MasterDataDTO>();
        CreateMap<Suppliers, SupplierDetailDto>();

        #endregion Supplier

        #region Store

        CreateMap<Stores, MasterDataDTO>();
        CreateMap<Stores, StoreDetailDto>();
        CreateMap<Stores, StoreDto>();
        CreateMap<StoreDto, Stores>();

        #endregion Store

        #region Product

        CreateMap<Products, MasterDataDTO>();
        CreateMap<Products, ProductDetailDto>()
            .Ignore(source => source.Attachments);
        CreateMap<Products, ProductMasterDataDto>();

        #endregion Product

        #region WarehouseTransferBill

        CreateMap<WarehouseTransferBillCreateRequest, WarehouseTransferBill>();
        CreateMap<WarehouseTransferBill, WarehouseTransferBillDetaildDto>();
        CreateMap<WarehouseTransferBill, WarehouseTransferBillDto>();
        CreateMap<WarehouseTransferBill, WarehouseTransferBillApproveDto>();

        #endregion WarehouseTransferBill

        #region WarehouseTransferBillProduct

        CreateMap<WarehouseTransferBillProductCreateRequest, WarehouseTransferBillProduct>();
        CreateMap<WarehouseTransferBillProduct, WarehouseTransferBillProductDetailDto>();
        CreateMap<WarehouseTransferBillProduct, WarehouseTransferBillProductApproveDto>()
            .ForMember(dto => dto.RequestQuantity, act => act.MapFrom(e => e.Quantity));

        #endregion WarehouseTransferBillProduct

        #region StoreProdut

        CreateMap<StoreProduct, StoreProductDto>();

        #endregion StoreProdut

        #region WarehousingBill

        CreateMap<CreateWarehousingBillRequest, WarehousingBill>();
        CreateMap<WarehousingBillProductRequest, WarehousingBillProduct>();
        CreateMap<WarehousingBill, WarehousingBillDto>();
        CreateMap<WarehousingBillProduct, WarehousingBillProductDto>();

        #endregion WarehousingBill

        #region Attachment

        CreateMap<Attachment, AttachmentDetailDto>();
        CreateMap<AttachmentDetailDto, AttachmentShortDto>();

        #endregion Attachment

        #region WarehousingBillLogs

        CreateMap<WarehousingBillLogs, WarehousingBillLogsDTO>().ForMember(x => x.WarehousingBillDto, opt => opt.UseDestinationValue());

        #endregion WarehousingBillLogs

        #region CustomerReturn

        CreateMap<CreateCustomerReturnRequest, CustomerReturn>();
        CreateMap<CustomerReturn, CustomerReturnDTO>();
        CreateMap<CustomerReturnProductDTO, CustomerReturnProduct>();
        CreateMap<CustomerReturnProduct, CustomerReturnProductDTO>();

        #endregion CustomerReturn

        #region Payment Receipt

        CreateMap<CreatePaymentReceiptRequest, PaymentReceipt>();
        CreateMap<UpdatePaymentReceiptRequest, PaymentReceipt>();
        CreateMap<PaymentReceipt, PaymentReceiptDTO>();

        #endregion Payment Receipt

        #region DraftTicket

        CreateMap<DraftTicketCreateRequest, DraftTicket>();
        CreateMap<DraftTicketCreateRequest, DraftTicket>();
        CreateMap<DraftTicket, DraftTicketDetailDto>();
        CreateMap<DraftTicket, DraftTicketDto>();
        CreateMap<DraftTicket, DraftTicketApproveDto>();

        #endregion DraftTicket

        #region DraftTicketProduct

        CreateMap<DraftTicketProductCreateRequest, DraftTicketProduct>();
        CreateMap<DraftTicketProduct, DraftTicketProductDetailDto>();
        CreateMap<DraftTicketProduct, DraftTicketProductApproveDto>()
            .ForMember(dto => dto.RequestQuantity, act => act.MapFrom(e => e.Quantity));

        #endregion DraftTicketProduct

        #region TransportInformation

        CreateMap<CreateTransportInformationDto, TransportInformation>();
        CreateMap<TransportInformation, TransportInformationDTO>();

        #endregion TransportInformation

        #region DebtReminderLog

        CreateMap<DebtReminderLogCreateRequest, DebtReminderLog>();

        #endregion DebtReminderLog

        #region PriceTable

        CreateMap<CreatePriceTableRequest, PriceTable>();

        #endregion PriceTable

        #region Promotion

        CreateMap<CreatePromotionRequest, Promotion>()
            .ForMember(dest => dest.ApplyStoreIds, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.ApplyStoreIds)))
            .ForMember(dest => dest.ApplyProductCategoryIds, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.ApplyProductCategoryIds)))
            .ForMember(dest => dest.ApplyProductIds, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.ApplyProductIds)));

        CreateMap<Promotion, PromotionDTO>();
        CreateMap<Promotion, DetailPromotionDTO>()
            .ForMember(dest => dest.ApplyStoreIds, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<Guid>>(src.ApplyStoreIds)))
            .ForMember(dest => dest.ApplyProductCategoryIds, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<Guid>>(src.ApplyProductCategoryIds)))
            .ForMember(dest => dest.ApplyProductIds, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<Guid>>(src.ApplyProductIds)));

        CreateMap<Voucher, VoucherDTO>();

        #endregion Promotion

        #region Permission

        CreateMap<Permission, PermissionDTO>();
        CreateMap<PermissionModule, PermissionModuleDTO>().ForMember(x => x.Groups, opt => opt.UseDestinationValue()); ;
        CreateMap<PermissionGroup, PermissionGroupDTO>().ForMember(x => x.Permissions, opt => opt.UseDestinationValue()); ;

        #endregion Permission

        #region ProductCategory

        CreateMap<CreateProductCategoryRequest, ProductCategories>();
        CreateMap<ProductCategories, DetailProductCategoryResponse>();
        CreateMap<ProductCategories, SearchProductCategoryResponse>();

        #endregion ProductCategory

        #region Enterprise/Agency
        CreateMap<VTECHERP.DTOs.BO.Tenants.Requests.CreateTenantRequest, Enterprise>();
        CreateMap<VTECHERP.DTOs.BO.Tenants.Requests.CreateTenantRequest, Agency>();
        CreateMap<Enterprise, VTECHERP.DTOs.BO.Tenants.Responses.LinkEnterpriseResponse>();
        CreateMap<Agency, VTECHERP.DTOs.BO.Tenants.Responses.DetailTenant>();
        CreateMap<Enterprise, VTECHERP.DTOs.BO.Tenants.Responses.DetailTenant>();

        CreateMap<Entities.Stores,StoreByTenantResponse>();
        #endregion

        #region StoreReport
        CreateMap<StoreReportDto, ExportStoreDto>();
        #endregion
    }
}