using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.BillCustomers.Params;
using VTECHERP.DTOs.BillCustomers.Respons;
using VTECHERP.Entities;
using VTECHERP.Enums;
using VTECHERP.Enums.Bills;

namespace VTECHERP.ServiceInterfaces
{
    public interface IBillCustomerService : IScopedDependency
    {
        Task<(BillCustomerDetail, string, bool) > GetDetail(Guid BillCustomerId);
        Task<(BillCustomerDto, string, bool)> CreateCustomerBill(BillCustomerCreateParam param);
        Task<(BillCustomerDto, string, bool)> UpdateBillCustomer(Guid BillCustomerId, BillCustomerCreateParam param);
        Task<(CustomerRespon, string, bool)> AddCustomerForBillCustomer(CustomerNewParam param);
        Task<(BillCustomerDetailById, string, bool)> GetDetailById(Guid BillCustomerId);
        Task<PagingLogBillCustomerResponse> GetLogBillByIdAsync(LogBillCustomerRequest request);
        Task<(List<BillCustomerProductDetail>, string, bool, int)> GetBillProductByBillCustomerId(BillCustomerByIdParam param);
        Task<(List<BillCustomerEntries>, string, bool, int)> GetEntriesByBillCustomerId(BillCustomerByIdParam param);
        Task<(string, bool)> DeleteBillCustomer(Guid BillCustomerId);
        string MapBillCustomerStatus(CustomerBillPayStatus? status);
        string MapBillCustomerTransportForm(TransportForm? transportForm);
        bool CheckRuleUpdateStatus(CustomerBillPayStatus? oldStatus, CustomerBillPayStatus? newStatus, TransportForm? transportForm);
        Task HandleDocumentBillCustomer(Guid BillCustomerId, CustomerBillPayStatus? billStatus);
        Task HandleTransportInformationForBill(BillCustomer billCustomer);
        Task<(bool isValid, string message)> CheckInventoryProduct(List<BillCustomerProductCheckValid> param, Guid? StoreId);
        Task<PagingResponse<HistoryBillResponse>> GetHistoryBillByCustomerId(HistoryBillRequest request);
        Task<byte[]> ExportBillCustomer(BillCustomerGetListParam request);
        Task<byte[]> DownloadTemplateImport();
        Task<(string message, bool success, Guid? data, byte[]? fileRespon)> ImportBillCustomer(BillCustomerImportParam param);

        Task DeleteBillCustomers(List<Guid> BillCustomerId);
    }
}
