using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.DebtReports;
using VTECHERP.DTOs.Suppliers;
using VTECHERP.Enums;

namespace VTECHERP.Debts
{
    public interface IDebtAppService : IApplicationService
    {
        Task<DebtDto> Create(CreateOrUpdateDebtDto input);
        Task<PagingResponse<DebtDto>> Search(SearchDebtRequest input);
        Task<DebtDto> GetDetail(Guid id);
        Task<PagingResponse<SupplierDetailDto>> GetListSuppliers(SupplierOrigin? type, string phoneNumber);
        //Task<List<DebtReportDto>> StatisticalDebt(DateTime input);
        Task<bool> StatisticalMonthDebt(DateTime input);
        Task<PagingResponse<DebtDetailDto>> GetDebtDetail(SearchDebtDetailRequest request);
    }
}
