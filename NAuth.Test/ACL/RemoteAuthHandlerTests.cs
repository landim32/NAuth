using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NAuth.ACL;
using NAuth.ACL.Interfaces;
using NAuth.DTO.Settings;
using NAuth.DTO.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Xunit;

namespace NAuth.Test.ACL
{
    public class RemoteAuthHandlerTests
    {
        private readonly Mock<IOptionsMonitor<AuthenticationSchemeOptions>> _mockOptions;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;
        private readonly Mock<ILogger<RemoteAuthHandler>> _mockLogger;
        private readonly Mock<ISystemClock> _mockClock;
        private readonly Mock<IUserClient> _mockUserClient;
        private readonly Mock<IOptions<NAuthSetting>> _mockNAuthSettings;
        private readonly NAuthSetting _nauthSetting;
        private readonly AuthenticationScheme _scheme;

        public RemoteAuthHandlerTests()
        {
            _mockOptions = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
            _mockLoggerFactory = new Mock<ILoggerFactory>();
            _mockLogger = new Mock<ILogger<RemoteAuthHandler>>();
            _mockClock = new Mock<ISystemClock>();
            _mockUserClient = new Mock<IUserClient>();
            _mockNAuthSettings = new Mock<IOptions<NAuthSetting>>();

            _nauthSetting = new NAuthSetting
            {
                JwtSecret = "your-super-secret-key-min-32-chars-long-12345678"
            };

            _mockNAuthSettings.Setup(x => x.Value).Returns(_nauthSetting);
            _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_mockLogger.Object);
            
            _scheme = new AuthenticationScheme("TestScheme", "TestScheme", typeof(RemoteAuthHandler));
            _mockOptions.Setup(x => x.Get(It.IsAny<string>())).Returns(new AuthenticationSchemeOptions());

            _mockClock.Setup(x => x.UtcNow).Returns(DateTimeOffset.UtcNow);
        }

        private RemoteAuthHandler CreateHandler()
        {
            return new RemoteAuthHandler(
                _mockOptions.Object,
                _mockLoggerFactory.Object,
                UrlEncoder.Default,
                _mockClock.Object,
                _mockUserClient.Object,
                _mockNAuthSettings.Object
            );
        }

        private async Task<RemoteAuthHandler> InitializeHandlerAsync(HttpContext context)
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
            // Com header vazio, o Parse falha e retorna "Missing Authorization Header"
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
            Assert.Equal("Missing Authorization Token", result.Failure?.Message);
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

        #region Default Development Token Tests

        [Fact]
        public async Task HandleAuthenticateAsync_WithDefaultToken_AndUserExists_ShouldSucceed()
        {
            // Arrange
            var defaultUser = new UserInfo
            {
                UserId = 1L,
                Name = "Rodrigo",
                Email = "rodrigo@emagine.com.br",
                Hash = "test-hash",
                IsAdmin = true
            };

            _mockUserClient
                .Setup(x => x.GetByEmailAsync("rodrigo@emagine.com.br"))
                .ReturnsAsync(defaultUser);

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer tokendoamor";
            var handler = await InitializeHandlerAsync(context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Ticket);
            Assert.NotNull(result.Principal);
            
            var userIdClaim = result.Principal.FindFirst("userId");
            Assert.NotNull(userIdClaim);
            Assert.Equal("1", userIdClaim.Value);

            _mockUserClient.Verify(x => x.GetByEmailAsync("rodrigo@emagine.com.br"), Times.Once);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_WithDefaultToken_AndUserNotFound_ShouldFail()
        {
            // Arrange
            _mockUserClient
                .Setup(x => x.GetByEmailAsync("rodrigo@emagine.com.br"))
                .ReturnsAsync((UserInfo)null);

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer tokendoamor";
            var handler = await InitializeHandlerAsync(context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Default user not found", result.Failure?.Message);
        }

        #endregion

        #region Valid JWT Token Tests

        [Fact]
        public async Task HandleAuthenticateAsync_WithValidJwtToken_AndUserExists_ShouldSucceed()
        {
            // Arrange
            var userId = 1L;
            var token = GenerateValidJwtToken(userId, "Test User", "test@test.com");

            var user = new UserInfo
            {
                UserId = userId,
                Name = "Test User",
                Email = "test@test.com",
                Hash = "test-hash",
                IsAdmin = false
            };

            _mockUserClient
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

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

            _mockUserClient.Verify(x => x.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_WithValidJwtToken_AndUserNotFound_ShouldFail()
        {
            // Arrange
            var userId = 999L;
            var token = GenerateValidJwtToken(userId, "Test User", "test@test.com");

            _mockUserClient
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync((UserInfo)null);

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = $"Bearer {token}";
            var handler = await InitializeHandlerAsync(context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("User not found or inactive", result.Failure?.Message);
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
                NotBefore = now.AddHours(-2), // Começa 2 horas atrás
                Expires = now.AddHours(-1), // Expirou há 1 hora
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
            // A validação de assinatura errada retorna "Invalid token" com detalhes
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
                    // Sem userId
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

        #region Claims Validation Tests

        [Fact]
        public async Task HandleAuthenticateAsync_WithValidToken_ShouldIncludeAllClaims()
        {
            // Arrange
            var userId = 1L;
            var token = GenerateValidJwtToken(userId, "Test User", "test@test.com");

            var user = new UserInfo
            {
                UserId = userId,
                Name = "Test User",
                Email = "test@test.com",
                Hash = "test-hash",
                IsAdmin = false
            };

            _mockUserClient
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

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

        #region Exception Handling Tests

        [Fact]
        public async Task HandleAuthenticateAsync_WhenUserClientThrows_ShouldFail()
        {
            // Arrange
            var userId = 1L;
            var token = GenerateValidJwtToken(userId, "Test User", "test@test.com");

            _mockUserClient
                .Setup(x => x.GetByIdAsync(userId))
                .ThrowsAsync(new Exception("API Error"));

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = $"Bearer {token}";
            var handler = await InitializeHandlerAsync(context);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Error validating token", result.Failure?.Message);
        }

        #endregion
    }
}
