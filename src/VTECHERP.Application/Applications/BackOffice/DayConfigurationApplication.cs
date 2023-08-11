using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VTECHERP.DTOs.DayConfiguration;
using VTECHERP.ServiceInterfaces;

namespace VTECHERP.Applications.BackOffice
{
    [Route("api/app/dayConfiguration/[action]")]
    [Authorize]
    public class DayConfigurationApplication : ApplicationService
    {
        private readonly IDayConfigurationService _dayConfigurationService;
        public DayConfigurationApplication(IDayConfigurationService dayConfigurationService)
        {
            _dayConfigurationService = dayConfigurationService;
        }
        [HttpPost]
        public async Task<bool> Create(DayConfigurationRequest request)
        {
            try
            {
                await _dayConfigurationService.CreateOrUpdateDayConfiguration(request);
                return true;
            }
            catch (Exception e)
            {
                await CurrentUnitOfWork.RollbackAsync();
                throw;
            }
        }
        [HttpGet]
        public async Task<DayConfigurationDto> GetCommonConfiguration()
        {
            return await _dayConfigurationService.GetCommonConfiguration();
        }
    }
}
