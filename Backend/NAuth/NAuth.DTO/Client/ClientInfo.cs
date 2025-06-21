using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NAuth.DTO.Client
{
    public class ClientInfo
    {
        [JsonPropertyName("clientId")]
        public long ClientId { get; set; }
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
