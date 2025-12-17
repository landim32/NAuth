using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NAuth.ACL;
using NAuth.DTO.Settings;
using NAuth.DTO.User;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Xunit;

namespace NAuth.Test.ACL
{
    public class RoleClientTests
    {
        private readonly Mock<IOptions<NAuthSetting>> _mockOptions;
        private readonly Mock<ILogger<RoleClient>> _mockLogger;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly NAuthSetting _nauthSetting;

        public RoleClientTests()
        {
            _mockOptions = new Mock<IOptions<NAuthSetting>>();
            _mockLogger = new Mock<ILogger<RoleClient>>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            _nauthSetting = new NAuthSetting
            {
                ApiUrl = "https://api.test.com"
            };

            _mockOptions.Setup(o => o.Value).Returns(_nauthSetting);
        }

        private RoleClient CreateRoleClient()
        {
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(_nauthSetting.ApiUrl)
            };

            // Usar reflexão para injetar o HttpClient customizado
            var roleClient = new RoleClient(_mockOptions.Object, _mockLogger.Object);
            
            var httpClientField = typeof(RoleClient).GetField("_httpClient", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            httpClientField?.SetValue(roleClient, httpClient);

            return roleClient;
        }

        #region ListAsync Tests

        [Fact]
        public async Task ListAsync_WithValidTake_ShouldReturnRoles()
        {
            // Arrange
            var take = 10;
            var roles = new List<RoleInfo>
            {
                new RoleInfo { RoleId = 1, Slug = "admin", Name = "Administrator" },
                new RoleInfo { RoleId = 2, Slug = "user", Name = "User" }
            };

            var jsonResponse = JsonConvert.SerializeObject(roles);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains($"/Role/list/{take}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var roleClient = CreateRoleClient();

            // Act
            var result = await roleClient.ListAsync(take);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("admin", result[0].Slug);
            Assert.Equal("user", result[1].Slug);
        }

        [Fact]
        public async Task ListAsync_WithEmptyResult_ShouldReturnEmptyList()
        {
            // Arrange
            var take = 10;
            var roles = new List<RoleInfo>();

            var jsonResponse = JsonConvert.SerializeObject(roles);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var roleClient = CreateRoleClient();

            // Act
            var result = await roleClient.ListAsync(take);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task ListAsync_WhenApiReturnsError_ShouldThrowException()
        {
            // Arrange
            var take = 10;
            var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("Internal Server Error")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var roleClient = CreateRoleClient();

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => roleClient.ListAsync(take));
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnRole()
        {
            // Arrange
            var roleId = 1L;
            var role = new RoleInfo
            {
                RoleId = roleId,
                Slug = "admin",
                Name = "Administrator"
            };

            var jsonResponse = JsonConvert.SerializeObject(role);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains($"/Role/getById/{roleId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var roleClient = CreateRoleClient();

            // Act
            var result = await roleClient.GetByIdAsync(roleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(roleId, result.RoleId);
            Assert.Equal("admin", result.Slug);
            Assert.Equal("Administrator", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistentId_ShouldThrowException()
        {
            // Arrange
            var roleId = 999L;
            var httpResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("Role not found")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var roleClient = CreateRoleClient();

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => roleClient.GetByIdAsync(roleId));
        }

        #endregion

        #region GetBySlugAsync Tests

        [Fact]
        public async Task GetBySlugAsync_WithValidSlug_ShouldReturnRole()
        {
            // Arrange
            var slug = "admin";
            var role = new RoleInfo
            {
                RoleId = 1L,
                Slug = slug,
                Name = "Administrator"
            };

            var jsonResponse = JsonConvert.SerializeObject(role);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains($"/Role/getBySlug/{slug}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var roleClient = CreateRoleClient();

            // Act
            var result = await roleClient.GetBySlugAsync(slug);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(slug, result.Slug);
            Assert.Equal("Administrator", result.Name);
        }

        [Fact]
        public async Task GetBySlugAsync_WithNonExistentSlug_ShouldThrowException()
        {
            // Arrange
            var slug = "nonexistent";
            var httpResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("Role not found")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var roleClient = CreateRoleClient();

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => roleClient.GetBySlugAsync(slug));
        }

        [Fact]
        public async Task GetBySlugAsync_WithEmptySlug_ShouldCallApi()
        {
            // Arrange
            var slug = "";
            var httpResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("Role not found")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var roleClient = CreateRoleClient();

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => roleClient.GetBySlugAsync(slug));
        }

        #endregion

        #region Logging Tests

        [Fact]
        public async Task ListAsync_ShouldLogUrlAndResult()
        {
            // Arrange
            var take = 10;
            var roles = new List<RoleInfo>
            {
                new RoleInfo { RoleId = 1, Slug = "admin", Name = "Administrator" }
            };

            var jsonResponse = JsonConvert.SerializeObject(roles);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var roleClient = CreateRoleClient();

            // Act
            await roleClient.ListAsync(take);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("ListAsync")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldLogUrlAndResult()
        {
            // Arrange
            var roleId = 1L;
            var role = new RoleInfo
            {
                RoleId = roleId,
                Slug = "admin",
                Name = "Administrator"
            };

            var jsonResponse = JsonConvert.SerializeObject(role);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var roleClient = CreateRoleClient();

            // Act
            await roleClient.GetByIdAsync(roleId);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("GetByIdAsync")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task GetBySlugAsync_ShouldLogUrlAndResult()
        {
            // Arrange
            var slug = "admin";
            var role = new RoleInfo
            {
                RoleId = 1L,
                Slug = slug,
                Name = "Administrator"
            };

            var jsonResponse = JsonConvert.SerializeObject(role);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var roleClient = CreateRoleClient();

            // Act
            await roleClient.GetBySlugAsync(slug);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("GetBySlugAsync")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce);
        }

        #endregion
    }
}
