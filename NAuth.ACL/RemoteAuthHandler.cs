using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NAuth.ACL.Interfaces;
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

namespace NAuth.ACL
{
    public class RemoteAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string TOKEN_DEFAULT = "tokendoamor";
        private const string EMAIL_DEFAULT = "rodrigo@emagine.com.br";

        private readonly IUserClient _userClient;
        private readonly NAuthSetting _nauthSetting;

        public RemoteAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserClient userClient,
            IOptions<NAuthSetting> nauthSetting
        )
            : base(options, logger, encoder, clock)
        {
            _userClient = userClient;
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
                var authHeaderValue = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrWhiteSpace(authHeaderValue))
                {
                    return AuthenticateResult.Fail("Missing Authorization Token");
                }

                var authHeader = AuthenticationHeaderValue.Parse(authHeaderValue);
                var token = authHeader.Parameter;

                if (string.IsNullOrEmpty(token))
                {
                    return AuthenticateResult.Fail("Missing Authorization Token");
                }

                UserInfo? user = null;

                // Verifica se é o token padrão de desenvolvimento
                if (token == TOKEN_DEFAULT)
                {
                    Logger.LogWarning("Using default development token");
                    user = await _userClient.GetByEmailAsync(EMAIL_DEFAULT);
                    
                    if (user == null)
                    {
                        return AuthenticateResult.Fail("Default user not found");
                    }

                    var devClaims = new[] {
                        new Claim("userId", user.UserId.ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
                        new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                        new Claim("hash", user.Hash ?? string.Empty),
                        new Claim("isAdmin", user.IsAdmin.ToString())
                    };
                    var devIdentity = new ClaimsIdentity(devClaims, Scheme.Name);
                    var devPrincipal = new ClaimsPrincipal(devIdentity);
                    var devTicket = new AuthenticationTicket(devPrincipal, Scheme.Name);

                    return AuthenticateResult.Success(devTicket);
                }

                // Valida JWT token (mesmo padrão do LocalAuthHandler)
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_nauthSetting.JwtSecret ?? "your-super-secret-key-min-32-chars-long-12345678");

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = "NAuth",
                    ValidateAudience = true,
                    ValidAudience = "NAuth.API",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
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

                // Busca o usuário via API remota
                user = await _userClient.GetByIdAsync(userId);
                if (user == null)
                {
                    return AuthenticateResult.Fail("User not found or inactive");
                }

                // Cria uma nova identidade com as claims do token
                var identity = new ClaimsIdentity(principal.Claims, Scheme.Name);
                var claimsPrincipal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

                Logger.LogInformation("JWT token validated successfully for remote user {UserId}", userId);

                return AuthenticateResult.Success(ticket);
            }
            catch (SecurityTokenExpiredException ex)
            {
                Logger.LogWarning(ex, "Token has expired");
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
