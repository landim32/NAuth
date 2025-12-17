using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NAuth.Domain.Models.Models;
using NAuth.Domain.Services.Interfaces;
using NAuth.DTO.Settings;
using NAuth.DTO.User;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace NAuth.Domain
{
    public class LocalAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserService _userService;
        private readonly NAuthSetting _nauthSetting;

        public LocalAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserService userService,
            IOptions<NAuthSetting> nauthSetting)
            : base(options, logger, encoder, clock)
        {
            _userService = userService;
            _nauthSetting = nauthSetting.Value;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var token = authHeader.Parameter;
                
                if (string.IsNullOrEmpty(token))
                {
                    return AuthenticateResult.Fail("Missing Authorization Token");
                }

                // Valida o JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_nauthSetting.JwtSecret);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = "NAuth",
                    ValidateAudience = true,
                    ValidAudience = "NAuth.API",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Remove o delay padrão de 5 minutos
                };

                // Valida e extrai as claims do token
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                // Verifica se é um JWT válido
                if (validatedToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return AuthenticateResult.Fail("Invalid token format");
                }

                // Extrai o userId do token
                var userIdClaim = principal.FindFirst("userId") ?? principal.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out long userId))
                {
                    return AuthenticateResult.Fail("Invalid user ID in token");
                }

                // Busca o usuário para garantir que ainda existe e está válido
                var user = _userService.GetUserByID(userId);
                if (user == null)
                {
                    return AuthenticateResult.Fail("User not found or inactive");
                }

                // Cria uma nova identidade com as claims do token
                var identity = new ClaimsIdentity(principal.Claims, Scheme.Name);
                var claimsPrincipal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

                Logger.LogInformation("JWT token validated successfully for user {UserId}", userId);

                return AuthenticateResult.Success(ticket);
            }
            catch (SecurityTokenExpiredException)
            {
                Logger.LogWarning("Token has expired");
                return AuthenticateResult.Fail("Token has expired");
            }
            catch (SecurityTokenException ex)
            {
                Logger.LogWarning(ex, "Invalid token");
                return AuthenticateResult.Fail($"Invalid token: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error validating token");
                return AuthenticateResult.Fail($"Error validating token: {ex.Message}");
            }
        }
    }
}
