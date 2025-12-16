using NAuth.DTO.Domain;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NAuth.DTO.User
{
    public class UserListPagedResult : StatusResult
    {
        [JsonPropertyName("users")]
        public IList<UserInfo> Users { get; set; }
        [JsonPropertyName("pageNum")]
        public int PageNum { get; set; }
        [JsonPropertyName("pageCount")]
        public int PageCount { get; set; }
    }
}
