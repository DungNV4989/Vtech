using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Timing;
using Volo.Abp.Users;
using Volo.Abp.Validation;
using VTECHERP.Constants;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.DebtReminderLogs;
using VTECHERP.Entities;
using VTECHERP.ServiceInterfaces;

namespace VTECHERP.Services
{
    public class DebtReminderLogService : IDebtReminderLogService
    {
        private readonly IRepository<DebtReminderLog> _debtReminderLogRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Stores> _storesRepository;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly IIdentityUserRepository _userRepository;
        private readonly ICurrentUser _currentUser;

        private readonly IObjectMapper _objectMapper;
        private readonly IClock _clock;

        public DebtReminderLogService(
            IRepository<DebtReminderLog> debtReminderLogRepository,
            IRepository<Customer> customerRepository,
            IRepository<Stores> storesRepository,
            IIdentityUserRepository userRepository,
            IObjectMapper objectMapper,
            IClock clock,
            IRepository<UserStore> userStoreRepository,
            ICurrentUser currentUser
            )
        {
            _debtReminderLogRepository = debtReminderLogRepository;
            _customerRepository = customerRepository;
            _storesRepository = storesRepository;
            _userRepository = userRepository;
            _objectMapper = objectMapper;
            _clock = clock;
            _userStoreRepository = userStoreRepository;
            _currentUser = currentUser;
        }

        public async Task<DebtReminderLogCreateRequest> AddAsync(DebtReminderLogCreateRequest request)
        {
            await ValidateAdd(request);

            var debtReminderLog = _objectMapper.Map<DebtReminderLogCreateRequest, DebtReminderLog>(request);
            debtReminderLog.CreationTime = _clock.Now;
            await _debtReminderLogRepository.InsertAsync(debtReminderLog);
            
            return request;
        }

        public async Task<PagingResponse<DebtReminderLogDto>> GetListAsync(SearchDebtReminderLogRequest request)
        {
            var result = new List<DebtReminderLogDto>();

            var userStoreQueryable = await _userStoreRepository.GetQueryableAsync();
            var storeOfUser = userStoreQueryable.Where(x => x.UserId == _currentUser.Id).Select(x => x.StoreId).ToList();

            var debtReminderLogs = (await _debtReminderLogRepository.GetQueryableAsync())
              
                .WhereIf(request.PayDateFrom.HasValue, x => x.PayDate.Value.Date >= request.PayDateFrom.Value.Date)
                .WhereIf(request.PayDateTo.HasValue, x => x.PayDate.Value.Date <= request.PayDateTo.Value.Date)
                .WhereIf(!request.Code.IsNullOrWhiteSpace(), x => x.Code.ToLower().Contains(request.Code.ToLower()))
                .ToList();

            if (!debtReminderLogs.Any())
                return new PagingResponse<DebtReminderLogDto>(0, result);

            var customers = (await _customerRepository.GetQueryableAsync())
                .Where(x => debtReminderLogs.Select(dRL => dRL.CustomerId).Any(customerId => customerId == x.Id));

            var handlerEmployeeIds = customers.Select(x => x.HandlerEmployeeId).Distinct();
            var users = (await _userRepository.GetListAsync())
                .Where(x => handlerEmployeeIds.Any(id => id == x.Id) ||
                debtReminderLogs.Select(dRL => dRL.CreatorId).Any(userId => userId == x.Id));

            var handlerStoreIds = customers.Select(x => x.HandlerStoreId).Distinct();
            var stores = (await _storesRepository.GetQueryableAsync())
               .Where(x => handlerStoreIds.Any(storesId => storesId == x.Id));

            foreach (var debtReminderLog in debtReminderLogs)
            {
                var debtReminderLogDto = new DebtReminderLogDto()
                {
                    Id = debtReminderLog.Id,
                    Code = debtReminderLog.Code,
                    CreatorId = debtReminderLog.CreatorId ?? Guid.Empty,
                    CreateTime = debtReminderLog.CreationTime,
                    PayDate = debtReminderLog.PayDate,
                    Content = debtReminderLog.Content,
                    CustomerId = debtReminderLog.CustomerId ?? Guid.Empty
                };

                var creator = users.FirstOrDefault(x => x.Id == debtReminderLog.CreatorId);
                debtReminderLogDto.CreateName = creator != null ? creator.Name : string.Empty;

                if (debtReminderLog.CustomerId != null)
                {
                    var customer = customers.FirstOrDefault(x => x.Id == debtReminderLog.CustomerId);
                    debtReminderLogDto.CustomerId = debtReminderLog.CustomerId;
                    debtReminderLogDto.CustomerName = customer != null ? customer.Name : string.Empty;
                    debtReminderLogDto.CustomerPhone = customer != null ? customer.PhoneNumber : string.Empty;

                    if (customer.HandlerStoreId != null)
                    {
                        var handlerStore = stores.FirstOrDefault(s => s.Id == customer.HandlerStoreId);
                        debtReminderLogDto.HandlerStoreIds = customer.HandlerStoreId;
                        debtReminderLogDto.HandlerStoreNames = handlerStore != null ? handlerStore.Name : string.Empty;
                    }

                    if (customer.HandlerEmployeeId != null)
                    {
                        var handlerEmployee = users.FirstOrDefault(u => u.Id == customer.HandlerEmployeeId);
                        debtReminderLogDto.HandlerEmployeeId = customer.HandlerEmployeeId;
                        debtReminderLogDto.HandlerEmployeeName = handlerEmployee != null ? handlerEmployee.Name : string.Empty;
                    }
                }

                result.Add(debtReminderLogDto);
            }

            var resultQuery = result
                .Where(x => storeOfUser.Contains(x.HandlerStoreIds ?? Guid.Empty))
                .WhereIf(request.HandlerStoreIds.Any(), x => request.HandlerStoreIds.Any(id => id == x.HandlerStoreIds))
                .WhereIf(request.CustomerId.HasValue, x => x.CustomerId == request.CustomerId)
                .WhereIf(request.CreatorId.HasValue, x => x.CreatorId == request.CreatorId)
                .WhereIf(request.HandlerEmployeeId.HasValue, x => x.HandlerEmployeeId == request.HandlerEmployeeId)
                .ToList();

            var debtReminderLogDtos = resultQuery
                .OrderByDescending(d => d.Code)
                .Skip(request.Offset)
                .Take(request.PageSize)
                .ToList();

            return new PagingResponse<DebtReminderLogDto>(resultQuery.Count(), debtReminderLogDtos);
        }

        public async Task<byte[]> ExportDebtReminderLogAsync(SearchDebtReminderLogRequest request)
        {
            request.PageIndex = 1;
            request.PageSize = int.MaxValue;
            var debtReminderLogs = (await GetListAsync(request)).Data;
            var exportData = new List<ExportDebtReminderLogResponse>();
            foreach (var debtReminderLog in debtReminderLogs)
            {
                exportData.Add(new ExportDebtReminderLogResponse()
                {
                    Id = debtReminderLog.Code,
                    CreateName = debtReminderLog.CreateName,
                    CreateTime = _clock.Normalize(debtReminderLog.CreateTime.Value).ToString("dd/MM/yyyy"),
                    PayDate = debtReminderLog.PayDate.Value.ToString("dd/MM/yyyy"),
                    CustomerName = debtReminderLog.CustomerName,
                    CustomerPhone = debtReminderLog.CustomerPhone,
                    HandlerStoreNames = debtReminderLog.HandlerStoreNames,
                    HandlerEmployeeName = debtReminderLog.HandlerEmployeeName,
                    Content = debtReminderLog.Content,
                });
            }

            return ExcelHelper.ExportExcel(exportData);
        }

        private async Task ValidateAdd(DebtReminderLogCreateRequest request)
        {
            var validationErrors = new List<ValidationResult>();

            var customer = await _customerRepository.FirstOrDefaultAsync(x => x.Id == request.CustomerId);
            if (customer == null)
                validationErrors.Add(new ValidationResult(ErrorMessages.DebtReminderLog.NotExist));

            var HandlerStore = (await _userStoreRepository.GetQueryableAsync()).FirstOrDefault(x => x.UserId == _currentUser.GetId() && x.StoreId == customer.HandlerStoreId.Value);

            if(HandlerStore == null && (customer.HandlerEmployeeId.Value != _currentUser.GetId()))
                validationErrors.Add(new ValidationResult(ErrorMessages.DebtReminderLog.NotCreate));

            if (validationErrors.Any())
                throw new AbpValidationException(validationErrors);
        }
    }
}