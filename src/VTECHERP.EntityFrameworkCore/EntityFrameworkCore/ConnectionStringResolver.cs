
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;

namespace VTECHERP.EntityFrameworkCore
{
    [Dependency(ReplaceServices = true)]
    public class CustomConnectionStringResolver : MultiTenantConnectionStringResolver
    {
        private readonly ICurrentUser _currentUser;
        private readonly ICurrentTenant _currentTenant;

        private readonly IDistributedCache _distributedCache;
        public CustomConnectionStringResolver
            (
                IOptionsMonitor<AbpDbConnectionOptions> options,
                ICurrentTenant currentTenant, 
                IServiceProvider serviceProvider,
                ICurrentUser currentUser,
                IDistributedCache distributedCache) :
            
            base(options, currentTenant, serviceProvider)
        {
            _currentTenant = currentTenant;
            _currentUser = currentUser;
            _distributedCache = distributedCache;
        }

        public override async Task<string> ResolveAsync(string connectionStringName = null)
        {
            if(connectionStringName != null)
            {
                // Tách để debug
                if(connectionStringName == "AbpBackgroundJobs")
                {
                    var connectionString = await base.ResolveAsync(connectionStringName);
                    return connectionString;
                }
                else
                {
                    var connectionString = await base.ResolveAsync(connectionStringName);
                    return connectionString;
                }
            }
            return null;
        }
    }
}
