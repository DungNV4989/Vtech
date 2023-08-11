using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Account.Web;
using Volo.Abp.Account.Web.Pages.Account;
using Volo.Abp.DependencyInjection;
using Volo.Abp.TenantManagement;
using IdentityUser = Volo.Abp.Identity.IdentityUser;
using Volo.Abp.MultiTenancy;
using System;
using Volo.Abp.Identity;

namespace VTECHERP.AuthServcer.Pages.Account
{
    [ExposeServices(typeof(LoginModel))]
    public class CustomLoginModel : LoginModel
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IIdentityUserRepository _identityUserRepository;

        public CustomLoginModel(
            IAuthenticationSchemeProvider schemeProvider,
            IOptions<AbpAccountOptions> accountOptions,
            IOptions<IdentityOptions> identityOptions,
            ITenantRepository tenantRepository,
            IIdentityUserRepository identityUserRepository)
            : base(schemeProvider, accountOptions, identityOptions)
        {
            _tenantRepository = tenantRepository;
            _identityUserRepository = identityUserRepository;
        }

        public override async Task<IActionResult> OnPostAsync(string action)
        {
            var user = await FindUserAsync(LoginInput.UserNameOrEmailAddress);
            using (CurrentTenant.Change(user?.TenantId))
            {
                return await base.OnPostAsync(action);
            }
        }

        public override async Task<IActionResult> OnGetExternalLoginCallbackAsync(string returnUrl = "", string returnUrlHash = "", string remoteError = null)
        {
            var user = await FindUserAsync(LoginInput.UserNameOrEmailAddress);
            using (CurrentTenant.Change(user?.TenantId))
            {
                return await base.OnGetExternalLoginCallbackAsync(returnUrl, returnUrlHash, remoteError);
            }
        }

        protected virtual async Task<IdentityUser> FindUserAsync(string uniqueUserNameOrEmailAddress)
        {
            try
            {
                IdentityUser user = null;
                Console.WriteLine("Current Tenant: ", CurrentTenant.Name);
                var users = await _identityUserRepository.GetListAsync();
                user = 
                    await UserManager.FindByNameAsync(LoginInput.UserNameOrEmailAddress) ??
                    await UserManager.FindByEmailAsync(LoginInput.UserNameOrEmailAddress);

                if (user != null)
                {
                    return user;
                }

                return null;
            }
            catch (Exception e)
            {
                return null;
            }

        }
    }
}
