using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.Accounts;
using VTECHERP.DTOs.Base;

namespace VTECHERP.Services
{
    public interface IAccountService: IScopedDependency
    {
        Task<(List<AccountDto> data, int count, bool isConfigDefault)> SearchAccounts(SearchAccountRequest request);
        Task<(bool success, string message)> CreateAsync(CreateAccountRequest request);
        Task<(bool success, string message)> UpdateAsync(UpdateAccountRequest request);
        Task UpdateStatusAsync(UpdateAccountStatusRequest request);
        Task DeleteAsync(Guid id);
        Task<AccountDto> GetDetailAsync(Guid id);
        Task<byte[]> ExportAccount(ExportAccountRequest request);
        Task<int> CheckParentAccountCode(string accountParrentCode);
    }
}
