using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using VTECHERP.Constants;
using VTECHERP.Datas;
using VTECHERP.DTOs.Accounts;
using VTECHERP.DTOs.Base;
using VTECHERP.Entities;
using VTECHERP.Services;

namespace VTECHERP
{
    [Route("api/app/account/[action]")]
    [Authorize]
    public class AccountApplication : ApplicationService
    {
        private readonly IAccountService _accountService;
        private readonly IRepository<Account> _accountRepository;
        private readonly IDataFilter _dataFilter;
        public AccountApplication(IAccountService accountService
            , IRepository<Account> accountRepository
            , IDataFilter dataFilter
            )
        {
            _accountService = accountService;
            _accountRepository = accountRepository;
            _dataFilter = dataFilter;
        }

        [HttpPost]
        public async Task<IActionResult> Search(SearchAccountRequest request)
        {
            var result = await _accountService.SearchAccounts(request);
            return new OkObjectResult(new
            {
                Data = result.data,
                Count = result.count,
                IsConfigDefault = result.isConfigDefault,
                Success = true
            });
        }

        [HttpPost]
        public async Task<IActionResult> Export(ExportAccountRequest request)
        {
            var file = await _accountService.ExportAccount(request);
            return new FileContentResult(file, ContentTypes.SPREADSHEET)
            {
                FileDownloadName = $"TKKT_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
            };
        }


        [HttpGet]
        public async Task<AccountDto> Get(Guid id)
        {
            return await _accountService.GetDetailAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAccountRequest request)
        {
            if (request == null)
            {
                return new ObjectResult(new
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ"
                })
                {
                    StatusCode = 400
                };
            }

            var result = await _accountService.CreateAsync(request);

            int statusCode = result.success ? 200 : 400;

            return new ObjectResult(new
            {
                Success = result.success,
                Message = result.message
            })
            {
                StatusCode = statusCode
            };
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateAccountRequest request)
        {
            if (request == null)
            {
                return new ObjectResult(new
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ"
                })
                {
                    StatusCode = 400
                };
            }
            var result = await _accountService.UpdateAsync(request);
            int statusCode = result.success ? 200 : 400;

            return new ObjectResult(new
            {
                Success = result.success,
                Message = result.message
            })
            {
                StatusCode = statusCode
            };
        }

        [HttpPut]
        public async Task<bool> UpdateStatus(UpdateAccountStatusRequest request)
        {
            await _accountService.UpdateStatusAsync(request);
            return true;
        }

        [HttpDelete]
        public async Task<bool> Delete(Guid id)
        {
            await _accountService.DeleteAsync(id);
            return true;
        }

        [HttpGet]
        public async Task<IActionResult> ConfigDefaultAccount()
        {
            var tenantId = CurrentTenant.Id;
            var lstAccount = new List<Account>();

            //using (_dataFilter.Disable<IMultiTenant>())
            //{
                foreach (var item in DefaultAccountConfig.Instance.LoadListDefaultAccount())
                {
                    var isExistAccount = await _accountRepository.AnyAsync(x => x.Code == item.Code);
                    if (isExistAccount)
                        continue;

                    var account = new Account()
                    {
                        //TenantId = tenantId,
                        IsDefaultConfig = true,
                        Code = item.Code,
                        Name = item.Name,
                        ParentAccountCode = item.ParrentCode,
                        IsActive = true
                    };

                    lstAccount.Add(account);
                }

                await _accountRepository.InsertManyAsync(lstAccount);
            //}

            return new OkObjectResult(new
            {
                Success = true,
                Message = "Thiết lập tài khoản thành công",
                Data = lstAccount.Select(x => new
                {
                    Code = x.Code,
                    Name = x.Name,
                }).ToList()
            });
        }

        /// <summary>
        /// Lấy ra danh sách ngoại trừ tài khoản cấp 4
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var accounts = await _accountRepository.GetListAsync();

            var result = accounts.Where(x => string.IsNullOrEmpty(x.ParentAccountCode)).ToList();

            var accountHasParrent = accounts.Where(x => !string.IsNullOrEmpty(x.ParentAccountCode)).ToList();
            foreach (var item in accountHasParrent)
            {
                if ((await _accountService.CheckParentAccountCode(item.ParentAccountCode)) < 4)
                    result.Add(item);
            }

            return new OkObjectResult(new
            {
                Success = true,
                Data = result.Select(x => new
                {
                    x.Code,
                    x.Name,
                    x.IsDefaultConfig
                }).ToList()
            });
        }
    }
}
