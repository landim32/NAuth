using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NAuth.ACL;
using NAuth.DTO.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Xunit;

namespace NAuth.Test.ACL
{
    public class NAuthHandlerTests
    {
        private readonly Mock<IOptionsMonitor<AuthenticationSchemeOptions>> _mockOptions;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;
        private readonly Mock<ILogger<NAuthHandler>> _mockLogger;
        private readonly Mock<ISystemClock> _mockClock;
        private readonly Mock<IOptions<NAuthSetting>> _mockNAuthSettings;
        private readonly NAuthSetting _nauthSetting;
        private readonly AuthenticationScheme _scheme;

        public NAuthHandlerTests()
        {
            _mockOptions = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
            _mockLoggerFactory = new Mock<ILoggerFactory>();
            _mockLogger = new Mock<ILogger<NAuthHandler>>();
            _mockClock = new Mock<ISystemClock>();
            _mockNAuthSettings = new Mock<IOptions<NAuthSetting>>();

            _nauthSetting = new NAuthSetting
            {
                JwtSecret = "your-super-secret-key-min-32-chars-long-12345678"
            };

            _mockNAuthSettings.Setup(x => x.Value).Returns(_nauthSetting);
            _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_mockLogger.Object);
            
            _scheme = new AuthenticationScheme("TestScheme", "TestScheme", typeof(NAuthHandler));
            _mockOptions.Setup(x => x.Get(It.IsAny<string>())).Returns(new AuthenticationSchemeOptions());

            _mockClock.Setup(x => x.UtcNow).Returns(DateTimeOffset.UtcNow);
        }

        private NAuthHandler CreateHandler()
        {
            return new NAuthHandler(
                _mockOptions.Object,
                _mockLoggerFactory.Object,
                UrlEncoder.Default,
                _mockClock.Object,
                _mockNAuthSettings.Object
            );
        }

        private async Task<NAuthHandler> InitializeHandlerAsync(HttpContext context)
        {
            var handler = CreateHandler();
            await handler.InitializeAsync(_scheme, context);
            return handler;
        }

        private string GenerateValidJwtToken(long userId, string name, string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_nauthSetting.JwtSecret);

            var claims = new[]
            {
                new Claim("userId", userId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email),
                new Claim("hash", "test-hash"),
                new Claim("isAdmin", "false")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Issuer = "NAuth",
                Audience = "NAuth.API"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        #region Missing Authorization Header Tests

        [Fact]
        public async Task HandleAuthenticateAsync_WithoutAuthorizationHeader_ShouldFail()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var handler = await InitializeHandlerAsync(context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Missing Authorization Header", result.Failure?.Message);
        }

        #endregion

        #region Empty/Invalid Authorization Header Tests

        [Fact]
        public async Task HandleAuthenticateAsync_WithEmptyAuthorizationHeader_ShouldFail()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "";
            var handler = await InitializeHandlerAsync(context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Missing Authorization Header", result.Failure?.Message);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_WithWhitespaceAuthorizationHeader_ShouldFail()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "   ";
            var handler = await InitializeHandlerAsync(context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Error validating token", result.Failure?.Message);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_WithBearerButNoToken_ShouldFail()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer ";
            var handler = await InitializeHandlerAsync(context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Missing Authorization Token", result.Failure?.Message);
        }

        #endregion

        #region Valid JWT Token Tests

        [Fact]
        public async Task HandleAuthenticateAsync_WithValidJwtToken_ShouldSucceed()
        {
            // Arrange
            var userId = 1L;
            var token = GenerateValidJwtToken(userId, "Test User", "test@test.com");

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = $"Bearer {token}";
            var handler = await InitializeHandlerAsync(context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Ticket);
            Assert.NotNull(result.Principal);

            var userIdClaim = result.Principal.FindFirst("userId");
            Assert.NotNull(userIdClaim);
            Assert.Equal(userId.ToString(), userIdClaim.Value);
        }

        #endregion

        #region Invalid JWT Token Tests

        [Fact]
        public async Task HandleAuthenticateAsync_WithInvalidJwtToken_ShouldFail()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer invalid.token.here";
            var handler = await InitializeHandlerAsync(context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Error validating token", result.Failure?.Message);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_WithMalformedJwtToken_ShouldFail()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer notajwttoken";
            var handler = await InitializeHandlerAsync(context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Error validating token", result.Failure?.Message);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_WithExpiredJwtToken_ShouldFail()
        {
            // Arrange
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_nauthSetting.JwtSecret);

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", "1"),
                    new Claim(ClaimTypes.NameIdentifier, "1")
                }),
                NotBefore = now.AddHours(-2),
                Expires = now.AddHours(-1),
                IssuedAt = now.AddHours(-2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Issuer = "NAuth",
                Audience = "NAuth.API"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var expiredToken = tokenHandler.WriteToken(token);

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = $"Bearer {expiredToken}";
            var handler = await InitializeHandlerAsync(context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Token has expired", result.Failure?.Message);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_WithWrongSigningKey_ShouldFail()
        {
            // Arrange
            var tokenHandler = new JwtSecurityTokenHandler();
            var wrongKey = Encoding.ASCII.GetBytes("different-secret-key-min-32-chars-long-12345678");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", "1"),
                    new Claim(ClaimTypes.NameIdentifier, "1")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(wrongKey),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Issuer = "NAuth",
                Audience = "NAuth.API"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var wrongToken = tokenHandler.WriteToken(token);

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = $"Bearer {wrongToken}";
            var handler = await InitializeHandlerAsync(context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.StartsWith("Invalid token", result.Failure?.Message);
        }

        #endregion

        #region Token Without UserId Tests

        [Fact]
        public async Task HandleAuthenticateAsync_WithTokenWithoutUserId_ShouldFail()
        {
            // Arrange
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_nauthSetting.JwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "Test User"),
                    new Claim(ClaimTypes.Email, "test@test.com")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Issuer = "NAuth",
                Audience = "NAuth.API"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = $"Bearer {tokenString}";
            var handler = await InitializeHandlerAsync(context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Invalid user ID in token", result.Failure?.Message);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_WithTokenWithInvalidUserId_ShouldFail()
        {
            // Arrange
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_nauthSetting.JwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", "invalid-not-a-number"),
                    new Claim(ClaimTypes.Name, "Test User")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Issuer = "NAuth",
                Audience = "NAuth.API"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = $"Bearer {tokenString}";
            var handler = await InitializeHandlerAsync(context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Invalid user ID in token", result.Failure?.Message);
        }

        #endregion

        #region Token Format Validation Tests

        [Fact]
        public async Task HandleAuthenticateAsync_WithNonHmacSha256Token_ShouldFail()
        {
            // Arrange
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("different-secret-key-min-64-chars-for-hs512-algorithm-test-12345678901234567890");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", "1"),
                    new Claim(ClaimTypes.NameIdentifier, "1")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512
                ),
                Issuer = "NAuth",
                Audience = "NAuth.API"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = $"Bearer {tokenString}";
            var handler = await InitializeHandlerAsync(context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.StartsWith("Invalid token", result.Failure?.Message);
        }

        #endregion

        #region Claims Validation Tests

        [Fact]
        public async Task HandleAuthenticateAsync_WithValidToken_ShouldIncludeAllClaims()
        {
            // Arrange
            var userId = 1L;
            var token = GenerateValidJwtToken(userId, "Test User", "test@test.com");

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = $"Bearer {token}";
            var handler = await InitializeHandlerAsync(context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Principal);

            Assert.NotNull(result.Principal.FindFirst("userId"));
            Assert.NotNull(result.Principal.FindFirst(ClaimTypes.NameIdentifier));
            Assert.NotNull(result.Principal.FindFirst(ClaimTypes.Name));
            Assert.NotNull(result.Principal.FindFirst(ClaimTypes.Email));
            Assert.NotNull(result.Principal.FindFirst("hash"));
            Assert.NotNull(result.Principal.FindFirst("isAdmin"));
        }

        #endregion

        #region Issuer and Audience Validation Tests

        [Fact]
        public async Task HandleAuthenticateAsync_WithWrongIssuer_ShouldFail()
        {
            // Arrange
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_nauthSetting.JwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", "1"),
                    new Claim(ClaimTypes.NameIdentifier, "1")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Issuer = "WrongIssuer",
                Audience = "NAuth.API"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = $"Bearer {tokenString}";
            var handler = await InitializeHandlerAsync(context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Invalid token", result.Failure?.Message);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_WithWrongAudience_ShouldFail()
        {
            // Arrange
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_nauthSetting.JwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", "1"),
                    new Claim(ClaimTypes.NameIdentifier, "1")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Issuer = "NAuth",
                Audience = "WrongAudience"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = $"Bearer {tokenString}";
            var handler = await InitializeHandlerAsync(context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Invalid token", result.Failure?.Message);
        }

        #endregion

        #region ClockSkew Tests

        [Fact]
        public async Task HandleAuthenticateAsync_WithJustExpiredToken_ShouldFailImmediately()
        {
            // Arrange
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_nauthSetting.JwtSecret);

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", "1"),
                    new Claim(ClaimTypes.NameIdentifier, "1")
                }),
                NotBefore = now.AddMinutes(-10),
                Expires = now.AddSeconds(-1),
                IssuedAt = now.AddMinutes(-10),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Issuer = "NAuth",
                Audience = "NAuth.API"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var expiredToken = tokenHandler.WriteToken(token);

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = $"Bearer {expiredToken}";
            var handler = await InitializeHandlerAsync(context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Token has expired", result.Failure?.Message);
        }

        #endregion
    }
}
