using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NAuth.DTO.Domain;
using NAuth.DTO.Settings;
using NAuth.DTO.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NAuth.Client
{
    public class UserClient: IUserClient
    {
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
            if (httpContext.User.Claims.Count() > 0)
            {
                return JsonConvert.DeserializeObject<UserInfo>(httpContext.User.Claims.First().Value);
            }
            return null;
        }

        public async Task<UserResult?> GetMeAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/getMe");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<UserResult?> GetByIdAsync(long userId)
        {
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/getById/{userId}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<UserResult?> GetByTokenAsync(string token)
        {
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/getByToken/{token}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<UserResult?> GetByEmailAsync(string email)
        {
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/getByEmail/{email}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<UserResult?> GetBySlugAsync(string slug)
        {
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/getBySlug/{slug}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<UserResult?> InsertAsync(UserInfo user)
        {
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_nauthSetting.Value.ApiUrl}/insert", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<UserResult?> UpdateAsync(UserInfo user, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_nauthSetting.Value.ApiUrl}/update", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<UserResult?> LoginWithEmailAsync(LoginParam param)
        {
            var content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_nauthSetting.Value.ApiUrl}/loginWithEmail", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<StatusResult?> HasPasswordAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/hasPassword");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<StatusResult>(json);
        }

        public async Task<StatusResult?> ChangePasswordAsync(ChangePasswordParam param, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_nauthSetting.Value.ApiUrl}/changePassword", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<StatusResult>(json);
        }

        public async Task<StatusResult?> SendRecoveryMailAsync(string email)
        {
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/sendRecoveryMail/{email}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<StatusResult>(json);
        }

        public async Task<StatusResult?> ChangePasswordUsingHashAsync(ChangePasswordUsingHashParam param)
        {
            var content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_nauthSetting.Value.ApiUrl}/changePasswordUsingHash", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<StatusResult>(json);
        }

        public async Task<UserListResult?> ListAsync(int take)
        {
            var response = await _httpClient.GetAsync($"{_nauthSetting.Value.ApiUrl}/list/{take}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserListResult>(json);
        }

        // Para upload de imagem, utilize MultipartFormDataContent
        public async Task<StringResult?> UploadImageUserAsync(Stream fileStream, string fileName, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(fileStream), "file", fileName);
            var response = await _httpClient.PostAsync($"{_nauthSetting.Value.ApiUrl}/uploadImageUser", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<StringResult>(json);
        }
    }

}
