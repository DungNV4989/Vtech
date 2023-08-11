using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VTECHERP.DTOs.DayConfiguration;
using VTECHERP.Entities;
using VTECHERP.ServiceInterfaces;

namespace VTECHERP.Services
{
    public class DayConfigurationService : IDayConfigurationService
    {
        private readonly IRepository<DayConfiguration> _dayConfigurationRepository;
        private readonly IRepository<Enterprise> _enterpriseRepository;
        //private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICurrentUser _currentUser;
        //private readonly ICurrentTenant _currentTenant;
        public DayConfigurationService(IRepository<DayConfiguration> dayConfigurationRepository, ICurrentUser currentUser, ICurrentTenant currentTenant, IRepository<Enterprise> enterpriseRepository)
        {
            _dayConfigurationRepository = dayConfigurationRepository;
            _currentUser = currentUser;
            _enterpriseRepository = enterpriseRepository;
            //_currentTenant = currentTenant;
        }

        public async Task CreateOrUpdateDayConfiguration(DayConfigurationRequest request)
        {
            try
            {
                var userTenantId = _currentUser.TenantId;
                var lstDayConfiguration = await _dayConfigurationRepository.GetQueryableAsync();
                var dayConfigurations = lstDayConfiguration.ToList();
                if (request.DayNumbers.HasValue)
                {
                    await CreateOrUpdateNumberToEdit(dayConfigurations, _dayConfigurationRepository, request.DayNumbers.Value, userTenantId.Value);
                }
                if (request.NumberOfDayAllowDeleteEntry.HasValue)
                {
                    await CreateOrUpdateNumberToDelete(dayConfigurations, _dayConfigurationRepository, request.NumberOfDayAllowDeleteEntry.Value, userTenantId.Value);
                }
                if (request.NumberOfDayAllowCreatePayRecieve.HasValue)
                {
                    await CreateOrUpdateNumberOfDayAllowCreatePayRecieve(dayConfigurations, _dayConfigurationRepository, request.NumberOfDayAllowCreatePayRecieve.Value, userTenantId.Value);
                }
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        private async Task CreateOrUpdateNumberToEdit(List<DayConfiguration> dayConfigurations, IRepository<DayConfiguration> _dayConfigurationRepository , int days, Guid userTenantId)
        {
            if (dayConfigurations != null && dayConfigurations.Count() > 0)
            {
                var exist = dayConfigurations.Where(x => x.TenantId == userTenantId).Any();
                if (exist)
                {
                    var configution = dayConfigurations.Where(x => x.TenantId == userTenantId).FirstOrDefault();
                    configution.DayNumbers = days;
                    await _dayConfigurationRepository.UpdateAsync(configution);
                }
                else
                {
                    var dayConfiguration = new DayConfiguration();
                    dayConfiguration.DayNumbers = days;
                    dayConfiguration.NumberOfDayAllowDeleteEntry = 5;
                    dayConfiguration.NumberOfDayAllowCreatePayRecieve = 5;
                    dayConfiguration.TenantId = userTenantId;
                    await _dayConfigurationRepository.InsertAsync(dayConfiguration);
                }
            }
            else
            {
                var dayConfiguration = new DayConfiguration();
                dayConfiguration.DayNumbers = days;
                dayConfiguration.NumberOfDayAllowDeleteEntry = 5;
                dayConfiguration.NumberOfDayAllowCreatePayRecieve = 5;
                dayConfiguration.TenantId = userTenantId;
                await _dayConfigurationRepository.InsertAsync(dayConfiguration);
            }
        }
        private async Task CreateOrUpdateNumberToDelete(List<DayConfiguration> dayConfigurations, IRepository<DayConfiguration> _dayConfigurationRepository, int days, Guid userTenantId)
        {
            if (dayConfigurations != null && dayConfigurations.Count() > 0)
            {
                var exist = dayConfigurations.Where(x => x.TenantId == userTenantId).Any();
                if (exist)
                {
                    var configution = dayConfigurations.Where(x => x.TenantId == userTenantId).FirstOrDefault();
                    configution.NumberOfDayAllowDeleteEntry = days;
                    await _dayConfigurationRepository.UpdateAsync(configution);
                }
                else
                {
                    var dayConfiguration = new DayConfiguration();
                    dayConfiguration.DayNumbers = 5;
                    dayConfiguration.NumberOfDayAllowDeleteEntry = days;
                    dayConfiguration.NumberOfDayAllowCreatePayRecieve = 5;
                    dayConfiguration.TenantId = userTenantId;
                    await _dayConfigurationRepository.InsertAsync(dayConfiguration);
                }
            }
            else
            {
                var dayConfiguration = new DayConfiguration();
                dayConfiguration.DayNumbers = 5;
                dayConfiguration.NumberOfDayAllowDeleteEntry = days;
                dayConfiguration.NumberOfDayAllowCreatePayRecieve = 5;
                dayConfiguration.TenantId = userTenantId;
                await _dayConfigurationRepository.InsertAsync(dayConfiguration);
            }
        }

        private async Task CreateOrUpdateNumberOfDayAllowCreatePayRecieve(List<DayConfiguration> dayConfigurations, IRepository<DayConfiguration> _dayConfigurationRepository, int days, Guid userTenantId)
        {
            if (dayConfigurations != null && dayConfigurations.Count() > 0)
            {
                var exist = dayConfigurations.Where(x => x.TenantId == userTenantId).Any();
                if (exist)
                {
                    var configution = dayConfigurations.Where(x => x.TenantId == userTenantId).FirstOrDefault();
                    configution.NumberOfDayAllowCreatePayRecieve = days;
                    await _dayConfigurationRepository.UpdateAsync(configution);
                }
                else
                {
                    var dayConfiguration = new DayConfiguration();
                    dayConfiguration.DayNumbers = 5;
                    dayConfiguration.NumberOfDayAllowDeleteEntry = 5;
                    dayConfiguration.NumberOfDayAllowCreatePayRecieve = days;
                    dayConfiguration.TenantId = userTenantId;
                    await _dayConfigurationRepository.InsertAsync(dayConfiguration);
                }
            }
            else
            {
                var dayConfiguration = new DayConfiguration();
                dayConfiguration.DayNumbers = 5;
                dayConfiguration.NumberOfDayAllowDeleteEntry = 5;
                dayConfiguration.NumberOfDayAllowCreatePayRecieve = days;
                dayConfiguration.TenantId = userTenantId;
                await _dayConfigurationRepository.InsertAsync(dayConfiguration);
            }
        }

        public async Task<DayConfigurationDto> GetCommonConfiguration()
        {
            var userTenantId = _currentUser.TenantId;
            var dayConfigurationDto = new DayConfigurationDto();
            var enterprices = await _enterpriseRepository.GetListAsync();
            var dayConfigurations = await _dayConfigurationRepository.GetListAsync();
            if (enterprices != null && enterprices.Count() > 0)
            {
                var enterprice = enterprices.FirstOrDefault(x => x.Id == userTenantId);
                if (enterprice != null)
                {
                    dayConfigurationDto.Code = enterprice.Code;
                    dayConfigurationDto.Name = enterprice.Name;
                    dayConfigurationDto.Address = enterprice.Address;
                    dayConfigurationDto.PhoneNumber = enterprice.PhoneNumber;
                    dayConfigurationDto.Email = enterprice.Email;
                    dayConfigurationDto.PictureLink = enterprice.PictureLink;
                    dayConfigurationDto.WebsiteLink = enterprice.WebsiteLink;
                    dayConfigurationDto.SoftwarePackage = enterprice.SoftwarePackage;
                }
                
            }
            if (dayConfigurations != null && dayConfigurations.Count() > 0)
            {
                var existDayConfiguration = dayConfigurations.FirstOrDefault(x => x.TenantId == userTenantId);
                if (existDayConfiguration != null)
                {
                    dayConfigurationDto.DayNumbers = existDayConfiguration.DayNumbers;
                    dayConfigurationDto.NumberOfDayAllowDeleteEntry = existDayConfiguration.NumberOfDayAllowDeleteEntry;
                    dayConfigurationDto.NumberOfDayAllowCreatePayRecieve = existDayConfiguration.NumberOfDayAllowCreatePayRecieve;
                }
            }
            return  dayConfigurationDto;
        }
    }
}
