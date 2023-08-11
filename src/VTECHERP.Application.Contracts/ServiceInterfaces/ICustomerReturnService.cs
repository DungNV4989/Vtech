using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.BillCustomers.Params;
using VTECHERP.DTOs.CustomerReturnImport.Params;
using VTECHERP.DTOs.ExchangeReturn;

namespace VTECHERP.ServiceInterfaces
{
    public interface ICustomerReturnService : IScopedDependency
    {
        Task<(CustomerReturnDTO, string, bool)> Create(CreateCustomerReturnRequest request);
        Task<(string, bool)> ConfirmAsync(Guid customerReturnId);
        Task<(CustomerReturnDTO, string, bool)> Update(CreateCustomerReturnRequest request);
        Task<(CustomerReturnDTO, string, bool)> UpdateNoteAsync(Guid id, string note);
        Task<(PagingResponse<CustomerReturnDTO>, string, bool)> Search(SearchCustomerReturnRequest searchRequest);
        Task<(CustomerReturnDTO, string, bool)> Get(Guid id);
        Task<(bool, string, bool)> Delete(Guid id);
        Task<byte[]> ExportCustomerReturn(SearchCustomerReturnRequest request);

        Task<byte[]> DownloadTemplateImport();
        Task<(string message, bool success, Guid? data, byte[]? fileRespon)> ImportBillCustomerReturn(CustomerReturnImportParam param);

    }
}
