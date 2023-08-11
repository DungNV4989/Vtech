using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace VTECHERP.Helper
{
    public class CustomAuthFilterAttribute : TypeFilterAttribute
    {

        public CustomAuthFilterAttribute(): base(typeof(CustomAuthFilter)) { }
        
    }

    public class CustomAuthFilter : IAsyncAuthorizationFilter
    {
        private readonly IConfiguration _configuration;
        public CustomAuthFilter(IConfiguration configuration) 
        { 
            _configuration = configuration;
        }

        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            context.HttpContext.Request.Headers.TryGetValue("secure-token", out var authHeader);
            context.HttpContext.Request.Headers.TryGetValue("Token", out var authToken);

            var token = string.IsNullOrEmpty(authHeader) ? authToken.ToString() : authHeader.ToString();
            var check = ValidateCurrentToken(token);
            if (!check)
            {
                context.Result = new StatusCodeResult(((int)HttpStatusCode.Forbidden));
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }

        private bool ValidateCurrentToken(string token)
        {
            var mySecret = _configuration.GetValue<string>("VTechAPIToken:Private.Key");
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            var myIssuer = _configuration.GetValue<string>("VTechAPIToken:Issuer");
            var myAudience = _configuration.GetValue<string>("VTechAPIToken:Audience");

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = myIssuer,
                    ValidAudience = myAudience,
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validatedToken);
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;
        }

        public string GenerateVTechApiToken(string id)
        {
            var mySecret = _configuration.GetValue<string>("VTechAPIToken:Private.Key");
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            var myIssuer = _configuration.GetValue<string>("VTechAPIToken:Issuer");
            var myAudience = _configuration.GetValue<string>("VTechAPIToken:Audience");

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, id),
                }),
                Issuer = myIssuer,
                Audience = myAudience,
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
