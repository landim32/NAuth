using NAuth.DTO.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NAuth.DTO.User
{
    public class UserListResult: StatusResult
    {
        [JsonPropertyName("users")]
        public IList<UserInfo> Users { get; set; }
    }
}
