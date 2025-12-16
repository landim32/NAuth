using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NAuth.Domain.Models.Models;
using NAuth.Domain.Services.Interfaces;
using NAuth.DTO.User;
using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace NAuth.Domain
{
    public class LocalAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserService _userService;

        public LocalAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserService userService)
            : base(options, logger, encoder, clock)
        {
            _userService = userService;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
            }

            IUserModel user = null;
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var token = authHeader.Parameter;
                if (string.IsNullOrEmpty(token))
                {
                    return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Token"));
                }

                user = _userService.GetUserByToken(token);
                if (user == null)
                {
                    return Task.FromResult(AuthenticateResult.Fail("Invalid Session"));
                }

            }
            catch (Exception)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
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

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
