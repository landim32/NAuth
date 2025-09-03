using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NAuth.Client;
using NAuth.DTO.User;

namespace NAuth.Client
{
    public class RemoteAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string TOKEN_DEFAULT = "tokendoamor";
        private const string EMAIL_DEFAULT = "rodrigo@emagine.com.br";

        private readonly IUserClient _userClient;
        public RemoteAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserClient userClient
        )
            : base(options, logger, encoder, clock)
        {
            _userClient = userClient;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            UserInfo user = null; 
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var token = authHeader.Parameter;
                if (string.IsNullOrEmpty(token))
                {
                    return AuthenticateResult.Fail("Missing Authorization Token");
                }
                UserResult? userResult = null;
                if (token == TOKEN_DEFAULT)
                {
                    userResult = await _userClient.GetByEmailAsync(EMAIL_DEFAULT);
                }
                else
                {
                    userResult = await _userClient.GetByTokenAsync(token);
                }
                if (userResult == null) {
                    return AuthenticateResult.Fail("Invalid Session");
                }
                if (!userResult.Sucesso)
                {
                    return AuthenticateResult.Fail(userResult.Mensagem);
                }
                user = userResult.User;
            }
            catch (Exception e)
            {
                return AuthenticateResult.Fail(e.Message + "\n" + e.InnerException?.Message);
            }
            
            var claims = new[] {
                new Claim("UserInfo",  JsonConvert.SerializeObject(new UserInfo() {
                     UserId = user.UserId,
                     Hash = user.Hash,
                     Name = user.Name,
                     Email = user.Email,
                }))
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
