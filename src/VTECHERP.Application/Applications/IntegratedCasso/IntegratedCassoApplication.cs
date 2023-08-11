using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VTECHERP.DTOs.IntegratedCasso;
using VTECHERP.Helper;
using VTECHERP.ServiceInterfaces;

namespace VTECHERP.Applications.IntergratedCasso
{
    [Route("api/app/casso/[action]")]
    public class IntegratedCassoApplication : ApplicationService
    {
        private readonly IIntegratedCassoService _integratedCassoService;
        public IntegratedCassoApplication(IIntegratedCassoService integratedCassoService)
        {
            _integratedCassoService = integratedCassoService;
        }

        [HttpPost]
        [CustomAuthFilter]
        public async Task<IActionResult> HandlerWebhookCasso(CassoWebhookDTO request)
        {
            return await _integratedCassoService.HandlerWebhookCasso(request);
        }

        [HttpGet]
        public async Task<IActionResult> GenerateToken(Guid userId)
        {
            var mySecret = "v$t^e23c42h34^&%&^%&^hjsdfb2%%%";
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            var myIssuer = "http://mysite.com";
            var myAudience = "http://myaudience.com";

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                }),
                Issuer = myIssuer,
                Audience = myAudience,
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new GenericActionResult(200, true, "Thành công", tokenHandler.WriteToken(token));
        }
        
    }
}
