using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.DayConfiguration;

namespace VTECHERP.ServiceInterfaces
{
    public interface IDayConfigurationService : IScopedDependency
    {
        Task CreateOrUpdateDayConfiguration(DayConfigurationRequest request);
        Task<DayConfigurationDto> GetCommonConfiguration();
    }
}
