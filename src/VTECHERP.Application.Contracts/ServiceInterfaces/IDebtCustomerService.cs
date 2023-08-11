using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.Debts;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.DebtCustomer;

namespace VTECHERP.Services
{
    public interface IDebtCustomerService : IScopedDependency
    {
        Task<PagingResponse<SearchDebtCustomerResponse>> Search(SearchDebtCustomerRequest request);
        Task<TotalDebtCustomerResponse> TotalDebtCustomer(SearchDebtCustomerRequest request);
        Task<PagingResponse<DebtDetailDto>> DetailDebtCustomer(SearchDebtCustomerDetailRequest request);
        Task<byte[]> ExportDebtCustomer(SearchDebtCustomerRequest request);
    }
}
