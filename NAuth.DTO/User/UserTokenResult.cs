using NAuth.DTO.Domain;
using System.Text.Json.Serialization;

namespace NAuth.DTO.User
{
    public class UserTokenResult : StatusResult
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
        [JsonPropertyName("user")]
        public UserInfo User { get; set; }
    }
}
