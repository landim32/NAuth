using NAuth.DTO.Domain;
using System;
using System.Text.Json.Serialization;

namespace NAuth.DTO.User
{
    public class UserResult : StatusResult
    {
        [JsonPropertyName("user")]
        public UserInfo User { get; set; }
    }
}
