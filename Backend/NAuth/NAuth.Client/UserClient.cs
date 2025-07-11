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
        private string _ApiURL = "https://emagine.com.br/auth-api";

        public UserClient(string apiURL)
        {
            _ApiURL = apiURL;
            _httpClient = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            });
        }

        public void SetApiURL(string apiURL)
        {
            _ApiURL = apiURL;
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
            var response = await _httpClient.PostAsync($"{_ApiURL}/gettokenauthorized", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserTokenResult>(json);
        }

        public async Task<UserResult?> GetMeAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{_ApiURL}/getme");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<UserResult?> GetByIdAsync(long userId)
        {
            var response = await _httpClient.GetAsync($"{_ApiURL}/getbyid/{userId}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<UserResult?> GetByTokenAsync(string token)
        {
            var response = await _httpClient.GetAsync($"{_ApiURL}/getbytoken/{token}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<UserResult?> GetByEmailAsync(string email)
        {
            var response = await _httpClient.GetAsync($"{_ApiURL}/getbyemail/{email}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<UserResult?> GetBySlugAsync(string slug)
        {
            var response = await _httpClient.GetAsync($"{_ApiURL}/getBySlug/{slug}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<UserResult?> InsertAsync(UserInfo user)
        {
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_ApiURL}/insert", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<UserResult?> UpdateAsync(UserInfo user, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_ApiURL}/update", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<UserResult?> LoginWithEmailAsync(LoginParam param)
        {
            var content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_ApiURL}/loginwithemail", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserResult>(json);
        }

        public async Task<StatusResult?> HasPasswordAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{_ApiURL}/haspassword");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<StatusResult>(json);
        }

        public async Task<StatusResult?> ChangePasswordAsync(ChangePasswordParam param, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_ApiURL}/changepassword", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<StatusResult>(json);
        }

        public async Task<StatusResult?> SendRecoveryMailAsync(string email)
        {
            var response = await _httpClient.GetAsync($"{_ApiURL}/sendrecoverymail/{email}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<StatusResult>(json);
        }

        public async Task<StatusResult?> ChangePasswordUsingHashAsync(ChangePasswordUsingHashParam param)
        {
            var content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_ApiURL}/changepasswordusinghash", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<StatusResult>(json);
        }

        public async Task<UserListResult?> ListAsync(int take)
        {
            var response = await _httpClient.GetAsync($"{_ApiURL}/list/{take}");
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
            var response = await _httpClient.PostAsync($"{_ApiURL}/uploadImageUser", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<StringResult>(json);
        }
    }

}
