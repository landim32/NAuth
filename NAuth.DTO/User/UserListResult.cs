using NAuth.DTO.Domain;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NAuth.DTO.User
{
    public class UserListResult : StatusResult
    {
        [JsonPropertyName("users")]
        public IList<UserInfo> Users { get; set; }
    }
}
