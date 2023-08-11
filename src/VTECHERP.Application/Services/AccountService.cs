using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;
using VTECHERP.Constants;
using VTECHERP.DTOs.Accounts;
using VTECHERP.DTOs.Base;
using VTECHERP.Entities;
using VTECHERP.Migrations.VTechDb;

namespace VTECHERP.Services
{
    public class AccountService: IAccountService
    {
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<Stores> _storeRepository;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IObjectMapper _objectMapper;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly ICurrentUser _userManager;
        private readonly IRepository<BillCustomer> _billCustomerRepository;
        private readonly IRepository<CustomerReturn> _customerReturnRepository;
        private readonly IRepository<PaymentReceipt> _paymnetReceiptRepository;
        private readonly IRepository<WarehousingBill> _wareHousingBillRepository;
        public AccountService(
            IRepository<Account> accountRepository,
            IRepository<Stores> storeRepository,
            IIdentityUserRepository userRepository,
            IObjectMapper objectMapper,
            IRepository<UserStore> userStoreRepository,
            ICurrentUser userManager,
            IRepository<BillCustomer> billCustomerRepository,
            IRepository<CustomerReturn> customerReturnRepository,
            IRepository<PaymentReceipt> paymnetReceiptRepository,
            IRepository<WarehousingBill> wareHousingBillRepository
            )
        {
            _accountRepository = accountRepository;
            _storeRepository = storeRepository;
            _userRepository = userRepository;
            _objectMapper = objectMapper;
            _userStoreRepository = userStoreRepository;
            _userManager = userManager;
            _billCustomerRepository = billCustomerRepository;
            _customerReturnRepository = customerReturnRepository;
            _paymnetReceiptRepository = paymnetReceiptRepository;
            _wareHousingBillRepository = wareHousingBillRepository;
        }

        public async Task<(List<AccountDto> data, int count, bool isConfigDefault)> SearchAccounts(SearchAccountRequest request)
        {
            var accounts = (await _accountRepository.GetQueryableAsync()).Where(p => !p.IsDeleted);
            var stores = (await _storeRepository.GetQueryableAsync()).Where(p => !p.IsDeleted);
            var allUsers = await _userRepository.GetListAsync();
            var userId = _userManager.Id;
            //var userStores = (await _userStoreRepository.GetQueryableAsync()).Where(x => x.UserId == userId);
            var query =
                from acc in accounts
                join store in stores on acc.StoreId equals store.Id into strs
                from store in strs.DefaultIfEmpty()
                //join userStore in userStores on store.StoreId equals userStore.StoreId
                where
                    (request.Code == null || request.Code == "" || acc.Code.Contains(request.Code))
                    && (request.Name == null || request.Code == "" || acc.Name.Contains(request.Name))
                    && (request.StoreName == null || (store != null && store.Name != "" && store.Name.Contains(request.StoreName)))
                    && (request.AccountType == null || request.AccountType.Count == 0 || (acc.AccountType != null && request.AccountType.Contains(acc.AccountType.Value)))
                select acc;

            var accountDatas = query.ToList();

            var accountDtos = new List<AccountDto>();

            var level1Accounts = accountDatas.Where(p => p.ParentAccountCode.IsNullOrEmpty() || !accountDatas.Any(parent => parent.Code == p.ParentAccountCode)).OrderBy(p => p.Code).ToList();
            var allAccounts = accounts.ToList();

            // lấy tất cả các account cấp 1
            // đệ quy add account con cho account cha để đảm bảo các account con sẽ ở ngay sau account cha
            foreach (var acc in level1Accounts)
            {
                RecursionAddAccount(accountDtos, acc, 1, allAccounts, null);
            }

            var paged = accountDtos.Skip(request.Offset).Take(request.PageSize).ToList();
            var storeIds = paged.Where(p => p.StoreId != null).Select(p => p.StoreId).ToList();
            var pagedStores = stores.Where(p => storeIds.Contains(p.Id)).ToList();
            var parentAccounts = paged.Where(p => !p.ParentAccountCode.IsNullOrWhiteSpace()).ToList();
            foreach (var item in paged)
            {
                if(item.StoreId != null)
                {
                    var store = pagedStores.FirstOrDefault(p => p.Id == item.StoreId);
                    item.StoreName = store?.Name;
                }

                if (!item.ParentAccountCode.IsNullOrWhiteSpace())
                {
                    var parentAccount = parentAccounts.Where(p => p.Code == item.ParentAccountCode).FirstOrDefault();
                }

                if (item.CreatorId != null)
                {
                    var creator = allUsers.FirstOrDefault(p => p.Id == item.CreatorId);
                    item.CreatorName = creator?.Name;
                }

                if (item.LastModifierId != null)
                {
                    var modifier = allUsers.FirstOrDefault(p => p.Id == item.LastModifierId);
                    item.LastModifierName = modifier?.Name;
                }
            }

            var isConfigDefault = await _accountRepository.AnyAsync(x => x.IsDefaultConfig == true);

            return new (paged, accountDtos.Count(), isConfigDefault);
        }

        public async Task<(bool success, string message)> CreateAsync(CreateAccountRequest request)
        {
            var resultValidate = await ValidateAdd(request);
            if (!resultValidate.isValid)
                return (resultValidate.isValid, resultValidate.message);

            var account = _objectMapper.Map<CreateAccountRequest, Account>(request);
            await _accountRepository.InsertAsync(account);
            return (true, "Tạo thành công");
        }

        public async Task<(bool success, string message)> UpdateAsync(UpdateAccountRequest request)
        {
            var resultValidate = await ValidateUpdate(request);
            if (!resultValidate.isValid)
                return (resultValidate.isValid, resultValidate.message);

            var account = await _accountRepository.GetAsync(p => p.Id == request.Id);

            //account.StoreId = request.StoreId;
            //account.Code = request.Code;
            //account.ParentAccountCode = request.ParentAccountCode;
            account.Name = request.Name;
            
            if (account.IsActive != request.IsActive)
            {
                var codeTrim = request.Code.Trim();

                var isExistBillCustomer = await _billCustomerRepository.AnyAsync(x => x.AccountCode == codeTrim || x.AccountCodeBanking == codeTrim);
                var isExistCustomerReturn = await _customerReturnRepository.AnyAsync(x => x.AccountCode == codeTrim || x.AccountCodeBanking == codeTrim);
                var isExistPaymentReceipt = await _paymnetReceiptRepository.AnyAsync(x => x.AccountCode == codeTrim || x.ReciprocalAccountCode == codeTrim);
                var isExistWarehoseBill = await _wareHousingBillRepository.AnyAsync(x => x.CashPaymentAccountCode == codeTrim || x.BankPaymentAccountCode == codeTrim);

                if (isExistBillCustomer || isExistCustomerReturn || isExistPaymentReceipt || isExistWarehoseBill)
                    return (false, "Tài khoản đã tồn tại ở các chứng từ");
            }

            account.IsActive = request.IsActive;

            await _accountRepository.UpdateAsync(account);
            return (true, "Cập nhật thành công");
        }

        public async Task UpdateStatusAsync(UpdateAccountStatusRequest request)
        {
            await ValidateExistAccountId(request.Id);
            var account = await _accountRepository.GetAsync(p => p.Id == request.Id);

            account.IsActive = request.IsActive;

            await _accountRepository.UpdateAsync(account);
        }

        public async Task DeleteAsync(Guid id)
        {
            await ValidateExistAccountId(id);
            var account = await _accountRepository.GetAsync(p => p.Id == id);

            await _accountRepository.DeleteAsync(account);
        }

        public async Task<AccountDto> GetDetailAsync(Guid id)
        {
            await ValidateExistAccountId(id);

            var account = await _accountRepository.GetAsync(p => p.Id == id);

            var accountDto = _objectMapper.Map<Account, AccountDto>(account);

            var store = (await _storeRepository.GetQueryableAsync()).FirstOrDefault(p => p.Id == accountDto.StoreId);
            accountDto.StoreName = store?.Name;

            var parentAccount = (await _accountRepository.GetQueryableAsync()).FirstOrDefault(p => p.Code == accountDto.ParentAccountCode);
            accountDto.ParentAccountName = parentAccount?.Name;

            return accountDto;
        }

        private async Task<(bool isValid, string message)> ValidateAdd(CreateAccountRequest request)
        {
            if (request.StoreId != null)
            {
                var storeExist = await _storeRepository.AnyAsync(p => p.Id == request.StoreId);
                if (!storeExist)
                    return (isValid: false, message: $"Không tồn tại cửa hàng có id là {request.StoreId}");
            }

            if (string.IsNullOrWhiteSpace(request.Code))
                return (isValid: false, message: $"Thiếu mã tài khoản");

            if (string.IsNullOrEmpty(request.Name))
                return (isValid: false, message: $"Thiếu tên tài khoản");

            var existCodeAcc = await _accountRepository.AnyAsync(p => p.Code.Trim() == request.Code.Trim());
            if(existCodeAcc)
                return (isValid: false, message: $"Trùng mã tài khoản");

            var existNameAcc = await _accountRepository.AnyAsync(p => p.Name.Trim() == request.Name.Trim());
            if (existNameAcc)
                return (isValid: false, message: $"Trùng tên tài khoản");

            if (!string.IsNullOrEmpty(request.ParentAccountCode) && (await CheckParentAccountCode(request.ParentAccountCode)) >= 4)
                return (isValid: false, message: $"Mã tài khoản cha không hợp lệ");

            return (true, "");
        }

        private async Task<(bool isValid, string message)> ValidateUpdate(UpdateAccountRequest request)
        {
            if (request.StoreId != null)
            {
                var storeExist = await _storeRepository.AnyAsync(p => p.Id == request.StoreId);
                if (!storeExist)
                    return (isValid: false, message: $"Không tồn tại cửa hàng có id là {request.StoreId}");
            }

            if (string.IsNullOrWhiteSpace(request.Code))
                return (isValid: false, message: $"Thiếu mã tài khoản");

            if (string.IsNullOrEmpty(request.Name))
                return (isValid: false, message: $"Thiếu tên tài khoản");

            var existCodeAcc = await _accountRepository.AnyAsync(p => p.Code.Trim() == request.Code.Trim() && p.Id != request.Id);
            if (existCodeAcc)
                return (isValid: false, message: $"Trùng mã tài khoản");

            var existNameAcc = await _accountRepository.AnyAsync(p => p.Name.Trim() == request.Name.Trim() && p.Id != request.Id);
            if (existNameAcc)
                return (isValid: false, message: $"Trùng tên tài khoản");

            if (!string.IsNullOrEmpty(request.ParentAccountCode) && (await CheckParentAccountCode(request.ParentAccountCode)) >= 4)
                return (isValid: false, message: $"Mã tài khoản cha không hợp lệ");

            return (true, "");
        }

        private async Task ValidateExistAccountId(Guid id)
        {
            var existIdAcc = await _accountRepository.GetAsync(p => p.Id == id);
            if (existIdAcc == null)
            {
                throw new BusinessException(ErrorMessages.Accounts.NotExist);
            }
        }

        public void RecursionAddAccount(List<AccountDto> list, Account current, int currentLevel, List<Account> alls, string? parentAccountName)
        {
            var dto = _objectMapper.Map<Account, AccountDto>(current);
            dto.Level = currentLevel;
            dto.ParentAccountName = parentAccountName;
            list.Add(dto);
            var currentChildrens = alls.Where(acc => acc.ParentAccountCode == current.Code).ToList();
            if (currentChildrens.Any())
            {
                foreach(var children in currentChildrens)
                {
                    RecursionAddAccount(list, children, currentLevel + 1, alls, current.Name);
                }
            }
        }

        public async Task<byte[]> ExportAccount(ExportAccountRequest request)
        {
            var accounts = (await _accountRepository.GetQueryableAsync()).Where(p => !p.IsDeleted);
            var stores = (await _accountRepository.GetQueryableAsync()).Where(p => !p.IsDeleted);
            var allUsers = await _userRepository.GetListAsync();

            var query =
                from acc in accounts
                join store in stores on acc.StoreId equals store.Id into strs
                from store in strs.DefaultIfEmpty()
                where
                    (request.Code == null || request.Code == "" || acc.Code.Contains(request.Code))
                    && (request.Name == null || request.Code == "" || acc.Name.Contains(request.Name))
                    && (request.StoreName == null || (store != null && store.Name != "" && store.Name.Contains(request.StoreName)))
                    && (request.AccountType == null || request.AccountType.Count == 0 || (acc.AccountType != null && request.AccountType.Contains(acc.AccountType.Value)))
                select acc;

            var accountDatas = query.ToList();

            var accountDtos = new List<AccountDto>();

            var level1Accounts = accountDatas.Where(p => p.ParentAccountCode.IsNullOrEmpty() || !accountDatas.Any(parent => parent.Code == p.ParentAccountCode)).OrderBy(p => p.Code).ToList();
            var allAccounts = accounts.ToList();

            // lấy tất cả các account cấp 1
            // đệ quy add account con cho account cha để đảm bảo các account con sẽ ở ngay sau account cha
            foreach (var acc in level1Accounts)
            {
                RecursionAddAccount(accountDtos, acc, 1, allAccounts, null);
            }

            var storeIds = accountDtos.Where(p => p.StoreId != null).Select(p => p.StoreId).ToList();
            var pagedStores = stores.Where(p => storeIds.Contains(p.Id)).ToList();
            var parentAccounts = accountDtos.Where(p => !p.ParentAccountCode.IsNullOrWhiteSpace()).ToList();
            var exports = new List<ExportAccountDto>();
            foreach (var item in accountDtos)
            {
                if (item.StoreId != null)
                {
                    var store = pagedStores.FirstOrDefault(p => p.Id == item.StoreId);
                    item.StoreName = store?.Name;
                }

                if (!item.ParentAccountCode.IsNullOrWhiteSpace())
                {
                    var parentAccount = parentAccounts.Where(p => p.Code == item.ParentAccountCode).FirstOrDefault();
                }

                if (item.CreatorId != null)
                {
                    var creator = allUsers.FirstOrDefault(p => p.Id == item.CreatorId);
                    item.CreatorName = creator?.Name;
                }

                if (item.LastModifierId != null)
                {
                    var modifier = allUsers.FirstOrDefault(p => p.Id == item.LastModifierId);
                    item.LastModifierName = modifier?.Name;
                }

                exports.Add(new ExportAccountDto
                {
                    Code = item.Code.Trim().PadLeft(item.Level * 5 + item.Code.Trim().Length),
                    Name = item.Name.Trim().PadLeft(item.Level * 5 + item.Name.Trim().Length),
                    CreatorName = item.CreatorName,
                    CreationTime = item.CreationTime.ToString("dd/MM/yyyy hh:mm:ss"),
                    IsActiveName = item.IsActive ? "Hoạt động" : "Không hoạt động",
                    StoreName = item.StoreName
                });
            }

            return ExcelHelper.ExportExcel(exports);
        }

        public async Task<int> CheckParentAccountCode(string accountParrentCode)
        {
            int result = 0;
            if (!string.IsNullOrEmpty(accountParrentCode))
            {
                result++;
                var parrentAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == accountParrentCode);
                result += await CheckParentAccountCode(parrentAccount.ParentAccountCode);
            }

            return result;
        }
    }
}
