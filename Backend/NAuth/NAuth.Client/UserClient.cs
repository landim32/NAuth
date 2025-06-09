using Microsoft.AspNetCore.Http;
using NAuth.DTO.Domain;
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
        private const string API_URL = "https://emagine.com.br/api-auth";

        public UserClient()
        {
            _httpClient = new HttpClient();
        }

        public UserInfo? GetUserInSession(HttpContext httpContext)
        {
            if (httpContext.User.Claims.Count() > 0)
            {
                return JsonConvert.DeserializeObject<UserInfo>(httpContext.User.Claims.First().Value);
            }
            return null;
        }
        public async Task<UserTokenResult?> GetTokenAuthorizedAsync(LoginParam login)
        {
            var content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{API_URL}/gettokenauthorized", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserTokenResult>(json);
        }

        public async Task<UserResult?> GetMeAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{API_URL}/getme");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<UserResult?> GetByIdAsync(long userId)
        {
            var response = await _httpClient.GetAsync($"{API_URL}/getbyid/{userId}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<UserResult?> GetByTokenAsync(string token)
        {
            var response = await _httpClient.GetAsync($"{API_URL}/getbytoken/{token}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<UserResult?> GetByEmailAsync(string email)
        {
            var response = await _httpClient.GetAsync($"{API_URL}/getbyemail/{email}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<UserResult?> GetBySlugAsync(string slug)
        {
            var response = await _httpClient.GetAsync($"{API_URL}/getBySlug/{slug}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<UserResult?> InsertAsync(UserInfo user)
        {
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{API_URL}/insert", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<UserResult?> UpdateAsync(UserInfo user, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{API_URL}/update", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<UserResult?> LoginWithEmailAsync(LoginParam param)
        {
            var content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{API_URL}/loginwithemail", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<StatusResult?> HasPasswordAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{API_URL}/haspassword");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<StatusResult>(json);
        }

        public async Task<StatusResult?> ChangePasswordAsync(ChangePasswordParam param, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{API_URL}/changepassword", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<StatusResult>(json);
        }

        public async Task<StatusResult?> SendRecoveryMailAsync(string email)
        {
            var response = await _httpClient.GetAsync($"{API_URL}/sendrecoverymail/{email}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<StatusResult>(json);
        }

        public async Task<StatusResult?> ChangePasswordUsingHashAsync(ChangePasswordUsingHashParam param)
        {
            var content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{API_URL}/changepasswordusinghash", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<StatusResult>(json);
        }

        public async Task<UserListResult?> ListAsync(int take)
        {
            var response = await _httpClient.GetAsync($"{API_URL}/list/{take}");
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
            var response = await _httpClient.PostAsync($"{API_URL}/uploadImageUser", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<StringResult>(json);
        }
    }

}
