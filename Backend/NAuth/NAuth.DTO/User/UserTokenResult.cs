using NAuth.DTO.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NAuth.DTO.User
{
    public class UserTokenResult: StatusResult
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
        [JsonPropertyName("user")]
        public UserInfo User { get; set; }
    }
}
