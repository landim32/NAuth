using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NAuth.DTO.Settings;
using NAuth.DTO.User;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace NAuth.ACL
{
    public class UserClient : IUserClient
    {
        private const string ApplicationJsonMediaType = "application/json";
        private const string BearerAuthenticationScheme = "Bearer";

        private readonly HttpClient _httpClient;
        private readonly IOptions<NAuthSetting> _nauthSetting;

        public UserClient(IOptions<NAuthSetting> nauthSetting)
        {
            _httpClient = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            });
            _nauthSetting = nauthSetting;
        }

        public UserInfo? GetUserInSession(HttpContext httpContext)
        {
            if (httpContext?.User?.Claims == null || !httpContext.User.Claims.Any())
            {
                return null;
            }

            var claims = httpContext.User.Claims.ToList();
            
            var userInfo = new UserInfo
            {
                UserId = long.TryParse(claims.FirstOrDefault(c => c.Type == "userId")?.Value, out var userId) ? userId : 0,
                Name = claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value,
                Email = claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value,
                Hash = claims.FirstOrDefault(c => c.Type == "hash")?.Value,
                IsAdmin = bool.TryParse(claims.FirstOrDefault(c => c.Type == "isAdmin")?.Value, out var isAdmin) && isAdmin,
                Roles = claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                    .Select(c => new RoleInfo { Slug = c.Value })
                    .ToList()
            };

            return userInfo;
        }

        public async Task<UserInfo?> GetMeAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BearerAuthenticationScheme, token);
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/getMe");
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<UserInfo>(await response.Content.ReadAsStringAsync());
        }

        public async Task<UserInfo?> GetByIdAsync(long userId)
        {
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/getById/{userId}");
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<UserInfo>(await response.Content.ReadAsStringAsync());
        }

        public async Task<UserInfo?> GetByTokenAsync(string token)
        {
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/getByToken/{token}");
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<UserInfo>(await response.Content.ReadAsStringAsync());
        }

        public async Task<UserInfo?> GetByEmailAsync(string email)
        {
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/getByEmail/{email}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserInfo>(json);
        }

        public async Task<UserInfo?> GetBySlugAsync(string slug)
        {
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/getBySlug/{slug}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserInfo>(json);
        }

        public async Task<UserInfo?> InsertAsync(UserInfo user)
        {
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, ApplicationJsonMediaType);
            var response = await _httpClient.PostAsync($"{_nauthSetting.Value.ApiUrl}/insert", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserInfo>(json);
        }

        public async Task<UserInfo?> UpdateAsync(UserInfo user, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BearerAuthenticationScheme, token);
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, ApplicationJsonMediaType);
            var response = await _httpClient.PostAsync($"{_nauthSetting.Value.ApiUrl}/update", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserInfo>(json);
        }

        public async Task<UserInfo?> LoginWithEmailAsync(LoginParam param)
        {
            var content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, ApplicationJsonMediaType);
            var response = await _httpClient.PostAsync($"{_nauthSetting.Value.ApiUrl}/loginWithEmail", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserInfo>(json);
        }

        public async Task<bool> HasPasswordAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BearerAuthenticationScheme, token);
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/hasPassword");
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordParam param, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BearerAuthenticationScheme, token);
            var content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, ApplicationJsonMediaType);
            var response = await _httpClient.PostAsync($"{_nauthSetting.Value.ApiUrl}/changePassword", content);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
        }

        public async Task<bool> SendRecoveryMailAsync(string email)
        {
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/sendRecoveryMail/{email}");
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
        }

        public async Task<bool> ChangePasswordUsingHashAsync(ChangePasswordUsingHashParam param)
        {
            var content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, ApplicationJsonMediaType);
            var response = await _httpClient.PostAsync($"{_nauthSetting.Value.ApiUrl}/changePasswordUsingHash", content);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
        }

        public async Task<IList<UserInfo>> ListAsync(int take)
        {
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/list/{take}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IList<UserInfo>>(json);
            return result ?? new List<UserInfo>();
        }

        public async Task<string> UploadImageUserAsync(Stream fileStream, string fileName, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BearerAuthenticationScheme, token);
            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(fileStream), "file", fileName);
            var response = await _httpClient.PostAsync($"{_nauthSetting.Value.ApiUrl}/uploadImageUser", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }

}
