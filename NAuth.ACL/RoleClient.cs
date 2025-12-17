using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NAuth.ACL.Interfaces;
using NAuth.DTO.Settings;
using NAuth.DTO.User;
using Newtonsoft.Json;

namespace NAuth.ACL
{
    public class RoleClient : IRoleClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<NAuthSetting> _nauthSetting;
        private readonly ILogger<RoleClient> _logger;

        public RoleClient(IOptions<NAuthSetting> nauthSetting, ILogger<RoleClient> logger, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _nauthSetting = nauthSetting;
            _logger = logger;
        }

        public async Task<IList<RoleInfo>> ListAsync(int take)
        {
            var url = $"{_nauthSetting.Value.ApiUrl}/Role/list/{take}";
            _logger.LogInformation("ListAsync - Accessing URL: {Url}, Take={Take}", url, take);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IList<RoleInfo>>(json);

            _logger.LogInformation("ListAsync - Retrieved {Count} roles", result?.Count ?? 0);

            return result ?? new List<RoleInfo>();
        }

        public async Task<RoleInfo?> GetByIdAsync(long roleId)
        {
            var url = $"{_nauthSetting.Value.ApiUrl}/Role/getById/{roleId}";
            _logger.LogInformation("GetByIdAsync - Accessing URL: {Url}", url);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<RoleInfo>(json);
            _logger.LogInformation("GetByIdAsync - Role retrieved: RoleId={RoleId}, Slug={Slug}, Name={Name}",
                result?.RoleId, result?.Slug, result?.Name);

            return result;
        }

        public async Task<RoleInfo?> GetBySlugAsync(string slug)
        {
            var url = $"{_nauthSetting.Value.ApiUrl}/Role/getBySlug/{slug}";
            _logger.LogInformation("GetBySlugAsync - Accessing URL: {Url}", url);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<RoleInfo>(json);
            _logger.LogInformation("GetBySlugAsync - Role retrieved: RoleId={RoleId}, Slug={Slug}, Name={Name}",
                result?.RoleId, result?.Slug, result?.Name);

            return result;
        }
    }
}
